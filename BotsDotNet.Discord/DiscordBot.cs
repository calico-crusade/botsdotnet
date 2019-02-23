using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.Discord
{
    using Handling;
    using Utilities;

    public partial class DiscordBot : IBot
    {
        public const string PLATFORM = "Discord";
        public DiscordSocketClient Connection;
        private readonly IPluginManager pluginManager;

        public IUser Profile => new User(Connection.CurrentUser);

        public IGroup[] Groups => Connection.GroupChannels.Select(t => new Group(t)).ToArray();

        public string Platform => PLATFORM;

        public string Token { get; private set; }

        private ConcurrentDictionary<Func<Message, bool>, TaskCompletionSource<Message>> awaitedMessages { get; set; }

        public DiscordBot(IPluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
            awaitedMessages = new ConcurrentDictionary<Func<Message, bool>, TaskCompletionSource<Message>>();
        }

        public async Task<bool> Start(string token)
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

        public Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public Task MessageReceived(SocketMessage message)
        {
            try
            {
                var msg = new Message(message, new User(message.Author), new Group(message.Channel));
                
                if (message.Author.Id.ToString() == Profile.Id)
                    return Task.CompletedTask;

                if (CheckMessage(msg))
                    return Task.CompletedTask;
                
                pluginManager.Process(this, msg);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Task.CompletedTask;
            }
        }

        public bool CheckMessage(Message msg)
        {
            var tests = awaitedMessages.ToArray();

            foreach (var t in tests)
            {
                if (!t.Key(msg))
                    continue;

                awaitedMessages.TryRemove(t.Key, out TaskCompletionSource<Message> output);
                output.SetResult(msg);
                return true;
            }

            return false;
        }

        public static DiscordBot Create()
        {
            return (DiscordBot)ReflectionUtility.DependencyInjection()
                                                .Use<IBot, DiscordBot>()
                                                .Build<IBot>();
        }
    }
}
