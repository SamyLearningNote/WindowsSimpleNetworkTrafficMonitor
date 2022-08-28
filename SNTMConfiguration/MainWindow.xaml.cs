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
        bool loadedShowBubbleCheck = true;

        bool configurationReaded = false;
        bool initCompleted = false;

        double maxFrequency;
        double minFrequency;

        bool autoStartChecked;

        string executableConfigName;
        string configFileName;
        string floatingWindowName;

        CommonSet commonSet = new CommonSet();

        // variables for record the selected index
        int NetworkInterfaceIndexTemp;
        int DisplayIndexTemp;
        int SizeIndexTemp;
        int ThemeIndexTemp;

        public MainWindow()
        {
            executableConfigName = commonSet.configurationExecutableName;
            configFileName = commonSet.configFileName;
            floatingWindowName = commonSet.floatingWindowExecutableName;

            InitializeComponent();

            initCompleted = true;

            // check if the background process is running, if no, show message and end the program
            if (!commonSet.CheckIfRunning(commonSet.baseProgramProcessName))
            {
                // **** removed
                /*if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    MessageBox.Show("請運行 \"StartProcess.exe\" 後進入配置視窗。");
                }
                else
                {
                    MessageBox.Show("Please run \"StartProcess.exe\" and use it to access the Configuration Window.");
                }*/
                MessageBox.Show("Please run \"StartProcess.exe\" and use it to access the Configuration Window.");
                this.Close();
            }

            // check if the program name is changed
            if (commonSet.CheckIfNameChanged(commonSet.configurationProcessName))
            {
                // *** removed
                /*if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    MessageBox.Show("執行檔 \".exe\" 已損壞或其檔案名稱已更改。\n請重新下載整個程式。");
                }
                else
                {
                    MessageBox.Show("Executable file name changed or corrupted.\nPlease download the program again.");
                }*/
                MessageBox.Show("Executable file name changed or corrupted.\nPlease download the program again.");
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
            if (!commonSet.StartProcess(floatingWindowName))
            {
                if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    MessageBox.Show("懸浮窗模組已損壞或不存在。\n請重新下載本程式。");
                }
                // *** Japanese is added
                else if (this.LanguageSelectionBox.SelectedIndex == 2)
                {
                    MessageBox.Show("フローティング・ウィンドウのファイルが破損しているか存在しない。\nもう一度プログラムをダウンロードしてください。");
                }
                else
                {
                    MessageBox.Show("Floating Window module is corrupted or doesn't exist.\nPlease download the program again.");
                }
            }/*
            try
            {
                System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + floatingWindowName);
            }
            catch
            {
                MessageBox.Show("Floating Window file corrupted or not exist\nPlease download the program again");
            }*/
        }

        private void GetInterfaceInformation()
        {
            // get network interface information

            // check if the network is available
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return;
            }

            // the first option is monitor all the network interface
            if (this.LanguageSelectionBox.SelectedIndex == 1)
            {
                this.NetworkInterfaceSelectionBox.Items.Add("全部");
            }
            else if (this.LanguageSelectionBox.SelectedIndex == 2)
            {
                this.NetworkInterfaceSelectionBox.Items.Add("全部");
            }
            else
            {
                this.NetworkInterfaceSelectionBox.Items.Add("All");
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
                                this.NetworkInterfaceSelectionBox.SelectedIndex);



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
                    file.WriteLine(this.LanguageSelectionBox.SelectedIndex);
                    file.WriteLine(this.NetworkInterfaceSelectionBox.Text);
                    file.WriteLine(String.Format("{0:F2}", updateFrequency));
                    file.WriteLine(this.SpeedUnitSelectionBox.SelectedIndex);
                    file.WriteLine(this.SpeedUnitSelectionBox.Text);
                    file.WriteLine(this.DisplaySelectionBox.SelectedIndex);
                    file.WriteLine(this.WindowSizeSelectionBox.SelectedIndex);
                    file.WriteLine(this.DarkThemeSelectionBox.SelectedIndex);
                    file.WriteLine(this.AutoRunCheckBox.IsChecked);
                    file.WriteLine(this.ShowBubbleCheckBox.IsChecked);
                }
                // display the create success message
                if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    MessageBox.Show("已套用設定。");
                }
                else if (this.LanguageSelectionBox.SelectedIndex == 2)
                {
                    MessageBox.Show("変更が適用される。");
                }
                else
                {
                    MessageBox.Show("Changes are applied.");
                }
            }
            catch
            {
                // show erroe message if error occur
                if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    MessageBox.Show("錯誤: 無法儲存新設定。\n請檢查修改或創建檔案 \"configuration\" 的權限。");
                }
                else if (this.LanguageSelectionBox.SelectedIndex == 2)
                {
                    MessageBox.Show("錯誤：設定が保存できない。\nファイル \"configuration\" 修正や作成の許可を確認してください。");
                }
                else
                {
                    MessageBox.Show("Error occur when saving the setting file.\nPlease check the permission of writing or creation of file \"configuration\".");
                }
            }
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
                loadedShowBubbleCheck = commonSet.loadedShowBubbleCheck;


                // set the readed status to true
                configurationReaded = true;

                // set the parameter in form into loaded information
                this.LanguageSelectionBox.SelectedIndex = loadedLanguageIndex;
                this.NetworkInterfaceSelectionBox.SelectedIndex = loadedInterfaceIndex;
                this.UpdateFrequencySlider.Value = loadedUpdateFrequency;
                this.SpeedUnitSelectionBox.SelectedIndex = loadedSpeedUnitIndex;
                this.DisplaySelectionBox.SelectedIndex = loadedDisplayMethodIndex;
                this.WindowSizeSelectionBox.SelectedIndex = loadedDisplaySizeIndex;
                this.DarkThemeSelectionBox.SelectedIndex = loadedDarkThemeIndex;
                this.AutoRunCheckBox.IsChecked = loadedAutoStartCheck;
                this.ShowBubbleCheckBox.IsChecked = loadedShowBubbleCheck;

                changeLanguages();

                changeTheme();
            }
            else
            {
                if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    // if the cannot read the configuration file successfully, show error message
                    MessageBox.Show("找不到檔案 \"configuration\" 或檔案 \"configuration\" 已損壞。\n請嘗試套用預設設定來修改此問題。");
                }
                else if (this.LanguageSelectionBox.SelectedIndex == 2)
                {
                    MessageBox.Show("ファイル \"configuration\" が見つからないまたはファイル \"configuration\" が破損している。\n初期設定が適用してください。");
                }
                else
                {
                    MessageBox.Show("Cannot find the configuration file or the configuration file is corrupted.\nPlease try to apply default setting to correct it.");
                }
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
            // check if the input is in range
            double updateFrequencyTemp;
            if (Double.TryParse(this.UpdateFrequencyLabel.Text, out updateFrequencyTemp))
            {
                if (updateFrequencyTemp < minFrequency)
                {
                    if (this.LanguageSelectionBox.SelectedIndex == 1)
                    {
                        MessageBox.Show(String.Format("刷新頻率不能少於 {0}。", minFrequency));
                    }
                    else if (this.LanguageSelectionBox.SelectedIndex == 2)
                    {
                        MessageBox.Show(String.Format("更新頻度は {0} より小さくすることはできない。", minFrequency));
                    }
                    else
                    {
                        MessageBox.Show(String.Format("Update frequency cannot be less than {0}.", minFrequency));
                    }
                    UpdateFrequencySlider.Value = minFrequency;
                    UpdateFrequencyLabel.Text = minFrequency.ToString();
                    return;
                }
                else if (updateFrequencyTemp > maxFrequency)
                {
                    if (this.LanguageSelectionBox.SelectedIndex == 1)
                    {
                        MessageBox.Show(String.Format("刷新頻率不能大於 {0}。", maxFrequency));
                    }
                    else if (this.LanguageSelectionBox.SelectedIndex == 2)
                    {
                        MessageBox.Show(String.Format("更新頻度は {0} より大きくすることはできない。", maxFrequency));
                    }
                    else
                    {
                        MessageBox.Show(String.Format("Update frequency cannot be larger than {0}.", maxFrequency));
                    }
                    UpdateFrequencySlider.Value = maxFrequency;
                    UpdateFrequencyLabel.Text = maxFrequency.ToString();
                    return;
                }
            }
            else
            {
                // tell the user to input a number
                if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    MessageBox.Show("請輸入1個有效的數字。\n例如: 1.50。");
                }
                else if (this.LanguageSelectionBox.SelectedIndex == 2)
                {
                    MessageBox.Show("10進数を入力してください。\n例：1.50。");
                }
                else
                {
                    MessageBox.Show("Please input a decimal number. E.g. 1.50.");
                }
                this.UpdateFrequencyLabel.Text = "1";
                return;
            }


            // apply auto start setting
            commonSet.ApplyAutoStartSetting(autoStartChecked);

            SaveSetting();

            // change the words as selected language
            //changeLanguages();

            // change the config window to selected theme
            changeTheme();


            // check if the floating window is opened, if yes turn off the floating window
            commonSet.EndProcess(commonSet.floatingWindowProcessName);

            /* this part is done by base process
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
            */

            // restart the base process
            commonSet.EndProcess(commonSet.baseProgramProcessName);
            if (!commonSet.StartProcess(commonSet.baseProgramExecutableName)) {
                // if the base cannot be started, show error message
                if (this.LanguageSelectionBox.SelectedIndex == 0)
                {
                    // show English message
                    MessageBox.Show("Main program may be corrupted\nPlease download this program again.");
                }
                else if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    // show Chinese message
                    MessageBox.Show("主程序可能已被損毀\n請重新下載本程式");
                }
                else if (this.LanguageSelectionBox.SelectedIndex == 2)
                {
                    // show Japanese message
                    MessageBox.Show("メインプログラムが破損している可能性がある。もう一度プログラムをダウンロードしてください。\n");
                }
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
                this.ShowBubbleCheckBox.IsChecked = loadedShowBubbleCheck;
                if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    MessageBox.Show("設定已重設。");
                }
                else if (this.LanguageSelectionBox.SelectedIndex == 2)
                {
                    MessageBox.Show("設定がリセットされる。");
                }
                else
                {
                    MessageBox.Show("Configuration is resetted to the status when this window is opened.");
                }
            }
            else
            {
                // if the configuration file is not readed, show error message
                if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    MessageBox.Show("無法讀取軟件設置檔案。\n請嘗試套用預設設定並重啟本程式。");
                }
                else if (this.LanguageSelectionBox.SelectedIndex == 2)
                {
                    // ***Please Translate Again
                    MessageBox.Show("設定ファイルが読み取れない。\n初期設定が適用して、再起動してください。");
                }
                else
                {
                    MessageBox.Show("Configuration cannot be readed.\nPlease try to apply default configuration and restart the programme.");
                }
            }
        }

        private void ResetTrafficInformation()
        {
            if (this.LanguageSelectionBox.SelectedIndex == 1)
            {
                this.UploadTrafficTextBlock.Text = "加載中...";
                this.DownloadTrafficTextBlock.Text = "加載中...";
            }
            // *** Please add Japanese version
            else if (this.LanguageSelectionBox.SelectedIndex == 2)
            {
                this.UploadTrafficTextBlock.Text = "ローディング...";
                this.DownloadTrafficTextBlock.Text = "ローディング...";
            }
            else
            {
                this.UploadTrafficTextBlock.Text = "Loading...";
                this.DownloadTrafficTextBlock.Text = "Loading...";
            }
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

            // reset the language
            // this.LanguageSelectionBox.SelectedIndex = 0;

            // reset the network interface
            this.NetworkInterfaceSelectionBox.SelectedIndex = 0;

            // reset the update frequency to 0.5 second
            this.UpdateFrequencySlider.Value = 0.5;

            // reset the speed unit to MB/s
            this.SpeedUnitSelectionBox.SelectedIndex = 3;

            // reset the way to display the traffic
            this.DisplaySelectionBox.SelectedIndex = 0;

            // reset the size of floating window
            this.WindowSizeSelectionBox.SelectedIndex = 0;

            // reset the theme
            this.DarkThemeSelectionBox.SelectedIndex = 0;

            // reset the auto run
            this.AutoRunCheckBox.IsChecked = true;

            // reset show bubble message
            this.ShowBubbleCheckBox.IsChecked = true;

            // show message of reset
            if (this.LanguageSelectionBox.SelectedIndex == 1)
            {
                MessageBox.Show("已載入預設設定。");
            }
            else if (this.LanguageSelectionBox.SelectedIndex == 2)
            {
                // *** Please Translate again
                MessageBox.Show("初期設定がロードされる。");
            }
            else
            {
                MessageBox.Show("Default setting is loaded.");
            }

            ResetTrafficInformation();
        }

        public void changeLanguages()
        {
            // save the selected index
            NetworkInterfaceIndexTemp = this.NetworkInterfaceSelectionBox.SelectedIndex;
            DisplayIndexTemp = this.DisplaySelectionBox.SelectedIndex;
            SizeIndexTemp = this.WindowSizeSelectionBox.SelectedIndex;
            ThemeIndexTemp = this.DarkThemeSelectionBox.SelectedIndex;
            
            if (this.LanguageSelectionBox.SelectedIndex == 1)
            {
                this.Height = 455;
                this.Width = 415;
                this.NetworkInterfaceGrid.Height = 22;
                this.DisplayGrid.Height = 22;
                this.ConfigTextBox.Text = "設定";
                this.LanguageTextBox.Text = "語言:";
                this.NetworkInterfaceTextBox.Text = "網路介面:";
                this.UpdateFrequencyTextBox.Text = "刷新頻率:";
                this.SpeedUnitTextBox.Text = "速度單位:";
                this.DisplayTextBox.Text = "顯示模式:";
                this.WindowSizeTextBox.Text = "懸浮窗大小:";
                this.DarkThemeTextBox.Text = "深色主題背景:";
                this.AutoRunCheckBox.Content = "登入後自動啟動";
                this.PreviewTextBox.Text = "預覽";
                this.UploadTextBox.Text = "U (上載流量):";
                this.DownloadTextBox.Text = "D (下載流量):";
                this.DefaultButton.Content = "預設";
                this.ResetButton.Content = "重設";
                this.ApplyButton.Content = "套用";
                this.CancelButton.Content = "關閉";

                // Set the Chinese version of combo box
                this.DisplaySelectionBox.Items[0] = "懸浮窗";
                this.DisplaySelectionBox.Items[1] = "工具列";
                this.DisplaySelectionBox.Items[2] = "懸浮窗及工具列";
                this.DisplaySelectionBox.Items[3] = "不顯示";


                this.WindowSizeSelectionBox.Items[0] = "小";
                this.WindowSizeSelectionBox.Items[1] = "中";
                this.WindowSizeSelectionBox.Items[2] = "大";
                
                this.DarkThemeSelectionBox.Items[0] = "關閉";
                this.DarkThemeSelectionBox.Items[1] = "開啟";

                this.ShowBubbleCheckBox.Content = "顯示提示訊息";

                this.NetworkInterfaceSelectionBox.Items[0] = "全部";

            }
            else if (this.LanguageSelectionBox.SelectedIndex == 2)
            {
                this.Height = 480;
                this.Width = 440;
                this.NetworkInterfaceGrid.Height = 35;
                this.DisplayGrid.Height = 35;
                this.ConfigTextBox.Text = "設定";
                this.LanguageTextBox.Text = "言語：";
                this.NetworkInterfaceTextBox.Text = "ネットワーク\nインターフェイス：";
                this.UpdateFrequencyTextBox.Text = "更新頻度：";
                this.SpeedUnitTextBox.Text = "スピードユニット：";
                this.DisplayTextBox.Text = "ディスプレイ\nモード：";
                this.WindowSizeTextBox.Text = "ウィンドウサイズ：";
                this.DarkThemeTextBox.Text = "ダークテーマ：";
                this.AutoRunCheckBox.Content = "ログイン後に自動的に実行される";
                this.PreviewTextBox.Text = "プレビュー";
                this.UploadTextBox.Text = "U (アップロード)：";
                this.DownloadTextBox.Text = "D (ダウンロード)：";
                this.DefaultButton.Content = "初期設定";
                this.ResetButton.Content = "リセット";
                this.ApplyButton.Content = "保存";
                this.CancelButton.Content = "閉じる";


                // *** Set the Japanese version of display method combo box
                this.DisplaySelectionBox.Items[0] = "フローティング・ウィンドウ";
                this.DisplaySelectionBox.Items[1] = "タスクバー";
                this.DisplaySelectionBox.Items[2] = "フローティング・ウィンドウとタスクバー";
                this.DisplaySelectionBox.Items[3] = "表示しない";


                // *** need double check
                this.WindowSizeSelectionBox.Items[0] = "スモール";
                this.WindowSizeSelectionBox.Items[1] = "ミドル";
                this.WindowSizeSelectionBox.Items[2] = "ラージ";

                this.DarkThemeSelectionBox.Items[0] = "オフ";
                this.DarkThemeSelectionBox.Items[1] = "オン";

                // ****
                this.ShowBubbleCheckBox.Content = "通知メッセージを表示する";

                this.NetworkInterfaceSelectionBox.Items[0] = "全部";
            }
            else
            {
                this.Height = 455;
                this.Width = 415;
                this.NetworkInterfaceGrid.Height = 22;
                this.DisplayGrid.Height = 22;
                this.ConfigTextBox.Text = "Configuration";
                this.LanguageTextBox.Text = "Language:";
                this.NetworkInterfaceTextBox.Text = "Network interface:";
                this.UpdateFrequencyTextBox.Text = "Update frequency:";
                this.SpeedUnitTextBox.Text = "Speed unit:";
                this.DisplayTextBox.Text = "Display mode:";
                this.WindowSizeTextBox.Text = "Size of window:";
                this.DarkThemeTextBox.Text = "Dark theme:";
                this.AutoRunCheckBox.Content = "Run automatically after you have logged in";
                this.PreviewTextBox.Text = "Preview";
                this.UploadTextBox.Text = "U (Upload traffic):";
                this.DownloadTextBox.Text = "D (Download traffic):";
                this.DefaultButton.Content = "Default";
                this.ResetButton.Content = "Reset";
                this.ApplyButton.Content = "Apply";
                this.CancelButton.Content = "Close";

                // Set the English version of combo box
                this.DisplaySelectionBox.Items[0] = "Floating window";
                this.DisplaySelectionBox.Items[1] = "Taskbar tool";
                this.DisplaySelectionBox.Items[2] = "Window and Tool";
                this.DisplaySelectionBox.Items[3] = "Do not display";


                this.WindowSizeSelectionBox.Items[0] = "Small";
                this.WindowSizeSelectionBox.Items[1] = "Medium";
                this.WindowSizeSelectionBox.Items[2] = "Large";

                this.DarkThemeSelectionBox.Items[0] = "Off";
                this.DarkThemeSelectionBox.Items[1] = "On";

                this.ShowBubbleCheckBox.Content = "Show bubble message";

                this.NetworkInterfaceSelectionBox.Items[0] = "All";
            }

            // restore the selected index 
            this.NetworkInterfaceSelectionBox.SelectedIndex = NetworkInterfaceIndexTemp;
            this.DisplaySelectionBox.SelectedIndex = DisplayIndexTemp;
            this.WindowSizeSelectionBox.SelectedIndex = SizeIndexTemp;
            this.DarkThemeSelectionBox.SelectedIndex = ThemeIndexTemp;
        }

        public void ChangeTextOfSize()
        {
            // save the size index
            SizeIndexTemp = this.WindowSizeSelectionBox.SelectedIndex;

            if (this.WindowSizeSelectionBox.SelectedIndex == 0)
            {
                // set English
            }
            else if (this.WindowSizeSelectionBox.SelectedIndex == 1)
            {
                // Set Chinese

            }
            else if (this.WindowSizeSelectionBox.SelectedIndex == 2)
            {
                // Set Japanese

            }

            // restore the index
            this.WindowSizeSelectionBox.SelectedIndex = SizeIndexTemp;

        }

        public void LanguageSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if (initCompleted)
            {
                changeLanguages();
            }
        }

        public void changeTheme()
        {
            // change to dark theme
            if (this.DarkThemeSelectionBox.SelectedIndex == 1)
            {
                this.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                this.Background = new SolidColorBrush(Color.FromArgb(255, 50, 54, 57));
                this.AutoRunCheckBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                this.ShowBubbleCheckBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                this.DefaultButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                this.DefaultButton.Background = new SolidColorBrush(Color.FromArgb(255, 50, 54, 57));
                this.ResetButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                this.ResetButton.Background = new SolidColorBrush(Color.FromArgb(255, 50, 54, 57));
                this.ApplyButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                this.ApplyButton.Background = new SolidColorBrush(Color.FromArgb(255, 50, 54, 57));
                this.CancelButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                this.CancelButton.Background = new SolidColorBrush(Color.FromArgb(255, 50, 54, 57));
            }

            // change to default theme
            else
            {
                this.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                this.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                this.AutoRunCheckBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                this.ShowBubbleCheckBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                this.DefaultButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                this.DefaultButton.Background = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
                this.ResetButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                this.ResetButton.Background = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
                this.ApplyButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                this.ApplyButton.Background = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
                this.CancelButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                this.CancelButton.Background = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
            }
        }

        public void DarkThemeSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
                // the input is in range
                updateFrequency = updateFrequencyTemp;
                this.UpdateFrequencySlider.Value = updateFrequency;
            }
            else
            {
                // check if the cell empty
                if (this.UpdateFrequencyLabel.Text != "")
                {
                    if (this.LanguageSelectionBox.SelectedIndex == 1)
                    {
                        MessageBox.Show("請輸入1個有效的數字。\n例如: 1.50。");
                    }
                    else if (this.LanguageSelectionBox.SelectedIndex == 2)
                    {
                        MessageBox.Show("10進数を入力してください。\n例：1.50。");
                    }
                    else
                    {
                        MessageBox.Show("Please input a decimal number. E.g. 1.50.");
                    }
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
                if (this.LanguageSelectionBox.SelectedIndex == 1)
                {
                    MessageBox.Show("暫時未能在工作列上顯示。\n請期待後續更新。");
                }
                else if (this.LanguageSelectionBox.SelectedIndex == 2)
                {
                    MessageBox.Show("タスクバーの表示はまだ利用できない。");
                }
                else
                {
                    MessageBox.Show("Taskbar display is not available yet.");
                }
            }
        }

        private void WindowSizeSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
