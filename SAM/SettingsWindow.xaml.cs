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
            if (System.IO.File.Exists(SamSettings.FILE_NAME))
            {
                // General
                accountsPerRowSpinBox.Text = settings.FileService.Read(SamSettings.ACCOUNTS_PER_ROW, SamSettings.SECTION_GENERAL);
                sleepTimeSpinBox.Text = settings.FileService.Read(SamSettings.SLEEP_TIME, SamSettings.SECTION_GENERAL);
                startupCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.START_WITH_WINDOWS, SamSettings.SECTION_GENERAL));
                startupMinCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.START_MINIMIZED, SamSettings.SECTION_GENERAL));
                minimizeToTrayCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.MINIMIZE_TO_TRAY, SamSettings.SECTION_GENERAL));
                passwordProtectCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.PASSWORD_PROTECT, SamSettings.SECTION_GENERAL));
                rememberLoginPasswordCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.REMEMBER_PASSWORD, SamSettings.SECTION_GENERAL));
                clearUserDataCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CLEAR_USER_DATA, SamSettings.SECTION_GENERAL));
                HideAddButtonCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.HIDE_ADD_BUTTON, SamSettings.SECTION_GENERAL));
                CheckForUpdatesCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CHECK_FOR_UPDATES, SamSettings.SECTION_GENERAL));
                CloseOnLoginCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CLOSE_ON_LOGIN, SamSettings.SECTION_GENERAL));
                ListViewCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.LIST_VIEW, SamSettings.SECTION_GENERAL));
                SandboxModeCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.SANDBOX_MODE, SamSettings.SECTION_GENERAL));

                // AutoLog
                if (Convert.ToBoolean(settings.FileService.Read(SamSettings.LOGIN_RECENT_ACCOUNT, SamSettings.SECTION_AUTOLOG)) == true)
                {
                    mostRecentCheckBox.IsChecked = true;
                    recentAccountLabel.Text = MainWindow.encryptedAccounts[Int32.Parse(settings.FileService.Read(SamSettings.RECENT_ACCOUNT_INDEX, SamSettings.SECTION_AUTOLOG))].Name;
                }
                else if (Convert.ToBoolean(settings.FileService.Read(SamSettings.LOGIN_SELECTED_ACCOUNT, SamSettings.SECTION_AUTOLOG)) == true)
                {
                    selectedAccountCheckBox.IsChecked = true;
                    selectedAccountLabel.Text = MainWindow.encryptedAccounts[Int32.Parse(settings.FileService.Read(SamSettings.SELECTED_ACCOUNT_INDEX, SamSettings.SECTION_AUTOLOG))].Name;
                }
                InputMethodSelectBox.SelectedItem = (VirtualInputMethod)Enum.Parse(typeof(VirtualInputMethod), settings.FileService.Read(SamSettings.INPUT_METHOD, SamSettings.SECTION_AUTOLOG));
                HandleImeCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.HANDLE_IME, SamSettings.SECTION_AUTOLOG));
                SteamGuardOnlyCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.IME_2FA_ONLY, SamSettings.SECTION_AUTOLOG));

                // Customize
                ThemeSelectBox.Text = settings.FileService.Read(SamSettings.THEME, SamSettings.SECTION_CUSTOMIZE);
                AccentSelectBox.Text = settings.FileService.Read(SamSettings.ACCENT, SamSettings.SECTION_CUSTOMIZE);
                buttonSizeSpinBox.Text = settings.FileService.Read(SamSettings.BUTTON_SIZE, SamSettings.SECTION_CUSTOMIZE);
                ButtonColorPicker.SelectedColor = (Color)ConvertFromString(settings.FileService.Read(SamSettings.BUTTON_COLOR, SamSettings.SECTION_CUSTOMIZE));
                ButtonFontSizeSpinBox.Text = settings.FileService.Read(SamSettings.BUTTON_FONT_SIZE, SamSettings.SECTION_CUSTOMIZE);
                ButtonFontColorPicker.SelectedColor = (Color)ConvertFromString(settings.FileService.Read(SamSettings.BUTTON_FONT_COLOR, SamSettings.SECTION_CUSTOMIZE));
                BannerColorPicker.SelectedColor = (Color)ConvertFromString(settings.FileService.Read(SamSettings.BUTTON_BANNER_COLOR, SamSettings.SECTION_CUSTOMIZE));
                BannerFontSizeSpinBox.Text = settings.FileService.Read(SamSettings.BUTTON_BANNER_FONT_SIZE, SamSettings.SECTION_CUSTOMIZE);
                BannerFontColorPicker.SelectedColor = (Color)ConvertFromString(settings.FileService.Read(SamSettings.BUTTON_BANNER_FONT_COLOR, SamSettings.SECTION_CUSTOMIZE));
                HideBanIconsCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.HIDE_BAN_ICONS, SamSettings.SECTION_CUSTOMIZE));

                // Steam
                SteamPathTextBox.Text = settings.FileService.Read(SamSettings.STEAM_PATH, SamSettings.SECTION_STEAM);
                ApiKeyTextBox.Text = settings.FileService.Read(SamSettings.STEAM_API_KEY, SamSettings.SECTION_STEAM);
                AutoReloadCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.AUTO_RELOAD_ENABLED, SamSettings.SECTION_STEAM));
                AutoReloadIntervalSpinBox.Text = settings.FileService.Read(SamSettings.AUTO_RELOAD_INTERVAL, SamSettings.SECTION_STEAM);

                // Parameters
                CafeAppLaunchCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CAFE_APP_LAUNCH_PARAMETER, SamSettings.SECTION_PARAMETERS));
                ClearBetaCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CLEAR_BETA_PARAMETER, SamSettings.SECTION_PARAMETERS));
                ConsoleCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CONSOLE_PARAMETER, SamSettings.SECTION_PARAMETERS));
                LoginCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.LOGIN_PARAMETER, SamSettings.SECTION_PARAMETERS));
                DeveloperCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.DEVELOPER_PARAMETER, SamSettings.SECTION_PARAMETERS));
                ForceServiceCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.FORCE_SERVICE_PARAMETER, SamSettings.SECTION_PARAMETERS));
                ConsoleCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.FORCE_SERVICE_PARAMETER, SamSettings.SECTION_PARAMETERS));
                NoCacheCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.FORCE_SERVICE_PARAMETER, SamSettings.SECTION_PARAMETERS));
                NoVerifyFilesCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.NO_VERIFY_FILES_PARAMETER, SamSettings.SECTION_PARAMETERS));
                SilentCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.SILENT_PARAMETER, SamSettings.SECTION_PARAMETERS));
                SingleCoreCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.SINGLE_CORE_PARAMETER, SamSettings.SECTION_PARAMETERS));
                TcpCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.TCP_PARAMETER, SamSettings.SECTION_PARAMETERS));
                TenFootCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.TEN_FOOT_PARAMETER, SamSettings.SECTION_PARAMETERS));
                CustomParametersCheckBox.IsChecked = Convert.ToBoolean(settings.FileService.Read(SamSettings.CUSTOM_PARAMETERS, SamSettings.SECTION_PARAMETERS));
                CustomParametersTextBox.Text = settings.FileService.Read(SamSettings.CUSTOM_PARAMETERS_VALUE, SamSettings.SECTION_PARAMETERS);
            }
        }

        private void SaveSettings(string apr)
        {
            settings.FileService = new IniFileService(SamSettings.FILE_NAME);

            if (passwordProtectCheckBox.IsChecked == true && !Convert.ToBoolean(settings.FileService.Read(SamSettings.PASSWORD_PROTECT, SamSettings.SECTION_GENERAL)))
            {
                var passwordDialog = new PasswordWindow();

                if (passwordDialog.ShowDialog() == true && passwordDialog.PasswordText != "")
                {
                    Password = passwordDialog.PasswordText;
                    settings.FileService.Write(SamSettings.PASSWORD_PROTECT, true.ToString(), SamSettings.SECTION_GENERAL);
                }
                else
                {
                    Password = "";
                }
            }
            else if (passwordProtectCheckBox.IsChecked == false && Convert.ToBoolean(settings.FileService.Read(SamSettings.PASSWORD_PROTECT, SamSettings.SECTION_GENERAL)))
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

                settings.FileService.Write(SamSettings.PASSWORD_PROTECT, false.ToString(), SamSettings.SECTION_GENERAL);
                Password = "";
                Decrypt = true;
            }
            else if (passwordProtectCheckBox.IsChecked == false)
            {
                settings.FileService.Write(SamSettings.PASSWORD_PROTECT, false.ToString(), SamSettings.SECTION_GENERAL);
            }

            // General
            settings.FileService.Write(SamSettings.REMEMBER_PASSWORD, rememberLoginPasswordCheckBox.IsChecked.ToString(), SamSettings.SECTION_GENERAL);
            settings.FileService.Write(SamSettings.CLEAR_USER_DATA, clearUserDataCheckBox.IsChecked.ToString(), SamSettings.SECTION_GENERAL);
            settings.FileService.Write(SamSettings.ACCOUNTS_PER_ROW, apr, SamSettings.SECTION_GENERAL);
            settings.FileService.Write(SamSettings.SLEEP_TIME, sleepTimeSpinBox.Text, SamSettings.SECTION_GENERAL);

            string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + SAMshortcut;
            if (startupCheckBox.IsChecked == true)
            {
                settings.FileService.Write(SamSettings.START_WITH_WINDOWS, true.ToString(), SamSettings.SECTION_GENERAL);

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

                settings.FileService.Write(SamSettings.START_WITH_WINDOWS, false.ToString(), SamSettings.SECTION_GENERAL);
            }

            settings.FileService.Write(SamSettings.START_MINIMIZED, startupMinCheckBox.IsChecked.ToString(), SamSettings.SECTION_GENERAL);
            settings.FileService.Write(SamSettings.MINIMIZE_TO_TRAY, minimizeToTrayCheckBox.IsChecked.ToString(), SamSettings.SECTION_GENERAL);
            settings.FileService.Write(SamSettings.HIDE_ADD_BUTTON, HideAddButtonCheckBox.IsChecked.ToString(), SamSettings.SECTION_GENERAL);
            settings.FileService.Write(SamSettings.CHECK_FOR_UPDATES, CheckForUpdatesCheckBox.IsChecked.ToString(), SamSettings.SECTION_GENERAL);
            settings.FileService.Write(SamSettings.CLOSE_ON_LOGIN, CloseOnLoginCheckBox.IsChecked.ToString(), SamSettings.SECTION_GENERAL);
            settings.FileService.Write(SamSettings.LIST_VIEW, ListViewCheckBox.IsChecked.ToString(), SamSettings.SECTION_GENERAL);
            settings.FileService.Write(SamSettings.SANDBOX_MODE, SandboxModeCheckBox.IsChecked.ToString(), SamSettings.SECTION_GENERAL);

            // Customize
            settings.FileService.Write(SamSettings.THEME, ThemeSelectBox.Text, SamSettings.SECTION_CUSTOMIZE);
            settings.FileService.Write(SamSettings.ACCENT, AccentSelectBox.Text, SamSettings.SECTION_CUSTOMIZE);
            settings.FileService.Write(SamSettings.BUTTON_SIZE, buttonSizeSpinBox.Text, SamSettings.SECTION_CUSTOMIZE);
            settings.FileService.Write(SamSettings.BUTTON_COLOR, new ColorConverter().ConvertToString(ButtonColorPicker.SelectedColor), SamSettings.SECTION_CUSTOMIZE);
            settings.FileService.Write(SamSettings.BUTTON_FONT_SIZE, ButtonFontSizeSpinBox.Text, SamSettings.SECTION_CUSTOMIZE);
            settings.FileService.Write(SamSettings.BUTTON_FONT_COLOR, new ColorConverter().ConvertToString(ButtonFontColorPicker.SelectedColor), SamSettings.SECTION_CUSTOMIZE);
            settings.FileService.Write(SamSettings.BUTTON_BANNER_COLOR, new ColorConverter().ConvertToString(BannerColorPicker.SelectedColor), SamSettings.SECTION_CUSTOMIZE);
            settings.FileService.Write(SamSettings.BUTTON_BANNER_FONT_SIZE, BannerFontSizeSpinBox.Text, SamSettings.SECTION_CUSTOMIZE);
            settings.FileService.Write(SamSettings.BUTTON_BANNER_FONT_COLOR, new ColorConverter().ConvertToString(BannerFontColorPicker.SelectedColor), SamSettings.SECTION_CUSTOMIZE);
            settings.FileService.Write(SamSettings.HIDE_BAN_ICONS, HideBanIconsCheckBox.IsChecked.ToString(), SamSettings.SECTION_CUSTOMIZE);

            // AutoLog
            settings.FileService.Write(SamSettings.LOGIN_RECENT_ACCOUNT, mostRecentCheckBox.IsChecked.ToString(), SamSettings.SECTION_AUTOLOG);
            settings.FileService.Write(SamSettings.LOGIN_SELECTED_ACCOUNT, selectedAccountCheckBox.IsChecked.ToString(), SamSettings.SECTION_AUTOLOG);
            settings.FileService.Write(SamSettings.INPUT_METHOD, InputMethodSelectBox.SelectedItem.ToString(), SamSettings.SECTION_AUTOLOG);
            settings.FileService.Write(SamSettings.HANDLE_IME, HandleImeCheckBox.IsChecked.ToString(), SamSettings.SECTION_AUTOLOG);
            settings.FileService.Write(SamSettings.IME_2FA_ONLY, SteamGuardOnlyCheckBox.IsChecked.ToString(), SamSettings.SECTION_AUTOLOG);

            // Steam
            settings.FileService.Write(SamSettings.STEAM_PATH, SteamPathTextBox.Text, SamSettings.SECTION_STEAM);
            settings.FileService.Write(SamSettings.STEAM_API_KEY, Regex.Replace(ApiKeyTextBox.Text, @"\s+", string.Empty), SamSettings.SECTION_STEAM);
            settings.FileService.Write(SamSettings.AUTO_RELOAD_ENABLED, AutoReloadCheckBox.IsChecked.ToString(), SamSettings.SECTION_STEAM);
            settings.FileService.Write(SamSettings.AUTO_RELOAD_INTERVAL, AutoReloadIntervalSpinBox.Text, SamSettings.SECTION_STEAM);

            // Parameters
            settings.FileService.Write(SamSettings.CAFE_APP_LAUNCH_PARAMETER, CafeAppLaunchCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.CLEAR_BETA_PARAMETER, ClearBetaCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.CONSOLE_PARAMETER, ConsoleCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.LOGIN_PARAMETER, LoginCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.DEVELOPER_PARAMETER, DeveloperCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.FORCE_SERVICE_PARAMETER, ForceServiceCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.NO_CACHE_PARAMETER, NoCacheCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.NO_VERIFY_FILES_PARAMETER, NoVerifyFilesCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.SILENT_PARAMETER, SilentCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.SINGLE_CORE_PARAMETER, SingleCoreCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.TCP_PARAMETER, TcpCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.TEN_FOOT_PARAMETER, TenFootCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.CUSTOM_PARAMETERS, CustomParametersCheckBox.IsChecked.ToString(), SamSettings.SECTION_PARAMETERS);
            settings.FileService.Write(SamSettings.CUSTOM_PARAMETERS_VALUE, CustomParametersTextBox.Text, SamSettings.SECTION_PARAMETERS);
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
                var idx = int.Parse(settings.FileService.Read(SamSettings.RECENT_ACCOUNT_INDEX, SamSettings.SECTION_AUTOLOG));

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
                var selectedEnabled = Convert.ToBoolean(settings.FileService.Read(SamSettings.LOGIN_SELECTED_ACCOUNT, SamSettings.SECTION_AUTOLOG));
                var idx = int.Parse(settings.FileService.Read(SamSettings.SELECTED_ACCOUNT_INDEX, SamSettings.SECTION_AUTOLOG));

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