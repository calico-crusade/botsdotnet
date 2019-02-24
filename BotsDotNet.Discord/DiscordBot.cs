using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.Discord
{
    using Handling;
    using Utilities;

    public partial class DiscordBot : BotImpl
    {
        public const string PLATFORM = "Discord";
        public DiscordSocketClient Connection;

        public override IUser Profile => new User(Connection.CurrentUser);
        public override IGroup[] Groups => Connection.GroupChannels.Select(t => new Group(t)).ToArray();
        public override string Platform => PLATFORM;

        public string Token { get; private set; }

        public DiscordBot(IPluginManager pluginManager) : base(pluginManager) { }

        public async Task<bool> Start(string token)
        {
            try
            {
                var tsc = new TaskCompletionSource<bool>();
                Token = token;
                Connection = new DiscordSocketClient();
                Connection.MessageReceived += async (m) => await MessageReceived(m);
                Connection.Log += Log;

                Connection.LoggedIn += () => { tsc?.SetResult(true); return Task.CompletedTask; };
                Connection.LoggedOut += () => { tsc?.SetResult(false); return Task.CompletedTask; };

                await Connection.LoginAsync(TokenType.Bot, token);
                await Connection.StartAsync();

                return await tsc.Task;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        public Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public Task MessageReceived(SocketMessage message)
        {
            try
            {
                var msg = new Message(message, new User(message.Author), new Group(message.Channel), this);

                return base.MessageReceived(msg);
            }
            catch (Exception ex)
            {
                Error(ex);
                return Task.CompletedTask;
            }
        }
        
        public static DiscordBot Create()
        {
            return (DiscordBot)ReflectionUtility.DependencyInjection()
                                                .Use<IBot, DiscordBot>()
                                                .Build<IBot>();
        }
    }
}
