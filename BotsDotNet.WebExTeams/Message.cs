namespace BotsDotNet.WebExTeams
{
    using SparkDotNet;

    public class Message : MessageImpl
    {
        public IUser[] Mentions { get; set; }
        
        public Message(SparkMessage message, IUser user, IGroup group, IBot bot) : base(message)
        {
            User = user;
            Group = group;
            Bot = bot;
            MessageType = message.RoomType == "direct" ? MessageType.Private : MessageType.Group;
            Content = message.Markdown ?? message.Text;
            TimeStamp = message.Created;
            UserId = message.PersonId;
            GroupId = message.RoomId;
            MimeType = "text";
            ContentType = ContentType.Text;

            if (!string.IsNullOrEmpty(message.Markdown))
                ContentType |= ContentType.Markup;

            if (!string.IsNullOrEmpty(message.Html))
                ContentType |= ContentType.Rich;

            if (message.Files != null &&
                message.Files.Length > 0)
                ContentType |= ContentType.File;
        }
    }
}
