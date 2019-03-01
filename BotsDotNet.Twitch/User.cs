using System.Linq;
using TwitchLib.Api.Core.Models.Undocumented.Chatters;
using TwitchUser = TwitchLib.Api.Helix.Models.Users.User;
using TwitchLib.Client.Models;

namespace BotsDotNet.Twitch
{
    public class User : BdnModel, IUser
    {
        public string Id { get; private set; }

        public string Nickname { get; private set; }

        public string Status { get; private set; }

        public string[] Attributes { get; private set; }
        
        public User(TwitchLibMessage msg) : base(msg)
        {
            Id = msg.UserId;
            Nickname = msg.DisplayName;
            Status = msg.Username;
            Attributes = msg.Badges.Select(t => t.Key + ":" + t.Value).ToArray();
        }

        public User(ChatterFormatted chatter) : base(chatter)
        {
            Id = chatter.Username;
            Nickname = chatter.Username;
            Status = "";
            Attributes = new string[0];
        }

        public User(TwitchUser user) : base(user)
        {
            Id = user.Id;
            Nickname = user.DisplayName;
            Status = user.Description;
            Attributes = new[]
            {
                "login:" + user.Login,
                "broadcasterType:" + user.BroadcasterType,
                "type:" + user.Type,
                "profileImageUrl:" + user.ProfileImageUrl,
                "offlineImageUrl:" + user.OfflineImageUrl,
                "viewCount:" + user.ViewCount,
                "email:" + user.Email
            };
        }

        public User(string username) : base(username)
        {
            Id = username;
            Nickname = username;
            Status = username;
            Attributes = new string[0];
        }
    }
}
