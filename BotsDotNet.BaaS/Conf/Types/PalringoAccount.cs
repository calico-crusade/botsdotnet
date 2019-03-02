using System;
using System.Collections.Generic;
using System.Linq;

namespace BotsDotNet.BaaS.Conf
{
    using Palringo;
    using Palringo.Types;

    public class PalringoAccount : IConfigType
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Prefix { get; set; } = null;
        public string PluginSet { get; set; } = null;
        public string OnlineStatus { get; set; }
        public string DeviceType { get; set; }
        public bool SpamFilter { get; set; } = false;

        public bool Validate(out string[] issues)
        {
            var tmpIssues = new List<string>();
            if (string.IsNullOrEmpty(Email))
                tmpIssues.Add("Email cannot be blank!");

            if (string.IsNullOrEmpty(Password))
                tmpIssues.Add("Password cannot be blank!");

            OnlineStatus = OnlineStatus ?? "Online";
            if (!TryParseEnum<AuthStatus>(OnlineStatus, out string[] validAuths))
                tmpIssues.Add("Invalid OnlineStatus. Valid options are: " + string.Join(", ", validAuths));

            DeviceType = DeviceType ?? "PC";
            if (!TryParseEnum<DeviceType>(DeviceType, out string[] validDevices))
                tmpIssues.Add("Invalid DeviceType. Valid options are: " + string.Join(", ", validDevices));

            issues = tmpIssues.ToArray();
            return tmpIssues.Count == 0;
        }

        public bool TryParseEnum<T>(string message, out string[] validOptions)
            where T: IConvertible
        {
            validOptions = default(T).AllFlags().Select(t => t.ToString()).ToArray();
            try
            {
                var tmp = message.ParseEnum<T>();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
