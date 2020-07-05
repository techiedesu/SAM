using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SAM.Services
{
    internal class UpdateService
    {
        /// <summary>
        /// String containing the latest version acquired from online text file.
        /// </summary>
        public static string LatestVersion { get; set; }

        private const string UpdaterFileName = "Updater.exe";
        private const string LatestUpdaterVersionUrl = "https://raw.githubusercontent.com/rex706/Updater/master/latest.txt";

        /// <summary>
        /// Check program for updates with the given text url.
        /// Returns 1 if the user chose not to update or 0 if there is no update available.
        /// </summary>
        public static async Task<int> CheckForUpdate(string updateUrl, string releasesUrl)
        {
            // Allows downloading files directly from GitHub repositories. 
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Nkosi Note: Always use asynchronous versions of network and IO methods.

            // Check for version updates
            var client = new HttpClient { Timeout = new TimeSpan(0, 0, 1, 0) };

            var current = Assembly.GetExecutingAssembly().GetName().Version;
            try
            {
                // Open the text file using a stream reader.
                using var stream = await client.GetStreamAsync(updateUrl);
                var reader = new StreamReader(stream);

                // Get current and latest versions of program.
                var latest = Version.Parse(await reader.ReadLineAsync());

                // Update latest version string class member.
                LatestVersion = latest.ToString();

                // If the version from the online text is newer than the current version,
                // ask user if they would like to download and install update now.
                if (latest > current)
                {
                    // Show message box that an update is available.
                    var answer = MessageBox.Show("A new version of " +
                                                 AppDomain.CurrentDomain.FriendlyName.Substring(0, AppDomain.CurrentDomain.FriendlyName.IndexOf('.')) +
                                                 " is available!\n\nCurrent Version     " + current + "\nLatest Version        " + latest +
                                                 "\n\nUpdate now?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    // Update is available, but user chose not to update just yet.
                    if (answer != MessageBoxResult.Yes)
                        return 1;

                    // Update is available, and user wants to update. Requires app to close.
                    // Delete updater if exists.
                    if (File.Exists(UpdaterFileName))
                        File.Delete(UpdaterFileName);

                    // Download latest updater.
                    using (var updaterStream = await client.GetStreamAsync(LatestUpdaterVersionUrl))
                    {
                        reader = new StreamReader(updaterStream);
                        var latestUpdaterUrl = await reader.ReadLineAsync();
                        await DownloadUpdater(latestUpdaterUrl);
                    }

                    // Setup update process information.
                    var startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = UpdaterFileName,
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                        Arguments = updateUrl,
                        Verb = "runas"
                    };

                    // Launch updater and exit.
                    try
                    {
                        Process.Start(startInfo);
                    }
                    catch
                    {
                        // Open browser to releases page.
                        Process.Start(releasesUrl);
                    }
                            
                    Environment.Exit(0);

                    return 2;
                }

                // No update available.
                return 0;
            }
            catch
            {
                // Some error occured or there is no internet connection.
                return -1;
            }
        }

        private static async Task DownloadUpdater(string url)
        {
            await new WebClient().DownloadFileTaskAsync(url, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdaterFileName));
        }
    }
}
