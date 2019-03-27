using RedditSharp.Things;

namespace BotsDotNet.Reddit
{
    public class User : BdnModel, IUser
    {
        public string Id { get; set; }

        public string Nickname { get; set; }

        public string Status { get; set; }

        public string[] Attributes { get; set; }

        public User(RedditUser user) : base(user)
        {
            Id = user.Id;
            Nickname = user.Name;
            Status = user.FullName;
            Attributes = null;
        }

        public User(Comment comment) : base(comment)
        {
            Id = comment.AuthorName;
            Nickname = comment.AuthorName;
            Status = comment.AuthorName;
            Attributes = null;
        }
    }
}
