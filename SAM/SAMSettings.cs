using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SAM.Services;

namespace SAM
{
    public class SamSettings
    {
        internal class Sections
        {
            public const string General = "Settings";
            public const string Autolog = "AutoLog";
            public const string Customize = "Customize";
            public const string Steam = "Steam";
            public const string Parameters = "Parameters";
            public const string Location = "Location";
            public const string Columns = "Columns";
        }

        public const string FileName = "SAMSettings.ini";

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
            { ClearUserData, Sections.General },
            { HideAddButton, Sections.General },
            { PasswordProtect, Sections.General },
            { MinimizeToTray, Sections.General },
            { RememberPassword, Sections.General },
            { StartMinimized, Sections.General },
            { StartWithWindows, Sections.General },
            { AccountsPerRow, Sections.General },
            { SleepTime, Sections.General },
            { CheckForUpdates, Sections.General },
            { CloseOnLogin, Sections.General },
            { ListView, Sections.General },
            { SandboxMode, Sections.General },

            { LoginRecentAccount, Sections.Autolog },
            { RecentAccountIndex, Sections.Autolog },
            { LoginSelectedAccount, Sections.Autolog },
            { SelectedAccountIndex, Sections.Autolog },
            { InputMethod, Sections.Autolog },
            { HandleIme, Sections.Autolog },
            { Ime2FaOnly, Sections.Autolog },

            { Theme, Sections.Customize },
            { Accent, Sections.Customize },
            { ButtonSize, Sections.Customize },
            { ButtonColor, Sections.Customize },
            { ButtonFontSize, Sections.Customize },
            { ButtonFontColor, Sections.Customize },
            { ButtonBannerColor, Sections.Customize },
            { ButtonBannerFontSize, Sections.Customize },
            { ButtonBannerFontColor, Sections.Customize },
            { HideBanIcons, Sections.Customize },

            { SteamPath, Sections.Steam },
            { SteamApiKey, Sections.Steam },
            { AutoReloadEnabled, Sections.Steam },
            { AutoReloadInterval, Sections.Steam },
            { LastAutoReload, Sections.Steam },

            { CafeAppLaunchParameter, Sections.Parameters },
            { ClearBetaParameter, Sections.Parameters },
            { ConsoleParameter, Sections.Parameters },
            { DeveloperParameter, Sections.Parameters },
            { ForceServiceParameter, Sections.Parameters },
            { LoginParameter, Sections.Parameters },
            { NoCacheParameter, Sections.Parameters },
            { NoVerifyFilesParameter, Sections.Parameters },
            { SilentParameter, Sections.Parameters },
            { SingleCoreParameter, Sections.Parameters },
            { TcpParameter, Sections.Parameters },
            { TenFootParameter, Sections.Parameters },
            { CustomParameters, Sections.Parameters },
            { CustomParametersValue, Sections.Parameters },

            { ListViewHeight, Sections.Location },
            { ListViewWidth, Sections.Location },

            { NameColumnIndex, Sections.Columns },
            { DescriptionColumnIndex, Sections.Columns },
            { TimeoutColumnIndex, Sections.Columns },
            { VacBansColumnIndex, Sections.Columns },
            { GameBansColumnIndex, Sections.Columns },
            { EcoBanColumnIndex, Sections.Columns },
            { LastBanColumnIndex, Sections.Columns },
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

        public readonly UserSettings User = new UserSettings();

        public void HandleDeprecatedSettings()
        {
            // Update Recent and Selected login setting names.
            if (FileService.KeyExists("Recent", Sections.Autolog))
            {
                FileService.Write(LoginRecentAccount, FileService.Read("Recent", Sections.Autolog), Sections.Autolog);
                FileService.DeleteKey("Recent", Sections.Autolog);
            }

            if (FileService.KeyExists("RecentAcc", Sections.Autolog))
            {
                FileService.Write(RecentAccountIndex, FileService.Read("RecentAcc", Sections.Autolog), Sections.Autolog);
                FileService.DeleteKey("RecentAcc", Sections.Autolog);
            }

            if (FileService.KeyExists("Selected", Sections.Autolog))
            {
                FileService.Write(LoginSelectedAccount, FileService.Read("Selected", Sections.Autolog), Sections.Autolog);
                FileService.DeleteKey("Selected", Sections.Autolog);
            }

            if (FileService.KeyExists("SelectedAcc", Sections.Autolog))
            {
                FileService.Write(SelectedAccountIndex, FileService.Read("SelectedAcc", Sections.Autolog), Sections.Autolog);
                FileService.DeleteKey("SelectedAcc", Sections.Autolog);
            }

            // Move Steam file path to it's own section.
            if (FileService.KeyExists(Sections.Steam, Sections.General))
            {
                FileService.Write(SteamPath, FileService.Read(Sections.Steam, Sections.General), Sections.Steam);
                FileService.DeleteKey(Sections.Steam, Sections.General);
            }

            // Move button size to 'Customize' section.
            if (FileService.KeyExists(ButtonSize, Sections.General))
            {
                FileService.Write(ButtonSize, FileService.Read(ButtonSize, Sections.General), Sections.Customize);
                FileService.DeleteKey(ButtonSize, Sections.General);
            }
        }
    }
}