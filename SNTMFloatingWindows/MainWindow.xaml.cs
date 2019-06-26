using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// import the library of network
using System.Net.NetworkInformation;

// import library for delay
using System.Threading;

namespace FloatingWindows
{
    /// <summary>
    /// PageFunction1.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        CommonSet commonSet = new CommonSet();
        
        NetworkInterface[] interfaces;

        int loadedInterfaceIndex = 0;
        double loadedUpdateFrequency = 1;
        int loadedSpeedUnitIndex = 0;
        string loadedSpeedUnit = "";
        bool loadedAutoStartCheck = true;
        int loadedDisplayMethodIndex = 0;
        int loadedDisplaySizeIndex = 0;

        bool needResetTraffic = true;

        string executableConfigName;
        string configFileName;

        bool sameProgramRunning = false;

        public MainWindow()
        {
            // set the floating window always on top
            this.Topmost = true;

            executableConfigName = commonSet.configurationName;
            configFileName = commonSet.configFileName;

            InitializeComponent();

            // check if the background process is running, if no, show message and end the program
            if (!commonSet.CheckIfRunning(commonSet.baseProgramProcessName))
            {
                MessageBox.Show("Please run \"StartProcess.exe\" and use it to access the Floating Window");
                this.Close();
            }

            // check if any same programm is running
            if (commonSet.CheckIfRunningSameProcess(commonSet.floatingWindowProcessName))
            {
                MessageBox.Show("Floating Window is opened already");
                this.Close();
            }

            // check if the program name is changed
            if (commonSet.CheckIfNameChanged(commonSet.floatingWindowProcessName))
            {
                MessageBox.Show("Executable file name changed or corrupted\nPlease download the program again");
                this.Close();
            }

            // do not show in taskbar
            this.ShowInTaskbar = false;

            // init common set
            commonSet.InitCommonSet();

            // get Interface information
            GetInterfaceInformation();

            // load setting
            LoadSetting();

            // resize window
            ResizeWindow();

            // Get Traffic information
            GetTrafficInformation();

            // check minized
            CheckMinimized();
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
                    if (loadedInterfaceIndex > -1)
                    {

                        // get the control of the object
                        this.Dispatcher.Invoke(() => {

                            // get data from common data
                            commonSet.GetTrafficInformation(loadedSpeedUnitIndex,
                                loadedInterfaceIndex, loadedUpdateFrequency);


                            if (!needResetTraffic)
                            {
                                // update the traffic value of the object
                                this.UploadTrafficTextBlock.Text = String.Format("{0:F2} "
                                    + loadedSpeedUnit, commonSet.uploadTrafficValue);

                                this.DownloadTrafficTextBlock.Text = String.Format("{0:F2} "
                                    + loadedSpeedUnit, commonSet.downloadTrafficValue);
                            }
                            else
                            {
                                needResetTraffic = false;
                            }

                            /*
                            // update the current value as previous value
                            previousUploadValue = uploadTrafficValueTemp;
                            previousDownloadValue = downloadTrafficValueTemp;*/
                        });
                    }
                    Thread.Sleep(Convert.ToInt32(loadedUpdateFrequency * 1000));
                }
            });
        }

        private void CheckMinimized()
        {
            Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        this.Dispatcher.Invoke(() =>
                        {

                            // check if the floating window is minimized, if so, close the window
                            if (this.WindowState == WindowState.Minimized)
                            {
                                this.Close();
                            }
                        });
                        Thread.Sleep(100);
                    }

                }
            );
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

        public void ApplyNewSetting()
        {
            this.UploadTrafficTextBlock.Text = "Loading";
            this.DownloadTrafficTextBlock.Text = "Loading";
            needResetTraffic = true;
            LoadSetting();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            //this.WindowState = WindowState.Minimized;
        }

        private void OnTopButton_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
        }

        private void ResizeWindow()
        {
            // check for the size option
            if (loadedDisplaySizeIndex == 0)
            {
                // resize to small size
                this.Height = 40;
                this.Width = 116;
                this.OuterBorder.Margin = new Thickness(0, 0, 0, -0.2);
                this.OuterContainer.Height = 40;
                this.OuterContainer.Margin = new Thickness(-4.8, -4.8, -6, -4.8);
                this.ButtonStack.Height = 40;
                this.ButtonStack.Width = 27;
                this.ButtonStack.Margin = new Thickness(0, -1, 0.4, 1);
                this.MinimizeButton.Height = 15;
                this.MinimizeButton.Margin = new Thickness(3, 5, 3, 2);
                this.OnTopButton.Height = 15;
                this.OnTopButton.Margin = new Thickness(3, 0, 3, 2);
                this.OuterTextBlock.Width = 87;
                this.OuterTextBlock.Margin = new Thickness(3, 3, 0, 3);
                this.UploadSpeedRow.Margin = new Thickness(2, 1, 0, 0);
                this.UploadSpeedLabelCol.Width = new GridLength(15);
                this.UploadLabel.FontSize = 11.5;
                this.UploadTrafficTextBlock.FontSize = 11.5;
                this.DownloadSpeedLabelCol.Width = new GridLength(15);
                this.DownloadLabel.FontSize = 11.5;
                this.DownloadTrafficTextBlock.FontSize = 11.5;
            }
            else if (loadedDisplaySizeIndex == 1)
            {
                // resize for median size
                this.Height = 60;
                this.Width = 167;
                this.OuterBorder.Margin = new Thickness(0, 0, 0, 0);
                this.OuterContainer.Height = 55;
                this.OuterContainer.Margin = new Thickness(-4.8, -2.8, -2.4, -2.6);
                this.ButtonStack.Height = 62;
                this.ButtonStack.Width = 28;
                this.ButtonStack.Margin = new Thickness(0, -4, -1, -2.8);
                this.ButtonStack.Width = 26;
                this.MinimizeButton.Height = 21;
                this.MinimizeButton.Margin = new Thickness(2, 10, 2, 0);
                this.OnTopButton.Height = 21;
                this.OnTopButton.Margin = new Thickness(2, 3, 2, 2);
                this.OuterTextBlock.Width = 133;
                this.OuterTextBlock.Margin = new Thickness(5, 3, 0, 1);
                this.UploadSpeedRow.Margin = new Thickness(2, 2, 0, 0);
                this.UploadSpeedLabelCol.Width = new GridLength(25);
                this.UploadLabel.FontSize = 18;
                this.UploadTrafficTextBlock.FontSize = 18;
                this.DownloadSpeedLabelCol.Width = new GridLength(25);
                this.DownloadLabel.FontSize = 18;
                this.DownloadTrafficTextBlock.FontSize = 18;
            }
            /*
            // set the logo of button
            Image img = new Image();
            Image img2 = new Image();
            try
            {
                // set image for minimize button
                img.Source = new BitmapImage(new Uri("minimize.png"));
                StackPanel stackPnl = new StackPanel();
                stackPnl.Orientation = Orientation.Horizontal;
                //stackPnl.Margin = new Thickness(10);
                stackPnl.Children.Add(img);

                this.MinimizeButton.Content = stackPnl;

                // set image for always on top button
                img2.Source = new BitmapImage(new Uri("pin.png"));
                StackPanel stackPnl2 = new StackPanel();
                stackPnl2.Orientation = Orientation.Horizontal;
                //stackPnl.Margin = new Thickness(10);
                stackPnl2.Children.Add(img2);

                this.OnTopButton.Content = stackPnl2;

            }
            catch
            {
                MessageBox.Show("Image files corrupted or cannot be accessed\nPlease download the program again");
            }*/
        }
    }
}
