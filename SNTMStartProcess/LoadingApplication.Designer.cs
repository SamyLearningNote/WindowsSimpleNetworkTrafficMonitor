namespace StartProcess
{
    partial class LoadingForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            this.BGWorkNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFloatingWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideFloatingWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BGWorkNotifyIcon
            // 
            this.BGWorkNotifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.BGWorkNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("BGWorkNotifyIcon.Icon")));
            this.BGWorkNotifyIcon.Text = "Simple Network Traffic Monitor";
            this.BGWorkNotifyIcon.Visible = true;
            this.BGWorkNotifyIcon.Click += new System.EventHandler(this.notifyIcon1_Click);
            this.BGWorkNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem,
            this.showFloatingWindowToolStripMenuItem,
            this.hideFloatingWindowToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(240, 134);
            this.contextMenuStrip1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.contextMenuStrip1_MouseClick);
            this.contextMenuStrip1.MouseLeave += new System.EventHandler(this.contextMenuStrip1_MouseLeave);
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Image = global::SNTMStartProcess.MenuResource.settings;
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(239, 26);
            this.configurationToolStripMenuItem.Text = "Configurations";
            this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
            // 
            // showFloatingWindowToolStripMenuItem
            // 
            this.showFloatingWindowToolStripMenuItem.Image = global::SNTMStartProcess.MenuResource.eye;
            this.showFloatingWindowToolStripMenuItem.Name = "showFloatingWindowToolStripMenuItem";
            this.showFloatingWindowToolStripMenuItem.Size = new System.Drawing.Size(239, 26);
            this.showFloatingWindowToolStripMenuItem.Text = "Show floating window";
            this.showFloatingWindowToolStripMenuItem.Click += new System.EventHandler(this.showFloatingWindowToolStripMenuItem_Click);
            // 
            // hideFloatingWindowToolStripMenuItem
            // 
            this.hideFloatingWindowToolStripMenuItem.Image = global::SNTMStartProcess.MenuResource.visibility;
            this.hideFloatingWindowToolStripMenuItem.Name = "hideFloatingWindowToolStripMenuItem";
            this.hideFloatingWindowToolStripMenuItem.Size = new System.Drawing.Size(239, 26);
            this.hideFloatingWindowToolStripMenuItem.Text = "Hide floating window";
            this.hideFloatingWindowToolStripMenuItem.Click += new System.EventHandler(this.hideFloatingWindowToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::SNTMStartProcess.MenuResource.information;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(239, 26);
            this.aboutToolStripMenuItem.Text = "About ...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::SNTMStartProcess.MenuResource.logout;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(239, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 165);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoadingForm";
            this.Text = "Loading...";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon BGWorkNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showFloatingWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideFloatingWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

