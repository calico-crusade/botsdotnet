using Discord;
using Microsoft.Extensions.Logging;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotsDotNet.BaaS
{
    using Conf;
    using Discord;
    using P = PalringoV3.PalBot;
    using Palringo;
    using Palringo.Types;
    using Twitch;
    using Utilities;

    public interface IBotManager
    {
        IEnumerable<IBot> GetBots();
        Task AddBotV3(PalringoAccount account);
        Task AddBot(PalringoAccount account);
        Task AddBot(DiscordAccount account);
        Task AddBot(TwitchAccount account);
    }

    public class BotManager : IMicroService, IBotManager
    {
        public List<IBot> Bots { get; private set; } = new List<IBot>();
        
        private Settings settings;
        private ILogger logger => settings.Logger;
        private IConfiguration configuration => settings.Configuration;

        public BotManager(Settings settings)
        {
            this.settings = settings;
        }

        public IEnumerable<IBot> GetBots()
        {
            return Bots.ToArray();
        }

        public async Task AddBotV3(PalringoAccount account)
        {
            var bot = (P)AddDeps(P.DependencyInjection()).Build<IBot>();
            bot.PluginSets = account.PluginSet;

            bot.OnError += (e) => logger.LogError(e, LogMessage(bot, account.Email, "Error occurred"));
            bot.OnDisconnected += () => logger.LogWarning(LogMessage(bot, account.Email, "Disconnected"));

            var loggedIn = await bot.Login(account.Email, account.Password, null, account.Prefix);

            if (!loggedIn)
            {
                logger.LogWarning(LogMessage(bot, account.Email, "Login Failed"));
                return;
            }

            logger.LogInformation(LogMessage(bot, account.Email, "Login Success"));
            settings.OnLoggedIn?.Invoke(bot);
            Bots.Add(bot);
        }

        public async Task AddBot(PalringoAccount account)
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
            settings.OnLoggedIn?.Invoke(bot);
            Bots.Add(bot);
        }

        public async Task AddBot(DiscordAccount account)
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
            settings.OnLoggedIn?.Invoke(bot);
            Bots.Add(bot);
        }

        public async Task AddBot(TwitchAccount account)
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
            settings.OnLoggedIn?.Invoke(bot);
            Bots.Add(bot);
        }

        #region Internal methods
        private async void BotStart()
        {
            var issues = new string[0];
            if (!configuration.Validate(out issues))
            {
                foreach (var issue in issues)
                    logger.LogError("Issue with configuration: " + issue);
                return;
            }

            foreach (var account in configuration?.Palringo ?? new PalringoAccount[0])
            {
                if (!account.Validate(out issues))
                {
                    foreach (var issue in issues)
                        logger.LogWarning(LogMessage("Palringo", "Configuration", issue));
                    continue;
                }

                if (account.UseV3)
                    await AddBotV3(account);
                else
                    await AddBot(account);
            }

            foreach (var account in configuration?.Discord ?? new DiscordAccount[0])
            {
                if (!account.Validate(out issues))
                {
                    foreach (var issue in issues)
                        logger.LogWarning(LogMessage("Discord", "Configuration", issue));
                    continue;
                }

                await AddBot(account);
            }

            foreach (var account in configuration?.Twitch ?? new TwitchAccount[0])
            {
                if (!account.Validate(out issues))
                {
                    foreach (var issue in issues)
                        logger.LogWarning(LogMessage("Twitch", "Configuration", issue));
                    continue;
                }

                await AddBot(account);
            }
        }

        private async Task BotStop()
        {
            foreach (var bot in Bots)
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

        public void Start()
        {
            BotStart();
        }

        public void Stop()
        {
            BotStop().Wait();
        }

        public MapHandler AddDeps(MapHandler handler)
        {
            settings.DependencyHandler?.Invoke(handler);

            return handler
                    .Use<IBotManager, BotManager>(this)
                    .Use<IConfiguration, IConfiguration>(configuration)
                    .Use<ILogger, ILogger>(logger);
        }

        public string LogMessage(string platform, string identifier, string message)
        {
            platform = platform.PadRight(BotPlatform.PalringoV3.Length + 1);

            return $"{platform} >> {identifier} :: {message}";
        }

        public string LogMessage(IBot bot, string identifier, string message)
        {
            return LogMessage(bot.Platform, bot?.Profile?.Nickname ?? identifier, message);
        }
        #endregion
    }

}
