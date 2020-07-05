using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace SAM.Helpers
{
    public static class SteamHelper
    {
        public static (bool HasValue, RegistryKey Value) TryGetRegistry(bool writable = false)
        {
            var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            var localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view);
            var registryKey = localKey.OpenSubKey(@"Software\\Valve\\Steam", writable);

            return (registryKey != null, registryKey);
        }

        public static void ClearAutoLoginUserKeyValues()
        {
            try
            {
                var (hasValue, localKey) = TryGetRegistry(true);

                if (!hasValue)
                    throw new Exception("Have you installed steam?");

                localKey.SetValue("AutoLoginUser", "", RegistryValueKind.String);
                localKey.SetValue("RememberPassword", 0, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string GetSteamPathFromRegistry()
        {
            try
            {
                var (hasValue, value) = TryGetRegistry();

                if (!hasValue)
                    throw new Exception("Cannot find installed Steam.");

                return $"{value.GetValue("SteamPath")}/";
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return string.Empty;
        }
    }
}