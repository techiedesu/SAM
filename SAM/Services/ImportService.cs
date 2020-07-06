using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;
using SAM.JsonConverters;

namespace SAM.Services
{
    public class LoginUser
    {
        public long Id { get; set; }
        public string AccountName { get; set; } = null!;
        public string PersonaName { get; set; } = null!;
        public bool IsRememberPassword { get; set; }
        public bool IsMostRecent { get; set; }

        [JsonConverter(typeof(CustomDateTimeOffsetJsonConverter))]
        public DateTimeOffset Timestamp { get; set; }
    }

    public interface IImportService
    {
        /// <summary>
        ///     Import accounts from Steam instance.
        /// </summary>
        /// <returns>Task</returns>
        void ImportFromSteamInstance();
    }

    public class ImportService : IImportService
    {
        /// <inheritdoc />
        public void ImportFromSteamInstance()
        {
            GetLoginUsers();
            throw new NotImplementedException();
        }

        private (bool HasValue, IReadOnlyCollection<LoginUser> Value) GetLoginUsers()
        {
            // TODO: Get `steam` path from config.
            const string steamPath = @"C:\Program Files (x86)\Steam";

            var loginUsersFilePath = Path.Combine(steamPath, "config", "loginusers.vdf");

            if (!File.Exists(loginUsersFilePath))
                return (false, null)!;

            var allLoggedInAccountsSerialized = File.ReadAllText(loginUsersFilePath);
            var foundAccounts = VdfConvert.Deserialize(allLoggedInAccountsSerialized).ToJson().Value.ToObject<Dictionary<long, LoginUser>>()!
                .Select(lu =>
                {
                    var loginUser = lu.Value;
                    loginUser.Id = lu.Key;
                    return loginUser;
                }).ToArray();

            return (true, foundAccounts);
        }
    }
}