namespace BotsDotNet.Palringo
{
    public class Message : MessageImpl
    {
        public MessagePacket Original { get; private set; }

        public Message(MessagePacket packet, IUser user, IGroup group, IBot bot)
        {
            MessageType = packet.MesgType == MessageType.Group ? MessageType.Group : MessageType.Private;
            Content = packet.Content;
            TimeStamp = packet.Timestamp;
            UserId = packet.UserId.ToString();
            GroupId = packet.GroupId?.ToString();
            MimeType = packet.MimeType;
            User = user;
            Group = group;
            Original = packet;
            Bot = bot;
            ContentType = (ContentType)(int)packet.ContentType;
        }
    }
}
