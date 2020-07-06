using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using HtmlAgilityPack;
using System.Text;
using Win32Interop.WinHandles;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using SAM.Extensions;
using SAM.Helpers;
using SAM.Models;
using SAM.Services;

namespace SAM
{
    // TODO: God object.
    internal class Utils
    {
        #region dll imports

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        #endregion

        public const int WmKeydown = 0x0100;
        public const int WmKeyup = 0x0101;
        public const int WmChar = 0x0102;
        public const int VkReturn = 0x0D;
        public const int VkTab = 0x09;
        public const int VkSpace = 0x20;

        public const int ApiKeyLength = 32;

        private static readonly IImportService ImportService = new ImportService();

        public static void Serialize(List<Account> accounts)
        {
            var serializer = new XmlSerializer(accounts.GetType());
            var sw = new StreamWriter("info.dat");
            serializer.Serialize(sw, accounts);
            sw.Close();
        }

        public static void PasswordSerialize(List<Account> accounts, string password)
        {
            var serializer = new XmlSerializer(accounts.GetType());
            var memStream = new MemoryStream();
            serializer.Serialize(memStream, accounts);

            var serializedAccounts = Encoding.UTF8.GetString(memStream.ToArray());
            var encryptedAccounts = StringCipher.Encrypt(serializedAccounts, password);

            File.WriteAllText("info.dat", encryptedAccounts);

            memStream.Close();
        }

        public static List<Account> Deserialize(string file)
        {
            object obj = null;

            try
            {
                var stream = new StreamReader(file);
                var ser = new XmlSerializer(typeof(List<Account>));
                obj = ser.Deserialize(stream);
                stream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return (List<Account>) obj;
        }

        public static List<Account> PasswordDeserialize(string file, string password)
        {
            object obj = null;

            try
            {
                var contents = File.ReadAllText(file);
                contents = StringCipher.Decrypt(contents, password);

                var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));

                var ser = new XmlSerializer(typeof(List<Account>));
                obj = ser.Deserialize(stream);

                stream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return (List<Account>) obj;
        }

        public static void ImportAccountsFromList(IEnumerable<Account> accounts)
        {
            try
            {
                MainWindow.EncryptedAccounts = MainWindow.EncryptedAccounts.Concat(accounts).ToList();
                Serialize(MainWindow.EncryptedAccounts);
                MessageBox.Show("Account(s) imported!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ImportAccountFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                DefaultExt = ".dat",
                Filter = "SAM DAT Files (*.dat)|*.dat",
            };

            var result = dialog.ShowDialog();

            if (result != true)
                return;

            try
            {
                var tempAccounts = Deserialize(dialog.FileName);
                MainWindow.EncryptedAccounts = MainWindow.EncryptedAccounts.Concat(tempAccounts).ToList();
                Serialize(MainWindow.EncryptedAccounts);
                MessageBox.Show("Accounts imported!");
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ImportAccountsFromSteamInstance()
        {
            ImportService.ImportFromSteamInstance();
        }

        public static void ExportAccountFile()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    File.Copy("info.dat", dialog.SelectedPath + "\\info.dat");
                    MessageBox.Show("File exported to:\n" + dialog.SelectedPath);
                }
                catch (Exception m)
                {
                    MessageBox.Show(m.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static void ExportSelectedAccounts(List<Account> accounts)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();

            if (result != System.Windows.Forms.DialogResult.OK)
                return;

            try
            {
                var serializer = new XmlSerializer(accounts.GetType());
                var sw = new StreamWriter($"{dialog.SelectedPath}\\info.dat");
                serializer.Serialize(sw, accounts);
                sw.Close();

                MessageBox.Show($"File exported to:\n{dialog.SelectedPath}");
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void SetRememberPasswordKeyValue(int value)
        {
            try
            {
                var (hasValue, registryKey) = SteamHelper.TryGetRegistry();

                if (!hasValue)
                    throw new Exception();

                registryKey.SetValue("RememberPassword", value, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string CheckSteamPath()
        {
            var settingsFile = new IniFileService(SamSettings.FileName);

            var steamPath = settingsFile.Read(SamSettings.SteamPath, SamSettings.Sections.Steam);

            var tryCount = 0;

            // If Steam's filepath was not specified in settings or is invalid, attempt to find it and save it.
            while (steamPath == null || !File.Exists(steamPath + "\\steam.exe"))
            {
                // Check registry keys first.
                var regPath = SteamHelper.GetSteamPathFromRegistry();
                var localPath = System.Windows.Forms.Application.StartupPath;

                if (Directory.Exists(regPath))
                {
                    steamPath = regPath;
                }

                // Check if SAM is installed in Steam directory.
                // Useful for users in portable mode.
                else if (File.Exists(localPath + "\\steam.exe"))
                {
                    steamPath = localPath;
                }

                // Prompt user for manual selection.
                else
                {
                    if (tryCount == 0)
                    {
                        MessageBox.Show("Could not find Steam path automatically.\n\nPlease select Steam manually.");
                    }

                    // Create OpenFileDialog 
                    var dlg = new OpenFileDialog
                    {
                        DefaultExt = ".exe",
                        Filter = "Steam (*.exe)|*.exe",
                    };

                    // Display OpenFileDialog by calling ShowDialog method 
                    var result = dlg.ShowDialog();

                    // Get the selected file path
                    if (result == true)
                    {
                        steamPath = Path.GetDirectoryName(dlg.FileName) + "\\";
                    }
                }

                if (steamPath.IsNullOrEmpty())
                {
                    var messageBoxResult = MessageBox.Show("Steam path required!\n\nTry again?", "Confirm", MessageBoxButton.YesNo);

                    if (messageBoxResult.Equals(MessageBoxResult.No))
                        Environment.Exit(0);
                }

                tryCount++;
            }

            // Save path to settings file.
            settingsFile.Write(SamSettings.SteamPath, steamPath, SamSettings.Sections.Steam);

            return steamPath;
        }

        public static async Task<string> GetSteamIdFromProfileUrl(string url)
        {
            dynamic steamId = null;

            if (ApiKeyExists())
            {
                // Get SteamId for either getUserSummaries (if in ID64 format) or vanity URL if (/id/) format.

                if (url.Contains("/id/"))
                {
                    // Vanity URL API call.

                    url = url.TrimEnd('/');
                    url = url.Split('/').Last();

                    dynamic userJson = await Utils.GetSteamIdFromVanityUrl(url);
                    if (url.Length > 0)
                    {
                        if (userJson != null)
                        {
                            try
                            {
                                steamId = userJson.response.steamid;
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }
                else if (url.Contains("/profiles/"))
                {
                    // Standard user summaries API call.
                    var userJson = await Utils.GetUserInfoFromWebApiBySteamId(url);

                    if (userJson != null)
                    {
                        try
                        {
                            steamId = userJson.response.players[0].steamid;
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }

            return steamId;
        }

        public static string GetSteamIdFromConfig(string userName)
        {
            dynamic steamId = null;

            try
            {
                var steamPath = new IniFileService(SamSettings.FileName).Read(SamSettings.SteamPath, SamSettings.Sections.Steam);

                // Attempt to find Steam Id from steam config.
                dynamic config = VdfConvert.Deserialize(File.ReadAllText(steamPath + "config\\config.vdf"));
                var accounts = config.Value.Software.Valve.Steam.Accounts;

                VObject accountsObj = accounts;

                accountsObj.TryGetValue(userName, out var value);

                dynamic user = value;
                VValue userId = user.SteamID;
                steamId = userId.Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Convert.ToString(steamId);
        }

        public static async Task<dynamic> GetUserInfoFromConfigAndWebApi(string userName)
        {
            dynamic userJson = null;
            string steamId = GetSteamIdFromConfig(userName);

            if (steamId != null)
            {
                userJson = await GetUserInfoFromWebApiBySteamId(steamId);
            }

            return userJson;
        }

        public static async Task<dynamic> GetUserInfoFromWebApiBySteamId(string steamId)
        {
            var settingsFile = new IniFileService(SamSettings.FileName);
            var apiKey = settingsFile.Read(SamSettings.SteamApiKey, SamSettings.Sections.Steam);

            dynamic userJson = null;

            if (ApiKeyExists())
            {
                try
                {
                    var userUri = new Uri("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + apiKey + "&steamids=" + steamId);

                    using var client = new WebClient();
                    var userJsonString = await client.DownloadStringTaskAsync(userUri);
                    userJson = JToken.Parse(userJsonString);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return userJson;
        }

        public static async Task<List<dynamic>> GetUserInfosFromWepApi(List<string> steamIds)
        {
            var settingsFile = new Services.IniFileService(SamSettings.FileName);
            var apiKey = settingsFile.Read(SamSettings.SteamApiKey, SamSettings.Sections.Steam);

            var userInfos = new List<dynamic>();

            if (ApiKeyExists())
            {
                while (steamIds.Count > 0)
                {
                    IEnumerable<string> currentChunk;

                    // Api can only process 100 accounts at a time.
                    if (steamIds.Count > 100)
                    {
                        currentChunk = steamIds.Take(100);
                        steamIds = steamIds.Skip(100).ToList();
                    }
                    else
                    {
                        currentChunk = new List<string>(steamIds);
                        steamIds.Clear();
                    }

                    var currentIds = string.Join(",", currentChunk);

                    try
                    {
                        var userUri = new Uri("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + apiKey + "&steamids=" + currentIds);

                        using var client = new WebClient();
                        var userJsonString = await client.DownloadStringTaskAsync(userUri);
                        dynamic userInfoJson = JToken.Parse(userJsonString);
                        userInfos.Add(userInfoJson);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

            return userInfos;
        }

        public static async Task<dynamic> GetSteamIdFromVanityUrl(string vanity)
        {
            var settingsFile = new IniFileService(SamSettings.FileName);
            var apiKey = settingsFile.Read(SamSettings.SteamApiKey, SamSettings.Sections.Steam);

            dynamic userInfoJson = null;

            if (apiKey != null && apiKey.Length == 32)
            {
                try
                {
                    var userUri = new Uri("http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=" + apiKey + "&vanityurl=" + vanity);

                    using var client = new WebClient();
                    var userJsonString = await client.DownloadStringTaskAsync(userUri);
                    userInfoJson = JToken.Parse(userJsonString);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return userInfoJson;
        }

        public static async Task<dynamic> GetPlayerBansFromWebApi(string steamId)
        {
            var userBansJson = await GetPlayerBansFromWebApi(new List<string> { steamId });

            return userBansJson.Any() ? userBansJson[0].players[0] : null;
        }

        public static async Task<List<dynamic>> GetPlayerBansFromWebApi(List<string> steamIds)
        {
            var settingsFile = new IniFileService(SamSettings.FileName);
            var apiKey = settingsFile.Read(SamSettings.SteamApiKey, SamSettings.Sections.Steam);

            var userBans = new List<dynamic>();

            if (ApiKeyExists())
            {
                while (steamIds.Count > 0)
                {
                    IEnumerable<string> currentChunk;

                    // Api can only process 100 accounts at a time.
                    if (steamIds.Count > 100)
                    {
                        currentChunk = steamIds.Take(100);
                        steamIds = steamIds.Skip(100).ToList();
                    }
                    else
                    {
                        currentChunk = new List<string>(steamIds);
                        steamIds.Clear();
                    }

                    var currentIds = string.Join(",", currentChunk);

                    try
                    {
                        Uri userUri = new Uri("http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=" + apiKey + "&steamids=" + currentIds);

                        using var client = new WebClient();
                        var userJsonString = await client.DownloadStringTaskAsync(userUri);
                        dynamic userInfoJson = JToken.Parse(userJsonString);
                        userBans.Add(userInfoJson);
                    }
                    catch (Exception m)
                    {
                        MessageBox.Show(m.Message);
                    }
                }
            }

            return userBans;
        }

        public static async Task<string> HtmlAviScrapeAsync(string profUrl)
        {
            // If user entered profile url, get avatar jpg url
            if (profUrl.Length > 2)
            {
                // Verify url starts with valid prefix for HtmlWeb
                if (!profUrl.StartsWith("https://") && !profUrl.StartsWith("http://"))
                {
                    profUrl = "https://" + profUrl;
                }

                try
                {
                    HtmlWeb htmlWeb = new HtmlWeb();
                    HtmlDocument document = await htmlWeb.LoadFromWebAsync(profUrl);
                    IEnumerable<HtmlNode> enumerable = document.DocumentNode.Descendants().Where(n => n.HasClass("playerAvatarAutoSizeInner"));
                    HtmlNode htmlNode = enumerable.First().SelectSingleNode("img");
                    string url = htmlNode.GetAttributeValue("src", null);
                    return url;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return "";
        }

        public static WindowHandle GetSteamLoginWindow()
        {
            return TopLevelWindowUtils.FindWindow(wh =>
            wh.GetClassName().Equals("vguiPopupWindow")
            && wh.GetWindowText().Contains("Steam")
            && !wh.GetWindowText().Contains("-")
            && !wh.GetWindowText().Contains("—")
            && wh.GetWindowText().Length > 5);
        }

        public static WindowHandle GetSteamGuardWindow()
        {
            // Also checking for vguiPopupWindow class name to avoid catching things like browser tabs.
            var windowHandle = TopLevelWindowUtils.FindWindow(wh =>
            wh.GetClassName().Equals("vguiPopupWindow") &&
            (wh.GetWindowText().StartsWith("Steam Guard") ||
             wh.GetWindowText().StartsWith("Steam 令牌") ||
             wh.GetWindowText().StartsWith("Steam ガード")));
            return windowHandle;
        }

        public static WindowHandle GetSteamWarningWindow()
        {
            return TopLevelWindowUtils.FindWindow(wh =>
            wh.GetClassName().Equals("vguiPopupWindow") &&
            (wh.GetWindowText().StartsWith("Steam - ") ||
             wh.GetWindowText().StartsWith("Steam — ")));
        }

        public static Process WaitForSteamProcess(WindowHandle windowHandle)
        {
            Process process = null;

            // Wait for valid process to wait for input idle.
            Console.WriteLine("Waiting for it to be idle.");
            while (process == null)
            {
                GetWindowThreadProcessId(windowHandle.RawPtr, out var procId);

                // Wait for valid process id from handle.
                while (procId == 0)
                {
                    Thread.Sleep(10);
                    GetWindowThreadProcessId(windowHandle.RawPtr, out procId);
                }

                try
                {
                    process = Process.GetProcessById(procId);
                }
                catch
                {
                    process = null;
                }
            }

            return process;
        }

        /**
         * Because CapsLock is handled by system directly, thus sending
         * it to one particular window is invalid - a window could not
         * respond to CapsLock, only the system can.
         * 
         * For this reason, I break into a low-level API, which may cause
         * an inconsistency to the original `SendWait` method.
         * 
         * https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-keybd_event
         */
        public static void SendCapsLockGlobally()
        {
            // Press key down
            keybd_event((byte) System.Windows.Forms.Keys.CapsLock, 0, 0, 0);
            // Press key up
            keybd_event((byte) System.Windows.Forms.Keys.CapsLock, 0, 0x2, 0);
        }

        public static void SendCharacter(IntPtr hwnd, VirtualInputMethod inputMethod, char @char)
        {
            switch (inputMethod)
            {
                case VirtualInputMethod.SendMessage:
                    SendMessage(hwnd, WmChar, @char, IntPtr.Zero);
                    break;

                case VirtualInputMethod.PostMessage:
                    PostMessage(hwnd, WmChar, (IntPtr)@char, IntPtr.Zero);
                    break;

                default:
                    if (@char.IsSpecial())
                    {
                        if (inputMethod == VirtualInputMethod.SendWait)
                            System.Windows.Forms.SendKeys.SendWait($"{{{@char}}}");
                        else
                            System.Windows.Forms.SendKeys.Send($"{{{@char}}}");
                    }
                    else
                    {
                        if (inputMethod == VirtualInputMethod.SendWait)
                            System.Windows.Forms.SendKeys.SendWait(@char.ToString());
                        else
                            System.Windows.Forms.SendKeys.Send(@char.ToString());
                    }
                    break;
            }
        }

        public static void SendEnter(IntPtr hwnd, VirtualInputMethod inputMethod)
        {
            switch (inputMethod)
            {
                case VirtualInputMethod.SendMessage:
                    SendMessage(hwnd, WmKeydown, VkReturn, IntPtr.Zero);
                    SendMessage(hwnd, WmKeyup, VkReturn, IntPtr.Zero);
                    break;

                case VirtualInputMethod.PostMessage:
                    PostMessage(hwnd, WmKeydown, (IntPtr) VkReturn, IntPtr.Zero);
                    PostMessage(hwnd, WmKeyup, (IntPtr) VkReturn, IntPtr.Zero);
                    break;

                case VirtualInputMethod.SendWait:
                    System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                    break;
            }
        }

        public static void SendTab(IntPtr hwnd, VirtualInputMethod inputMethod)
        {
            switch (inputMethod)
            {
                case VirtualInputMethod.SendMessage:
                    SendMessage(hwnd, WmKeydown, VkTab, IntPtr.Zero);
                    SendMessage(hwnd, WmKeyup, VkTab, IntPtr.Zero);
                    break;

                case VirtualInputMethod.PostMessage:
                    PostMessage(hwnd, WmKeydown, (IntPtr)VkTab, IntPtr.Zero);
                    PostMessage(hwnd, WmKeyup, (IntPtr)VkTab, IntPtr.Zero);
                    break;

                case VirtualInputMethod.SendWait:
                    System.Windows.Forms.SendKeys.SendWait("{TAB}");
                    break;
            }
        }

        public static void SendSpace(IntPtr hwnd, VirtualInputMethod inputMethod)
        {
            switch (inputMethod)
            {
                case VirtualInputMethod.SendMessage:
                    SendMessage(hwnd, WmKeydown, VkSpace, IntPtr.Zero);
                    SendMessage(hwnd, WmKeyup, VkSpace, IntPtr.Zero);
                    break;

                case VirtualInputMethod.PostMessage:
                    PostMessage(hwnd, WmKeydown, (IntPtr)VkSpace, IntPtr.Zero);
                    PostMessage(hwnd, WmKeyup, (IntPtr)VkSpace, IntPtr.Zero);
                    break;

                case VirtualInputMethod.SendWait:
                    System.Windows.Forms.SendKeys.SendWait(" ");
                    break;
            }
        }

        public static void ClearSteamUserDataFolder(string steamPath, int sleepTime, int maxRetry)
        {
            var steamLoginWindow = GetSteamLoginWindow();
            var waitCount = 0;

            while (steamLoginWindow.IsValid && waitCount < maxRetry)
            {
                Thread.Sleep(sleepTime);
                waitCount++;
            }

            var path = $"{steamPath}\\userdata";

            if (Directory.Exists(path))
            {
                Console.WriteLine("Deleting userdata files...");
                Directory.Delete(path, true);
                Console.WriteLine("userdata files deleted!");
            }
            else
            {
                Console.WriteLine("userdata directory not found.");
            }
        }

        public static bool ApiKeyExists()
        {
            var settingsFile = new IniFileService(SamSettings.FileName);
            var apiKey = settingsFile.Read(SamSettings.SteamApiKey, SamSettings.Sections.Steam);
            return apiKey != null && apiKey.Length == ApiKeyLength;
        }

        public static bool ShouldAutoReload(DateTime? lastReload, int interval)
        {
            if (!lastReload.HasValue)
                return true;

            var offset = lastReload.Value.AddMinutes(interval);

            return offset.CompareTo(DateTime.Now) <= 0;
        }

        
    }
}