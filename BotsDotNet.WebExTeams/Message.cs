namespace BotsDotNet.WebExTeams
{
    using SparkDotNet;

    public class Message : MessageImpl
    {
        public SparkMessage Original { get; private set; }

        public IUser[] Mentions { get; set; }

        public Message() { }

        public Message(SparkMessage message, IUser user, IGroup group, IBot bot)
        {
            Original = message;
            User = user;
            Group = group;
            Bot = bot;
            MessageType = message.RoomType == "direct" ? MessageType.Private : MessageType.Group;
            Content = message.Markdown ?? message.Text;
            TimeStamp = message.Created;
            UserId = message.PersonId;
            GroupId = message.RoomId;
        }
    }
}
