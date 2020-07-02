using System;
using System.Collections.Generic;

namespace SAM
{
    class UserSettings
    {
        #region General 

        public bool ClearUserData { get { return (bool)KeyValuePairs[SamSettings.ClearUserData]; } set { KeyValuePairs[SamSettings.ClearUserData] = value; } }
        public bool HideAddButton { get { return (bool)KeyValuePairs[SamSettings.HideAddButton]; } set { KeyValuePairs[SamSettings.HideAddButton] = value; } }
        public bool PasswordProtect { get { return (bool)KeyValuePairs[SamSettings.PasswordProtect]; } set { KeyValuePairs[SamSettings.PasswordProtect] = value; } }
        public bool MinimizeToTray { get { return (bool)KeyValuePairs[SamSettings.MinimizeToTray]; } set { KeyValuePairs[SamSettings.MinimizeToTray] = value; } }
        public bool RememberPassword { get { return (bool)KeyValuePairs[SamSettings.RememberPassword]; } set { KeyValuePairs[SamSettings.RememberPassword] = value; } }
        public bool StartMinimized { get { return (bool)KeyValuePairs[SamSettings.StartMinimized]; } set{ KeyValuePairs[SamSettings.StartMinimized] = value; } }
        public bool StartWithWindows { get { return (bool)KeyValuePairs[SamSettings.StartWithWindows]; } set { KeyValuePairs[SamSettings.StartWithWindows] = value; } }
        public int AccountsPerRow { get { return (int)KeyValuePairs[SamSettings.AccountsPerRow]; } set { KeyValuePairs[SamSettings.AccountsPerRow] = value; } }
        public int SleepTime { get { return (int)KeyValuePairs[SamSettings.SleepTime]; } set { KeyValuePairs[SamSettings.SleepTime] = value; } }
        public bool CheckForUpdates { get { return (bool)KeyValuePairs[SamSettings.CheckForUpdates]; } set { KeyValuePairs[SamSettings.CheckForUpdates] = value;  } }
        public bool CloseOnLogin { get { return (bool)KeyValuePairs[SamSettings.CloseOnLogin]; } set { KeyValuePairs[SamSettings.CheckForUpdates] = value; } }
        public bool ListView { get { return (bool)KeyValuePairs[SamSettings.ListView]; } set { KeyValuePairs[SamSettings.ListView] = value; } }
        public bool SandboxMode { get { return (bool)KeyValuePairs[SamSettings.SandboxMode]; } set { KeyValuePairs[SamSettings.SandboxMode] = value; } }

        #endregion

        #region AutoLog
        
        public bool LoginRecentAccount { get { return (bool)KeyValuePairs[SamSettings.LoginRecentAccount] ; } set { KeyValuePairs[SamSettings.LoginRecentAccount] = value; } }
        public int RecentAccountIndex { get { return (int)KeyValuePairs[SamSettings.RecentAccountIndex]; } set{ KeyValuePairs[SamSettings.RecentAccountIndex] = value; } }
        public bool LoginSelectedAccount { get { return (bool)KeyValuePairs[SamSettings.LoginSelectedAccount]; } set { KeyValuePairs[SamSettings.LoginSelectedAccount] = value; } }
        public int SelectedAccountIndex { get { return (int)KeyValuePairs[SamSettings.SelectedAccountIndex]; } set { KeyValuePairs[SamSettings.SelectedAccountIndex] = value; } }
        public VirtualInputMethod VirtualInputMethod { get { return (VirtualInputMethod)KeyValuePairs[SamSettings.InputMethod]; } set { KeyValuePairs[SamSettings.InputMethod] = value; } }
        public bool HandleMicrosoftIME { get { return (bool)KeyValuePairs[SamSettings.HandleIme]; } set { KeyValuePairs[SamSettings.HandleIme] = value; } }
        public bool IME2FAOnly { get { return (bool)KeyValuePairs[SamSettings.Ime2FaOnly]; } set { KeyValuePairs[SamSettings.Ime2FaOnly] = value; } }

        #endregion

        #region Customize

        public string Theme { get { return (string)KeyValuePairs[SamSettings.Theme]; } set { KeyValuePairs[SamSettings.Theme] = value; } }
        public string Accent { get { return (string)KeyValuePairs[SamSettings.Accent]; } set { KeyValuePairs[SamSettings.Accent] = value; } }
        public int ButtonSize { get { return (int)KeyValuePairs[SamSettings.ButtonSize]; } set { KeyValuePairs[SamSettings.ButtonSize] = value; } }
        public string ButtonColor { get { return (string)KeyValuePairs[SamSettings.ButtonColor]; } set { KeyValuePairs[SamSettings.ButtonColor] = value; } }
        public int ButtonFontSize { get { return (int)KeyValuePairs[SamSettings.ButtonFontSize]; } set { KeyValuePairs[SamSettings.ButtonFontSize] = value; } }
        public string ButtonFontColor { get { return (string)KeyValuePairs[SamSettings.ButtonFontColor]; } set { KeyValuePairs[SamSettings.ButtonFontColor] = value; } }
        public string ButtonBannerColor { get { return (string)KeyValuePairs[SamSettings.ButtonBannerColor]; } set { KeyValuePairs[SamSettings.ButtonBannerColor] = value; } }
        public int BannerFontSize { get { return (int)KeyValuePairs[SamSettings.ButtonBannerFontSize]; } set { KeyValuePairs[SamSettings.ButtonBannerFontSize] = value; } }
        public string BannerFontColor { get { return (string)KeyValuePairs[SamSettings.ButtonBannerFontColor]; } set { KeyValuePairs[SamSettings.ButtonBannerFontColor] = value; } }
        public bool HideBanIcons { get { return (bool)KeyValuePairs[SamSettings.HideBanIcons]; } set { KeyValuePairs[SamSettings.HideBanIcons] = value; } }

        #endregion

        #region Steam

        public string SteamPath { get { return (string)KeyValuePairs[SamSettings.SteamPath]; } set { KeyValuePairs[SamSettings.SteamPath] = value; } }
        public string ApiKey { get { return (string)KeyValuePairs[SamSettings.SteamApiKey]; } set { KeyValuePairs[SamSettings.SteamApiKey] = value; } }
        public bool AutoReloadEnabled { get { return (bool)KeyValuePairs[SamSettings.AutoReloadEnabled]; } set { KeyValuePairs[SamSettings.AutoReloadEnabled] = value; } }
        public int AutoReloadInterval { get { return (int)KeyValuePairs[SamSettings.AutoReloadInterval]; } set { KeyValuePairs[SamSettings.AutoReloadInterval] = value; } }
        public DateTime? LastAutoReload { 

            get {
                try
                {
                    return Convert.ToDateTime(KeyValuePairs[SamSettings.LastAutoReload]);
                }
                catch
                {
                    return null;
                }
            } 
            set { KeyValuePairs[SamSettings.LastAutoReload] = value; } 
        }
         
        #endregion

        #region Parameters

        public bool CafeAppLaunch { get { return (bool)KeyValuePairs[SamSettings.CafeAppLaunchParameter]; } set { KeyValuePairs[SamSettings.CafeAppLaunchParameter] = value; } }
        public bool ClearBeta { get { return (bool)KeyValuePairs[SamSettings.ClearBetaParameter]; } set { KeyValuePairs[SamSettings.ClearBetaParameter] = value; } }
        public bool Console { get { return (bool)KeyValuePairs[SamSettings.ConsoleParameter]; } set { KeyValuePairs[SamSettings.ConsoleParameter] = value; } }
        public bool Developer { get { return (bool)KeyValuePairs[SamSettings.DeveloperParameter]; } set { KeyValuePairs[SamSettings.DeveloperParameter] = value; } }
        public bool ForceService { get { return (bool)KeyValuePairs[SamSettings.ForceServiceParameter]; } set { KeyValuePairs[SamSettings.ForceServiceParameter] = value; } }
        public bool Login { get { return (bool)KeyValuePairs[SamSettings.LoginParameter]; } set { KeyValuePairs[SamSettings.LoginParameter] = value; } }
        public bool NoCache { get { return (bool)KeyValuePairs[SamSettings.NoCacheParameter]; } set { KeyValuePairs[SamSettings.NoCacheParameter] = value; } }
        public bool NoVerifyFiles { get { return (bool)KeyValuePairs[SamSettings.NoVerifyFilesParameter]; } set { KeyValuePairs[SamSettings.NoVerifyFilesParameter] = value; } }
        public bool Silent { get { return (bool)KeyValuePairs[SamSettings.SilentParameter]; } set { KeyValuePairs[SamSettings.SilentParameter] = value; } }
        public bool SingleCore { get { return (bool)KeyValuePairs[SamSettings.SingleCoreParameter]; } set { KeyValuePairs[SamSettings.SingleCoreParameter] = value; } }
        public bool TCP { get { return (bool)KeyValuePairs[SamSettings.TcpParameter]; } set { KeyValuePairs[SamSettings.TcpParameter] = value; } }
        public bool TenFoot { get { return (bool)KeyValuePairs[SamSettings.TenFootParameter]; } set { KeyValuePairs[SamSettings.TenFootParameter] = value; } }
        public bool CustomParameters { get { return (bool)KeyValuePairs[SamSettings.CustomParameters]; } set { KeyValuePairs[SamSettings.CustomParameters] = value; } }
        public string CustomParametersValue { get { return (string)KeyValuePairs[SamSettings.CustomParametersValue]; } set { KeyValuePairs[SamSettings.CustomParametersValue] = value; } }

        #endregion

        #region Location

        public double WindowTop { get; set; }
        public double WindowLeft { get; set; }
        public double ListViewHeight { get { return Convert.ToDouble(KeyValuePairs[SamSettings.ListViewHeight]); } set { KeyValuePairs[SamSettings.ListViewHeight] = value; } }
        public double ListViewWidth { get { return Convert.ToDouble(KeyValuePairs[SamSettings.ListViewWidth]); } set { KeyValuePairs[SamSettings.ListViewWidth] = value; } }

        #endregion

        #region Columns

        public int NameColumnIndex { get { return (int)KeyValuePairs[SamSettings.NameColumnIndex]; } set { KeyValuePairs[SamSettings.NameColumnIndex] = value; } }
        public int DescriptionColumnIndex { get { return (int)KeyValuePairs[SamSettings.DescriptionColumnIndex]; } set { KeyValuePairs[SamSettings.DescriptionColumnIndex] = value; } }
        public int TimeoutColumnIndex { get { return (int)KeyValuePairs[SamSettings.TimeoutColumnIndex]; } set { KeyValuePairs[SamSettings.TimeoutColumnIndex] = value; } }
        public int VacBansColumnIndex { get { return (int)KeyValuePairs[SamSettings.VacBansColumnIndex]; } set { KeyValuePairs[SamSettings.VacBansColumnIndex] = value; } }
        public int GameBanColumnIndex { get { return (int)KeyValuePairs[SamSettings.GameBansColumnIndex]; } set { KeyValuePairs[SamSettings.GameBansColumnIndex] = value; } }
        public int EconomyBanColumnIndex { get { return (int)KeyValuePairs[SamSettings.EcoBanColumnIndex]; } set { KeyValuePairs[SamSettings.EcoBanColumnIndex] = value; } }
        public int LastBanColumnIndex { get { return (int)KeyValuePairs[SamSettings.LastBanColumnIndex]; } set { KeyValuePairs[SamSettings.LastBanColumnIndex] = value; } }

        #endregion

        public Dictionary<string, object> KeyValuePairs = new Dictionary<string, object>()
        {
            { SamSettings.ClearUserData, false },
            { SamSettings.HideAddButton, false },
            { SamSettings.PasswordProtect, false },
            { SamSettings.MinimizeToTray, false },
            { SamSettings.RememberPassword, false },
            { SamSettings.StartMinimized, false },
            { SamSettings.StartWithWindows, false },
            { SamSettings.AccountsPerRow, 5 },
            { SamSettings.SleepTime, 2 },
            { SamSettings.CheckForUpdates, true },
            { SamSettings.CloseOnLogin, false },
            { SamSettings.ListView, false },
            { SamSettings.SandboxMode, false },

            { SamSettings.LoginRecentAccount, false },
            { SamSettings.RecentAccountIndex, -1 },
            { SamSettings.LoginSelectedAccount, false },
            { SamSettings.SelectedAccountIndex, -1 },
            { SamSettings.InputMethod, VirtualInputMethod.SendMessage },
            { SamSettings.HandleIme, false },
            { SamSettings.Ime2FaOnly, false },

            { SamSettings.SteamPath, string.Empty },
            { SamSettings.SteamApiKey, string.Empty },
            { SamSettings.AutoReloadEnabled, false },
            { SamSettings.AutoReloadInterval, 30 },
            { SamSettings.LastAutoReload, string.Empty },

            { SamSettings.Theme, "BaseDark" },
            { SamSettings.Accent, "Blue" },
            { SamSettings.ButtonSize, 100 },
            { SamSettings.ButtonColor, "#FFDDDDDD" },
            { SamSettings.ButtonFontSize, 0 },
            { SamSettings.ButtonFontColor, "#FF000000" },
            { SamSettings.ButtonBannerColor, "#7F000000" },
            { SamSettings.ButtonBannerFontSize, 0 },
            { SamSettings.ButtonBannerFontColor, "#FFFFFF" },
            { SamSettings.HideBanIcons, false },

            { SamSettings.CafeAppLaunchParameter, false },
            { SamSettings.ClearBetaParameter, false },
            { SamSettings.ConsoleParameter, false },
            { SamSettings.DeveloperParameter, false },
            { SamSettings.ForceServiceParameter, false },
            { SamSettings.LoginParameter, true },
            { SamSettings.NoCacheParameter, false },
            { SamSettings.NoVerifyFilesParameter, false },
            { SamSettings.SilentParameter, false },
            { SamSettings.SingleCoreParameter, false },
            { SamSettings.TcpParameter, false },
            { SamSettings.TenFootParameter, false },
            { SamSettings.CustomParameters, false },
            { SamSettings.CustomParametersValue, string.Empty },

            { SamSettings.ListViewHeight, 300 },
            { SamSettings.ListViewWidth, 750 },

            { SamSettings.NameColumnIndex, 0},
            { SamSettings.DescriptionColumnIndex, 1 },
            { SamSettings.TimeoutColumnIndex, 2 },
            { SamSettings.VacBansColumnIndex, 3 },
            { SamSettings.GameBansColumnIndex, 4 },
            { SamSettings.EcoBanColumnIndex, 5 },
            { SamSettings.LastBanColumnIndex, 6 }
        };
    }
}
