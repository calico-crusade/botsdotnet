using Discord.WebSocket;

namespace BotsDotNet.Discord
{
    public class User : BdnModel, IUser
    {
        public string Id { get; set; }

        public string Nickname { get; set; }

        public string Status { get; set; }

        public string[] Attributes { get; set; }
        
        public User(SocketUser user) : base(user)
        {
            Id = user.Id.ToString();
            Nickname = user.Username;
            Status = user.Status.ToString();
            Attributes = new string[0];
        }
    }
}
