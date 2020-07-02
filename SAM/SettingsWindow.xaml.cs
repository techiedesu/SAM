using IWshRuntimeLibrary;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using SAM.Services;
using static System.Windows.Media.ColorConverter;

namespace SAM
{
    /// <summary>
    /// Interaction logic for settings window. 
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public int AutoAccIdx { get; set; }

        public int AccountsPerRow
        {
            get
            {
                if (!Regex.IsMatch(accountsPerRowSpinBox.Text, @"^\d+$") || Int32.Parse(accountsPerRowSpinBox.Text) < 1)
                    return 1;
                else
                    return Int32.Parse(accountsPerRowSpinBox.Text);
            }
            set
            {
                accountsPerRowSpinBox.Text = value.ToString();
            }
        }

        public int buttonSize { get; set; }

        public string Password { get; set; }

        public bool Decrypt { get; set; }

        private SamSettings settings;

        private string SAMshortcut = @"\SAM.lnk";
        private string SAMexe = @"\SAM.exe";
        public SettingsWindow()
        {
            settings = new SamSettings();

            InitializeComponent();

            this.Loaded += new RoutedEventHandler(SettingsWindow_Loaded);
            this.Decrypt = false;

            InputMethodSelectBox.ItemsSource = Enum.GetValues(typeof(VirtualInputMethod)).Cast<VirtualInputMethod>();
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(SamSettings.FileName))
            {
                // General
                accountsPerRowSpinBox.Text = settings.FileService.Read(SamSettings.AccountsPerRow, SamSettings.SectionGeneral);
                sleepTimeSpinBox.Text = settings.FileService.Read(SamSettings.SleepTime, SamSettings.SectionGeneral);
                startupCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.StartWithWindows, SamSettings.SectionGeneral));
                startupMinCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.StartMinimized, SamSettings.SectionGeneral));
                minimizeToTrayCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.MinimizeToTray, SamSettings.SectionGeneral));
                passwordProtectCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.PasswordProtect, SamSettings.SectionGeneral));
                rememberLoginPasswordCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.RememberPassword, SamSettings.SectionGeneral));
                clearUserDataCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.ClearUserData, SamSettings.SectionGeneral));
                HideAddButtonCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.HideAddButton, SamSettings.SectionGeneral));
                CheckForUpdatesCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CheckForUpdates, SamSettings.SectionGeneral));
                CloseOnLoginCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CloseOnLogin, SamSettings.SectionGeneral));
                ListViewCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.ListView, SamSettings.SectionGeneral));
                SandboxModeCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.SandboxMode, SamSettings.SectionGeneral));

                // AutoLog
                if (Convert.ToBoolean(settings.FileService.Read(SamSettings.LoginRecentAccount, SamSettings.SectionAutolog)) == true)
                {
                    mostRecentCheckBox.IsChecked = true;
                    recentAccountLabel.Text = MainWindow.encryptedAccounts[Int32.Parse(settings.FileService.Read(SamSettings.RecentAccountIndex, SamSettings.SectionAutolog))].Name;
                }
                else if (Convert.ToBoolean(settings.FileService.Read(SamSettings.LoginSelectedAccount, SamSettings.SectionAutolog)) == true)
                {
                    selectedAccountCheckBox.IsChecked = true;
                    selectedAccountLabel.Text = MainWindow.encryptedAccounts[Int32.Parse(settings.FileService.Read(SamSettings.SelectedAccountIndex, SamSettings.SectionAutolog))].Name;
                }
                InputMethodSelectBox.SelectedItem = (VirtualInputMethod)Enum.Parse(typeof(VirtualInputMethod), settings.FileService.Read(SamSettings.InputMethod, SamSettings.SectionAutolog));
                HandleImeCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.HandleIme, SamSettings.SectionAutolog));
                SteamGuardOnlyCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.Ime2FaOnly, SamSettings.SectionAutolog));

                // Customize
                ThemeSelectBox.Text = settings.FileService.Read(SamSettings.Theme, SamSettings.SectionCustomize);
                AccentSelectBox.Text = settings.FileService.Read(SamSettings.Accent, SamSettings.SectionCustomize);
                buttonSizeSpinBox.Text = settings.FileService.Read(SamSettings.ButtonSize, SamSettings.SectionCustomize);
                ButtonColorPicker.SelectedColor = (Color)ConvertFromString(settings.FileService.Read(SamSettings.ButtonColor, SamSettings.SectionCustomize));
                ButtonFontSizeSpinBox.Text = settings.FileService.Read(SamSettings.ButtonFontSize, SamSettings.SectionCustomize);
                ButtonFontColorPicker.SelectedColor = (Color)ConvertFromString(settings.FileService.Read(SamSettings.ButtonFontColor, SamSettings.SectionCustomize));
                BannerColorPicker.SelectedColor = (Color)ConvertFromString(settings.FileService.Read(SamSettings.ButtonBannerColor, SamSettings.SectionCustomize));
                BannerFontSizeSpinBox.Text = settings.FileService.Read(SamSettings.ButtonBannerFontSize, SamSettings.SectionCustomize);
                BannerFontColorPicker.SelectedColor = (Color)ConvertFromString(settings.FileService.Read(SamSettings.ButtonBannerFontColor, SamSettings.SectionCustomize));
                HideBanIconsCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.HideBanIcons, SamSettings.SectionCustomize));

                // Steam
                SteamPathTextBox.Text = settings.FileService.Read(SamSettings.SteamPath, SamSettings.SectionSteam);
                ApiKeyTextBox.Text = settings.FileService.Read(SamSettings.SteamApiKey, SamSettings.SectionSteam);
                AutoReloadCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.AutoReloadEnabled, SamSettings.SectionSteam));
                AutoReloadIntervalSpinBox.Text = settings.FileService.Read(SamSettings.AutoReloadInterval, SamSettings.SectionSteam);

                // Parameters
                CafeAppLaunchCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CafeAppLaunchParameter, SamSettings.SectionParameters));
                ClearBetaCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.ClearBetaParameter, SamSettings.SectionParameters));
                ConsoleCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.ConsoleParameter, SamSettings.SectionParameters));
                LoginCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.LoginParameter, SamSettings.SectionParameters));
                DeveloperCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.DeveloperParameter, SamSettings.SectionParameters));
                ForceServiceCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.ForceServiceParameter, SamSettings.SectionParameters));
                ConsoleCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.ForceServiceParameter, SamSettings.SectionParameters));
                NoCacheCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.ForceServiceParameter, SamSettings.SectionParameters));
                NoVerifyFilesCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.NoVerifyFilesParameter, SamSettings.SectionParameters));
                SilentCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.SilentParameter, SamSettings.SectionParameters));
                SingleCoreCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.SingleCoreParameter, SamSettings.SectionParameters));
                TcpCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.TcpParameter, SamSettings.SectionParameters));
                TenFootCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.TenFootParameter, SamSettings.SectionParameters));
                CustomParametersCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CustomParameters, SamSettings.SectionParameters));
                CustomParametersTextBox.Text = settings.FileService.Read(SamSettings.CustomParametersValue, SamSettings.SectionParameters);
            }
        }

        private void SaveSettings(string apr)
        {
            settings.FileService = new IniFileService(SamSettings.FileName);

            if (passwordProtectCheckBox.IsChecked == true && !Convert.ToBoolean(settings.FileService.Read(SamSettings.PasswordProtect, SamSettings.SectionGeneral)))
            {
                var passwordDialog = new PasswordWindow();

                if (passwordDialog.ShowDialog() == true && passwordDialog.PasswordText != "")
                {
                    Password = passwordDialog.PasswordText;
                    settings.FileService.Write(SamSettings.PasswordProtect, true.ToString(), SamSettings.SectionGeneral);
                }
                else
                {
                    Password = "";
                }
            }
            else if (passwordProtectCheckBox.IsChecked == false && Convert.ToBoolean(settings.FileService.Read(SamSettings.PasswordProtect, SamSettings.SectionGeneral)))
            {
                var messageBoxResult = MessageBox.Show("Are you sure you want to decrypt your data file?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var passwordDialog = new PasswordWindow();

                    if (passwordDialog.ShowDialog() == true)
                    {
                        messageBoxResult = MessageBoxResult.OK;

                        while (messageBoxResult == MessageBoxResult.OK)
                        {
                            try
                            {
                                Utils.PasswordDeserialize("info.dat", passwordDialog.PasswordText);
                                messageBoxResult = MessageBoxResult.None;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                messageBoxResult = MessageBox.Show("Invalid Password", "Invalid", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                                if (messageBoxResult == MessageBoxResult.Cancel)
                                {
                                    passwordProtectCheckBox.IsChecked = true;
                                    return;
                                }

                                passwordDialog = new PasswordWindow();
                                passwordDialog.ShowDialog();
                            }
                        }
                    }
                }
                else
                {
                    passwordProtectCheckBox.IsChecked = true;
                    return;
                }

                settings.FileService.Write(SamSettings.PasswordProtect, false.ToString(), SamSettings.SectionGeneral);
                Password = "";
                Decrypt = true;
            }
            else if (passwordProtectCheckBox.IsChecked == false)
            {
                settings.FileService.Write(SamSettings.PasswordProtect, false.ToString(), SamSettings.SectionGeneral);
            }

            // General
            settings.FileService.Write(SamSettings.RememberPassword, rememberLoginPasswordCheckBox.IsChecked.ToString(), SamSettings.SectionGeneral);
            settings.FileService.Write(SamSettings.ClearUserData, clearUserDataCheckBox.IsChecked.ToString(), SamSettings.SectionGeneral);
            settings.FileService.Write(SamSettings.AccountsPerRow, apr, SamSettings.SectionGeneral);
            settings.FileService.Write(SamSettings.SleepTime, sleepTimeSpinBox.Text, SamSettings.SectionGeneral);

            string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + SAMshortcut;
            if (startupCheckBox.IsChecked == true)
            {
                settings.FileService.Write(SamSettings.StartWithWindows, true.ToString(), SamSettings.SectionGeneral);

                var shell = new WshShell();
                var currentAssemblyLocation = Assembly.GetEntryAssembly()!.Location;

                var shortcut = (IWshShortcut) shell.CreateShortcut(shortcutAddress);
                shortcut.Description = "Start with windows shortcut for SAM.";
                shortcut.TargetPath = Path.Combine(Path.GetDirectoryName(currentAssemblyLocation)!, SAMexe);
                shortcut.WorkingDirectory = Path.GetDirectoryName(currentAssemblyLocation);
                shortcut.Save();
            }   
            else
            {
                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + SAMshortcut;

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                settings.FileService.Write(SamSettings.StartWithWindows, false.ToString(), SamSettings.SectionGeneral);
            }

            settings.FileService.Write(SamSettings.StartMinimized, startupMinCheckBox.IsChecked.ToString(), SamSettings.SectionGeneral);
            settings.FileService.Write(SamSettings.MinimizeToTray, minimizeToTrayCheckBox.IsChecked.ToString(), SamSettings.SectionGeneral);
            settings.FileService.Write(SamSettings.HideAddButton, HideAddButtonCheckBox.IsChecked.ToString(), SamSettings.SectionGeneral);
            settings.FileService.Write(SamSettings.CheckForUpdates, CheckForUpdatesCheckBox.IsChecked.ToString(), SamSettings.SectionGeneral);
            settings.FileService.Write(SamSettings.CloseOnLogin, CloseOnLoginCheckBox.IsChecked.ToString(), SamSettings.SectionGeneral);
            settings.FileService.Write(SamSettings.ListView, ListViewCheckBox.IsChecked.ToString(), SamSettings.SectionGeneral);
            settings.FileService.Write(SamSettings.SandboxMode, SandboxModeCheckBox.IsChecked.ToString(), SamSettings.SectionGeneral);

            // Customize
            settings.FileService.Write(SamSettings.Theme, ThemeSelectBox.Text, SamSettings.SectionCustomize);
            settings.FileService.Write(SamSettings.Accent, AccentSelectBox.Text, SamSettings.SectionCustomize);
            settings.FileService.Write(SamSettings.ButtonSize, buttonSizeSpinBox.Text, SamSettings.SectionCustomize);
            settings.FileService.Write(SamSettings.ButtonColor, new ColorConverter().ConvertToString(ButtonColorPicker.SelectedColor), SamSettings.SectionCustomize);
            settings.FileService.Write(SamSettings.ButtonFontSize, ButtonFontSizeSpinBox.Text, SamSettings.SectionCustomize);
            settings.FileService.Write(SamSettings.ButtonFontColor, new ColorConverter().ConvertToString(ButtonFontColorPicker.SelectedColor), SamSettings.SectionCustomize);
            settings.FileService.Write(SamSettings.ButtonBannerColor, new ColorConverter().ConvertToString(BannerColorPicker.SelectedColor), SamSettings.SectionCustomize);
            settings.FileService.Write(SamSettings.ButtonBannerFontSize, BannerFontSizeSpinBox.Text, SamSettings.SectionCustomize);
            settings.FileService.Write(SamSettings.ButtonBannerFontColor, new ColorConverter().ConvertToString(BannerFontColorPicker.SelectedColor), SamSettings.SectionCustomize);
            settings.FileService.Write(SamSettings.HideBanIcons, HideBanIconsCheckBox.IsChecked.ToString(), SamSettings.SectionCustomize);

            // AutoLog
            settings.FileService.Write(SamSettings.LoginRecentAccount, mostRecentCheckBox.IsChecked.ToString(), SamSettings.SectionAutolog);
            settings.FileService.Write(SamSettings.LoginSelectedAccount, selectedAccountCheckBox.IsChecked.ToString(), SamSettings.SectionAutolog);
            settings.FileService.Write(SamSettings.InputMethod, InputMethodSelectBox.SelectedItem.ToString(), SamSettings.SectionAutolog);
            settings.FileService.Write(SamSettings.HandleIme, HandleImeCheckBox.IsChecked.ToString(), SamSettings.SectionAutolog);
            settings.FileService.Write(SamSettings.Ime2FaOnly, SteamGuardOnlyCheckBox.IsChecked.ToString(), SamSettings.SectionAutolog);

            // Steam
            settings.FileService.Write(SamSettings.SteamPath, SteamPathTextBox.Text, SamSettings.SectionSteam);
            settings.FileService.Write(SamSettings.SteamApiKey, Regex.Replace(ApiKeyTextBox.Text, @"\s+", string.Empty), SamSettings.SectionSteam);
            settings.FileService.Write(SamSettings.AutoReloadEnabled, AutoReloadCheckBox.IsChecked.ToString(), SamSettings.SectionSteam);
            settings.FileService.Write(SamSettings.AutoReloadInterval, AutoReloadIntervalSpinBox.Text, SamSettings.SectionSteam);

            // Parameters
            settings.FileService.Write(SamSettings.CafeAppLaunchParameter, CafeAppLaunchCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.ClearBetaParameter, ClearBetaCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.ConsoleParameter, ConsoleCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.LoginParameter, LoginCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.DeveloperParameter, DeveloperCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.ForceServiceParameter, ForceServiceCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.NoCacheParameter, NoCacheCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.NoVerifyFilesParameter, NoVerifyFilesCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.SilentParameter, SilentCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.SingleCoreParameter, SingleCoreCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.TcpParameter, TcpCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.TenFootParameter, TenFootCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.CustomParameters, CustomParametersCheckBox.IsChecked.ToString(), SamSettings.SectionParameters);
            settings.FileService.Write(SamSettings.CustomParametersValue, CustomParametersTextBox.Text, SamSettings.SectionParameters);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(accountsPerRowSpinBox.Text, @"^\d+$") || Int32.Parse(accountsPerRowSpinBox.Text) < 1)
                SaveSettings("1");
            else
                SaveSettings(accountsPerRowSpinBox.Text);

            Close();
        }

        private void AutologRecentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var idx = int.Parse(settings.FileService.Read(SamSettings.RecentAccountIndex, SamSettings.SectionAutolog));

                // If index is invalid, uncheck box.
                if (idx < 0)
                    mostRecentCheckBox.IsChecked = false;
                else
                {
                    AutoAccIdx = idx;
                    recentAccountLabel.Text = MainWindow.encryptedAccounts[idx].Name;
                    selectedAccountCheckBox.IsChecked = false;
                    selectedAccountLabel.Text = "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                mostRecentCheckBox.IsChecked = false;
            }
        }

        private void AutologRecentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            recentAccountLabel.Text = "";
        }

        private void SelectedAccountLabel_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: possible duplicate?
                var selectedEnabled = Convert.ToBoolean(settings.FileService.Read(SamSettings.LoginSelectedAccount, SamSettings.SectionAutolog));
                var idx = int.Parse(settings.FileService.Read(SamSettings.SelectedAccountIndex, SamSettings.SectionAutolog));

                if (selectedEnabled == false || idx < 0)
                {
                    selectedAccountCheckBox.IsChecked = false;
                }
                else
                {
                    mostRecentCheckBox.IsChecked = false;
                    recentAccountLabel.Text = "";
                    AutoAccIdx = idx;
                    selectedAccountLabel.Text = MainWindow.encryptedAccounts[idx].Name;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                selectedAccountCheckBox.IsChecked = false;
            }
        }

        private void SelectedAccountLabel_Unchecked(object sender, RoutedEventArgs e)
        {
            selectedAccountLabel.Text = "";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void ChangePathButton_Click(object sender, RoutedEventArgs e)
        {

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".exe",
                Filter = "Steam (*.exe)|*.exe",
                InitialDirectory = Environment.SpecialFolder.MyComputer.ToString()
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file path
            if (result == true)
            {
                // Prompt user to find steam install
                // Save path to settings file.
                var path = Path.GetDirectoryName(dlg.FileName) + "\\";
                SteamPathTextBox.Text = path;
            }
        }

        private void AutoPathButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SteamPathTextBox.Text = Utils.CheckSteamPath();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            clearUserDataCheckBox.IsChecked = settings.Default.ClearUserData;
            rememberLoginPasswordCheckBox.IsChecked = settings.Default.RememberPassword;
            startupCheckBox.IsChecked = settings.Default.StartWithWindows;
            startupMinCheckBox.IsChecked = settings.Default.StartMinimized;
            minimizeToTrayCheckBox.IsChecked = settings.Default.MinimizeToTray;
            accountsPerRowSpinBox.Text = settings.Default.AccountsPerRow.ToString();
            sleepTimeSpinBox.Text = settings.Default.SleepTime.ToString();
            CheckForUpdatesCheckBox.IsChecked = settings.Default.CheckForUpdates;
            CloseOnLoginCheckBox.IsChecked = settings.Default.CloseOnLogin;
            ListViewCheckBox.IsChecked = settings.Default.ListView;
            SandboxModeCheckBox.IsChecked = settings.Default.SandboxMode;

            // Ignore password protect checkbox.
            //passwordProtectCheckBox.IsChecked = settings.Default.PasswordProtect;

            mostRecentCheckBox.IsChecked = settings.Default.LoginRecentAccount;
            selectedAccountCheckBox.IsChecked = settings.Default.LoginSelectedAccount;
            InputMethodSelectBox.SelectedItem = settings.Default.VirtualInputMethod;
            HandleImeCheckBox.IsChecked = settings.Default.HandleMicrosoftIME;
            SteamGuardOnlyCheckBox.IsChecked = settings.Default.IME2FAOnly;
            
            SteamPathTextBox.Text = Utils.CheckSteamPath();
            ApiKeyTextBox.Text = settings.Default.ApiKey;
            AutoReloadCheckBox.IsChecked = settings.Default.AutoReloadEnabled;
            AutoReloadIntervalSpinBox.Text = settings.Default.AutoReloadInterval.ToString();

            ThemeSelectBox.Text = settings.Default.Theme;
            AccentSelectBox.Text = settings.Default.Accent;
            buttonSizeSpinBox.Text = settings.Default.ButtonSize.ToString();
            ButtonColorPicker.SelectedColor = (Color) ConvertFromString(settings.Default.ButtonColor);
            ButtonFontSizeSpinBox.Text = settings.Default.ButtonFontSize.ToString();
            ButtonFontColorPicker.SelectedColor = (Color) ConvertFromString(settings.Default.ButtonFontColor);
            BannerColorPicker.SelectedColor = (Color) ConvertFromString(settings.Default.ButtonBannerColor);
            BannerFontSizeSpinBox.Text = settings.Default.BannerFontSize.ToString();
            BannerFontColorPicker.SelectedColor = (Color) ConvertFromString(settings.Default.BannerFontColor);
            HideBanIconsCheckBox.IsChecked = settings.Default.HideBanIcons;

            CafeAppLaunchCheckBox.IsChecked = settings.Default.CafeAppLaunch;
            ClearBetaCheckBox.IsChecked = settings.Default.ClearBeta;
            ConsoleCheckBox.IsChecked = settings.Default.Console;
            DeveloperCheckBox.IsChecked = settings.Default.Developer;
            ForceServiceCheckBox.IsChecked = settings.Default.ForceService;
            LoginCheckBox.IsChecked = settings.Default.Login;
            NoCacheCheckBox.IsChecked = settings.Default.NoCache;
            NoVerifyFilesCheckBox.IsChecked = settings.Default.NoVerifyFiles;
            SilentCheckBox.IsChecked = settings.Default.Silent;
            SingleCoreCheckBox.IsChecked = settings.Default.SingleCore;
            TcpCheckBox.IsChecked = settings.Default.TCP;
            TenFootCheckBox.IsChecked = settings.Default.TenFoot;
            CustomParametersCheckBox.IsChecked = settings.Default.CustomParameters;
            CustomParametersTextBox.Text = settings.Default.CustomParametersValue;
        }

        private void CustomParametersCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CustomParametersTextBox.IsEnabled = true;
        }

        private void CustomParametersCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CustomParametersTextBox.IsEnabled = false;
        }

        private void ApiKeyHelpButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://steamcommunity.com/dev/apikey");
        }

        private void ApiKeyTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (ApiKeyTextBox.Text.Length == 32)
            {
                AutoReloadCheckBox.IsEnabled = true;
            }
            else
            {
                AutoReloadCheckBox.IsEnabled = false;
                AutoReloadCheckBox.IsChecked = false;
            }
        }

        private void AutoReloadCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AutoReloadIntervalSpinBox.IsEnabled = true;
        }

        private void AutoReloadCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AutoReloadIntervalSpinBox.IsEnabled = false;
        }

        private void CustomParamsHelpButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://developer.valvesoftware.com/wiki/Command_Line_Options#Steam_.28Windows.29");
        }

        private void HandleImeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SteamGuardOnlyCheckBox.IsEnabled = true;
        }

        private void HandleImeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SteamGuardOnlyCheckBox.IsEnabled = false;
            SteamGuardOnlyCheckBox.IsChecked = false;
        }
    }
}