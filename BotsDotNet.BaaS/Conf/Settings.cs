using Microsoft.Extensions.Logging;
using System;

namespace BotsDotNet.BaaS.Conf
{
    using Utilities;

    public class Settings
    {
        public const string DEFAULT_SETTINGSFILE = "bot.settings.json";

        public Action<MapHandler> DependencyHandler { get; set; }

        public Action<IBot> OnLoggedIn { get; set; }

        public IConfiguration Configuration { get; set; }

        public ILogger Logger { get; set; }

        public bool AsAService { get; set; } = true;

        public string SettingsPath { get; set; } = DEFAULT_SETTINGSFILE;
    }
}
