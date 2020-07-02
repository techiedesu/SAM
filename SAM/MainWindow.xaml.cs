using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MahApps.Metro;
using MahApps.Metro.Controls;
using SAM.Helpers;
using SAM.Models;
using SteamAuth;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Clipboard = System.Windows.Clipboard;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Control = System.Windows.Forms.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Timer = System.Timers.Timer;

namespace SAM
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [Serializable]
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // If no settings file exists, create one and initialize values.
            if (!File.Exists(SamSettings.FILE_NAME)) GenerateSettings();

            LoadSettings();

            Loaded += MainWindow_Loaded;
            BackgroundBorder.PreviewMouseLeftButtonDown += (s, e) => { DragMove(); };

            _timer.Tick += Timer_Tick;
            _timer.Interval = 10;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Version number from assembly
            assemblyVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var ver = new MenuItem();
            var newExistMenuItem = (MenuItem)FileMenu.Items[2];
            ver.Header = "v" + assemblyVer;
            ver.IsEnabled = false;
            newExistMenuItem.Items.Add(ver);

            // Delete updater if exists.
            if (File.Exists("Updater.exe"))
                File.Delete("Updater.exe");

            // Check for a new version if enabled.
            if (settings.User.CheckForUpdates && await UpdateCheck.CheckForUpdate(UpdateCheckUrl, releasesUrl) == 1)
            {
                // An update is available, but user has chosen not to update.
                ver.Header = "Update Available!";
                ver.Click += Ver_Click;
                ver.IsEnabled = true;
            }

            loginThreads = new List<Thread>();

            // Save New Button inital margin.
            initialAddButtonGridMargin = AddButtonGrid.Margin;

            // Save initial window height and width;
            originalHeight = Height;
            originalWidth = Width;

            // Load window with account buttons.
            RefreshWindow();

            // Login to auto log account if enabled and Steam is not already open.
            var steamProc = Process.GetProcessesByName("Steam");

            if (steamProc.Length == 0)
            {
                if (settings.User.LoginRecentAccount)
                    Login(settings.User.RecentAccountIndex, 0);
                else if (settings.User.LoginSelectedAccount)
                    Login(settings.User.SelectedAccountIndex, 0);
            }
        }

        private string VerifyAndSetPassword()
        {
            var messageBoxResult = MessageBoxResult.No;

            while (messageBoxResult == MessageBoxResult.No)
            {
                var passwordDialog = new PasswordWindow();

                if (passwordDialog.ShowDialog() == true && passwordDialog.PasswordText != "")
                {
                    ePassword = passwordDialog.PasswordText;

                    return true.ToString();
                }

                if (passwordDialog.PasswordText == "")
                    messageBoxResult = MessageBox.Show("No password detected, are you sure?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }

            return false.ToString();
        }

        private bool VerifyPassword()
        {
            var messageBoxResult = MessageBoxResult.No;

            while (messageBoxResult == MessageBoxResult.No)
            {
                var passwordDialog = new PasswordWindow();

                if (passwordDialog.ShowDialog() == true && passwordDialog.PasswordText != "")
                {
                    try
                    {
                        encryptedAccounts = Utils.PasswordDeserialize(dataFile, passwordDialog.PasswordText);
                        messageBoxResult = MessageBoxResult.None;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        messageBoxResult = MessageBox.Show("Invalid Password", "Invalid", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                        return messageBoxResult != MessageBoxResult.Cancel && VerifyPassword();
                    }

                    return true;
                }

                if (passwordDialog.PasswordText == "")
                    messageBoxResult = MessageBox.Show("No password detected, are you sure?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }

            return false;
        }

        private void GenerateSettings()
        {
            settings = new SamSettings();

            settings.FileService.Write("Version", assemblyVer, "System");

            foreach (var entry in settings.KeyValuePairs)
            {
                settings.FileService.Write(entry.Key, settings.Default.KeyValuePairs[entry.Key].ToString(), entry.Value);
            }

            var messageBoxResult = MessageBox.Show("Do you want to password protect SAM?", "Protect", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (messageBoxResult == MessageBoxResult.Yes)
                settings.FileService.Write(SamSettings.PASSWORD_PROTECT, VerifyAndSetPassword(), SamSettings.SECTION_GENERAL);
            else
                settings.FileService.Write(SamSettings.PASSWORD_PROTECT, false.ToString(), SamSettings.SECTION_GENERAL);
        }

        private void LoadSettings()
        {
            settings = new SamSettings();

            isLoadingSettings = true;
            launchParameters = new List<string>();

            settings.HandleDeprecatedSettings();

            foreach (var entry in settings.KeyValuePairs)
            {
                if (!settings.FileService.KeyExists(entry.Key, entry.Value))
                    settings.FileService.Write(entry.Key, settings.Default.KeyValuePairs[entry.Key].ToString(), entry.Value);
                else
                    switch (entry.Key)
                    {
                        case SamSettings.ACCOUNTS_PER_ROW:
                            var accountsPerRowString = settings.FileService.Read(SamSettings.ACCOUNTS_PER_ROW, SamSettings.SECTION_GENERAL);

                            if (!Regex.IsMatch(accountsPerRowString, @"^\d+$") || int.Parse(accountsPerRowString) < 1)
                            {
                                settings.FileService.Write(SamSettings.ACCOUNTS_PER_ROW, settings.Default.AccountsPerRow.ToString(), SamSettings.SECTION_GENERAL);
                                settings.User.AccountsPerRow = settings.Default.AccountsPerRow;
                            }

                            settings.User.AccountsPerRow = int.Parse(accountsPerRowString);
                            break;

                        case SamSettings.SLEEP_TIME:
                            var sleepTimeString = settings.FileService.Read(SamSettings.SLEEP_TIME, SamSettings.SECTION_GENERAL);
                            var sleepTime = 0;

                            if (!Regex.IsMatch(sleepTimeString, @"^\d+$") || !int.TryParse(sleepTimeString, out sleepTime) || sleepTime < 0 || sleepTime > 100)
                            {
                                settings.FileService.Write(SamSettings.SLEEP_TIME, settings.Default.SleepTime.ToString(), SamSettings.SECTION_GENERAL);
                                settings.User.SleepTime = settings.Default.SleepTime * 1000;
                            }
                            else
                                settings.User.SleepTime = sleepTime * 1000;

                            break;

                        case SamSettings.START_MINIMIZED:
                            settings.User.StartMinimized = Convert.ToBoolean(settings.FileService.Read(SamSettings.START_MINIMIZED, SamSettings.SECTION_GENERAL));
                            if (settings.User.StartMinimized) WindowState = WindowState.Minimized;
                            break;

                        case SamSettings.BUTTON_SIZE:
                            var buttonSizeString = settings.FileService.Read(SamSettings.BUTTON_SIZE, SamSettings.SECTION_CUSTOMIZE);
                            var buttonSize = 0;

                            if (!Regex.IsMatch(buttonSizeString, @"^\d+$") || !int.TryParse(buttonSizeString, out buttonSize) || buttonSize < 50 ||
                                buttonSize > 200)
                            {
                                settings.FileService.Write(SamSettings.BUTTON_SIZE, "100", SamSettings.SECTION_CUSTOMIZE);
                                settings.User.ButtonSize = 100;
                            }
                            else
                            {
                                settings.User.ButtonSize = buttonSize;
                            }

                            break;

                        case SamSettings.INPUT_METHOD:
                            settings.User.VirtualInputMethod = (VirtualInputMethod)Enum.Parse(typeof(VirtualInputMethod),
                                settings.FileService.Read(SamSettings.INPUT_METHOD, SamSettings.SECTION_AUTOLOG));
                            break;

                        default:
                            switch (Type.GetTypeCode(settings.User.KeyValuePairs[entry.Key].GetType()))
                            {
                                case TypeCode.Boolean:
                                    settings.User.KeyValuePairs[entry.Key] = Convert.ToBoolean(settings.FileService.Read(entry.Key, entry.Value));
                                    if (entry.Value.Equals(SamSettings.SECTION_PARAMETERS) && (bool)settings.User.KeyValuePairs[entry.Key] &&
                                        !entry.Key.StartsWith("custom")) launchParameters.Add("-" + entry.Key);
                                    break;

                                case TypeCode.Int32:
                                    settings.User.KeyValuePairs[entry.Key] = Convert.ToInt32(settings.FileService.Read(entry.Key, entry.Value));
                                    break;

                                case TypeCode.Double:
                                    settings.User.KeyValuePairs[entry.Key] = Convert.ToDouble(settings.FileService.Read(entry.Key, entry.Value));
                                    break;

                                default:
                                    settings.User.KeyValuePairs[entry.Key] = settings.FileService.Read(entry.Key, entry.Value);
                                    break;
                            }

                            break;
                    }
            }

            //Load and validate saved window location.
            if (settings.FileService.KeyExists(SamSettings.WINDOW_LEFT, SamSettings.SECTION_LOCATION) &&
                settings.FileService.KeyExists(SamSettings.WINDOW_TOP, SamSettings.SECTION_LOCATION))
            {
                Left = double.Parse(settings.FileService.Read(SamSettings.WINDOW_LEFT, SamSettings.SECTION_LOCATION));
                Top = double.Parse(settings.FileService.Read(SamSettings.WINDOW_TOP, SamSettings.SECTION_LOCATION));
                SetWindowSettingsIntoScreenArea();
            }
            else
                SetWindowToCenter();

            if (settings.User.ListView)
            {
                AddButtonGrid.Visibility = Visibility.Collapsed;

                Height = settings.User.ListViewHeight;
                Width = settings.User.ListViewWidth;

                ResizeMode = ResizeMode.CanResize;

                foreach (var column in AccountsDataGrid.Columns)
                {
                    column.DisplayIndex = (int)settings.User.KeyValuePairs[settings.ListViewColumns[column.Header.ToString()]];
                }

                AccountsDataGrid.ItemsSource = encryptedAccounts;
                AccountsDataGrid.Visibility = Visibility.Visible;
            }

            if (settings.User.AutoReloadEnabled)
            {
                var interval = settings.User.AutoReloadInterval;

                if (settings.User.LastAutoReload.HasValue)
                {
                    var minutesSince = (DateTime.Now - settings.User.LastAutoReload.Value).TotalMinutes;

                    if (minutesSince < interval) interval -= Convert.ToInt32(minutesSince);
                }

                if (interval <= 0) interval = settings.User.AutoReloadInterval;

                autoReloadApiTimer = new Timer();
                autoReloadApiTimer.Elapsed += AutoReloadApiTimer_Elapsed;
                autoReloadApiTimer.Interval = 60000 * interval;
                autoReloadApiTimer.Start();
            }
            else
            {
                if (autoReloadApiTimer != null)
                {
                    autoReloadApiTimer.Stop();
                    autoReloadApiTimer.Dispose();
                }
            }

            // Set user's theme settings.
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(settings.User.Accent), ThemeManager.GetAppTheme(settings.User.Theme));

            // Apply theme settings for extended toolkit and tabItem brushes.
            if (settings.User.Theme == SamSettings.DARK_THEME)
            {
                Application.Current.Resources["xctkForegoundBrush"] = Brushes.White;
                Application.Current.Resources["xctkColorPickerBackground"] = new BrushConverter().ConvertFromString("#303030");
                Application.Current.Resources["GrayNormalBrush"] = Brushes.White;
            }
            else
            {
                Application.Current.Resources["xctkForegoundBrush"] = Brushes.Black;
                Application.Current.Resources["xctkColorPickerBackground"] = Brushes.White;
                Application.Current.Resources["GrayNormalBrush"] = Brushes.Black;
            }

            if (settings.User.PasswordProtect && ePassword.Length == 0) VerifyAndSetPassword();

            Utils.CheckSteamPath();
            settings.FileService.Write("Version", assemblyVer, "System");
            isLoadingSettings = false;
        }

        private void AutoReloadApiTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => { _ = ReloadAccountsAsync(); });
        }

        public void RefreshWindow()
        {
            decryptedAccounts = new List<Account>();

            buttonGrid.Children.Clear();

            TaskBarIconLoginContextMenu.Items.Clear();
            TaskBarIconLoginContextMenu.IsEnabled = false;

            AddButtonGrid.Height = settings.User.ButtonSize;
            AddButtonGrid.Width = settings.User.ButtonSize;

            // Check if info.dat exists
            if (File.Exists(dataFile))
            {
                // Deserialize file
                if (ePassword.Length > 0)
                {
                    var messageBoxResult = MessageBoxResult.OK;

                    while (messageBoxResult == MessageBoxResult.OK)
                    {
                        try
                        {
                            encryptedAccounts = Utils.PasswordDeserialize(dataFile, ePassword);
                            messageBoxResult = MessageBoxResult.None;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            messageBoxResult = MessageBox.Show("Invalid Password", "Invalid", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                            if (messageBoxResult == MessageBoxResult.Cancel)
                                Close();
                            else
                                VerifyAndSetPassword();
                        }
                    }
                }
                else
                {
                    encryptedAccounts = Utils.Deserialize(dataFile);
                }

                PostDeserializedRefresh(true);
            }
            else
            {
                encryptedAccounts = new List<Account>();
                SerializeAccounts();
            }

            AccountsDataGrid.ItemsSource = encryptedAccounts;

            if (firstLoad && settings.User.AutoReloadEnabled && Utils.ShouldAutoReload(settings.User.LastAutoReload, settings.User.AutoReloadInterval))
            {
                firstLoad = false;
                _ = ReloadAccountsAsync();
            }
        }

        private async Task ReloadAccount(Account account)
        {
            dynamic userJson;

            if (!string.IsNullOrEmpty(account.SteamId))
                userJson = await Utils.GetUserInfoFromWebApiBySteamId(account.SteamId);
            else
                userJson = await Utils.GetUserInfoFromConfigAndWebApi(account.Name);

            if (userJson != null)
            {
                account.ProfUrl = userJson.response.players[0].profileurl;
                account.AviUrl = userJson.response.players[0].avatarfull;
                account.SteamId = userJson.response.players[0].steamid;
            }
            else
            {
                account.AviUrl = await Utils.HtmlAviScrapeAsync(account.ProfUrl);
            }

            if (!string.IsNullOrEmpty(account.SteamId) && Utils.ApiKeyExists())
            {
                var userBanJson = await Utils.GetPlayerBansFromWebApi(account.SteamId);

                if (userBanJson != null)
                {
                    account.CommunityBanned = Convert.ToBoolean(userBanJson.CommunityBanned);
                    account.IsVacBanned = Convert.ToBoolean(userBanJson.VACBanned);
                    account.NumberOfVacBans = Convert.ToInt32(userBanJson.NumberOfVACBans);
                    account.NumberOfGameBans = Convert.ToInt32(userBanJson.NumberOfGameBans);
                    account.DaysSinceLastBan = Convert.ToInt32(userBanJson.DaysSinceLastBan);
                    account.EconomyBan = userBanJson.EconomyBan;
                }
            }
        }

        public async Task ReloadAccountsAsync()
        {
            Title = "SAM Loading...";

            var steamIds = new List<string>();

            // TODO: Something weird with var naming.
            foreach (var account in encryptedAccounts)
            {
                if (!string.IsNullOrEmpty(account.SteamId))
                {
                    steamIds.Add(account.SteamId);
                }
                else
                {
                    var steamId = Utils.GetSteamIdFromConfig(account.Name);
                    if (!string.IsNullOrEmpty(steamId))
                    {
                        steamIds.Add(steamId);
                    }
                    else if (!string.IsNullOrEmpty(account.ProfUrl))
                    {
                        // Try to get steamId from profile URL via web API.

                        dynamic steamIdFromProfileUrl = await Utils.GetSteamIdFromProfileUrl(account.ProfUrl);

                        if (steamIdFromProfileUrl != null)
                        {
                            account.SteamId = steamIdFromProfileUrl;
                            steamIds.Add(steamIdFromProfileUrl);
                        }

                        Thread.Sleep(new Random().Next(10, 16));
                    }
                }
            }

            var userInfos = await Utils.GetUserInfosFromWepApi(new List<string>(steamIds));

            foreach (var userInfosJson in userInfos)
            {
                foreach (var userInfoJson in userInfosJson.response.players)
                {
                    var account = encryptedAccounts.SingleOrDefault(a => a.SteamId == Convert.ToString(userInfoJson.steamid));

                    if (account != null)
                    {
                        account.ProfUrl = userInfoJson.profileurl;
                        account.AviUrl = userInfoJson.avatarfull;
                    }
                }
            }

            if (Utils.ApiKeyExists())
            {
                var userBans = await Utils.GetPlayerBansFromWebApi(new List<string>(steamIds));

                foreach (var userBansJson in userBans)
                {
                    foreach (var userBanJson in userBansJson.players)
                    {
                        var account = encryptedAccounts.SingleOrDefault(a => a.SteamId == Convert.ToString(userBanJson.SteamId));

                        if (account != null)
                        {
                            account.CommunityBanned = Convert.ToBoolean(userBanJson.CommunityBanned);
                            account.IsVacBanned = Convert.ToBoolean(userBanJson.VACBanned);
                            account.NumberOfVacBans = Convert.ToInt32(userBanJson.NumberOfVACBans);
                            account.NumberOfGameBans = Convert.ToInt32(userBanJson.NumberOfGameBans);
                            account.DaysSinceLastBan = Convert.ToInt32(userBanJson.DaysSinceLastBan);
                            account.EconomyBan = userBanJson.EconomyBan;
                        }
                    }
                }
            }

            settings.FileService.Write(SamSettings.LAST_AUTO_RELOAD, DateTime.Now.ToString(CultureInfo.InvariantCulture), SamSettings.SECTION_STEAM);
            settings.User.LastAutoReload = DateTime.Now;

            SerializeAccounts();

            Title = "SAM";
        }

        private void PostDeserializedRefresh(bool seedAcc)
        {
            SetMainScrollViewerBarsVisibility(ScrollBarVisibility.Hidden);

            // Dispose and reinitialize timers each time grid is refreshed as to not clog up more resources than necessary. 
            if (timeoutTimers != null)
                foreach (var timer in timeoutTimers)
                {
                    timer.Stop();
                    timer.Dispose();
                }

            timeoutTimers = new List<Timer>();

            if (encryptedAccounts != null)
            {
                foreach (var account in encryptedAccounts)
                {
                    var tempPass = StringCipher.Decrypt(account.Password, eKey);

                    if (seedAcc)
                    {
                        string temp2fa = null;
                        string steamId = null;

                        if (!string.IsNullOrEmpty(account.SharedSecret)) temp2fa = StringCipher.Decrypt(account.SharedSecret, eKey);
                        if (!string.IsNullOrEmpty(account.SteamId)) steamId = account.SteamId;

                        decryptedAccounts.Add(new Account
                        {
                            Name = account.Name,
                            Alias = account.Alias,
                            Password = tempPass,
                            SharedSecret = temp2fa,
                            ProfUrl = account.ProfUrl,
                            AviUrl = account.AviUrl,
                            SteamId = steamId,
                            Timeout = account.Timeout,
                            Description = account.Description,
                        });
                    }
                }

                if (settings.User.ListView)
                {
                    SetMainScrollViewerBarsVisibility(ScrollBarVisibility.Auto);

                    for (var i = 0; i < encryptedAccounts.Count; i++)
                    {
                        var account = encryptedAccounts[i];

                        var index = i;

                        TaskBarIconLoginContextMenu.IsEnabled = true;
                        TaskBarIconLoginContextMenu.Items.Add(GenerateTaskBarMenuItem(index, account));

                        if (Utils.AccountHasActiveTimeout(account))
                        {
                            // Set up timer event to update timeout label
                            var timeLeft = account.Timeout - DateTime.Now;

                            var timeoutTimer = new Timer();
                            timeoutTimers.Add(timeoutTimer);

                            timeoutTimer.Elapsed += delegate { Dispatcher.Invoke(() => { TimeoutTimer_Tick(index, timeoutTimer); }); };
                            timeoutTimer.Interval = 1000;
                            timeoutTimer.Enabled = true;
                        }
                    }
                }
                else
                {
                    AccountsDataGrid.Visibility = Visibility.Collapsed;
                    AddButtonGrid.Visibility = Visibility.Visible;

                    var bCounter = 0;
                    var xCounter = 0;
                    var yCounter = 0;

                    var buttonOffset = settings.User.ButtonSize + 5;

                    // Create new button and textblock for each account
                    foreach (var account in encryptedAccounts)
                    {
                        var accountButtonGrid = new Grid();

                        var accountButton = new Button();
                        var accountText = new TextBlock();
                        var timeoutTextBlock = new TextBlock();

                        var accountImage = new Border();

                        accountButton.Style = (Style)Resources["SAMButtonStyle"];
                        accountButton.Tag = bCounter.ToString();

                        if (!string.IsNullOrEmpty(account.Alias))
                            accountText.Text = account.Alias;
                        else
                            accountText.Text = account.Name;

                        // If there is a description, set up tooltip.
                        if (!string.IsNullOrEmpty(account.Description)) accountButton.ToolTip = account.Description;

                        accountButtonGrid.HorizontalAlignment = HorizontalAlignment.Left;
                        accountButtonGrid.VerticalAlignment = VerticalAlignment.Top;
                        accountButtonGrid.Margin = new Thickness(xCounter * buttonOffset, yCounter * buttonOffset, 0, 0);

                        accountButton.Height = settings.User.ButtonSize;
                        accountButton.Width = settings.User.ButtonSize;
                        accountButton.BorderBrush = null;
                        accountButton.HorizontalAlignment = HorizontalAlignment.Center;
                        accountButton.VerticalAlignment = VerticalAlignment.Center;
                        accountButton.Background = Brushes.Transparent;

                        accountText.Width = settings.User.ButtonSize;
                        if (settings.User.ButtonFontSize > 0)
                            accountText.FontSize = settings.User.ButtonFontSize;
                        else
                            accountText.FontSize = settings.User.ButtonSize / 8;

                        accountText.HorizontalAlignment = HorizontalAlignment.Center;
                        accountText.VerticalAlignment = VerticalAlignment.Bottom;
                        accountText.Margin = new Thickness(0, 0, 0, 7);
                        accountText.Padding = new Thickness(0, 0, 0, 1);
                        accountText.TextAlignment = TextAlignment.Center;
                        accountText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.User.BannerFontColor));
                        accountText.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.User.ButtonBannerColor));
                        accountText.Visibility = Visibility.Collapsed;

                        timeoutTextBlock.Width = settings.User.ButtonSize;
                        timeoutTextBlock.FontSize = settings.User.ButtonSize / 8;
                        timeoutTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                        timeoutTextBlock.VerticalAlignment = VerticalAlignment.Center;
                        timeoutTextBlock.Padding = new Thickness(0, 0, 0, 1);
                        timeoutTextBlock.TextAlignment = TextAlignment.Center;
                        timeoutTextBlock.Foreground = new SolidColorBrush(Colors.White);
                        timeoutTextBlock.Background = new SolidColorBrush(new Color { A = 128, R = 255, G = 0, B = 0 });

                        accountImage.Height = settings.User.ButtonSize;
                        accountImage.Width = settings.User.ButtonSize;
                        accountImage.HorizontalAlignment = HorizontalAlignment.Center;
                        accountImage.VerticalAlignment = VerticalAlignment.Center;
                        accountImage.CornerRadius = new CornerRadius(3);

                        if (account.ProfUrl == "" || account.AviUrl == null || account.AviUrl == "" || account.AviUrl == " ")
                        {
                            accountImage.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.User.ButtonColor));
                            accountButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.User.ButtonFontColor));
                            timeoutTextBlock.Margin = new Thickness(0, 0, 0, 50);

                            if (!string.IsNullOrEmpty(account.Alias))
                                accountButton.Content = account.Alias;
                            else
                                accountButton.Content = account.Name;
                        }
                        else
                        {
                            try
                            {
                                var imageBrush = new ImageBrush();
                                var image1 = new BitmapImage(new Uri(account.AviUrl));
                                imageBrush.ImageSource = image1;
                                accountImage.Background = imageBrush;
                            }
                            catch (Exception m)
                            {
                                // Probably no internet connection or avatar url is bad.
                                Console.WriteLine("Error: " + m.Message);

                                accountImage.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.User.ButtonColor));
                                accountButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.User.ButtonFontColor));
                                timeoutTextBlock.Margin = new Thickness(0, 0, 0, 50);

                                if (account.Alias != null && account.Alias.Length > 0)
                                    accountButton.Content = account.Alias;
                                else
                                    accountButton.Content = account.Name;
                            }
                        }

                        accountButton.Click += AccountButton_Click;
                        accountButton.PreviewMouseLeftButtonDown += AccountButton_MouseDown;
                        accountButton.PreviewMouseLeftButtonUp += AccountButton_MouseUp;
                        //accountButton.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(AccountButton_MouseMove);
                        accountButton.MouseLeave += AccountButton_MouseLeave;
                        accountButton.MouseEnter += delegate { AccountButton_MouseEnter(accountText); };
                        accountButton.MouseLeave += delegate { AccountButton_MouseLeave(accountText); };

                        accountButtonGrid.Children.Add(accountImage);

                        var buttonIndex = int.Parse(accountButton.Tag.ToString());

                        if (Utils.AccountHasActiveTimeout(account))
                        {
                            // Set up timer event to update timeout label
                            var timeLeft = account.Timeout - DateTime.Now;

                            var timeoutTimer = new Timer();
                            timeoutTimers.Add(timeoutTimer);

                            timeoutTimer.Elapsed += delegate { Dispatcher.Invoke(() => { TimeoutTimer_Tick(buttonIndex, timeoutTextBlock, timeoutTimer); }); };
                            timeoutTimer.Interval = 1000;
                            timeoutTimer.Enabled = true;
                            timeoutTextBlock.Text = Utils.FormatTimespanString(timeLeft.Value);
                            timeoutTextBlock.Visibility = Visibility.Visible;

                            accountButtonGrid.Children.Add(timeoutTextBlock);
                        }

                        accountButtonGrid.Children.Add(accountText);
                        accountButtonGrid.Children.Add(accountButton);

                        if (settings.User.HideBanIcons == false && (account.NumberOfVacBans > 0 || account.NumberOfGameBans > 0))
                        {
                            var banInfoImage = new Image
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Height = 14,
                                Width = 14,
                                Margin = new Thickness(10, 10, 10, 10),
                                Source = new BitmapImage(new Uri(@"error_red_18dp.png", UriKind.RelativeOrAbsolute)),
                                ToolTip = "VAC Bans: " + account.NumberOfVacBans +
                                          "\nGame Bans: " + account.NumberOfGameBans +
                                          "\nCommunity Banned: " + account.CommunityBanned +
                                          "\nEconomy Ban: " + account.EconomyBan +
                                          "\nDays Since Last Ban:" + account.DaysSinceLastBan
                            };



                            accountButtonGrid.Children.Add(banInfoImage);
                        }

                        accountButton.ContextMenu = GenerateAccountContextMenu(account, buttonIndex);
                        accountButton.ContextMenuOpening += ContextMenu_ContextMenuOpening;

                        buttonGrid.Children.Add(accountButtonGrid);

                        TaskBarIconLoginContextMenu.IsEnabled = true;
                        TaskBarIconLoginContextMenu.Items.Add(GenerateTaskBarMenuItem(bCounter, account));

                        bCounter++;
                        xCounter++;

                        if (bCounter % settings.User.AccountsPerRow == 0 &&
                            (!settings.User.HideAddButton || settings.User.HideAddButton && bCounter != encryptedAccounts.Count))
                        {
                            yCounter++;
                            xCounter = 0;
                        }
                    }

                    if (bCounter > 0)
                    {
                        // Adjust window size and info positions
                        var xVal = settings.User.AccountsPerRow;

                        AddButtonGrid.Visibility = settings.User.HideAddButton ? Visibility.Collapsed : Visibility.Visible;

                        xVal = yCounter switch
                        {
                            0 when !settings.User.HideAddButton => xCounter + 1,
                            0 => xCounter,
                            _ => xVal
                        };

                        var newHeight = buttonOffset * (yCounter + 1) + 57;
                        var newWidth = buttonOffset * xVal + 7;

                        Resize(newHeight, newWidth);

                        // Adjust new account and export/delete buttons
                        AddButtonGrid.HorizontalAlignment = HorizontalAlignment.Left;
                        AddButtonGrid.VerticalAlignment = VerticalAlignment.Top;
                        AddButtonGrid.Margin = new Thickness(xCounter * buttonOffset + 5, yCounter * buttonOffset + 25, 0, 0);
                    }
                    else
                    {
                        // Reset New Button position.
                        Resize(180, 138);

                        AddButtonGrid.HorizontalAlignment = HorizontalAlignment.Center;
                        AddButtonGrid.VerticalAlignment = VerticalAlignment.Center;
                        AddButtonGrid.Margin = initialAddButtonGridMargin;
                        ResizeMode = ResizeMode.CanMinimize;
                    }
                }
            }
        }

        private MenuItem GenerateTaskBarMenuItem(int index, Account account)
        {
            var taskBarIconLoginItem = new MenuItem { Tag = index };
            taskBarIconLoginItem.Click += TaskbarIconLoginItem_Click;

            if (!string.IsNullOrEmpty(account.Alias))
                taskBarIconLoginItem.Header = account.Alias;
            else
                taskBarIconLoginItem.Header = account.Name;

            return taskBarIconLoginItem;
        }

        private ContextMenu GenerateAccountContextMenu(Account account, int index)
        {
            var accountContext = new ContextMenu();

            var deleteItem = new MenuItem();
            var editItem = new MenuItem();
            var exportItem = new MenuItem();
            var reloadItem = new MenuItem();

            var setTimeoutItem = new MenuItem();

            var thirtyMinuteTimeoutItem = new MenuItem();
            var twoHourTimeoutItem = new MenuItem();
            var twentyOneHourTimeoutItem = new MenuItem();
            var twentyFourHourTimeoutItem = new MenuItem();
            var sevenDayTimeoutItem = new MenuItem();
            var customTimeoutItem = new MenuItem();

            thirtyMinuteTimeoutItem.Header = "30 Minutes";
            twoHourTimeoutItem.Header = "2 Hours";
            twentyOneHourTimeoutItem.Header = "21 Hours";
            twentyFourHourTimeoutItem.Header = "24 Hours";
            sevenDayTimeoutItem.Header = "7 Days";
            customTimeoutItem.Header = "Custom";

            setTimeoutItem.Items.Add(thirtyMinuteTimeoutItem);
            setTimeoutItem.Items.Add(twoHourTimeoutItem);
            setTimeoutItem.Items.Add(twentyOneHourTimeoutItem);
            setTimeoutItem.Items.Add(twentyFourHourTimeoutItem);
            setTimeoutItem.Items.Add(sevenDayTimeoutItem);
            setTimeoutItem.Items.Add(customTimeoutItem);

            var clearTimeoutItem = new MenuItem();
            var copyUsernameItem = new MenuItem();
            var copyPasswordItem = new MenuItem();
            var copyProfileUrlItem = new MenuItem();

            if (!Utils.AccountHasActiveTimeout(account)) clearTimeoutItem.IsEnabled = false;

            deleteItem.Header = "Delete";
            editItem.Header = "Edit";
            exportItem.Header = "Export";
            reloadItem.Header = "Reload";
            setTimeoutItem.Header = "Set Timeout";
            clearTimeoutItem.Header = "Clear Timeout";
            copyUsernameItem.Header = "Copy Username";
            copyPasswordItem.Header = "Copy Password";
            copyProfileUrlItem.Header = "Copy Profile URL";

            deleteItem.Click += delegate { DeleteEntry(index); };
            editItem.Click += delegate
            {
                var editEntryAsync = EditEntryAsync(index);
            };
            exportItem.Click += delegate { ExportAccount(index); };
            reloadItem.Click += async delegate { await ReloadAccount_ClickAsync(index); };
            thirtyMinuteTimeoutItem.Click += delegate { AccountButtonSetTimeout_Click(index, DateTime.Now.AddMinutes(30)); };
            twoHourTimeoutItem.Click += delegate { AccountButtonSetTimeout_Click(index, DateTime.Now.AddHours(2)); };
            twentyOneHourTimeoutItem.Click += delegate { AccountButtonSetTimeout_Click(index, DateTime.Now.AddHours(21)); };
            twentyFourHourTimeoutItem.Click += delegate { AccountButtonSetTimeout_Click(index, DateTime.Now.AddDays(1)); };
            sevenDayTimeoutItem.Click += delegate { AccountButtonSetTimeout_Click(index, DateTime.Now.AddDays(7)); };
            customTimeoutItem.Click += delegate { AccountButtonSetCustomTimeout_Click(index); };
            clearTimeoutItem.Click += delegate { AccountButtonClearTimeout_Click(index); };
            copyUsernameItem.Click += delegate { CopyUsernameToClipboard(index); };
            copyPasswordItem.Click += delegate { CopyPasswordToClipboard(index); };
            copyProfileUrlItem.Click += delegate { CopyProfileUrlToClipboard(index); };

            accountContext.Items.Add(editItem);
            accountContext.Items.Add(deleteItem);
            accountContext.Items.Add(exportItem);
            accountContext.Items.Add(reloadItem);
            accountContext.Items.Add(setTimeoutItem);
            accountContext.Items.Add(clearTimeoutItem);
            accountContext.Items.Add(copyUsernameItem);
            accountContext.Items.Add(copyPasswordItem);
            accountContext.Items.Add(copyProfileUrlItem);

            return accountContext;
        }

        private ContextMenu GenerateAltActionContextMenu(string altActionType)
        {
            var contextMenu = new ContextMenu();
            var actionMenuItem = new MenuItem();

            if (altActionType == AltActionType.DELETING)
            {
                actionMenuItem.Header = "Delete Selected";
                actionMenuItem.Click += delegate { DeleteSelectedAccounts(); };
            }
            else if (altActionType == AltActionType.EXPORTING)
            {
                actionMenuItem.Header = "Export Selected";
                actionMenuItem.Click += delegate { ExportSelectedAccounts(); };
            }

            var cancelMenuItem = new MenuItem { Header = "Cancel" };
            cancelMenuItem.Click += delegate { ResetFromExportOrDelete(); };

            contextMenu.Items.Add(actionMenuItem);
            contextMenu.Items.Add(cancelMenuItem);

            return contextMenu;
        }

        private async void AddAccount()
        {
            // User entered info
            var dialog = new TextDialog();

            if (dialog.ShowDialog() == true && dialog.AccountText != "" && dialog.PasswordText != "")
            {
                account = dialog.AccountText;
                var password = dialog.PasswordText;
                var sharedSecret = dialog.SharedSecretText;

                string aviUrl;
                if (dialog.AviText != null && dialog.AviText.Length > 1)
                    aviUrl = dialog.AviText;
                else
                    aviUrl = await Utils.HtmlAviScrapeAsync(dialog.UrlText);

                var steamId = dialog.SteamId;

                // If the auto login checkbox was checked, update settings file and global variables. 
                if (dialog.AutoLogAccountIndex)
                {
                    settings.FileService.Write(SamSettings.SELECTED_ACCOUNT_INDEX, (encryptedAccounts.Count + 1).ToString(), SamSettings.SECTION_AUTOLOG);
                    settings.FileService.Write(SamSettings.LOGIN_SELECTED_ACCOUNT, true.ToString(), SamSettings.SECTION_AUTOLOG);
                    settings.FileService.Write(SamSettings.LOGIN_RECENT_ACCOUNT, false.ToString(), SamSettings.SECTION_AUTOLOG);
                    settings.User.LoginSelectedAccount = true;
                    settings.User.LoginRecentAccount = false;
                    settings.User.SelectedAccountIndex = encryptedAccounts.Count + 1;
                }

                try
                {
                    var newAccount = new Account
                    {
                        Name = dialog.AccountText,
                        Alias = dialog.AliasText,
                        Password = StringCipher.Encrypt(password, eKey),
                        SharedSecret = StringCipher.Encrypt(sharedSecret, eKey),
                        ProfUrl = dialog.UrlText,
                        AviUrl = aviUrl,
                        SteamId = steamId,
                        Description = dialog.DescriptionText,
                    };

                    await ReloadAccount(newAccount);

                    encryptedAccounts.Add(newAccount);
                    SerializeAccounts();
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    var itemToRemove = encryptedAccounts.Single(r => r.Name == dialog.AccountText);
                    encryptedAccounts.Remove(itemToRemove);

                    SerializeAccounts();
                    AddAccount();
                }
            }
        }

        private async Task EditEntryAsync(int index)
        {
            var dialog = new TextDialog
            {
                AccountText = decryptedAccounts[index].Name,
                AliasText = decryptedAccounts[index].Alias,
                PasswordText = decryptedAccounts[index].Password,
                SharedSecretText = decryptedAccounts[index].SharedSecret,
                UrlText = decryptedAccounts[index].ProfUrl,
                DescriptionText = decryptedAccounts[index].Description,
            };

            // Reload selected boolean
            settings.User.LoginSelectedAccount =
                settings.FileService.Read(SamSettings.LOGIN_SELECTED_ACCOUNT, SamSettings.SECTION_AUTOLOG) == true.ToString();

            if (settings.User.LoginSelectedAccount && settings.User.SelectedAccountIndex == index)
                dialog.autoLogCheckBox.IsChecked = true;

            if (dialog.ShowDialog() == true)
            {
                string aviUrl;
                if (dialog.AviText != null && dialog.AviText.Length > 1)
                    aviUrl = dialog.AviText;
                else
                    aviUrl = await Utils.HtmlAviScrapeAsync(dialog.UrlText);

                var steamId = dialog.SteamId;

                // If the auto login checkbox was checked, update settings file and global variables. 
                if (dialog.AutoLogAccountIndex)
                {
                    settings.FileService.Write(SamSettings.SELECTED_ACCOUNT_INDEX, index.ToString(), SamSettings.SECTION_AUTOLOG);
                    settings.FileService.Write(SamSettings.LOGIN_SELECTED_ACCOUNT, true.ToString(), SamSettings.SECTION_AUTOLOG);
                    settings.FileService.Write(SamSettings.LOGIN_RECENT_ACCOUNT, false.ToString(), SamSettings.SECTION_AUTOLOG);
                    settings.User.LoginSelectedAccount = true;
                    settings.User.LoginRecentAccount = false;
                    settings.User.SelectedAccountIndex = index;
                }
                else
                {
                    settings.FileService.Write(SamSettings.SELECTED_ACCOUNT_INDEX, "-1", SamSettings.SECTION_AUTOLOG);
                    settings.FileService.Write(SamSettings.LOGIN_SELECTED_ACCOUNT, false.ToString(), SamSettings.SECTION_AUTOLOG);
                    settings.User.LoginSelectedAccount = false;
                    settings.User.SelectedAccountIndex = -1;
                }

                try
                {
                    encryptedAccounts[index].Name = dialog.AccountText;
                    encryptedAccounts[index].Alias = dialog.AliasText;
                    encryptedAccounts[index].Password = StringCipher.Encrypt(dialog.PasswordText, eKey);
                    encryptedAccounts[index].SharedSecret = StringCipher.Encrypt(dialog.SharedSecretText, eKey);
                    encryptedAccounts[index].ProfUrl = dialog.UrlText;
                    encryptedAccounts[index].AviUrl = aviUrl;
                    encryptedAccounts[index].SteamId = dialog.SteamId;
                    encryptedAccounts[index].Description = dialog.DescriptionText;

                    SerializeAccounts();
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    await EditEntryAsync(index);
                }
            }
        }

        private void DeleteEntry(int index)
        {
            var result = MessageBox.Show("Are you sure you want to delete this entry?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                encryptedAccounts.RemoveAt(index);
                SerializeAccounts();
            }
        }

        private void Login(int index, int tryCount)
        {
            if (tryCount == maxRetry)
            {
                MessageBox.Show("Login Failed! Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Utils.AccountHasActiveTimeout(encryptedAccounts[index]))
            {
                var result = MessageBox.Show("Account timeout is active!\nLogin anyway?", "Timeout", MessageBoxButton.YesNo, MessageBoxImage.Warning, 0,
                    MessageBoxOptions.DefaultDesktopOnly);

                if (result == MessageBoxResult.No) return;
            }

            foreach (var loginThread in loginThreads)
            {
                loginThread.Abort();
            }

            // Update the most recently used account index.
            settings.User.RecentAccountIndex = index;
            settings.FileService.Write(SamSettings.RECENT_ACCOUNT_INDEX, index.ToString(), SamSettings.SECTION_AUTOLOG);

            // Verify Steam file path.
            settings.User.SteamPath = Utils.CheckSteamPath();

            if (!settings.User.SandboxMode)
            {
                // Shutdown Steam process via command if it is already open.
                var stopInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = settings.User.SteamPath + "steam.exe",
                    WorkingDirectory = settings.User.SteamPath,
                    Arguments = "-shutdown",
                };

                try
                {
                    var steamProc = Process.GetProcessesByName("Steam")[0];
                    Process.Start(stopInfo);
                    steamProc.WaitForExit();
                }
                catch
                {
                    Console.WriteLine("No steam process found or steam failed to shutdown.");
                }
            }

            // Make sure Username field is empty and Remember Password checkbox is unchecked.
            if (!settings.User.Login) Utils.ClearAutoLoginUserKeyValues();

            var parametersBuilder = new StringBuilder();

            if (settings.User.CustomParameters) parametersBuilder.Append(settings.User.CustomParametersValue).Append(" ");

            foreach (var parameter in launchParameters)
            {
                parametersBuilder.Append(parameter).Append(" ");

                if (parameter.Equals("-login"))
                {
                    var passwordBuilder = new StringBuilder();

                    foreach (var c in decryptedAccounts[index].Password)
                    {
                        if (c.Equals('"'))
                            passwordBuilder.Append('\\').Append(c);
                        else
                            passwordBuilder.Append(c);
                    }

                    parametersBuilder.Append(decryptedAccounts[index].Name).Append(" \"").Append(passwordBuilder).Append("\" ");
                }
            }

            // Start Steam process with the selected path.
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = Path.Combine(settings.User.SteamPath, "steam.exe"),
                WorkingDirectory = settings.User.SteamPath,
                Arguments = parametersBuilder.ToString(),
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (settings.User.Login)
            {
                if (settings.User.RememberPassword) Utils.SetRememeberPasswordKeyValue(1);

                if (!string.IsNullOrEmpty(decryptedAccounts[index].SharedSecret))
                    Task.Run(() => Type2Fa(index, 0));
                else
                    PostLogin();
            }
            else
            {
                Task.Run(() => TypeCredentials(index, 0));
            }
        }

        private void TypeCredentials(int index, int tryCount)
        {
            loginThreads.Add(Thread.CurrentThread);

            var steamLoginWindow = Utils.GetSteamLoginWindow();

            while (!steamLoginWindow.IsValid)
            {
                Thread.Sleep(10);
                steamLoginWindow = Utils.GetSteamLoginWindow();
            }

            // Debug
            //StringBuilder windowTitleBuilder = new StringBuilder(Utils.GetWindowTextLength(steamLoginWindow.RawPtr) + 1);
            //Utils.GetWindowText(steamLoginWindow.RawPtr, windowTitleBuilder, windowTitleBuilder.Capacity);

            var steamLoginProcess = Utils.WaitForSteamProcess(steamLoginWindow);
            steamLoginProcess.WaitForInputIdle();

            Thread.Sleep(settings.User.SleepTime);
            Utils.SetForegroundWindow(steamLoginWindow.RawPtr);
            Thread.Sleep(100);

            // Enable Caps-Lock, to prevent IME problems.
            var capsLockEnabled = Control.IsKeyLocked(Keys.CapsLock);
            if (settings.User.HandleMicrosoftIME && !settings.User.IME2FAOnly && !capsLockEnabled) Utils.SendCapsLockGlobally();

            foreach (var c in decryptedAccounts[index].Name)
            {
                Utils.SetForegroundWindow(steamLoginWindow.RawPtr);
                Thread.Sleep(10);
                Utils.SendCharacter(steamLoginWindow.RawPtr, settings.User.VirtualInputMethod, c);
            }

            Thread.Sleep(100);
            Utils.SendTab(steamLoginWindow.RawPtr, settings.User.VirtualInputMethod);
            Thread.Sleep(100);

            foreach (var c in decryptedAccounts[index].Password)
            {
                Utils.SetForegroundWindow(steamLoginWindow.RawPtr);
                Thread.Sleep(10);
                Utils.SendCharacter(steamLoginWindow.RawPtr, settings.User.VirtualInputMethod, c);
            }

            if (settings.User.RememberPassword)
            {
                Utils.SetForegroundWindow(steamLoginWindow.RawPtr);

                Thread.Sleep(100);
                Utils.SendTab(steamLoginWindow.RawPtr, settings.User.VirtualInputMethod);
                Thread.Sleep(100);
                Utils.SendSpace(steamLoginWindow.RawPtr, settings.User.VirtualInputMethod);
            }

            Utils.SetForegroundWindow(steamLoginWindow.RawPtr);

            Thread.Sleep(100);
            Utils.SendEnter(steamLoginWindow.RawPtr, settings.User.VirtualInputMethod);

            // Restore CapsLock back if CapsLock is off before we start typing.
            if (settings.User.HandleMicrosoftIME && !settings.User.IME2FAOnly && !capsLockEnabled) Utils.SendCapsLockGlobally();

            var waitCount = 0;

            // Only handle 2FA if shared secret was entered.
            if (!string.IsNullOrEmpty(decryptedAccounts[index].SharedSecret))
            {
                var steamGuardWindow = Utils.GetSteamGuardWindow();

                while (!steamGuardWindow.IsValid && waitCount < maxRetry)
                {
                    Thread.Sleep(settings.User.SleepTime);

                    steamGuardWindow = Utils.GetSteamGuardWindow();

                    // Check for Steam warning window.
                    var steamWarningWindow = Utils.GetSteamWarningWindow();
                    if (steamWarningWindow.IsValid)
                        //Cancel the 2FA process since Steam connection is likely unavailable. 
                        return;

                    waitCount++;
                }

                // 2FA window not found, login probably failed. Try again.
                if (waitCount == maxRetry)
                {
                    Dispatcher.Invoke(delegate { Login(index, tryCount + 1); });
                    return;
                }

                Type2Fa(index, 0);
            }
            else
            {
                PostLogin();
            }
        }

        private void Type2Fa(int index, int tryCount)
        {
            // Need both the Steam Login and Steam Guard windows.
            // Can't focus the Steam Guard window directly.
            var steamLoginWindow = Utils.GetSteamLoginWindow();
            var steamGuardWindow = Utils.GetSteamGuardWindow();

            while (!steamLoginWindow.IsValid || !steamGuardWindow.IsValid)
            {
                Thread.Sleep(10);
                steamLoginWindow = Utils.GetSteamLoginWindow();
                steamGuardWindow = Utils.GetSteamGuardWindow();

                // Check for Steam warning window.
                var steamWarningWindow = Utils.GetSteamWarningWindow();
                if (steamWarningWindow.IsValid)
                    //Cancel the 2FA process since Steam connection is likely unavailable. 
                    return;
            }

            Console.WriteLine("Found windows.");

            var steamGuardProcess = Utils.WaitForSteamProcess(steamGuardWindow);
            steamGuardProcess.WaitForInputIdle();

            // Wait a bit for the window to fully initialize just in case.
            Thread.Sleep(settings.User.SleepTime);

            // Generate 2FA code, then send it to the client.
            Console.WriteLine("It is idle now, typing code...");

            Utils.SetForegroundWindow(steamGuardWindow.RawPtr);

            // Enable Caps-Lock, to prevent IME problems.
            var capsLockEnabled = Control.IsKeyLocked(Keys.CapsLock);
            if (settings.User.HandleMicrosoftIME && !capsLockEnabled) Utils.SendCapsLockGlobally();

            Thread.Sleep(10);

            foreach (var c in Generate2FaCode(decryptedAccounts[index].SharedSecret))
            {
                Utils.SetForegroundWindow(steamGuardWindow.RawPtr);
                Thread.Sleep(10);

                // Can also send keys to login window handle, but nothing works unless it is the foreground window.
                Utils.SendCharacter(steamGuardWindow.RawPtr, settings.User.VirtualInputMethod, c);
            }

            Utils.SetForegroundWindow(steamGuardWindow.RawPtr);

            Thread.Sleep(10);

            Utils.SendEnter(steamGuardWindow.RawPtr, settings.User.VirtualInputMethod);

            // Restore CapsLock back if CapsLock is off before we start typing.
            if (settings.User.HandleMicrosoftIME && !capsLockEnabled)
                Utils.SendCapsLockGlobally();

            // Need a little pause here to more reliably check for popup later.
            Thread.Sleep(settings.User.SleepTime);

            // Check if we still have a 2FA popup, which means the previous one failed.
            steamGuardWindow = Utils.GetSteamGuardWindow();

            if (tryCount < maxRetry && steamGuardWindow.IsValid)
            {
                Console.WriteLine("2FA code failed, retrying...");
                Type2Fa(index, tryCount + 1);
                return;
            }

            if (tryCount == maxRetry && steamGuardWindow.IsValid)
            {
                var result = MessageBox.Show("2FA Failed\nPlease wait or bring the Steam Guard\nwindow to the front before clicking OK", "Error",
                    MessageBoxButton.OKCancel, MessageBoxImage.Error);

                if (result == MessageBoxResult.OK)
                    Type2Fa(index, tryCount + 1);
            }
            else if (tryCount == maxRetry + 1 && steamGuardWindow.IsValid)
            {
                MessageBox.Show("2FA Failed\nPlease verify your shared secret is correct!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            PostLogin();
        }

        private void PostLogin()
        {
            if (settings.User.ClearUserData)
                Utils.ClearSteamUserDataFolder(settings.User.SteamPath, settings.User.SleepTime, maxRetry);

            if (settings.User.CloseOnLogin)
                Dispatcher.Invoke(Close);
        }

        private string Generate2FaCode(string sharedSecret)
        {
            var authAccount = new SteamGuardAccount { SharedSecret = sharedSecret };
            var code = authAccount.GenerateSteamGuardCode();
            return code;
        }

        private void SortAccounts(int type)
        {
            if (encryptedAccounts.Count > 0)
            {
                encryptedAccounts = type switch
                {
                    // Alphabetical sort based on account name.
                    0 => encryptedAccounts.OrderBy(a => a.Name).ToList(),
                    1 => encryptedAccounts.OrderBy(a => Guid.NewGuid()).ToList(),
                    _ => encryptedAccounts,
                };

                SerializeAccounts();
            }
        }

        private void SerializeAccounts()
        {
            if (IsPasswordProtected() && ePassword.Length > 0)
                Utils.PasswordSerialize(encryptedAccounts, ePassword);
            else
                Utils.Serialize(encryptedAccounts);

            RefreshWindow();
        }

        private void ExportAccount(int index)
        {
            Utils.ExportSelectedAccounts(new List<Account> { encryptedAccounts[index] });
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (exporting)
            {
                GenerateAltActionContextMenu(AltActionType.EXPORTING).IsOpen = true;
                e.Handled = true;
            }
            else if (deleting)
            {
                GenerateAltActionContextMenu(AltActionType.DELETING).IsOpen = true;
                e.Handled = true;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    break;
                case WindowState.Minimized:
                    if (settings.User.MinimizeToTray)
                    {
                        Visibility = Visibility.Hidden;
                        ShowInTaskbar = false;
                    }

                    break;
                case WindowState.Normal:
                    Visibility = Visibility.Visible;
                    ShowInTaskbar = true;
                    break;
            }
        }

        private void ImportDelimitedTextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var importDelimitedWindow = new ImportDelimited(eKey);
            importDelimitedWindow.ShowDialog();

            RefreshWindow();
        }

        private void ExposeCredentialsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show("Are you sure you want to expose all account credentials in plain text?", "Confirm", MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (messageBoxResult == MessageBoxResult.No || IsPasswordProtected() && !VerifyPassword()) 
                return;

            var exposedCredentialsWindow = new ExposedInfoWindow(decryptedAccounts);
            exposedCredentialsWindow.ShowDialog();
        }

        private bool IsPasswordProtected()
        {
            if (settings.User.PasswordProtect)
                return true;
            try
            {
                if (!File.Exists(dataFile))
                    return false;

                var lines = File.ReadAllLines(dataFile);
                if (lines.Length == 0 || lines.Length > 1)
                    return false;

                Utils.Deserialize(dataFile);
            }
            catch
            {
                return true;
            }

            return false;
        }

        private void TimeoutTimer_Tick(int index, TextBlock timeoutLabel, Timer timeoutTimer)
        {
            var timeLeft = encryptedAccounts[index].Timeout - DateTime.Now;

            if (timeLeft.Value.CompareTo(TimeSpan.Zero) <= 0)
            {
                timeoutTimer.Stop();
                timeoutTimer.Dispose();

                timeoutLabel.Visibility = Visibility.Hidden;
                AccountButtonClearTimeout_Click(index);
            }
            else
            {
                timeoutLabel.Text = Utils.FormatTimespanString(timeLeft.Value);
                timeoutLabel.Visibility = Visibility.Visible;
            }
        }

        private void TimeoutTimer_Tick(int index, Timer timeoutTimer)
        {
            var timeLeft = encryptedAccounts[index].Timeout - DateTime.Now;

            if (timeLeft.Value.CompareTo(TimeSpan.Zero) <= 0)
            {
                timeoutTimer.Stop();
                timeoutTimer.Dispose();

                encryptedAccounts[index].TimeoutTimeLeft = null;
                AccountButtonClearTimeout_Click(index);
            }
            else
            {
                encryptedAccounts[index].TimeoutTimeLeft = Utils.FormatTimespanString(timeLeft.Value);
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (!isLoadingSettings && settings.FileService != null && IsInBounds())
            {
                settings.FileService.Write(SamSettings.WINDOW_LEFT, Left.ToString(CultureInfo.InvariantCulture), SamSettings.SECTION_LOCATION);
                settings.FileService.Write(SamSettings.WINDOW_TOP, Top.ToString(CultureInfo.InvariantCulture), SamSettings.SECTION_LOCATION);
            }
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!isLoadingSettings && settings.FileService != null && settings.User.ListView)
            {
                settings.User.ListViewHeight = Height;
                settings.User.ListViewWidth = Width;

                settings.FileService.Write(SamSettings.LIST_VIEW_HEIGHT, Height.ToString(CultureInfo.InvariantCulture), SamSettings.SECTION_LOCATION);
                settings.FileService.Write(SamSettings.LIST_VIEW_WIDTH, Width.ToString(CultureInfo.InvariantCulture), SamSettings.SECTION_LOCATION);
            }
        }

        private void SetMainScrollViewerBarsVisibility(ScrollBarVisibility visibility)
        {
            MainScrollViewer.VerticalScrollBarVisibility = visibility;
            MainScrollViewer.HorizontalScrollBarVisibility = visibility;
        }

        private void SetWindowSettingsIntoScreenArea()
        {
            if (IsInBounds() == false)
                SetWindowToCenter();
        }

        private bool IsInBounds()
        {
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains((int) Left, (int) Top))
                    return true;
            }

            return false;
        }

        private void SetWindowToCenter()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            Left = screenWidth / 2 - Width / 2;
            Top = screenHeight / 2 - Height / 2;
        }

        private void AccountsDataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            foreach (var column in AccountsDataGrid.Columns)
            {
                settings.FileService.Write(settings.ListViewColumns[column.Header.ToString()], column.DisplayIndex.ToString(), SamSettings.SECTION_COLUMNS);
            }
        }

        private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #region Globals

        public static List<Account> encryptedAccounts;
        private static List<Account> decryptedAccounts;
        private static Dictionary<int, Account> exportAccounts;
        private static Dictionary<int, Account> deleteAccounts;

        private static SamSettings settings;

        private static List<Thread> loginThreads;
        private static List<Timer> timeoutTimers;

        // TODO: Move to config file.
        private static readonly string UpdateCheckUrl = "https://raw.githubusercontent.com/rex706/SAM/master/latest.txt";
        private static readonly string repositoryUrl = "https://github.com/rex706/SAM";
        private static readonly string releasesUrl = repositoryUrl + "/releases";

        private static bool isLoadingSettings = true;
        private static bool firstLoad = true;

        // TODO: Move to config file.
        private static readonly string dataFile = "info.dat";

        // Keys are changed before releases/updates
        private static readonly string eKey = "PRIVATE_KEY";
        private static string ePassword = "";

        private static string account;

        private static List<string> launchParameters;

        private static double originalHeight;
        private static double originalWidth;
        private static Thickness initialAddButtonGridMargin;

        private static string assemblyVer;

        private static bool exporting;
        private static bool deleting;

        private static Button holdingButton;
        private static bool dragging;
        private static Timer mouseHoldTimer;

        private static Timer autoReloadApiTimer;

        private static readonly int maxRetry = 2;

        // Resize animation variables
        private static System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private int _Stop;
        private double _ratioHeight;
        private double _ratioWidth;
        private double _height;
        private double _width;

        #endregion

        #region Resize and Resize Timer

        public void Resize(double passedHeight, double passedWidth)
        {
            _height = passedHeight;
            _width = passedWidth;

            _timer.Enabled = true;
            _timer.Start();
        }

        private void Timer_Tick(object myObject, EventArgs myEventArgs)
        {
            if (_Stop == 0)
            {
                _ratioHeight = (Height - _height) / 5 * -1;
                _ratioWidth = (Width - _width) / 5 * -1;
            }

            _Stop++;

            Height += _ratioHeight;
            Width += _ratioWidth;

            if (_Stop == 5)
            {
                _timer.Stop();
                _timer.Enabled = false;
                _timer.Dispose();

                _Stop = 0;

                Height = _height;
                Width = _width;

                SetMainScrollViewerBarsVisibility(ScrollBarVisibility.Auto);
            }
        }

        #endregion

        #region Click Events

        private void AccountButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button btn)
            {
                btn.Opacity = 0.5;

                holdingButton = btn;

                mouseHoldTimer = new Timer(1000);
                mouseHoldTimer.Elapsed += MouseHoldTimer_Elapsed;
                mouseHoldTimer.Enabled = true;
                mouseHoldTimer.Start();
            }
        }

        private void MouseHoldTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            mouseHoldTimer.Stop();

            if (holdingButton != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => holdingButton.Opacity = 1));
                dragging = true;
            }
        }

        private void AccountButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button btn)
            {
                holdingButton = null;
                btn.Opacity = 1;
                dragging = false;
            }
        }

        private void AccountButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button btn)
            {
                holdingButton = null;
                btn.Opacity = 1;
                dragging = false;
            }
        }

        private void AccountButton_MouseLeave(TextBlock accountText)
        {
            accountText.Visibility = Visibility.Collapsed;
        }

        private void AccountButton_MouseEnter(TextBlock accountText)
        {
            accountText.Visibility = Visibility.Visible;
        }

        private void AccountButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Button btn)
            {
                if (dragging == false) return;

                btn.Opacity = 1;

                var mousePoint = e.GetPosition(this);

                var marginLeft = (int)mousePoint.X - (int)btn.Width / 2;
                var marginTop = (int)mousePoint.Y - (int)btn.Height / 2;

                btn.Margin = new Thickness(marginLeft, marginTop, 0, 0);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddAccount();
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                // Login with clicked button's index, which stored in Tag.
                var index = int.Parse(btn.Tag.ToString());
                Login(index, 0);
            }
        }

        private void TaskbarIconLoginItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item) Login(int.Parse(item.Tag.ToString()), 0);
        }

        private void AccountButtonSetTimeout_Click(int index, DateTime timeout)
        {
            if (timeout != null && timeout != new DateTime())
                encryptedAccounts[index].Timeout = timeout;
            else
                //MessageBox.Show("Error setting account timeout.");
                return;

            SerializeAccounts();
        }

        private void AccountButtonSetCustomTimeout_Click(int index)
        {
            var setTimeoutWindow = new SetTimeoutWindow(encryptedAccounts[index].Timeout);
            setTimeoutWindow.ShowDialog();

            if (setTimeoutWindow.timeout != null && setTimeoutWindow.timeout != new DateTime())
            {
                encryptedAccounts[index].Timeout = setTimeoutWindow.timeout;
            }
            else
            {
                MessageBox.Show("Error setting account timeout.");
                return;
            }

            SerializeAccounts();
        }

        private void AccountButtonClearTimeout_Click(int index)
        {
            encryptedAccounts[index].Timeout = null;
            SerializeAccounts();
        }

        private void AccountButtonExport_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var index = int.Parse(btn.Tag.ToString());

                // Check if this index has already been added.
                // Remove if it is, add if it isn't.
                if (exportAccounts.ContainsKey(index))
                {
                    exportAccounts.Remove(index);
                    btn.Opacity = 1;
                }
                else
                {
                    exportAccounts.Add(index, encryptedAccounts[index]);
                    btn.Opacity = 0.5;
                }
            }
        }

        public async Task ReloadAccount_ClickAsync(int index)
        {
            await ReloadAccount(encryptedAccounts[index]);
            SerializeAccounts();
            MessageBox.Show("Done!");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new SettingsWindow();
            settingsDialog.ShowDialog();

            settings.User.AccountsPerRow = settingsDialog.AccountsPerRow;

            var previousPass = ePassword;

            if (settingsDialog.Decrypt)
            {
                Utils.Serialize(encryptedAccounts);
                ePassword = "";
            }
            else if (settingsDialog.Password != null)
            {
                ePassword = settingsDialog.Password;

                if (previousPass != ePassword) Utils.PasswordSerialize(encryptedAccounts, ePassword);
            }

            LoadSettings();
            RefreshWindow();
        }

        private void DeleteBannedAccounts_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete all banned accounts?" +
                                         "\nThis action is perminant and cannot be undone!", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var accountsToDelete = encryptedAccounts.Where(a => a.NumberOfVacBans > 0 || a.NumberOfGameBans > 0).ToList();
                accountsToDelete.ForEach(a => encryptedAccounts.Remove(a));


                SerializeAccounts();
            }
        }

        private void GitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(repositoryUrl);
        }

        private async void Ver_Click(object sender, RoutedEventArgs e)
        {
            if (await UpdateCheck.CheckForUpdate(UpdateCheckUrl, repositoryUrl) < 1)
                MessageBox.Show(Process.GetCurrentProcess().ProcessName + " is up to date!");
        }

        private void RefreshMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RefreshWindow();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SortAlphabetical_Click(object sender, RoutedEventArgs e)
        {
            SortAccounts(0);
        }

        private void ShuffleAccounts_Click(object sender, RoutedEventArgs e)
        {
            SortAccounts(1);
        }

        private void ImportFromFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Utils.ImportAccountFile();
            RefreshWindow();
        }

        private void ExportAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Utils.ExportAccountFile();
        }

        private void ExportSelectedAccount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn) ExportAccount(int.Parse(btn.Tag.ToString()));
        }

        private async void ReloadAccounts_Click(object sender, RoutedEventArgs e)
        {
            await ReloadAccountsAsync();
        }


        private void ShowWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private void CopyUsernameToClipboard(int index)
        {
            try
            {
                Clipboard.SetText(decryptedAccounts[index].Name);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void CopyPasswordToClipboard(int index)
        {
            try
            {
                Clipboard.SetText(decryptedAccounts[index].Password);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void CopyProfileUrlToClipboard(int index)
        {
            try
            {
                Clipboard.SetText(decryptedAccounts[index].ProfUrl);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion

        #region Account Button State Handling

        private void ExportSelectedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            exporting = true;
            FileMenuItem.IsEnabled = false;
            EditMenuItem.IsEnabled = false;

            exportAccounts = new Dictionary<int, Account>();

            if (settings.User.ListView)
            {
                AccountsDataGrid.SelectionMode = DataGridSelectionMode.Extended;
                Application.Current.Resources["AccountGridActionHighlightColor"] = Brushes.Green;
            }
            else
            {
                AddButton.Visibility = Visibility.Hidden;
                ExportButton.Visibility = Visibility.Visible;
                CancelExportButton.Visibility = Visibility.Visible;

                var buttonGridCollection = buttonGrid.Children.OfType<Grid>();

                foreach (var accountButtonGrid in buttonGridCollection)
                {
                    var accountButton = accountButtonGrid.Children.OfType<Button>().FirstOrDefault();

                    accountButton.Style = (Style)Resources["ExportButtonStyle"];
                    accountButton.Click -= AccountButton_Click;
                    accountButton.Click += AccountButtonExport_Click;
                    accountButton.PreviewMouseLeftButtonDown -= AccountButton_MouseDown;
                    accountButton.PreviewMouseLeftButtonUp -= AccountButton_MouseUp;
                    accountButton.MouseLeave -= AccountButton_MouseLeave;
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportSelectedAccounts();
        }

        private void ExportSelectedAccounts()
        {
            if (settings.User.ListView)
                for (var i = 0; i < AccountsDataGrid.SelectedItems.Count; i++)
                {
                    exportAccounts.Add(i, AccountsDataGrid.SelectedItems[i] as Account);
                }

            if (exportAccounts.Count > 0)
                Utils.ExportSelectedAccounts(exportAccounts.Values.ToList());
            else
                MessageBox.Show("No accounts selected to export!");

            ResetFromExportOrDelete();
        }

        private void CancelExportButton_Click(object sender, RoutedEventArgs e)
        {
            ResetFromExportOrDelete();
        }

        private void DeleteAllAccountsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (encryptedAccounts.Count > 0)
            {
                var messageBoxResult =
                    MessageBox.Show("Are you sure you want to delete all accounts?\nThis action will perminantly delete the account data file.", "Confirm",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (messageBoxResult == MessageBoxResult.Yes)
                    if (IsPasswordProtected() && VerifyPassword() || !IsPasswordProtected())
                    {
                        try
                        {
                            File.Delete(dataFile);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                        RefreshWindow();
                    }
            }
        }

        private void DeleteSelectedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            deleting = true;
            deleteAccounts = new Dictionary<int, Account>();

            FileMenuItem.IsEnabled = false;
            EditMenuItem.IsEnabled = false;

            if (settings.User.ListView)
            {
                AccountsDataGrid.SelectionMode = DataGridSelectionMode.Extended;
                Application.Current.Resources["AccountGridActionHighlightColor"] = Brushes.Red;
            }
            else
            {
                AddButton.Visibility = Visibility.Hidden;
                DeleteButton.Visibility = Visibility.Visible;
                CancelExportButton.Visibility = Visibility.Visible;

                var buttonGridCollection = buttonGrid.Children.OfType<Grid>();

                foreach (var accountButtonGrid in buttonGridCollection)
                {
                    var accountButton = accountButtonGrid.Children.OfType<Button>().FirstOrDefault();

                    accountButton.Style = (Style)Resources["DeleteButtonStyle"];
                    accountButton.Click -= AccountButton_Click;
                    accountButton.Click += AccountButtonDelete_Click;
                    accountButton.PreviewMouseLeftButtonDown -= AccountButton_MouseDown;
                    accountButton.PreviewMouseLeftButtonUp -= AccountButton_MouseUp;
                    accountButton.MouseLeave -= AccountButton_MouseLeave;
                }
            }
        }

        private void AccountButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var index = int.Parse(btn.Tag.ToString());

                // Check if this index has already been added.
                // Remove if it is, add if it isn't.
                if (deleteAccounts.ContainsKey(index))
                {
                    deleteAccounts.Remove(index);
                    btn.Opacity = 1;
                }
                else
                {
                    deleteAccounts.Add(index, encryptedAccounts[index]);
                    btn.Opacity = 0.5;
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedAccounts();
        }

        private void DeleteSelectedAccounts()
        {
            if (settings.User.ListView)
                for (var i = 0; i < AccountsDataGrid.SelectedItems.Count; i++)
                {
                    deleteAccounts.Add(i, AccountsDataGrid.SelectedItems[i] as Account);
                }

            if (deleteAccounts.Count > 0)
            {
                var messageBoxResult = MessageBox.Show("Are you sure you want to delete the selected accounts?", "Confirm", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    foreach (var account in deleteAccounts.Values.ToList())
                    {
                        encryptedAccounts.Remove(account);
                    }

                    SerializeAccounts();
                }
            }
            else
            {
                MessageBox.Show("No accounts selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            ResetFromExportOrDelete();
        }

        private void ResetFromExportOrDelete()
        {
            FileMenuItem.IsEnabled = true;
            EditMenuItem.IsEnabled = true;

            if (settings.User.ListView)
            {
                AccountsDataGrid.SelectionMode = DataGridSelectionMode.Single;
            }
            else
            {
                if (settings.User.HideAddButton) AddButton.Visibility = Visibility.Visible;

                DeleteButton.Visibility = Visibility.Hidden;
                ExportButton.Visibility = Visibility.Hidden;
                CancelExportButton.Visibility = Visibility.Hidden;

                var buttonGridCollection = buttonGrid.Children.OfType<Grid>();

                foreach (var accountButtonGrid in buttonGridCollection)
                {
                    var accountButton = accountButtonGrid.Children.OfType<Button>().FirstOrDefault();

                    accountButton.Style = (Style)Resources["SAMButtonStyle"];
                    accountButton.Click -= AccountButtonExport_Click;
                    accountButton.Click -= AccountButtonDelete_Click;
                    accountButton.Click += AccountButton_Click;
                    accountButton.PreviewMouseLeftButtonDown += AccountButton_MouseDown;
                    accountButton.PreviewMouseLeftButtonUp += AccountButton_MouseUp;
                    accountButton.MouseLeave += AccountButton_MouseLeave;

                    accountButton.Opacity = 1;
                }
            }

            deleting = false;
            exporting = false;
        }

        private void AccountsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AccountsDataGrid.SelectedItem != null && deleting == false)
            {
                var account = AccountsDataGrid.SelectedItem as Account;
                var index = encryptedAccounts.FindIndex(a => a.Name == account.Name);
                Login(index, 0);
            }
        }

        private void AccountsDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (exporting)
            {
                AccountsDataGrid.ContextMenu = GenerateAltActionContextMenu(AltActionType.EXPORTING);
            }
            else if (deleting)
            {
                AccountsDataGrid.ContextMenu = GenerateAltActionContextMenu(AltActionType.DELETING);
            }
            else if (AccountsDataGrid.SelectedItem != null)
            {
                var account = AccountsDataGrid.SelectedItem as Account;
                var index = encryptedAccounts.FindIndex(a => a.Name == account.Name);
                AccountsDataGrid.ContextMenu = GenerateAccountContextMenu(account, index);
            }
        }

        private void AccountsDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var depObject = (DependencyObject)e.OriginalSource;

            while (depObject != null && !(depObject is DataGridColumnHeader) && !(depObject is DataGridRow))
            {
                depObject = VisualTreeHelper.GetParent(depObject);
            }

            if (depObject == null || depObject is DataGridColumnHeader) AccountsDataGrid.ContextMenu = null;
        }

        #endregion
    }
}