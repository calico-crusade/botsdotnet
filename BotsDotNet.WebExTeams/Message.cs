using System;

namespace BotsDotNet.WebExTeams
{
    using SparkDotNet;

    public class Message : IMessage
    {
        public MessageType MessageType { get; private set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; private set; }

        public string UserId { get; private set; }

        public string GroupId { get; private set; }

        public string ContentType { get; private set; }

        public IUser User { get; private set; }

        public IGroup Group { get; private set; }

        public string ReturnAddress => MessageType == MessageType.Group ? GroupId : UserId;

        public SparkMessage Original { get; private set; }

        public IUser[] Mentions { get; set; }

        public Message() { }

        public Message(SparkMessage message, IUser user, IGroup group)
        {
            Original = message;
            User = user;
            Group = group;
            MessageType = message.RoomType == "direct" ? MessageType.Private : MessageType.Group;
            Content = message.Markdown ?? message.Text;
            TimeStamp = message.Created;
            UserId = message.PersonId;
            GroupId = message.RoomId;
        }
    }
}
