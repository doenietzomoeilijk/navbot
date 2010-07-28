using EveMarketTool;
using System.Configuration;

namespace NavBot
{
    partial class InitialForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitialForm));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.iconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.iconMenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.runButton = new System.Windows.Forms.Button();
            this.eveDirectory = new System.Windows.Forms.TextBox();
            this.browseFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.browseFolderButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.checkForErrorsTimer = new System.Windows.Forms.Timer(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iconMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;

            if (Constants.IsBetaVersion)
            {
                this.notifyIcon.BalloonTipText = Constants.BetaTooltipMessage;
            }
            else
            {
                this.notifyIcon.BalloonTipText = "NavBot is now running. Open " + ConfigurationSettings.AppSettings["URLPrefix"] + " in EVE's In-Game Browser. It has been copied into the clipboard for you!";
            }

            this.notifyIcon.BalloonTipTitle = "NavBot Rev." + Constants.Revision;
            this.notifyIcon.ContextMenuStrip = this.iconMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "NavBot Rev. " + Constants.Revision;
            this.notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
            // 
            // iconMenu
            // 
            this.iconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.iconMenuExit});
            this.iconMenu.Name = "iconMenu";
            this.iconMenu.Size = new System.Drawing.Size(224, 48);
            // 
            // iconMenuExit
            // 
            this.iconMenuExit.Name = "iconMenuExit";
            this.iconMenuExit.Size = new System.Drawing.Size(223, 22);
            this.iconMenuExit.Text = "E&xit";
            this.iconMenuExit.ToolTipText = "Stop running NavBot";
            this.iconMenuExit.Click += new System.EventHandler(this.iconMenuExit_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(51, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(355, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = "NavBot needs to know where EVE stores your market logs:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(34, 45);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.Enabled = false;
            this.runButton.Location = new System.Drawing.Point(187, 71);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(141, 27);
            this.runButton.TabIndex = 2;
            this.runButton.Text = "&Run NavBot in Taskbar";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.run_Click);
            // 
            // eveDirectory
            // 
            this.eveDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.eveDirectory.Location = new System.Drawing.Point(54, 37);
            this.eveDirectory.Name = "eveDirectory";
            this.eveDirectory.Size = new System.Drawing.Size(314, 20);
            this.eveDirectory.TabIndex = 3;
            this.eveDirectory.WordWrap = false;
            this.eveDirectory.TextChanged += new System.EventHandler(this.eveDirectory_TextChanged);
            // 
            // browseFolderButton
            // 
            this.browseFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseFolderButton.Location = new System.Drawing.Point(374, 33);
            this.browseFolderButton.Name = "browseFolderButton";
            this.browseFolderButton.Size = new System.Drawing.Size(30, 27);
            this.browseFolderButton.TabIndex = 4;
            this.browseFolderButton.Text = "...";
            this.browseFolderButton.UseVisualStyleBackColor = true;
            this.browseFolderButton.Click += new System.EventHandler(this.browseFolderButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.Location = new System.Drawing.Point(334, 71);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(70, 27);
            this.exitButton.TabIndex = 5;
            this.exitButton.Text = "E&xit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // checkForErrorsTimer
            // 
            this.checkForErrorsTimer.Enabled = true;
            this.checkForErrorsTimer.Interval = 500;
            this.checkForErrorsTimer.Tick += new System.EventHandler(this.checkForErrorsTimer_Tick);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.showToolStripMenuItem.Text = "&Choose EVE Export Directory";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // InitialForm
            // 
            this.ClientSize = new System.Drawing.Size(419, 110);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.browseFolderButton);
            this.Controls.Add(this.eveDirectory);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InitialForm";
            this.Text = "NavBot rev. D";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResizeEnd += new System.EventHandler(this.InitialForm_ResizeEnd);
            this.Load += new System.EventHandler(this.InitialForm_Load);
            this.iconMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.ContextMenuStrip iconMenu;
        private System.Windows.Forms.ToolStripMenuItem iconMenuExit;
        private System.Windows.Forms.TextBox eveDirectory;
        private System.Windows.Forms.FolderBrowserDialog browseFolderDialog;
        private System.Windows.Forms.Button browseFolderButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Timer checkForErrorsTimer;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
    }
}

