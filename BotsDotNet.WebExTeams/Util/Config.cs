namespace BotsDotNet.WebExTeams.Util
{
    public interface IConfig
    {
        string BaseUri { get; }
        string CallbackApi { get; }
        string WebhookPart { get; }
        string ConfigPart { get; }
        string HookName { get; }
        string WebhookUri { get; }
        string ConfigUri { get; }
        string BotName { get; }
        string ApiKey { get; }
        string Prefix { get; }
    }

    public class Config : IConfig
    {
        public string BaseUri { get; set; }
        public string CallbackApi { get; set; }
        public string WebhookPart { get; set; }
        public string ConfigPart { get; set; }
        public string HookName { get; set; }
        public string BotName { get; set; }
        public string ApiKey { get; set; }
        public string Prefix { get; set; }
        public string WebhookUri => BaseUri + CallbackApi + WebhookPart;
        public string ConfigUri => BaseUri + CallbackApi + ConfigPart;
    }
}
