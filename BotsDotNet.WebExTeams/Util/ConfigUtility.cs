namespace BotsDotNet.WebExTeams.Util
{
    public interface IConfigUtility
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
    }

    public class ConfigUtility : IConfigUtility
    {
        public string BaseUri { get; set; }
        public string CallbackApi { get; set; }
        public string WebhookPart { get; set; }
        public string ConfigPart { get; set; }
        public string HookName { get; set; }
        public string BotName { get; set; }
        public string ApiKey { get; set; }
        public string WebhookUri => BaseUri + CallbackApi + WebhookPart;
        public string ConfigUri => BaseUri + CallbackApi + ConfigPart;
    }
}
