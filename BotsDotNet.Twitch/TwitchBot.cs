using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace BotsDotNet.Twitch
{
    using Handling;
    using Utilities;

    public partial class TwitchBot : BotImpl
    {
        public TwitchClient Connection { get; private set; }
        public TwitchAPI Api { get; private set; }
        public string Username { get; private set; }
        public string Token { get; private set; }
        public bool Debug { get; set; } = false;

        public override IUser Profile => new User(Connection.TwitchUsername);

        public override IGroup[] Groups => new IGroup[0];

        public override string Platform => BotPlatform.Twitch;

        public TwitchBot(IPluginManager pluginManager) : base(pluginManager) { }

        public Task<bool> Login(string username, string clientid, string token, string prefix = null, string channel = null)
        {
            var tsc = new TaskCompletionSource<bool>();
            Username = username;
            Token = token;
            Prefix = prefix;

            var creds = new ConnectionCredentials(username, token);

            Connection = new TwitchClient();

            Connection.OnConnected += (s, e) => { Log("Connected"); tsc?.SetResult(true); };
            Connection.OnConnectionError += (s, e) => { Log("Connection Error"); tsc?.SetResult(false); };
            Connection.OnMessageReceived += (s, m) => HandleMessage(m);
            Connection.OnWhisperReceived += (s, m) => HandleWhisper(m);
            Connection.OnError += (s, e) => Log("Error", e.Exception);
            Connection.OnLog += (s, e) => { Log("Log", e); };
            Connection.OnIncorrectLogin += (s, e) => { Log("Login Failed", e.Exception); tsc?.SetResult(false); };

            Connection.Initialize(creds, autoReListenOnExceptions: true, channel: channel);
            Connection.Connect();

            Api = new TwitchAPI();
            Api.Settings.ClientId = clientid;
            Api.Settings.AccessToken = token;

            return tsc.Task;
        }

        public void JoinChannel(params string[] channels)
        {
            foreach(var channel in channels)
                Connection.JoinChannel(channel);
        }

        private async void HandleMessage(OnMessageReceivedArgs args)
        {
            Log("Message", args.ChatMessage);
            if (args.ChatMessage.IsMe)
                return;

            var msg = new Message(this, args.ChatMessage);
            await MessageReceived(msg);
        }

        private async void HandleWhisper(OnWhisperReceivedArgs args)
        {
            Log("Whisper", args.WhisperMessage);
            var msg = new Message(this, args.WhisperMessage);
            await MessageReceived(msg);
        }

        public void Log(string message, object item = null)
        {
            if (!Debug)
                return;

            if (item == null)
            {
                System.Console.WriteLine($"[{System.DateTime.Now}] {message}");
                return;
            }

            var ser = Newtonsoft.Json.JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented);
            System.Console.WriteLine($"[{System.DateTime.Now}] {message}: {ser}");
        }

        public static TwitchBot Create()
        {
            return (TwitchBot)DependencyResolution().Build<IBot>();
        }

        public static MapHandler DependencyResolution()
        {
            return ReflectionUtility.DependencyInjection()
                                    .Use<IBot, TwitchBot>();
        }
    }
}
