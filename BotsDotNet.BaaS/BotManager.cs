using Discord;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotsDotNet.BaaS
{
    using Conf;
    using Discord;
    using Palringo;
    using Palringo.Types;
    using Twitch;
    using Utilities;

    public interface IBotManager
    {
        IEnumerable<IBot> GetBots();
        void AddBot(PalringoAccount account, Action<IBot> onloggedin = null);
        void AddBot(DiscordAccount account, Action<IBot> onloggedin = null);
        void AddBot(TwitchAccount account, Action<IBot> onloggedin = null);
        void Start(Action<IBot> onloggedin = null);
        Task Stop();
    }

    public class BotManager<T> : IBotManager
    {
        public List<IBot> Bots { get; private set; } = new List<IBot>();
        private readonly ILogger logger;
        private readonly IConfiguration<T> configuration;

        public BotManager(ILogger logger, IConfiguration<T> configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public void AddBot(IBot bot)
        {
            Bots.Add(bot);
        }

        #region Exposed Methods
        public IEnumerable<IBot> GetBots()
        {
            return Bots.ToArray();
        }

        public async void AddBot(PalringoAccount account, Action<IBot> onloggedin = null)
        {
            var bot = (PalBot)AddDeps(PalBot.DependencyResolution()).Build<IBot>();
            bot.PluginSets = account.PluginSet;
            
            bot.OnError += (e) => logger.LogError(e, LogMessage(bot, account.Email, "Error occurred"));
            bot.On.LoginFailed += (b, reason) => logger.LogWarning(LogMessage(bot, account.Email, "Login failed: " + reason));
            bot.On.Disconnected += () => logger.LogWarning(LogMessage(bot, account.Email, "Disconnected"));

            var loggedIn = await bot.Login(account.Email, account.Password,
                                           account.Prefix, account.OnlineStatus.ParseEnum<AuthStatus>(),
                                           account.DeviceType.ParseEnum<DeviceType>(), account.SpamFilter);

            if (!loggedIn)
                return;

            logger.LogInformation(LogMessage(bot, account.Email, "Login Success"));
            Bots.Add(bot);
        }

        public async void AddBot(DiscordAccount account, Action<IBot> onloggedin = null)
        {
            var bot = (DiscordBot)AddDeps(DiscordBot.DependencyResolution()).Build<IBot>();
            bot.PluginSets = account.PluginSet;

            var tok = account.Token.SafeRemove(10, 999);
            bot.OnError += (e) => logger.LogError(e, LogMessage(bot, tok, "Error occurred"));

            var loggedIn = await bot.Start(account.Token, account.Prefix);

            if (!loggedIn)
            {
                logger.LogWarning(LogMessage(bot, tok, "Login Failed."));
                return;
            }

            logger.LogInformation(LogMessage(bot, tok, "Login Success"));
            Bots.Add(bot);
        }

        public async void AddBot(TwitchAccount account, Action<IBot> onloggedin = null)
        {
            var bot = (TwitchBot)AddDeps(TwitchBot.DependencyResolution()).Build<IBot>();
            bot.PluginSets = account.PluginSet;

            bot.OnError += (e) => logger.LogError(e, LogMessage(bot, account.Username, "Error occurred"));

            var loggedIn = await bot.Login(account.Username, account.ClientId, account.Token, account.Prefix);

            if (!loggedIn)
            {
                logger.LogWarning(LogMessage(bot, account.Username, "Login Failed"));
                return;
            }

            if (account.Channels != null && account.Channels.Length > 0)
                bot.JoinChannel(account.Channels);

            logger.LogInformation(LogMessage(bot, account.Username, "Login Success"));
            Bots.Add(bot);
        }

        public void Start(Action<IBot> onloggedin = null)
        {
            var issues = new string[0];
            if (!configuration.Validate(out issues))
            {
                foreach(var issue in issues)
                    logger.LogError("Issue with configuration: " + issue);
                return;
            }

            foreach(var account in configuration?.Palringo ?? new PalringoAccount[0])
            {
                if (!account.Validate(out issues))
                {
                    foreach (var issue in issues)
                        logger.LogWarning(LogMessage("Palringo", "Configuration", issue));
                    continue;
                }

                AddBot(account, onloggedin);
            }

            foreach (var account in configuration?.Discord ?? new DiscordAccount[0])
            {
                if (!account.Validate(out issues))
                {
                    foreach (var issue in issues)
                        logger.LogWarning(LogMessage("Discord", "Configuration", issue));
                    continue;
                }

                AddBot(account, onloggedin);
            }

            foreach (var account in configuration?.Twitch ?? new TwitchAccount[0])
            {
                if (!account.Validate(out issues))
                {
                    foreach (var issue in issues)
                        logger.LogWarning(LogMessage("Twitch", "Configuration", issue));
                    continue;
                }

                AddBot(account, onloggedin);
            }
        }

        public async Task Stop()
        {
            foreach(var bot in Bots)
            {
                if (bot is PalBot palbot)
                {
                    if (!palbot.Connected)
                        continue;

                    await palbot.Disconnect();
                    continue;
                }

                if (bot is DiscordBot disc)
                {
                    if (disc.Connection.ConnectionState != ConnectionState.Connected)
                        continue;

                    await disc.Connection.LogoutAsync();
                    continue;
                }

                if (bot is TwitchBot twitch)
                {
                    if (!twitch.Connection.IsConnected)
                        continue;

                    twitch.Connection.Disconnect();
                    continue;
                }
            }
        }
        #endregion

        #region Internal methods
        public MapHandler AddDeps(MapHandler handler)
        {
            return handler
                    .Use<IBotManager, BotManager<T>>(this)
                    .Use<IConfiguration<T>, IConfiguration<T>>(configuration)
                    .Use<ILogger, ILogger>(logger);
        }

        public string LogMessage(string platform, string identifier, string message)
        {
            platform = platform.PadRight(BotPlatform.Palringo.Length + 1);

            return $"{platform} >> {identifier} :: {message}";
        }

        public string LogMessage(IBot bot, string identifier, string message)
        {
            return LogMessage(bot.Platform, bot?.Profile?.Nickname ?? identifier, message);
        }
        #endregion
    }
}
