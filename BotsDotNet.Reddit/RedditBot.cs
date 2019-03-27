using RedditSharp;
using System;
using System.Threading;

namespace BotsDotNet.Reddit
{
    using Handling;
    using Utilities;
    using Red = RedditSharp.Reddit;

    public partial class RedditBot : BotImpl
    {
        public const string PLATFORM = BotPlatform.Reddit;

        public BotWebAgent Agent { get; private set; }
        public Red Connection { get; private set; }
        public RedditOptions Options { get; private set; }

        private IUser _userprofile;

        public override IUser Profile => _userprofile;

        public override IGroup[] Groups => new IGroup[0];

        public override string Platform => PLATFORM;

        public RedditBot(IPluginManager manager) : base(manager) { }

        public bool Start(RedditOptions options, string prefix = null)
        {
            try
            {
                Options = options;

                if (!options.Valid())
                {
                    Error(new Exception("Invalid options. Please make sure all options are specified."));
                    return false;
                }

                Prefix = prefix;

                Agent = new BotWebAgent(options.Username, options.Password, options.ClientId, options.ClientSecret, options.RedirectUrl)
                {
                    UserAgent = options.UserAgent
                };
                Connection = new Red(Agent, true);
                _userprofile = new User(Connection.User);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }
        
        public bool Start(string id, string secret, string username, string password, string redirectUrl, string useragent, string prefix = null)
        {
            return Start(new RedditOptions
            {
                ClientId = id,
                ClientSecret = secret,
                Password = password,
                RedirectUrl = redirectUrl,
                UserAgent = useragent,
                Username = username
            }, prefix);
        }

        public void MonitorSubReddit(params string[] communites)
        {
            foreach (var community in communites)
                Monitor(community);
        }

        private async void Monitor(string sub)
        {
            var sr = await Connection.GetSubredditAsync(sub);
           
            var stream = sr.GetComments().Stream();
            
            stream.Subscribe(new MessageObserver(this));

            await stream.Enumerate(CancellationToken.None);
        }

        public static RedditBot Create()
        {
            return (RedditBot)DependencyResolution().Build<IBot>();
        }

        public static MapHandler DependencyResolution()
        {
            return ReflectionUtility.DependencyInjection()
                                    .Use<IBot, RedditBot>();
        }
    }
}
