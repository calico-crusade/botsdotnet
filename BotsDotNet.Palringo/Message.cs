using System;

namespace BotsDotNet.Palringo
{
    using SubProfile;

    public class Message : IMessage
    {
        public MessageType MessageType { get; private set; }

        public string Content { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public string UserId { get; private set; }

        public string GroupId { get; private set; }

        public string ContentType { get; private set; }

        public IUser User { get; private set; }

        public IGroup Group { get; private set; }

        public string ReturnAddress => MessageType == MessageType.Group ? GroupId : UserId;

        public MessagePacket Original { get; private set; }

        public Message(MessagePacket packet, IUser user, IGroup group)
        {
            MessageType = packet.MesgType == MessageType.Group ? MessageType.Group : MessageType.Private;
            Content = packet.Content;
            TimeStamp = packet.Timestamp;
            UserId = packet.UserId.ToString();
            GroupId = packet.GroupId?.ToString();
            ContentType = packet.MimeType;
            User = user;
            Group = group;
            Original = packet;
        }
    }
}
