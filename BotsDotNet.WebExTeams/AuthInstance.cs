namespace BotsDotNet.WebExTeams
{
    public class AuthInstance
    {
        public string FriendlyName { get; private set; }
        public string AccessToken { get; private set; }
        
        public AuthInstance(string token, string name, string siteUrl)
        {
            FriendlyName = name;
            AccessToken = token;
            CallbackAddress = CreateCallback(siteUrl, name);
        }
    }
}
