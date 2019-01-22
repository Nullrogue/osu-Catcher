using System;
using System.Windows.Forms;

namespace BeatmapCatcher
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();
			this.Visible = false;
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Visible = false;
			e.Cancel = true;
			base.OnFormClosing(e);
		}

		public void setRunCheck(bool val)
		{
			if (this.runCheck.InvokeRequired)
			{
				this.runCheck.Invoke(new Action<bool>(setRunCheck), new object[] { val });
				return;
			}

			this.runCheck.Checked = val;
		}

		public void setMinCheck(bool val)
		{
			if (this.minimizedCheck.InvokeRequired)
			{
				this.minimizedCheck.Invoke(new Action<bool>(setMinCheck), new object[] { val });
				return;
			}

			this.minimizedCheck.Checked = val;
		}

		public void setCloseCheck(bool val)
		{
			if (this.closeCheck.InvokeRequired)
			{
				this.closeCheck.Invoke(new Action<bool>(setCloseCheck), new object[] { val });
				return;
			}

			this.closeCheck.Checked = val;
		}

		public void setPathBox(string s)
		{
			if (this.pathBox.InvokeRequired)
			{
				this.pathBox.Invoke(new Action<string>(setPathBox), new object[] { s });
				return;
			}

			this.pathBox.Text = s;
		}

		private void runCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (runCheck.Checked)
				Program.CreateStartupLnk();
			else
				Program.DeleteStartupLnk();

			Program.settings.RunOnStartup = runCheck.Checked;
			Program.settings.writeSettings();
		}

		private void minimizedCheck_CheckedChanged(object sender, EventArgs e)
		{
			Program.settings.StartMinimized = minimizedCheck.Checked;
			Program.settings.writeSettings();
		}

		private void closeCheck_CheckedChanged(object sender, EventArgs e)
		{
			Program.settings.MinimizeOnClose = closeCheck.Checked;
			Program.settings.writeSettings();
		}

		private void browseButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					if (System.IO.Directory.Exists(folderBrowserDialog.SelectedPath + "\\Songs\\"))
					{
						Program.settings.OsuPath = folderBrowserDialog.SelectedPath;
						Program.settings.writeSettings();

						Program.settingsForm.setPathBox(folderBrowserDialog.SelectedPath);
						
						Program.Watcher.Path = folderBrowserDialog.SelectedPath + "\\Songs\\";

						Program.startWatch();
					} else {
						Program.mainForm.log("ERROR: Valid Osu! installation not found in: " + folderBrowserDialog.SelectedPath);
					}
				}
			} catch (Exception ex) {
				Program.mainForm.log("ERROR: Failed while selecting file path\n" + ex.Message + "\n" + ex.StackTrace);
			}
		}
	}
}
