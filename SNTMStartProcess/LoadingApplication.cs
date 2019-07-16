using SNTMStartProcess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartProcess
{
    public partial class LoadingForm : Form
    {
        CommonSet commonSet = new CommonSet();

        int loadedLanguageIndex = 0;
        int loadedInterfaceIndex = 0;
        double loadedUpdateFrequency = 1;
        int loadedSpeedUnitIndex = 0;
        string loadedSpeedUnit = "";
        int loadedDisplayMethodIndex = 4;
        int loadedDisplaySizeIndex = 0;
        int loadedDarkModeIndex = 0;
        bool loadedAutoStartCheck = true;

        public LoadingForm()
        {
            InitializeComponent();
        }

        

        private void LoadSetting()
        {
            // load the configuration file by using common loading function
            if (commonSet.LoadSetting())
            {
                // if the read is success, assign the value to different variable
                loadedLanguageIndex = commonSet.loadedLanguageIndex;
                loadedInterfaceIndex = commonSet.loadedInterfaceIndex;
                loadedUpdateFrequency = commonSet.loadedUpdateFrequency;
                loadedSpeedUnitIndex = commonSet.loadedSpeedUnitIndex;
                loadedSpeedUnit = commonSet.loadedSpeedUnit;
                loadedDisplayMethodIndex = commonSet.loadedDisplayMethodIndex;
                loadedDisplaySizeIndex = commonSet.loadedDisplaySizeIndex;
                loadedDarkModeIndex = commonSet.loadedDarkThemeIndex;
                loadedAutoStartCheck = commonSet.loadedAutoStartCheck;

                // *** added
                if (loadedLanguageIndex == 1)
                {
                    this.configurationToolStripMenuItem.Text = "設定";
                    this.aboutToolStripMenuItem.Text = "關於 ...";
                    this.floatingWindowToolStripMenuItem.Text = "懸浮窗";
                    this.showFloatingWindowToolStripMenuItem.Text = "顯示懸浮窗";
                    this.hideFloatingWindowToolStripMenuItem.Text = "隱藏懸浮窗";
                    this.resetFloatingWindowPositionToolStripMenuItem.Text = "重設位置於此";
                    this.exitToolStripMenuItem.Text = "離開";
                }
                else if (loadedLanguageIndex == 2)
                {
                    this.configurationToolStripMenuItem.Text = "設定";
                    this.aboutToolStripMenuItem.Text = "概要 ...";
                    this.floatingWindowToolStripMenuItem.Text = "フローティング・ウインドウ";
                    this.showFloatingWindowToolStripMenuItem.Text = "フローティング・ウインドウを表示する";
                    this.hideFloatingWindowToolStripMenuItem.Text = "フローティング・ウインドウを隠す";
                    this.resetFloatingWindowPositionToolStripMenuItem.Text = "ここに位置をリセット";
                    this.exitToolStripMenuItem.Text = "出口";
                }
                else
                {                    
                    this.configurationToolStripMenuItem.Text = "Configurations";
                    this.aboutToolStripMenuItem.Text = "About ...";
                    this.floatingWindowToolStripMenuItem.Text = "Floating Window";
                    this.showFloatingWindowToolStripMenuItem.Text = "Show floating window";
                    this.hideFloatingWindowToolStripMenuItem.Text = "Hide floating window";
                    this.resetFloatingWindowPositionToolStripMenuItem.Text = "Reset position here";
                    this.exitToolStripMenuItem.Text = "Exit";
                }
            }
            else
            {
                // if the cannot read the configuration file successfully, show error message
                MessageBox.Show("Cannot find the configuration file or the configuration file corrupted.\nPlease try to apply default setting to correct it.");
            }            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // init the commonset
            commonSet.InitCommonSet();

            // check if this computer has any network interfaces
            if (!commonSet.CheckIfHaveInterface())
            {
                MessageBox.Show("There is no network interface enabled in this computer.\nPlease make sure your network interfaces are enabled and start this monitor again.");
                commonSet.EndAllProcess();
            }


            // check if the same program is running already
            if (commonSet.CheckIfRunningSameProcess(commonSet.baseProgramProcessName))
            {
                MessageBox.Show("Simple Network Traffic Monitor is running already.\nPlease try to find it in system tray.");
                this.Close();
            }

            // check if the program name is changed
            if (commonSet.CheckIfNameChanged(commonSet.baseProgramProcessName))
            {
                MessageBox.Show("Executable file name changed or corrupted.\nPlease download the program again.");
                this.Close();
            }

            // Load configuration file
            LoadSetting();

            // check if need to show bubble message
            if (commonSet.loadedShowBubbleCheck)
            {
                // show message to tell te user the program is running in background
                if (commonSet.loadedLanguageIndex == 0)
                {
                    // show English message
                    BGWorkNotifyIcon.BalloonTipText = "Windows Simple Network Monitor is running in system tray.\n(right click the icon to open menu)";
                }
                else if (commonSet.loadedLanguageIndex == 1)
                {
                    // show Chinese message
                    BGWorkNotifyIcon.BalloonTipText = "Windows 網絡監測器現於系統盤中運行\n(右擊圖標以顯示操作選項)";
                }
                else if (commonSet.loadedLanguageIndex == 2)
                {
                    // show Japanese message
                    BGWorkNotifyIcon.BalloonTipText = "ネットワークモニタはシステムトレイで実行されている。\n (アイコンを右クリックして、メニューを開く)";
                }

                BGWorkNotifyIcon.ShowBalloonTip(1000);
            }

            // Check if need to show the floating window
            if (loadedDisplayMethodIndex == 0 || loadedDisplayMethodIndex == 2)
            {
                if (!commonSet.StartProcess(commonSet.floatingWindowExecutableName))
                {
                    if (loadedLanguageIndex == 1)
                    {
                        MessageBox.Show("不能顯示懸浮窗。\n請重新下載本程式。");
                    }
                    else if (loadedLanguageIndex == 2)
                    {
                        MessageBox.Show("フローティング・ウィンドウが表示できない。\nもう一度プログラムをダウンロードしてください。");
                    }
                    else
                    {
                        // show erroe message if the floating window cannot be shown
                        MessageBox.Show("Cannot show the floating window.\nPlease download the program again.");
                    }                    
                }
            }


            // hide the loading window
            this.ShowInTaskbar = false;
            this.Visible = false;
            //this.WindowState = FormWindowState.Minimized;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            //BGWorkNotifyIcon.ContextMenuStrip.Show(Cursor.Position);
            //BGWorkNotifyIcon.ContextMenuStrip.Visible = true;
            //contextMenuStrip1.Show(Cursor.Position);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            commonSet.EndAllProcess();
        }

        private void showFloatingWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!commonSet.StartProcess(commonSet.floatingWindowExecutableName))
            {
                if (loadedLanguageIndex == 1)
                {
                    MessageBox.Show("不能顯示懸浮窗。\n請重新下載本程式。");
                }
                else if (loadedLanguageIndex == 2)
                {
                    MessageBox.Show("フローティング・ウィンドウが表示できない。\nもう一度プログラムをダウンロードしてください。");
                }
                else
                {
                    // show erroe message if the floating window cannot be shown
                    MessageBox.Show("Cannot show the floating window.\nPlease download the program again.");
                }
            }
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!commonSet.StartProcess(commonSet.configurationExecutableName))
            {
                if (loadedLanguageIndex == 1)
                {
                    MessageBox.Show("不能顯示設定視窗。\n請重新下載本程式。");
                }
                else if (loadedLanguageIndex == 2)
                {
                    MessageBox.Show("設定ウィンドウが表示できない。\nもう一度プログラムをダウンロードしてください。");
                }
                else
                {
                    // show erroe message if the floating window cannot be shown
                    MessageBox.Show("Cannot show the configuration window.\nPlease download the program again.");
                }
            }
        }

        private void hideFloatingWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName(commonSet.floatingWindowProcessName);
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }

        private void contextMenuStrip1_MouseClick(object sender, MouseEventArgs e)
        {
            //this.contextMenuStrip1.Hide();
        }

        private void contextMenuStrip1_MouseLeave(object sender, EventArgs e)
        {
            //this.contextMenuStrip1.Hide();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.Show();
        }

        private void showFloatingWindowToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void resetFloatingWindowPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // save cursor position as floating window position
            commonSet.SaveFloatingWindowPosition(Control.MousePosition.X, Control.MousePosition.Y);

            // close floating window
            commonSet.EndProcess(commonSet.floatingWindowProcessName);

            // open floating window
            if (!commonSet.StartProcess(commonSet.floatingWindowExecutableName))
            {
                if (loadedLanguageIndex == 1)
                {
                    MessageBox.Show("不能顯示懸浮窗。\n請重新下載本程式。");
                }
                else if (loadedLanguageIndex == 2)
                {
                    MessageBox.Show("フローティング・ウィンドウが表示できない。\nもう一度プログラムをダウンロードしてください。");
                }
                else
                {
                    // show erroe message if the floating window cannot be shown
                    MessageBox.Show("Cannot show the floating window.\nPlease download the program again.");
                }
            }
        }

        private void BGWorkNotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
        }
    }
}
