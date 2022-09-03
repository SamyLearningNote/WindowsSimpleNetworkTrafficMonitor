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

        int loadedLanguageIndex = 0;
        string loadedInterfaceName = "";
        int loadedInterfaceIndex = 0;
        double loadedUpdateFrequency = 1;
        int loadedSpeedUnitIndex = 0;
        string loadedSpeedUnit = "";
        int loadedDisplayMethodIndex = 0;
        int loadedDisplaySizeIndex = 0;
        int loadedDarkThemeIndex = 0;
        bool loadedAutoStartCheck = true;

        bool needResetTraffic = true;

        string executableConfigName;
        string configFileName;

        bool sameProgramRunning = false;

        // variables for recording the position
        double xPos;
        double yPos;
        bool canRecordPos = true;

        public MainWindow()
        {
            // set the floating window always on top
            this.Topmost = true;

            executableConfigName = commonSet.configurationExecutableName;
            configFileName = commonSet.configFileName;

            InitializeComponent();

            // check if the background process is running, if no, show message and end the program
            if (!commonSet.CheckIfRunning(commonSet.baseProgramProcessName))
            {            
                MessageBox.Show("Please run \"StartProcess.exe\" and use it to access the Floating Window.");
                this.Close();
            }

            // check if any same programm is running
            if (commonSet.CheckIfRunningSameProcess(commonSet.floatingWindowProcessName))
            {
                MessageBox.Show("Floating Window is opened already.");
                this.Close();
            }

            // check if the program name is changed
            if (commonSet.CheckIfNameChanged(commonSet.floatingWindowProcessName))
            {
                MessageBox.Show("Executable file name changed or corrupted.\nPlease download the program again.");
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

            // apply auto start setting
            commonSet.ApplyAutoStartSetting(loadedAutoStartCheck);

            // resize window
            ResizeWindow();

            // Get Traffic information
            GetTrafficInformation();

            // Load and set the position of floating window
            LoadPosition();

            // Save position of floating window
            SavePosition();

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
                                loadedInterfaceIndex);


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

        private void SavePosition()
        {
            double xPosTemp = -100000.0;
            double yPosTemp = -100000.0;
            Task.Factory.StartNew(() => {

                // try to access the position file
                try
                {
                    while (true)
                    {
                        this.Dispatcher.Invoke(() => {
                            // record the position of this window
                            xPos = Application.Current.MainWindow.Left;
                            yPos = Application.Current.MainWindow.Top;

                            // check if the position is changed or not
                            if (xPos != xPosTemp && yPos != yPosTemp)
                            {
                                if (!commonSet.SaveFloatingWindowPosition(xPos, yPos))
                                {
                                    canRecordPos = false;
                                }
                            }

                            xPosTemp = xPos;
                            yPosTemp = yPos;

                        });
                        Thread.Sleep(500);
                    }
                }
                catch
                {
                    canRecordPos = false;
                }
                if (!canRecordPos)
                {
                    if (loadedLanguageIndex == 1)
                    {
                        MessageBox.Show("無法儲存懸浮窗的坐標。\n請重新下載本程式。");
                    }
                    else if (loadedLanguageIndex == 2)
                    {
                        MessageBox.Show("フローティング・ウィンドウの位置は保存できない。\nもう一度プログラムをダウンロードしてください。");
                    }
                    else
                    {                                             
                        // if the position cannot be recorded, show error message and end the record loop
                        MessageBox.Show("The position of the floating window cannot be saved.\nPlease download the program again.");                                               
                    }                    
                }
            });
        }

        private void LoadPosition()
        {
            double xLoadedPos;
            double yLoadedPos;
            try
            {
                // try to load the position of floating window
                using (System.IO.StreamReader file = new System.IO.StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + commonSet.floatingPositionFileName))
                {
                    if (!Double.TryParse(file.ReadLine(), out xLoadedPos))
                    {
                        // if the position cannot be converted to double, there is a format error, throw exception
                        throw new Exception();
                    }
                    if (!Double.TryParse(file.ReadLine(), out yLoadedPos))
                    {
                        // if the position cannot be converted to double, there is a format error, throw exception
                        throw new Exception();
                    }

                }
                // set the position of floating window as loaded position
                this.Left = xLoadedPos;
                this.Top = yLoadedPos;
            }
            catch
            {
                if (loadedLanguageIndex == 1)
                {
                    MessageBox.Show("懸浮窗的坐標系統可能存在一些問題。\n請重新下載本程式。");
                }
                else if (loadedLanguageIndex == 2)
                {
                    MessageBox.Show("フローティング・ウィンドウの位置に問題がある可能性がある。\nもう一度プログラムをダウンロードしてください。");
                }
                else
                {
                    // tell the user that the position recording may not be working well
                    MessageBox.Show("There may have some problem with the position of floating window.\nPlease download the program again.");
                }                
            }
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
                        Thread.Sleep(1000);
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
                loadedLanguageIndex = commonSet.loadedLanguageIndex;
                loadedInterfaceName = commonSet.loadedInterfaceName;
                if (commonSet.GetInterfaceIndexWithName(loadedInterfaceName) != -1)
                {
                    loadedInterfaceIndex = commonSet.GetInterfaceIndexWithName(loadedInterfaceName);
                }
                else
                {
                    // set default index if the interfaced cannot be found
                    loadedInterfaceIndex = 0;
                }
                loadedUpdateFrequency = commonSet.loadedUpdateFrequency;
                loadedSpeedUnitIndex = commonSet.loadedSpeedUnitIndex;
                loadedSpeedUnit = commonSet.loadedSpeedUnit;
                loadedDisplayMethodIndex = commonSet.loadedDisplayMethodIndex;
                loadedDisplaySizeIndex = commonSet.loadedDisplaySizeIndex;
                loadedDarkThemeIndex = commonSet.loadedDarkThemeIndex;
                loadedAutoStartCheck = commonSet.loadedAutoStartCheck;

                // set "loading" for different languages
                if (commonSet.loadedLanguageIndex == 0)
                {
                    this.UploadTrafficTextBlock.Text = "Loading...";
                    this.DownloadTrafficTextBlock.Text = "Loading...";
                }
                else if (commonSet.loadedLanguageIndex == 1)
                {
                    // for Chinese
                    this.UploadTrafficTextBlock.Text = "加載中...";
                    this.DownloadTrafficTextBlock.Text = "加載中...";
                }
                else if (commonSet.loadedLanguageIndex == 2)
                {
                    // *** please add Japanese version
                    this.UploadTrafficTextBlock.Text = "ローディング...";
                    this.DownloadTrafficTextBlock.Text = "ローディング...";
                }
            }
            else
            {
                // if the cannot read the configuration file successfully, show error message
                MessageBox.Show("Cannot find the configuration file or the configuration file corrupted.\nPlease try to apply default setting to correct it.");
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
