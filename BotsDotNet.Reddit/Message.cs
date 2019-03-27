using RedditSharp.Things;

namespace BotsDotNet.Reddit
{
    public class Message : MessageImpl
    {
        public Message(IBot bot, Comment original) : base(original)
        {
            Bot = bot;

            UserId = original.AuthorName;
            User = new User(original);
            MimeType = "text/plain";
            ContentType = ContentType.Text;
            TimeStamp = original.Created;
            Content = original.Body;
            MessageType = MessageType.Group;
            GroupId = original.Shortlink;
            Group = new Group(original.Subreddit, original.Id);
        }
    }
}
