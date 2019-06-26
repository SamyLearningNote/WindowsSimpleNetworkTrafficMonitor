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
        
        int loadedInterfaceIndex = 0;
        double loadedUpdateFrequency = 1;
        int loadedSpeedUnitIndex = 0;
        string loadedSpeedUnit = "";
        bool loadedAutoStartCheck = true;
        int loadedDisplayMethodIndex = 4;
        int loadedDisplaySizeIndex = 0;

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
                loadedInterfaceIndex = commonSet.loadedInterfaceIndex;
                loadedUpdateFrequency = commonSet.loadedUpdateFrequency;
                loadedSpeedUnitIndex = commonSet.loadedSpeedUnitIndex;
                loadedSpeedUnit = commonSet.loadedSpeedUnit;
                loadedAutoStartCheck = commonSet.loadedAutoStartCheck;
                loadedDisplayMethodIndex = commonSet.loadedDisplayMethodIndex;
                loadedDisplaySizeIndex = commonSet.loadedDisplaySizeIndex;
            }
            else
            {
                // if the cannot read the configuration file successfully, show error message
                MessageBox.Show("Cannot find the configuration file or the configuration file corrupted\nPlease try to apply default setting to correct it");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // init the commonset
            commonSet.InitCommonSet();

            // show message to tell te user the program is running in background
            BGWorkNotifyIcon.BalloonTipText = "Windows Simple Network Monitor is running in system tray\n(right click the icon to open menu)";
            BGWorkNotifyIcon.ShowBalloonTip(1000);

            // check if the same program is running already
            if (commonSet.CheckIfRunningSameProcess(commonSet.baseProgramProcessName))
            {
                MessageBox.Show("Simple Network Traffic Monitor is running already\nPlease try to find it in system tray");
                this.Close();
            }

            // check if the program name is changed
            if (commonSet.CheckIfNameChanged(commonSet.baseProgramProcessName))
            {
                MessageBox.Show("Executable file name changed or corrupted\nPlease download the program again");
                this.Close();
            }

            // Load configuration file
            LoadSetting();

            // Check if need to show the floating window
            if (loadedDisplayMethodIndex == 0 || loadedDisplayMethodIndex == 2)
            {
                System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + commonSet.floatingWindowName);
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
            // close all the related application
            // check if floating window is opened, if yes close all the floating window
            Process[] processes = Process.GetProcessesByName(commonSet.floatingWindowProcessName);
            foreach (var process in processes)
            {
                process.Kill();
            }

            // check if configuration window is opened, if yes close all the floating window
            processes = Process.GetProcessesByName(commonSet.configurationProcessName);
            foreach (var process in processes)
            {
                process.Kill();
            }

            // close this application
            Close();
        }

        private void showFloatingWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + commonSet.floatingWindowName);
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + commonSet.configurationName);
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
    }
}
