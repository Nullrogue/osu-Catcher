using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace osuCatcher
{
	static class Program
	{
		public static MainForm mainForm;
		public static SettingsForm settingsForm;
		static string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		static string VersionNum = "1.2.0";
		public static Dictionary<string, string> settings = new Dictionary<string, string>();
		public static FileSystemWatcher Watcher;
		public static List<string> imagePaths = new List<string>();
		public static List<string> osuPaths = new List<string>();

		public static void writeConfig()
		{
			string content = "";

			foreach (KeyValuePair<string, string> kvp in settings)
				content = content + kvp.Key + "=" + kvp.Value + "\n";

			File.WriteAllText(exeDirectory + "\\settings.cfg", content);
		}

		public static void setWatch(bool state)
		{
			try
			{
				Watcher.EnableRaisingEvents = state;
				mainForm.setStateButton((state ? "Stop" : "Start"));

				mainForm.Log((state ? "Started" : "Stopped") + " watching for beatmaps in " + Watcher.Path);
			}
			catch (Exception e)
			{
				mainForm.ErrorLog("ERROR: Failed while " + (state ? "starting" : "stopping") + " the fileWatcher\n" + e.Message + "\n" + e.StackTrace);
			}
		}

		public static string getExtension(string path) { return path.Split('.')[path.Split('.').Length - 1]; }

		public static string parseOsu(string path)
		{
			try
			{
				using (StreamReader read = new StreamReader(path))
					while (true)
					{
						string line = read.ReadLine();

						if (line == null) break;

						if (line == "[Events]")
						{
							line = read.ReadLine();
							line = read.ReadLine();

							if (line.IndexOf("Video,") != -1) line = read.ReadLine();

							int Index = line.IndexOf("0,0,\"");
							int lastIndex = line.LastIndexOf("\"");

							if (Index < 0) break;

							Index = line.IndexOf("\"");

							string image = path.Substring(0, path.LastIndexOf('\\')) + "\\" + line.Substring(Index + 1, lastIndex - Index - 1);

							return image;
						}
					}
				return null;
			}
			catch (Exception e)
			{
				mainForm.ErrorLog("ERROR: Parsing osu file\n" + e.Message + "\n" + e.StackTrace);
				return null;
			}
		}

		private static void OnCreated(object source, FileSystemEventArgs e)
		{
			if (!Directory.Exists(e.FullPath) && getExtension(e.FullPath).Contains("osu"))
				osuPaths.Add(e.FullPath);
		}

		private static void OnDeleted(object source, FileSystemEventArgs e)
		{
			try
			{
				if (getExtension(e.FullPath).Contains("osz"))
				{
					foreach (string s in osuPaths)
						imagePaths.Add(parseOsu(s));

					for (int i = imagePaths.Count - 1; i >= 0; i--)
					{
						if (imagePaths[i] != null && File.Exists(imagePaths[i]))
						{
							File.Delete(imagePaths[i]);
							mainForm.Log("Deleted background [" + imagePaths[i].Substring(imagePaths[i].LastIndexOf('\\') + 1) + "] from beatmap [" + imagePaths[i].Substring(0, imagePaths[i].LastIndexOf('\\')).Substring(imagePaths[i].Substring(0, imagePaths[i].LastIndexOf('\\')).LastIndexOf('\\') + 1) + "]");
						}

						imagePaths.RemoveAt(i);
					}
				}
			}
			catch (Exception ex)
			{
				mainForm.ErrorLog("ERROR: Deleting  background\n" + ex.Message + "\n" + ex.StackTrace);
			}
		}

		[STAThread]
		static void Main()
		{
			// Check to make sure that no other duplicate process is being run.
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1) { MessageBox.Show("osu!Catcher already running. Only one instance can be running at a time."); return; }

			mainForm = new MainForm();
			settingsForm = new SettingsForm();

			// Set default settings to be written or replaced with config settings.
			settings.Add("OsuPath", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\osu!\\Songs\\");
			settings.Add("RunOnStartup", "False");
			settings.Add("StartMinimized", "False");
			settings.Add("MinimizeOnClose", "True");

			Watcher = new FileSystemWatcher
			{
				NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
				Filter = "*.*",
				IncludeSubdirectories = true
			};

			Watcher.Deleted += new FileSystemEventHandler(OnDeleted);
			Watcher.Created += new FileSystemEventHandler(OnCreated);

			try
			{
				if (File.Exists(exeDirectory + "\\settings.cfg"))
				{
					// Read the settings from the settings file and set the corresponding values.
					try
					{
						using (StreamReader read = new StreamReader(exeDirectory + "\\settings.cfg"))
							while (true)
							{
								string line = read.ReadLine();

								if (line == null || line == "") break;

								string[] values = line.Split('=');
								settings[values[0]] = values[1];
							}
					} catch (Exception e) {
						mainForm.ErrorLog("ERROR: Parsing settings.cfg\n" + e.Message + "\n" + e.StackTrace);
					}
				} else {
					mainForm.WarningLog("WARNING: Settings not found, generating default settings.json file.");

					// Create settings file and write default settings object in json format.
					writeConfig();
				}
				
				mainForm.Log("osu!Catcher Version " + VersionNum);

				mainForm.Minimized = bool.Parse(settings["StartMinimized"]);

				if (mainForm.Minimized) mainForm.showNotifyIcon();

				// Set the form checkboxes to match the current settings.
				settingsForm.setRunCheck(bool.Parse(settings["RunOnStartup"]));
				settingsForm.setMinCheck(bool.Parse(settings["StartMinimized"]));
				settingsForm.setPathBox(settings["OsuPath"]);
				settingsForm.setCloseCheck(bool.Parse(settings["MinimizeOnClose"]));

				if (Directory.Exists(settings["OsuPath"]))
				{
					Watcher.Path = settings["OsuPath"];

					// Start watching the path for file changes.
					setWatch(true);
				} else {
					mainForm.ErrorLog("ERROR: No valid Osu installation found in: " + settings["OsuPath"]);
				}

				Application.EnableVisualStyles();
				Application.Run(mainForm);
			} catch (Exception e) {
				mainForm.ErrorLog("ERROR: Initializing application\n" + e.Message + "\n" + e.StackTrace);
			}
		}
	}
}
