﻿using Newtonsoft.Json;
using System;
using System.IO;

namespace BeatmapCatcher
{
	class Settings
	{
		public string OsuPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\osu!";
		public bool RunOnStartup = false;
		public bool StartMinimized = false;
		public bool MinimizeOnClose = true;

		public void writeSettings()
		{
			File.WriteAllText(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\settings.json", JsonConvert.SerializeObject(this));
		}
	}
}