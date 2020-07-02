using System;
using System.Collections.Generic;

namespace SAM
{
    class UserSettings
    {
        #region General 

        public bool ClearUserData { get { return (bool)KeyValuePairs[SamSettings.CLEAR_USER_DATA]; } set { KeyValuePairs[SamSettings.CLEAR_USER_DATA] = value; } }
        public bool HideAddButton { get { return (bool)KeyValuePairs[SamSettings.HIDE_ADD_BUTTON]; } set { KeyValuePairs[SamSettings.HIDE_ADD_BUTTON] = value; } }
        public bool PasswordProtect { get { return (bool)KeyValuePairs[SamSettings.PASSWORD_PROTECT]; } set { KeyValuePairs[SamSettings.PASSWORD_PROTECT] = value; } }
        public bool MinimizeToTray { get { return (bool)KeyValuePairs[SamSettings.MINIMIZE_TO_TRAY]; } set { KeyValuePairs[SamSettings.MINIMIZE_TO_TRAY] = value; } }
        public bool RememberPassword { get { return (bool)KeyValuePairs[SamSettings.REMEMBER_PASSWORD]; } set { KeyValuePairs[SamSettings.REMEMBER_PASSWORD] = value; } }
        public bool StartMinimized { get { return (bool)KeyValuePairs[SamSettings.START_MINIMIZED]; } set{ KeyValuePairs[SamSettings.START_MINIMIZED] = value; } }
        public bool StartWithWindows { get { return (bool)KeyValuePairs[SamSettings.START_WITH_WINDOWS]; } set { KeyValuePairs[SamSettings.START_WITH_WINDOWS] = value; } }
        public int AccountsPerRow { get { return (int)KeyValuePairs[SamSettings.ACCOUNTS_PER_ROW]; } set { KeyValuePairs[SamSettings.ACCOUNTS_PER_ROW] = value; } }
        public int SleepTime { get { return (int)KeyValuePairs[SamSettings.SLEEP_TIME]; } set { KeyValuePairs[SamSettings.SLEEP_TIME] = value; } }
        public bool CheckForUpdates { get { return (bool)KeyValuePairs[SamSettings.CHECK_FOR_UPDATES]; } set { KeyValuePairs[SamSettings.CHECK_FOR_UPDATES] = value;  } }
        public bool CloseOnLogin { get { return (bool)KeyValuePairs[SamSettings.CLOSE_ON_LOGIN]; } set { KeyValuePairs[SamSettings.CHECK_FOR_UPDATES] = value; } }
        public bool ListView { get { return (bool)KeyValuePairs[SamSettings.LIST_VIEW]; } set { KeyValuePairs[SamSettings.LIST_VIEW] = value; } }
        public bool SandboxMode { get { return (bool)KeyValuePairs[SamSettings.SANDBOX_MODE]; } set { KeyValuePairs[SamSettings.SANDBOX_MODE] = value; } }

        #endregion

        #region AutoLog
        
        public bool LoginRecentAccount { get { return (bool)KeyValuePairs[SamSettings.LOGIN_RECENT_ACCOUNT] ; } set { KeyValuePairs[SamSettings.LOGIN_RECENT_ACCOUNT] = value; } }
        public int RecentAccountIndex { get { return (int)KeyValuePairs[SamSettings.RECENT_ACCOUNT_INDEX]; } set{ KeyValuePairs[SamSettings.RECENT_ACCOUNT_INDEX] = value; } }
        public bool LoginSelectedAccount { get { return (bool)KeyValuePairs[SamSettings.LOGIN_SELECTED_ACCOUNT]; } set { KeyValuePairs[SamSettings.LOGIN_SELECTED_ACCOUNT] = value; } }
        public int SelectedAccountIndex { get { return (int)KeyValuePairs[SamSettings.SELECTED_ACCOUNT_INDEX]; } set { KeyValuePairs[SamSettings.SELECTED_ACCOUNT_INDEX] = value; } }
        public VirtualInputMethod VirtualInputMethod { get { return (VirtualInputMethod)KeyValuePairs[SamSettings.INPUT_METHOD]; } set { KeyValuePairs[SamSettings.INPUT_METHOD] = value; } }
        public bool HandleMicrosoftIME { get { return (bool)KeyValuePairs[SamSettings.HANDLE_IME]; } set { KeyValuePairs[SamSettings.HANDLE_IME] = value; } }
        public bool IME2FAOnly { get { return (bool)KeyValuePairs[SamSettings.IME_2FA_ONLY]; } set { KeyValuePairs[SamSettings.IME_2FA_ONLY] = value; } }

        #endregion

        #region Customize

        public string Theme { get { return (string)KeyValuePairs[SamSettings.THEME]; } set { KeyValuePairs[SamSettings.THEME] = value; } }
        public string Accent { get { return (string)KeyValuePairs[SamSettings.ACCENT]; } set { KeyValuePairs[SamSettings.ACCENT] = value; } }
        public int ButtonSize { get { return (int)KeyValuePairs[SamSettings.BUTTON_SIZE]; } set { KeyValuePairs[SamSettings.BUTTON_SIZE] = value; } }
        public string ButtonColor { get { return (string)KeyValuePairs[SamSettings.BUTTON_COLOR]; } set { KeyValuePairs[SamSettings.BUTTON_COLOR] = value; } }
        public int ButtonFontSize { get { return (int)KeyValuePairs[SamSettings.BUTTON_FONT_SIZE]; } set { KeyValuePairs[SamSettings.BUTTON_FONT_SIZE] = value; } }
        public string ButtonFontColor { get { return (string)KeyValuePairs[SamSettings.BUTTON_FONT_COLOR]; } set { KeyValuePairs[SamSettings.BUTTON_FONT_COLOR] = value; } }
        public string ButtonBannerColor { get { return (string)KeyValuePairs[SamSettings.BUTTON_BANNER_COLOR]; } set { KeyValuePairs[SamSettings.BUTTON_BANNER_COLOR] = value; } }
        public int BannerFontSize { get { return (int)KeyValuePairs[SamSettings.BUTTON_BANNER_FONT_SIZE]; } set { KeyValuePairs[SamSettings.BUTTON_BANNER_FONT_SIZE] = value; } }
        public string BannerFontColor { get { return (string)KeyValuePairs[SamSettings.BUTTON_BANNER_FONT_COLOR]; } set { KeyValuePairs[SamSettings.BUTTON_BANNER_FONT_COLOR] = value; } }
        public bool HideBanIcons { get { return (bool)KeyValuePairs[SamSettings.HIDE_BAN_ICONS]; } set { KeyValuePairs[SamSettings.HIDE_BAN_ICONS] = value; } }

        #endregion

        #region Steam

        public string SteamPath { get { return (string)KeyValuePairs[SamSettings.STEAM_PATH]; } set { KeyValuePairs[SamSettings.STEAM_PATH] = value; } }
        public string ApiKey { get { return (string)KeyValuePairs[SamSettings.STEAM_API_KEY]; } set { KeyValuePairs[SamSettings.STEAM_API_KEY] = value; } }
        public bool AutoReloadEnabled { get { return (bool)KeyValuePairs[SamSettings.AUTO_RELOAD_ENABLED]; } set { KeyValuePairs[SamSettings.AUTO_RELOAD_ENABLED] = value; } }
        public int AutoReloadInterval { get { return (int)KeyValuePairs[SamSettings.AUTO_RELOAD_INTERVAL]; } set { KeyValuePairs[SamSettings.AUTO_RELOAD_INTERVAL] = value; } }
        public DateTime? LastAutoReload { 

            get {
                try
                {
                    return Convert.ToDateTime(KeyValuePairs[SamSettings.LAST_AUTO_RELOAD]);
                }
                catch
                {
                    return null;
                }
            } 
            set { KeyValuePairs[SamSettings.LAST_AUTO_RELOAD] = value; } 
        }
         
        #endregion

        #region Parameters

        public bool CafeAppLaunch { get { return (bool)KeyValuePairs[SamSettings.CAFE_APP_LAUNCH_PARAMETER]; } set { KeyValuePairs[SamSettings.CAFE_APP_LAUNCH_PARAMETER] = value; } }
        public bool ClearBeta { get { return (bool)KeyValuePairs[SamSettings.CLEAR_BETA_PARAMETER]; } set { KeyValuePairs[SamSettings.CLEAR_BETA_PARAMETER] = value; } }
        public bool Console { get { return (bool)KeyValuePairs[SamSettings.CONSOLE_PARAMETER]; } set { KeyValuePairs[SamSettings.CONSOLE_PARAMETER] = value; } }
        public bool Developer { get { return (bool)KeyValuePairs[SamSettings.DEVELOPER_PARAMETER]; } set { KeyValuePairs[SamSettings.DEVELOPER_PARAMETER] = value; } }
        public bool ForceService { get { return (bool)KeyValuePairs[SamSettings.FORCE_SERVICE_PARAMETER]; } set { KeyValuePairs[SamSettings.FORCE_SERVICE_PARAMETER] = value; } }
        public bool Login { get { return (bool)KeyValuePairs[SamSettings.LOGIN_PARAMETER]; } set { KeyValuePairs[SamSettings.LOGIN_PARAMETER] = value; } }
        public bool NoCache { get { return (bool)KeyValuePairs[SamSettings.NO_CACHE_PARAMETER]; } set { KeyValuePairs[SamSettings.NO_CACHE_PARAMETER] = value; } }
        public bool NoVerifyFiles { get { return (bool)KeyValuePairs[SamSettings.NO_VERIFY_FILES_PARAMETER]; } set { KeyValuePairs[SamSettings.NO_VERIFY_FILES_PARAMETER] = value; } }
        public bool Silent { get { return (bool)KeyValuePairs[SamSettings.SILENT_PARAMETER]; } set { KeyValuePairs[SamSettings.SILENT_PARAMETER] = value; } }
        public bool SingleCore { get { return (bool)KeyValuePairs[SamSettings.SINGLE_CORE_PARAMETER]; } set { KeyValuePairs[SamSettings.SINGLE_CORE_PARAMETER] = value; } }
        public bool TCP { get { return (bool)KeyValuePairs[SamSettings.TCP_PARAMETER]; } set { KeyValuePairs[SamSettings.TCP_PARAMETER] = value; } }
        public bool TenFoot { get { return (bool)KeyValuePairs[SamSettings.TEN_FOOT_PARAMETER]; } set { KeyValuePairs[SamSettings.TEN_FOOT_PARAMETER] = value; } }
        public bool CustomParameters { get { return (bool)KeyValuePairs[SamSettings.CUSTOM_PARAMETERS]; } set { KeyValuePairs[SamSettings.CUSTOM_PARAMETERS] = value; } }
        public string CustomParametersValue { get { return (string)KeyValuePairs[SamSettings.CUSTOM_PARAMETERS_VALUE]; } set { KeyValuePairs[SamSettings.CUSTOM_PARAMETERS_VALUE] = value; } }

        #endregion

        #region Location

        public double WindowTop { get; set; }
        public double WindowLeft { get; set; }
        public double ListViewHeight { get { return Convert.ToDouble(KeyValuePairs[SamSettings.LIST_VIEW_HEIGHT]); } set { KeyValuePairs[SamSettings.LIST_VIEW_HEIGHT] = value; } }
        public double ListViewWidth { get { return Convert.ToDouble(KeyValuePairs[SamSettings.LIST_VIEW_WIDTH]); } set { KeyValuePairs[SamSettings.LIST_VIEW_WIDTH] = value; } }

        #endregion

        #region Columns

        public int NameColumnIndex { get { return (int)KeyValuePairs[SamSettings.NAME_COLUMN_INDEX]; } set { KeyValuePairs[SamSettings.NAME_COLUMN_INDEX] = value; } }
        public int DescriptionColumnIndex { get { return (int)KeyValuePairs[SamSettings.DESCRIPTION_COLUMN_INDEX]; } set { KeyValuePairs[SamSettings.DESCRIPTION_COLUMN_INDEX] = value; } }
        public int TimeoutColumnIndex { get { return (int)KeyValuePairs[SamSettings.TIMEOUT_COLUMN_INDEX]; } set { KeyValuePairs[SamSettings.TIMEOUT_COLUMN_INDEX] = value; } }
        public int VacBansColumnIndex { get { return (int)KeyValuePairs[SamSettings.VAC_BANS_COLUMN_INDEX]; } set { KeyValuePairs[SamSettings.VAC_BANS_COLUMN_INDEX] = value; } }
        public int GameBanColumnIndex { get { return (int)KeyValuePairs[SamSettings.GAME_BANS_COLUMN_INDEX]; } set { KeyValuePairs[SamSettings.GAME_BANS_COLUMN_INDEX] = value; } }
        public int EconomyBanColumnIndex { get { return (int)KeyValuePairs[SamSettings.ECO_BAN_COLUMN_INDEX]; } set { KeyValuePairs[SamSettings.ECO_BAN_COLUMN_INDEX] = value; } }
        public int LastBanColumnIndex { get { return (int)KeyValuePairs[SamSettings.LAST_BAN_COLUMN_INDEX]; } set { KeyValuePairs[SamSettings.LAST_BAN_COLUMN_INDEX] = value; } }

        #endregion

        public Dictionary<string, object> KeyValuePairs = new Dictionary<string, object>()
        {
            { SamSettings.CLEAR_USER_DATA, false },
            { SamSettings.HIDE_ADD_BUTTON, false },
            { SamSettings.PASSWORD_PROTECT, false },
            { SamSettings.MINIMIZE_TO_TRAY, false },
            { SamSettings.REMEMBER_PASSWORD, false },
            { SamSettings.START_MINIMIZED, false },
            { SamSettings.START_WITH_WINDOWS, false },
            { SamSettings.ACCOUNTS_PER_ROW, 5 },
            { SamSettings.SLEEP_TIME, 2 },
            { SamSettings.CHECK_FOR_UPDATES, true },
            { SamSettings.CLOSE_ON_LOGIN, false },
            { SamSettings.LIST_VIEW, false },
            { SamSettings.SANDBOX_MODE, false },

            { SamSettings.LOGIN_RECENT_ACCOUNT, false },
            { SamSettings.RECENT_ACCOUNT_INDEX, -1 },
            { SamSettings.LOGIN_SELECTED_ACCOUNT, false },
            { SamSettings.SELECTED_ACCOUNT_INDEX, -1 },
            { SamSettings.INPUT_METHOD, VirtualInputMethod.SendMessage },
            { SamSettings.HANDLE_IME, false },
            { SamSettings.IME_2FA_ONLY, false },

            { SamSettings.STEAM_PATH, string.Empty },
            { SamSettings.STEAM_API_KEY, string.Empty },
            { SamSettings.AUTO_RELOAD_ENABLED, false },
            { SamSettings.AUTO_RELOAD_INTERVAL, 30 },
            { SamSettings.LAST_AUTO_RELOAD, string.Empty },

            { SamSettings.THEME, "BaseDark" },
            { SamSettings.ACCENT, "Blue" },
            { SamSettings.BUTTON_SIZE, 100 },
            { SamSettings.BUTTON_COLOR, "#FFDDDDDD" },
            { SamSettings.BUTTON_FONT_SIZE, 0 },
            { SamSettings.BUTTON_FONT_COLOR, "#FF000000" },
            { SamSettings.BUTTON_BANNER_COLOR, "#7F000000" },
            { SamSettings.BUTTON_BANNER_FONT_SIZE, 0 },
            { SamSettings.BUTTON_BANNER_FONT_COLOR, "#FFFFFF" },
            { SamSettings.HIDE_BAN_ICONS, false },

            { SamSettings.CAFE_APP_LAUNCH_PARAMETER, false },
            { SamSettings.CLEAR_BETA_PARAMETER, false },
            { SamSettings.CONSOLE_PARAMETER, false },
            { SamSettings.DEVELOPER_PARAMETER, false },
            { SamSettings.FORCE_SERVICE_PARAMETER, false },
            { SamSettings.LOGIN_PARAMETER, true },
            { SamSettings.NO_CACHE_PARAMETER, false },
            { SamSettings.NO_VERIFY_FILES_PARAMETER, false },
            { SamSettings.SILENT_PARAMETER, false },
            { SamSettings.SINGLE_CORE_PARAMETER, false },
            { SamSettings.TCP_PARAMETER, false },
            { SamSettings.TEN_FOOT_PARAMETER, false },
            { SamSettings.CUSTOM_PARAMETERS, false },
            { SamSettings.CUSTOM_PARAMETERS_VALUE, string.Empty },

            { SamSettings.LIST_VIEW_HEIGHT, 300 },
            { SamSettings.LIST_VIEW_WIDTH, 750 },

            { SamSettings.NAME_COLUMN_INDEX, 0},
            { SamSettings.DESCRIPTION_COLUMN_INDEX, 1 },
            { SamSettings.TIMEOUT_COLUMN_INDEX, 2 },
            { SamSettings.VAC_BANS_COLUMN_INDEX, 3 },
            { SamSettings.GAME_BANS_COLUMN_INDEX, 4 },
            { SamSettings.ECO_BAN_COLUMN_INDEX, 5 },
            { SamSettings.LAST_BAN_COLUMN_INDEX, 6 }
        };
    }
}
