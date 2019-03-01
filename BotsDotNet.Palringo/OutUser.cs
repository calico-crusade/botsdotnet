using System.Linq;

namespace BotsDotNet.Palringo
{
    using SubProfile;

    public class OutUser : BdnModel, IUser
    {
        public string Id { get; private set; }

        public string Nickname { get; private set; }

        public string Status { get; private set; }

        public string[] Attributes { get; private set; }

        public OutUser(User user) : base(user)
        {
            Id = user.Id;
            Nickname = user.Nickname;
            Status = user.Status;
            Attributes = user.PrivTags.Select(t => t.ToString()).ToArray();
        }

        public static implicit operator OutUser(User user)
        {
            return new OutUser(user);
        }
    }
}
