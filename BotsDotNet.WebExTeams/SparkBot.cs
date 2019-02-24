using System;
using System.Threading.Tasks;

namespace BotsDotNet.WebExTeams
{
    using Handling;
    using SparkDotNet;
    using Util;

    public partial class SparkBot : BotImpl
    {
        public const string PLATFORM = "Spark";
        private static string _accessToken;
        private static string _friendlyName;
        private static string _hookId;
        private static Spark _connection;

        private IUser _prof;

        public override IUser Profile => _prof;

        public override IGroup[] Groups => throw new NotImplementedException();

        public override string Platform => PLATFORM;

        public string AccessToken => _accessToken;
        public string FriendlyName => _friendlyName;
        public Spark Connection => _connection;
        public string MyId => Profile.Id;
        public string HookId => _hookId;
        
        private ICacheUtility cacheUtility;
        private IConfigUtility configUtility;

        public SparkBot(
            IPluginManager pluginManager,
            ICacheUtility cacheUtility,
            IConfigUtility configUtility) : base(pluginManager)
        {
            this.cacheUtility = cacheUtility;
            this.configUtility = configUtility;
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

            _prof = await Connection.GetMeAsync();
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

            await MessageReceived(msg);
        }
    }
}
