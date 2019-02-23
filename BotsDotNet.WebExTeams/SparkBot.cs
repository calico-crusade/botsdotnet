using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BotsDotNet.WebExTeams
{
    using Handling;
    using SparkDotNet;
    using Util;

    public partial class SparkBot : IBot
    {
        public const string PLATFORM = "WebExTeams";
        private static string _accessToken;
        private static string _friendlyName;
        private static string _hookId;
        private static Spark _connection;

        public IUser Profile { get; private set; }

        public IGroup[] Groups => throw new NotImplementedException();

        public string Platform => PLATFORM;

        public string AccessToken => _accessToken;
        public string FriendlyName => _friendlyName;
        public Spark Connection => _connection;
        public string MyId => Profile.Id;
        public string HookId => _hookId;

        private IPluginManager pluginManager;
        private ICacheUtility cacheUtility;
        private IConfigUtility configUtility;

        private ConcurrentDictionary<Func<Message, bool>, TaskCompletionSource<Message>> awaitedMessages { get; set; }

        public SparkBot(
            IPluginManager pluginManager,
            ICacheUtility cacheUtility,
            IConfigUtility configUtility)
        {
            this.pluginManager = pluginManager;
            this.cacheUtility = cacheUtility;
            this.configUtility = configUtility;
            awaitedMessages = new ConcurrentDictionary<Func<Message, bool>, TaskCompletionSource<Message>>();
        }

        public async Task Initialize()
        {
            await Initialize(configUtility.ApiKey, configUtility.BotName);
        }

        public async Task Initialize(string token, string name)
        {
            if (_connection != null)
                return;

            _accessToken = token;
            _friendlyName = name;
            _connection = new Spark(token);

            Profile = await Connection.GetMeAsync();
            await HandleHooks();
        }

        public async Task HandleHooks()
        {
            var targetUrl = configUtility.WebhookUri + FriendlyName;
            var hooks = await Connection.GetWebhooksAsync();

            if (hooks == null || hooks.Count == 0)
            {
                _hookId = await CreateResourceHook();
                return;
            }

            bool allResourceHook = false;

            foreach (var hook in hooks)
            {
                if (hook.TargetUrl == targetUrl &&
                    hook.Status == "active" &&
                    hook.Resource == "all" &&
                    hook.Event == "all" &&
                    !allResourceHook)
                {
                    allResourceHook = true;
                    _hookId = hook.Id;
                    continue;
                }

                await Connection.DeleteWebhookAsync(hook.Id);
            }

            if (!allResourceHook)
                _hookId = await CreateResourceHook();

            return;
        }

        public async Task<string> CreateResourceHook()
        {
            var targetUrl = configUtility.WebhookUri;
            return (await Connection.CreateWebhookAsync
            (
                configUtility.HookName,
                targetUrl,
                "all",
                "all",
                ""
            )).Id;
        }

        public async Task HandleCallback(Callback call)
        {
            if (call.Resource != "messages")
                return;

            string id = call.Data.id;
            var msg = await cacheUtility.MessageFromId(this, id);

            var prof = (Person)Profile;
            if (msg.Content.Trim().StartsWith(prof.Nickname))
                msg.Content = msg.Content.Trim().Remove(0, prof.Nickname.Length).Trim();

            if (msg.User.Id == MyId)
                return;

            if (CheckMessage(msg))
                return;

            var resp = await pluginManager.Process(this, msg);
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
    }
}
