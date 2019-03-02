using System.Collections.Generic;

namespace BotsDotNet.BaaS.Conf
{
    public class TwitchAccount : IConfigType
    {
        public string Username { get; set; }
        public string ClientId { get; set; }
        public string Token { get; set; }
        public string Prefix { get; set; } = null;
        public string PluginSet { get; set; } = null;
        public string[] Channels { get; set; } = new string[0];
        public bool DebugLog { get; set; } = false;

        public bool Validate(out string[] issues)
        {
            var tmpIssues = new List<string>();

            if (string.IsNullOrEmpty(Username))
                tmpIssues.Add("Invalid username!");

            if (string.IsNullOrEmpty(ClientId))
                tmpIssues.Add("Invalid Clinet ID");

            if (string.IsNullOrEmpty(Token))
                tmpIssues.Add("Invalid Token");

            issues = tmpIssues.ToArray();
            return tmpIssues.Count == 0;
        }
    }
}
