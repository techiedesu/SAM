using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SAM.Services;

namespace SAM
{
    internal class SamSettings
    {
        public const string FileName = "SAMSettings.ini";

        public const string SectionGeneral = "Settings";
        public const string SectionAutolog = "AutoLog";
        public const string SectionCustomize = "Customize";
        public const string SectionSteam = "Steam";
        public const string SectionParameters = "Parameters";
        public const string SectionLocation = "Location";
        public const string SectionColumns = "Columns";

        public const string ClearUserData = "ClearUserData";
        public const string HideAddButton = "HideAddButton";
        public const string PasswordProtect = "PasswordProtect";
        public const string MinimizeToTray = "MinimizeToTray";
        public const string RememberPassword = "RememberPassword";
        public const string StartMinimized = "StartMinimized";
        public const string StartWithWindows = "StartWithWindows";
        public const string AccountsPerRow = "AccountsPerRow";
        public const string SleepTime = "SleepTime";
        public const string CheckForUpdates = "CheckForUpdates";
        public const string CloseOnLogin = "CloseOnLogin";
        public const string ListView = "ListView";
        public const string SandboxMode = "SandboxMode";

        public const string LoginRecentAccount = "LoginRecentAccount";
        public const string RecentAccountIndex = "RecentAccountIndex";
        public const string LoginSelectedAccount = "LoginSelectedAccount";
        public const string SelectedAccountIndex = "SelectedAccountIndex";
        public const string InputMethod = "InputMethod";
        public const string HandleIme = "HandleIME";
        public const string Ime2FaOnly = "IME_2FA_ONLY";

        public const string Theme = "Theme";
        public const string Accent = "Accent";
        public const string ButtonSize = "ButtonSize";
        public const string ButtonColor = "ButtonColor";
        public const string ButtonFontSize = "ButtonFontSize";
        public const string ButtonFontColor = "ButtonFontColor";
        public const string ButtonBannerColor = "ButtonBannerColor";
        public const string ButtonBannerFontSize = "ButtonBannerFontSize";
        public const string ButtonBannerFontColor = "ButtonBannerFontColor";
        public const string HideBanIcons = "HideBanIcons";

        public const string SteamPath = "Path";
        public const string SteamApiKey = "ApiKey";
        public const string AutoReloadEnabled = "AutoReloadEnabled";
        public const string AutoReloadInterval = "AutoReloadInterval";
        public const string LastAutoReload = "LastAutoReload";

        public const string CafeAppLaunchParameter = "cafeapplaunch";
        public const string ClearBetaParameter = "clearbeta";
        public const string ConsoleParameter = "console";
        public const string DeveloperParameter = "developer";
        public const string ForceServiceParameter = "forceservice";
        public const string LoginParameter = "login";
        public const string NoCacheParameter = "nocache";
        public const string NoVerifyFilesParameter = "noverifyfiles";
        public const string SilentParameter = "silent";
        public const string SingleCoreParameter = "single_core";
        public const string TcpParameter = "tcp";
        public const string TenFootParameter = "tenfoot";
        public const string CustomParameters = "customParameters";
        public const string CustomParametersValue = "customParametersValue";

        public const string WindowTop = "WindowTop";
        public const string WindowLeft = "WindowLeft";
        public const string ListViewHeight = "ListViewHeight";
        public const string ListViewWidth = "ListViewWidth";

        public const string LightTheme = "BaseLight";
        public const string DarkTheme = "BaseDark";

        public const string NameColumnIndex = "NameColumnIndex";
        public const string DescriptionColumnIndex = "DescriptionColumnIndex";
        public const string TimeoutColumnIndex = "TimeoutColumnIndex";
        public const string VacBansColumnIndex = "VacBansColumnIndex";
        public const string GameBansColumnIndex = "GameBansColumnIndex";
        public const string EcoBanColumnIndex = "EcoBanColumnIndex";
        public const string LastBanColumnIndex = "LastBanColumnIndex";
        public readonly UserSettings Default = new UserSettings();

        public IniFileService FileService = new IniFileService(FileName);

        public readonly Dictionary<string, string> KeyValuePairs = new Dictionary<string, string>
        {
            { ClearUserData, SectionGeneral },
            { HideAddButton, SectionGeneral },
            { PasswordProtect, SectionGeneral },
            { MinimizeToTray, SectionGeneral },
            { RememberPassword, SectionGeneral },
            { StartMinimized, SectionGeneral },
            { StartWithWindows, SectionGeneral },
            { AccountsPerRow, SectionGeneral },
            { SleepTime, SectionGeneral },
            { CheckForUpdates, SectionGeneral },
            { CloseOnLogin, SectionGeneral },
            { ListView, SectionGeneral },
            { SandboxMode, SectionGeneral },

            { LoginRecentAccount, SectionAutolog },
            { RecentAccountIndex, SectionAutolog },
            { LoginSelectedAccount, SectionAutolog },
            { SelectedAccountIndex, SectionAutolog },
            { InputMethod, SectionAutolog },
            { HandleIme, SectionAutolog },
            { Ime2FaOnly, SectionAutolog },

            { Theme, SectionCustomize },
            { Accent, SectionCustomize },
            { ButtonSize, SectionCustomize },
            { ButtonColor, SectionCustomize },
            { ButtonFontSize, SectionCustomize },
            { ButtonFontColor, SectionCustomize },
            { ButtonBannerColor, SectionCustomize },
            { ButtonBannerFontSize, SectionCustomize },
            { ButtonBannerFontColor, SectionCustomize },
            { HideBanIcons, SectionCustomize },

            { SteamPath, SectionSteam },
            { SteamApiKey, SectionSteam },
            { AutoReloadEnabled, SectionSteam },
            { AutoReloadInterval, SectionSteam },
            { LastAutoReload, SectionSteam },

            { CafeAppLaunchParameter, SectionParameters },
            { ClearBetaParameter, SectionParameters },
            { ConsoleParameter, SectionParameters },
            { DeveloperParameter, SectionParameters },
            { ForceServiceParameter, SectionParameters },
            { LoginParameter, SectionParameters },
            { NoCacheParameter, SectionParameters },
            { NoVerifyFilesParameter, SectionParameters },
            { SilentParameter, SectionParameters },
            { SingleCoreParameter, SectionParameters },
            { TcpParameter, SectionParameters },
            { TenFootParameter, SectionParameters },
            { CustomParameters, SectionParameters },
            { CustomParametersValue, SectionParameters },

            { ListViewHeight, SectionLocation },
            { ListViewWidth, SectionLocation },

            { NameColumnIndex, SectionColumns },
            { DescriptionColumnIndex, SectionColumns },
            { TimeoutColumnIndex, SectionColumns },
            { VacBansColumnIndex, SectionColumns },
            { GameBansColumnIndex, SectionColumns },
            { EcoBanColumnIndex, SectionColumns },
            { LastBanColumnIndex, SectionColumns },
        };

        public readonly Dictionary<string, string> ListViewColumns = new Dictionary<string, string>
        {
            { "Name", NameColumnIndex },
            { "Description", DescriptionColumnIndex },
            { "Timeout", TimeoutColumnIndex },
            { "VAC Bans", VacBansColumnIndex },
            { "Game Bans", GameBansColumnIndex },
            { "Economy Ban", EcoBanColumnIndex },
            { "Last Ban (Days)", LastBanColumnIndex },
        };

        public UserSettings User = new UserSettings();

        public void HandleDeprecatedSettings()
        {
            // Update Recent and Selected login setting names.
            if (FileService.KeyExists("Recent", SectionAutolog))
            {
                FileService.Write(LoginRecentAccount, FileService.Read("Recent", SectionAutolog), SectionAutolog);
                FileService.DeleteKey("Recent", SectionAutolog);
            }

            if (FileService.KeyExists("RecentAcc", SectionAutolog))
            {
                FileService.Write(RecentAccountIndex, FileService.Read("RecentAcc", SectionAutolog), SectionAutolog);
                FileService.DeleteKey("RecentAcc", SectionAutolog);
            }

            if (FileService.KeyExists("Selected", SectionAutolog))
            {
                FileService.Write(LoginSelectedAccount, FileService.Read("Selected", SectionAutolog), SectionAutolog);
                FileService.DeleteKey("Selected", SectionAutolog);
            }

            if (FileService.KeyExists("SelectedAcc", SectionAutolog))
            {
                FileService.Write(SelectedAccountIndex, FileService.Read("SelectedAcc", SectionAutolog), SectionAutolog);
                FileService.DeleteKey("SelectedAcc", SectionAutolog);
            }

            // Move Steam file path to it's own section.
            if (FileService.KeyExists(SectionSteam, SectionGeneral))
            {
                FileService.Write(SteamPath, FileService.Read(SectionSteam, SectionGeneral), SectionSteam);
                FileService.DeleteKey(SectionSteam, SectionGeneral);
            }

            // Move button size to 'Customize' section.
            if (FileService.KeyExists(ButtonSize, SectionGeneral))
            {
                FileService.Write(ButtonSize, FileService.Read(ButtonSize, SectionGeneral), SectionCustomize);
                FileService.DeleteKey(ButtonSize, SectionGeneral);
            }
        }
    }
}