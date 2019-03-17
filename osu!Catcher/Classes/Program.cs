using IWshRuntimeLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace osuCatcher
{
	static class Program
	{
		public static MainForm mainForm;
		public static SettingsForm settingsForm;
		static string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		static string VersionNum = "1.0.5";
		public static Settings settings;
		public static string[] ExeArgs;
		public static FileSystemWatcher Watcher;
		public static List<String> imagePaths = new List<String>();

		[STAThread]
		static void Main(string[] args)
		{
			ExeArgs = args;

			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
			{
				MessageBox.Show("osu!Catcher already running. Only one instance can be running at a time.");
				return;
			}

			try
			{
				if (System.IO.File.Exists(exeDirectory + "\\settings.json"))
				{
					settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText(exeDirectory + "\\settings.json"));
					mainForm = new MainForm();
					settingsForm = new SettingsForm();
				}
				else
				{
					settings = new Settings();
					if (System.IO.File.Exists(exeDirectory + "\\settings.json"))
						System.IO.File.Create(exeDirectory + "\\settings.json");

					settings.writeSettings();

					mainForm = new MainForm();
					settingsForm = new SettingsForm();
					mainForm.WarningLog("WARNING: Settings not found, generating default settings.json file.");
				}

				if (ExeArgs.Length != 0 && ExeArgs[0] == "-s" && settings.StartMinimized)
					mainForm.Minimized = true;

				mainForm.Log("osu!Catcher Version " + VersionNum);

				mainForm.Minimized = settings.StartMinimized;
				settingsForm.setRunCheck(settings.RunOnStartup);
				settingsForm.setMinCheck(settings.StartMinimized);
				settingsForm.setPathBox(settings.OsuPath);
				settingsForm.setCloseCheck(settings.MinimizeOnClose);

				Watcher = new FileSystemWatcher
				{
					NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
					Filter = "*.*",
					IncludeSubdirectories = true
				};

				Watcher.Deleted += new FileSystemEventHandler(OnDeleted);

				if (Directory.Exists(settings.OsuPath + "\\Songs\\"))
				{
					if (settings.StartMinimized)
						mainForm.Minimized = true;

					Watcher.Path = settings.OsuPath + "\\Songs\\";

					startWatch();
				} else {
					mainForm.ErrorLog("ERROR: Valid Osu installation not found in: " + settings.OsuPath);
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
				shortcut.Arguments = "-s";
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

		public static void startWatch()
		{
			try
			{
				Watcher.EnableRaisingEvents = true;
				mainForm.Started = true;
				mainForm.setStateButton("Stop");

				mainForm.Log("Started watching for beatmaps in " + Watcher.Path);
			} catch (Exception e) {
				mainForm.ErrorLog("ERROR: Failed while starting the fileWatcher\n" + e.Message + "\n" + e.StackTrace);
			}
		}

		public static void stopWatch()
		{
			try
			{
				Watcher.EnableRaisingEvents = false;
				mainForm.Started = false;
				mainForm.setStateButton("Start");

				mainForm.Log("Stopped watching for beatmaps in " + Watcher.Path);
			}
			catch (Exception e)
			{
				mainForm.ErrorLog("ERROR: Failed while stopping the fileWatcher\n" + e.Message + "\n" + e.StackTrace);
			}
		}

		public static string getExtension(String path) { return path.Split('.')[path.Split('.').Length - 1]; }

		public static void parseOsu(string path)
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

							imagePaths.Add(image);

							break;
						}
					}
				}
			} catch (Exception e) {
				mainForm.ErrorLog("ERROR: Parsing osu file\n" + e.Message + "\n" + e.StackTrace);
			}
		}

		private static void OnDeleted(object source, FileSystemEventArgs e)
		{
			try
			{
				if (getExtension(e.FullPath).Contains("osz"))
				{
					string dir = e.FullPath.Substring(0, e.FullPath.LastIndexOf('\\'));
					string path = e.FullPath.Substring(e.FullPath.LastIndexOf('\\'), (e.FullPath.Substring(e.FullPath.LastIndexOf('\\')).Length - 4)).Replace(".", "");

					if (path.Length > 84)
					{
						mainForm.ErrorLog("84 " + path);
						path = path.Substring(0, 84);
					}

					dir += path;

					string[] osuFiles = Directory.GetFiles(dir, "*.osu");

					foreach (string s in osuFiles)
						parseOsu(s);

					for (int i = imagePaths.Count - 1; i >= 0; i--)
					{
						if (System.IO.File.Exists(imagePaths[i]))
						{
							mainForm.Log("Deleted background [" + imagePaths[i].Substring(imagePaths[i].LastIndexOf('\\') + 1) + "] from beatmap [" + imagePaths[i].Substring(0, imagePaths[i].LastIndexOf('\\')).Substring(imagePaths[i].Substring(0, imagePaths[i].LastIndexOf('\\')).LastIndexOf('\\') + 1) + "]");
							System.IO.File.Delete(imagePaths[i]);
						}

						imagePaths.RemoveAt(i);
					}
				}
			} catch (Exception ex)
			{
				mainForm.ErrorLog("ERROR: Deleting  background\n" + ex.Message + "\n" + ex.StackTrace);
			}
		}
	}
}
