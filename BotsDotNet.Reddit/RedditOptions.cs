namespace BotsDotNet.Reddit
{
    using Utilities;

    public class RedditOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RedirectUrl { get; set; }
        public string UserAgent { get; set; }

        public bool Valid()
        {
            return ClientId.NotNullOrEmpty(ClientSecret, Username, Password, RedirectUrl, UserAgent);
        }
    }
}
