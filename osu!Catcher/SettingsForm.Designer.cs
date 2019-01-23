namespace osuCatcher
{
	partial class SettingsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
			this.closeCheck = new System.Windows.Forms.CheckBox();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.pathBox = new System.Windows.Forms.TextBox();
			this.browseButton = new System.Windows.Forms.Button();
			this.runCheck = new System.Windows.Forms.CheckBox();
			this.minimizedCheck = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// closeCheck
			// 
			this.closeCheck.AutoSize = true;
			this.closeCheck.Location = new System.Drawing.Point(12, 58);
			this.closeCheck.Name = "closeCheck";
			this.closeCheck.Size = new System.Drawing.Size(198, 17);
			this.closeCheck.TabIndex = 2;
			this.closeCheck.Text = "Minimize to system tray when closing";
			this.closeCheck.UseVisualStyleBackColor = true;
			this.closeCheck.CheckedChanged += new System.EventHandler(this.closeCheck_CheckedChanged);
			// 
			// pathBox
			// 
			this.pathBox.Location = new System.Drawing.Point(12, 104);
			this.pathBox.Name = "pathBox";
			this.pathBox.ReadOnly = true;
			this.pathBox.Size = new System.Drawing.Size(240, 20);
			this.pathBox.TabIndex = 5;
			// 
			// browseButton
			// 
			this.browseButton.Location = new System.Drawing.Point(258, 104);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new System.Drawing.Size(75, 20);
			this.browseButton.TabIndex = 6;
			this.browseButton.Text = "Browse";
			this.browseButton.UseVisualStyleBackColor = true;
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// runCheck
			// 
			this.runCheck.AutoSize = true;
			this.runCheck.Location = new System.Drawing.Point(12, 12);
			this.runCheck.Name = "runCheck";
			this.runCheck.Size = new System.Drawing.Size(140, 17);
			this.runCheck.TabIndex = 7;
			this.runCheck.Text = "Run on windows startup";
			this.runCheck.UseVisualStyleBackColor = true;
			this.runCheck.CheckedChanged += new System.EventHandler(this.runCheck_CheckedChanged);
			// 
			// minimizedCheck
			// 
			this.minimizedCheck.AutoSize = true;
			this.minimizedCheck.Location = new System.Drawing.Point(12, 35);
			this.minimizedCheck.Name = "minimizedCheck";
			this.minimizedCheck.Size = new System.Drawing.Size(203, 17);
			this.minimizedCheck.TabIndex = 8;
			this.minimizedCheck.Text = "Start program minimized in system tray";
			this.minimizedCheck.UseVisualStyleBackColor = true;
			this.minimizedCheck.CheckedChanged += new System.EventHandler(this.minimizedCheck_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 87);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(121, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Osu installation directory";
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(347, 135);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.minimizedCheck);
			this.Controls.Add(this.runCheck);
			this.Controls.Add(this.browseButton);
			this.Controls.Add(this.pathBox);
			this.Controls.Add(this.closeCheck);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SettingsForm";
			this.Text = "Settings";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.CheckBox closeCheck;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.TextBox pathBox;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.CheckBox runCheck;
		private System.Windows.Forms.CheckBox minimizedCheck;
		private System.Windows.Forms.Label label1;
	}
}