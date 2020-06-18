using IWshRuntimeLibrary;
using Newtonsoft.Json;
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
		static string VersionNum = "1.1.3";
		public static Settings settings = new Settings();
		public static FileSystemWatcher Watcher;
		public static List<String> imagePaths = new List<String>();
		public static List<String> osuPaths = new List<String>();

		[STAThread]
		static void Main()
		{
			// Check to make sure that no other duplicate process is being run.
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
			{
				MessageBox.Show("osu!Catcher already running. Only one instance can be running at a time.");
				return;
			}

			mainForm = new MainForm();
			settingsForm = new SettingsForm();

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
			if (System.IO.File.Exists(exeDirectory + "\\settings.json"))
				{
				// Store the settings from the already compiled settings.json file to the settings object.
					settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText(exeDirectory + "\\settings.json"));
				} else {
					mainForm.WarningLog("WARNING: Settings not found, generating default settings.json file.");

					// Create settings file and write default settings object in json format.
					settings.writeSettings();
				}
				
				mainForm.Log("osu!Catcher Version " + VersionNum);

				mainForm.Minimized = settings.StartMinimized;

				if (mainForm.Minimized)
					mainForm.showNotifyIcon();

				// Set the form checkboxes to match the current settings.
				settingsForm.setRunCheck(settings.RunOnStartup);
				settingsForm.setMinCheck(settings.StartMinimized);
				settingsForm.setPathBox(settings.OsuPath);
				settingsForm.setCloseCheck(settings.MinimizeOnClose);

				if (Directory.Exists(settings.OsuPath))
				{
					Watcher.Path = settings.OsuPath;

					// Start watching the path for file changes.
					setWatch(true);
				} else {
					mainForm.ErrorLog("ERROR: No valid Osu installation found in: " + settings.OsuPath);
				}

				Application.EnableVisualStyles();
				Application.Run(mainForm);
			} catch (Exception e) {
				mainForm.ErrorLog("ERROR: Initializing application\n" + e.Message + "\n" + e.StackTrace);
			}
		}

		public static void CreateStartupLnk()
		{
			try
			{
				WshShellClass wshShell = new WshShellClass();

				IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + Application.ProductName + ".lnk");

				shortcut.TargetPath = Application.ExecutablePath;
				shortcut.WorkingDirectory = Application.StartupPath;

				shortcut.Save();
			} catch (Exception e) {
				mainForm.ErrorLog("ERROR: Creating shortcut\n" + e.Message + "\n" + e.StackTrace);
			}
		}

		public static void DeleteStartupLnk()
		{
			try
			{
				System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + Application.ProductName + ".lnk");
			} catch (Exception e) {
				mainForm.ErrorLog("ERROR: Deleting  shortcut\n" + e.Message + "\n" + e.StackTrace);
			}
		}

		public static void setWatch(bool state)
		{
			try
			{
				Watcher.EnableRaisingEvents = state;
				mainForm.setStateButton((state ? "Stop" : "Start"));

				mainForm.Log((state ? "Started" : "Stopped") + " watching for beatmaps in " + Watcher.Path);
			} catch (Exception e) {
				mainForm.ErrorLog("ERROR: Failed while " + (state ? "starting" : "stopping") + " the fileWatcher\n" + e.Message + "\n" + e.StackTrace);
			}
		}

		public static string getExtension(String path) { return path.Split('.')[path.Split('.').Length - 1]; }

		public static String parseOsu(string path)
		{
			try
			{
				using (StreamReader read = new StreamReader(path))
				{
					while (true)
					{
						string line = read.ReadLine();

						if (line == null)
							break;

						if (line == "[Events]")
						{
							line = read.ReadLine();
							line = read.ReadLine();

							if (line.IndexOf("Video,") != -1)
								line = read.ReadLine();

							int Index = line.IndexOf("0,0,\"");
							int lastIndex = line.LastIndexOf("\"");

							if (Index < 0)
								break;

							Index = line.IndexOf("\"");

							string image = path.Substring(0, path.LastIndexOf('\\')) + "\\" + line.Substring(Index + 1, lastIndex - Index - 1);

							return image;
						}
					}
				}

				return null;
			} catch (Exception e) {
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
						if (imagePaths[i] != null && System.IO.File.Exists(imagePaths[i]))
						{
							System.IO.File.Delete(imagePaths[i]);
							mainForm.Log("Deleted background [" + imagePaths[i].Substring(imagePaths[i].LastIndexOf('\\') + 1) + "] from beatmap [" + imagePaths[i].Substring(0, imagePaths[i].LastIndexOf('\\')).Substring(imagePaths[i].Substring(0, imagePaths[i].LastIndexOf('\\')).LastIndexOf('\\') + 1) + "]");
						}

						imagePaths.RemoveAt(i);
					}
				}
			} catch (Exception ex) {
				mainForm.ErrorLog("ERROR: Deleting  background\n" + ex.Message + "\n" + ex.StackTrace);
			}
		}
	}
}
