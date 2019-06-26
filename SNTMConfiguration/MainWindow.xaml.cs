using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// import library for background work
using System.Threading.Tasks;

// import library for delay
using System.Threading;

// import the library of network
using System.Net.NetworkInformation;

// import the library for auto start
using Microsoft.Win32;
using System.Diagnostics;

namespace WindowsNetworkMonitorWPF
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        double updateFrequency;

        NetworkInterface[] interfaces;
        int selectedInterfaceIndex = -1;

        bool needResetTraffic = true;

        int loadedInterfaceIndex = 0;
        double loadedUpdateFrequency = 1;
        int loadedSpeedUnitIndex = 0;
        string loadedSpeedUnit = "";
        bool loadedAutoStartCheck = true;
        int loadedDisplayMethodIndex = 0;
        int loadedDisplaySizeIndex = 0;

        bool configurationReaded = false;

        double maxFrequency;
        double minFrequency;

        bool autoStartChecked;

        string executableConfigName;
        string configFileName;
        string floatingWindowName;
        
        CommonSet commonSet = new CommonSet();
        
        //SmallFloatingWindow sfw;

        public MainWindow()
        {
            executableConfigName = commonSet.configurationName;
            configFileName = commonSet.configFileName;
            floatingWindowName = commonSet.floatingWindowName;

            InitializeComponent();

            // check if the background process is running, if no, show message and end the program
            if (!commonSet.CheckIfRunning(commonSet.baseProgramProcessName))
            {
                MessageBox.Show("Please run \"StartProcess.exe\" and use it to access the Configuration Window");
                this.Close();
            }

            // check if any same process running, if yes, close all of them
            if (commonSet.CheckIfRunningSameProcess(commonSet.configurationProcessName))
            {
                MessageBox.Show("Configuration Window is opened already");
                this.Close();
            }

            // check if the program name is changed
            if (commonSet.CheckIfNameChanged(commonSet.configurationProcessName))
            {
                MessageBox.Show("Executable file name changed or corrupted\nPlease download the program again");
                this.Close();
            }

            // get parameters from commonset
            minFrequency = commonSet.minFrequency;
            maxFrequency = commonSet.maxFrequency;

            // set the range of value of slider
            this.UpdateFrequencySlider.Minimum = minFrequency;
            this.UpdateFrequencySlider.Maximum = maxFrequency;

            // set the index of speed unit
            this.SpeedUnitSelectionBox.SelectedIndex = 1;

            // init common set
            commonSet.InitCommonSet();

            GetInterfaceInformation();
            LoadSetting();
            GetTrafficInformation();

        }

        private void ShowFloatingWindow()
        {
            // check the size of floating window
            /*if (loadedDisplaySizeIndex == 0)
            {
                sfw = new SmallFloatingWindow();
                sfw.Show();
            }*/
            try
            {
                System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + floatingWindowName);
            }
            catch
            {
                MessageBox.Show("Floating Window file corrupted or not exist\nPlease download the program again");
            }
        }

        private void GetInterfaceInformation()
        {
            // get network interface information

            // check if the network is available
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return;
            }

            // get the list of network interface
            interfaces = NetworkInterface.GetAllNetworkInterfaces();

            // set interface to be selected
            foreach (NetworkInterface ni in interfaces)
            {
                this.NetworkInterfaceSelectionBox.Items.Add(ni.Name);
            }

        }

        private void GetTrafficInformation()
        {

            // use Task to get the information in background
            Task.Factory.StartNew(() =>
            {
                // set the loop to update the value
                while (true)
                {
                    //divisionParameter = 1.0;
                    if (selectedInterfaceIndex > -1)
                    {

                        // get the control of the object
                        this.Dispatcher.Invoke(() => {

                            // get data from common data
                            commonSet.GetTrafficInformation(this.SpeedUnitSelectionBox.SelectedIndex,
                                this.NetworkInterfaceSelectionBox.SelectedIndex, updateFrequency);



                            if (!needResetTraffic)
                            {
                                // update the traffic value of the object
                                this.UploadTrafficTextBlock.Text = String.Format("{0:F2} "
                                    + this.SpeedUnitSelectionBox.Text, commonSet.uploadTrafficValue);

                                this.DownloadTrafficTextBlock.Text = String.Format("{0:F2} "
                                    + this.SpeedUnitSelectionBox.Text, commonSet.downloadTrafficValue);
                            }
                            else
                            {
                                needResetTraffic = false;
                            }
                            
                        });
                    }
                    Thread.Sleep(Convert.ToInt32(updateFrequency * 1000));
                }
            });
        }

        private void SaveSetting()
        {
            // save setting file
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + configFileName, false))
                {
                    file.WriteLine(this.NetworkInterfaceSelectionBox.SelectedIndex);
                    file.WriteLine(String.Format("{0:F2}", updateFrequency));
                    file.WriteLine(this.SpeedUnitSelectionBox.SelectedIndex);
                    file.WriteLine(this.SpeedUnitSelectionBox.Text);
                    file.WriteLine(this.DisplaySelectionBox.SelectedIndex);
                    file.WriteLine(this.WindowSizeSelectionBox.SelectedIndex);
                    file.WriteLine(this.AutoRunCheckBox.IsChecked);
                }
                // display the create success message
                MessageBox.Show("Changes applied");
            }
            catch
            {
                // show erroe message if error occur
                MessageBox.Show("Error occur when saving the setting file\nPlease check the permission of writing or creation of file \"configuration\"");
            }
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

                // set the readed status to true
                configurationReaded = true;

                // set the parameter in form into loaded information
                this.NetworkInterfaceSelectionBox.SelectedIndex = loadedInterfaceIndex;
                this.UpdateFrequencySlider.Value = loadedUpdateFrequency;
                this.SpeedUnitSelectionBox.SelectedIndex = loadedSpeedUnitIndex;
                this.DisplaySelectionBox.SelectedIndex = loadedDisplayMethodIndex;
                this.WindowSizeSelectionBox.SelectedIndex = loadedDisplaySizeIndex;
                this.AutoRunCheckBox.IsChecked = loadedAutoStartCheck;
            }
            else
            {
                // if the cannot read the configuration file successfully, show error message
                MessageBox.Show("Cannot find the configuration file or the configuration file corrupted\nPlease try to apply default setting to correct it");
            }

            #region Old Loading code
            /*
            try
            {
                using (System.IO.StreamReader file = new System.IO.StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + configFileName))
                {
                    bool formatError = false;
                    // read interface index
                    if (!Int32.TryParse(file.ReadLine(), out loadedInterfaceIndex))
                    {
                        formatError = true;
                    }
                    else
                    {
                        // check if the index is in range
                        if (loadedInterfaceIndex < 0 || loadedInterfaceIndex >= interfaces.Length)
                        {
                            formatError = true;
                        }
                    }

                    // read update frequency
                    if (!Double.TryParse(file.ReadLine(), out loadedUpdateFrequency))
                    {
                        formatError = true;
                    }
                    // check if the update frequency is in range
                    else if (loadedUpdateFrequency < minFrequency || loadedUpdateFrequency > maxFrequency)
                    {
                        formatError = true;
                    }

                    // read speed unit index
                    if (!Int32.TryParse(file.ReadLine(), out loadedSpeedUnitIndex))
                    {
                        formatError = true;
                    }
                    else if (loadedSpeedUnitIndex < 0 || loadedSpeedUnitIndex > numberOfSpeedUnit)
                    {
                        formatError = true;
                    }

                    // read speed unit
                    loadedSpeedUnit = file.ReadLine();

                    // read show method index
                    if (!Int32.TryParse(file.ReadLine(), out loadedDisplayMethodIndex))
                    {
                        formatError = true;
                    }
                    else if (loadedDisplayMethodIndex < 0 || loadedDisplayMethodIndex > 3)
                    {
                        formatError = true;
                    }

                    // read floating window size index
                    if (!Int32.TryParse(file.ReadLine(), out loadedDisplaySizeIndex))
                    {
                        formatError = true;
                    }
                    else if (loadedDisplaySizeIndex < 0 || loadedDisplaySizeIndex >= numberOfWindowSize)
                    {
                        formatError = true;
                    }

                    // read auto start option
                    if (!Boolean.TryParse(file.ReadLine(), out loadedAutoStartCheck))
                    {
                        formatError = true;
                    }

                    // check if the format of file correct
                    if (formatError)
                    {
                        // format of the file is wrong, show error message
                        MessageBox.Show("Configuration file corrupted\nPlease try to apply default setting to correct it");
                    }
                    else
                    {
                        configurationReaded = true;
                    }

                }
            }
            catch
            {
                // error occur, show error message
                MessageBox.Show("Cannot find the configuration file or the configuration file corrupted\nPlease try to apply default setting to correct it");
            }*/
            #endregion
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            // enable or disable if the auto start check box is checked or not
            if (autoStartChecked)
            {
                rkApp.SetValue("SNTMauto", System.AppDomain.CurrentDomain.BaseDirectory + commonSet.baseProgramName);
            }
            else
            {
                rkApp.DeleteValue("SNTMauto", false);
            }

            SaveSetting();

            // check if the floating window is opened, if yes turn off the floating window
            Process[] processes = Process.GetProcessesByName(commonSet.floatingWindowProcessName);
            foreach (var process in processes)
            {
                process.Kill();
            }
            
            // check if floating window need to be shown
            if (this.DisplaySelectionBox.SelectedIndex == 0 || this.DisplaySelectionBox.SelectedIndex == 2)
            {
                // check if the floating window is showing
                ShowFloatingWindow();
            }
            else
            {
                //fw.Close();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

            // check if the file is readed successfully
            if (configurationReaded)
            {
                // reset the configuration from the readed configuration
                this.NetworkInterfaceSelectionBox.SelectedIndex = loadedInterfaceIndex;
                this.UpdateFrequencySlider.Value = loadedUpdateFrequency;
                this.SpeedUnitSelectionBox.SelectedIndex = loadedSpeedUnitIndex;
                this.DisplaySelectionBox.SelectedIndex = loadedDisplayMethodIndex;
                this.WindowSizeSelectionBox.SelectedIndex = loadedDisplaySizeIndex;
                this.AutoRunCheckBox.IsChecked = loadedAutoStartCheck;
                MessageBox.Show("Configuration is resetted to the status when the program is started");
            }
            else
            {
                // if the configuration file is not readed, show error message
                MessageBox.Show("Configuration cannot be readed when programme started\nPlease try to apply default configuration and restart the programme");
            }
        }

        private void ResetTrafficInformation()
        {
            this.UploadTrafficTextBlock.Text = "Loading";
            this.DownloadTrafficTextBlock.Text = "Loading";
            needResetTraffic = true;
        }

        private void UpdateFrequencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            updateFrequency = Math.Round(this.UpdateFrequencySlider.Value, 2);
            this.UpdateFrequencyLabel.Text = String.Format("{0}", updateFrequency);

            ResetTrafficInformation();
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            // reset the setting to default setting

            // reset the network interface
            this.NetworkInterfaceSelectionBox.SelectedIndex = 0;

            // reset the update frequency to 1 second
            this.UpdateFrequencySlider.Value = 1;

            // reset the speed unit to MB/s
            this.SpeedUnitSelectionBox.SelectedIndex = 3;

            // reset the auto run
            this.AutoRunCheckBox.IsChecked = true;

            // reset the way to display the traffic
            this.DisplaySelectionBox.SelectedIndex = 0;

            // reset the size of floating window
            this.WindowSizeSelectionBox.SelectedIndex = 0;

            // show message of reset
            MessageBox.Show("Configuration is setted to default state");

            ResetTrafficInformation();
        }

        private void NetworkInterfaceSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedInterfaceIndex = this.NetworkInterfaceSelectionBox.SelectedIndex;

            ResetTrafficInformation();
        }

        private void SpeedUnitSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetTrafficInformation();

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateFrequencyLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            double updateFrequencyTemp;
            // check if the imput is a float number
            if (Double.TryParse(this.UpdateFrequencyLabel.Text, out updateFrequencyTemp))
            {
                // check if the input is in range
                if (updateFrequencyTemp < minFrequency)
                {
                    MessageBox.Show(String.Format("Update frequency cannot be less then {0}", minFrequency));
                }
                else if (updateFrequencyTemp > maxFrequency)
                {
                    MessageBox.Show(String.Format("Update frequency cannot be larger theb {0}", maxFrequency));
                }
                else
                {
                    // the input is in range
                    updateFrequency = updateFrequencyTemp;
                    this.UpdateFrequencySlider.Value = updateFrequency;
                }
            }
            else
            {
                // check if the cell empty
                if (this.UpdateFrequencyLabel.Text != "")
                {
                    MessageBox.Show("Please input a decimal number");
                    this.UpdateFrequencyLabel.Text = "1";
                }
            }
        }

        private void AutoRunCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            autoStartChecked = true;
        }

        private void AutoRunCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            autoStartChecked = false;
        }

        private void DisplaySelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DisplaySelectionBox.SelectedIndex == 1 || this.DisplaySelectionBox.SelectedIndex == 2)
            {
                MessageBox.Show("Taskbar display is not available yet");
                this.DisplaySelectionBox.SelectedIndex = 0;
            }
        }

        private void WindowSizeSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
