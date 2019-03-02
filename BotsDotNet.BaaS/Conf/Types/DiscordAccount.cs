namespace BotsDotNet.BaaS.Conf
{
    public class DiscordAccount : IConfigType
    {
        public string Token { get; set; }
        public string Prefix { get; set; } = null;
        public string PluginSet { get; set; } = null;

        public bool Validate(out string[] issues)
        {
            if (string.IsNullOrEmpty(Token))
            {
                issues = new[] { "Invalid Discord Token!" };
                return false;
            }

            issues = new string[0];
            return true;
        }
    }
}
