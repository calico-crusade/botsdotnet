using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BotsDotNet.BaaS.Conf
{
    public interface IConfiguration : IValidator
    {
        PalringoAccount[] Palringo { get; }
        DiscordAccount[] Discord { get; }
        TwitchAccount[] Twitch { get; }

        string ServiceName { get; }
        string ServiceDisplayName { get; }
        string ServiceDescription { get; }
    }

    public class Configuration : IConfiguration
    {
        public PalringoAccount[] Palringo { get; set; }
        public DiscordAccount[] Discord { get; set; }
        public TwitchAccount[] Twitch { get; set; }

        public string ServiceName { get; set; }
        public string ServiceDisplayName { get; set; }
        public string ServiceDescription { get; set; }

        public bool Validate(out string[] issues)
        {
            var tmpIssues = new List<string>();
            if ((Palringo == null || Palringo.Length <= 0) &&
                (Discord == null || Discord.Length <= 0) &&
                (Twitch == null || Twitch.Length <= 0))
                tmpIssues.Add("Please specify at least one type of bot.");

            if (string.IsNullOrEmpty(ServiceName))
                tmpIssues.Add("ServiceName is required. Please specify a service name.");

            if (string.IsNullOrEmpty(ServiceDisplayName))
                tmpIssues.Add("ServiceDisplayName is required. Please specify a service display name");

            ServiceDescription = ServiceDescription ?? "";
            
            issues = tmpIssues.ToArray();
            return tmpIssues.Count == 0;
        }
        
        public static Configuration FromJson(string filename)
        {
            if (!File.Exists(filename))
            {
                var def = FromDefault();
                var ser = JsonConvert.SerializeObject(def, Formatting.Indented);
                File.WriteAllText(filename, ser);
                return null;
            }

            var contents = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Configuration>(contents);
        }

        public static Configuration FromDefault()
        {
            return new Configuration
            {
                ServiceName = "MyBotService",
                ServiceDisplayName = "My Super Awesome Bot Service",
                ServiceDescription = "Service for running some super awesome bots!",
                Palringo = new[]
                {
                    new PalringoAccount
                    {
                        Email = "",
                        Password = "",
                        Prefix = "!",
                        PluginSet = "MyPlugins",
                        OnlineStatus = "Online",
                        DeviceType = "PC",
                        SpamFilter = false
                    }
                },
                Discord = new[]
                {
                    new DiscordAccount
                    {
                        //Get from: https://discordapp.com/developers/applications
                        Token = "",
                        Prefix = "$",
                        PluginSet = "MyPlugins"
                    }
                },
                Twitch = new[]
                {
                    new TwitchAccount
                    {
                        Username = "twitch_account_name",
                        //Get from: https://dev.twitch.tv/docs/authentication/
                        ClientId = "",
                        //Get from: https://twitchtokengenerator.com/
                        Token = "",
                        Prefix = "!",
                        PluginSet = "MyPlugins",
                        Channels = new []
                        {
                            "my_awesome_channel"
                        },
                        DebugLog = false
                    }
                }
            };
        }
    }
}
