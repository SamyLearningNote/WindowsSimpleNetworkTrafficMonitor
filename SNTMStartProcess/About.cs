/*
 * MIT License
 * 
 * Copyright (c) 2020 SamyLearningNote
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * GitHub:
 * https://github.com/SamyLearningNote
 */
 
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
