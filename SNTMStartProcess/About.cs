using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SNTMStartProcess
{
    public partial class About : Form
    {
        string version = "v1.2.1.0";
        CommonSet commonSet = new CommonSet();
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            commonSet.InitCommonSet();
            commonSet.LoadSetting();
            // Choose Language
            if (commonSet.loadedLanguageIndex == 0)
            {
                // show English
                this.label1.Text = "Version: " + version;
                this.linkLabel1.Text = "Check for Updates";
            }
            else if (commonSet.loadedLanguageIndex == 1)
            {
                // show Chinese
                this.label1.Text = "版本：" + version;
                this.linkLabel1.Text = "檢查更新";
            }
            else if (commonSet.loadedLanguageIndex == 2)
            {
                // show Japanese
                this.label1.Text = "バージョン：" + version;
                this.linkLabel1.Text = "更新を確認する";

            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {/*
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Hide this window instead of close
            this.Hide();*/
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/SamyLearningNote/WindowsSimpleNetworkMonitor/releases");
        }
    }
}
