using Discord.WebSocket;

namespace BotsDotNet.Discord
{
    public class Message : MessageImpl
    {
        public ISocketMessageChannel ReturnChannel { get; private set; }

        public Message(SocketMessage message, IUser user, IGroup group, IBot bot) : base(message)
        {
            User = user;
            Group = group;

            MessageType = message.Channel is ISocketPrivateChannel ? MessageType.Private : MessageType.Group;
            ReturnChannel = message.Channel;
            Content = message.Content;
            TimeStamp = message.CreatedAt.DateTime;
            UserId = message.Author.Id.ToString();
            GroupId = message?.Channel?.Id.ToString();
            MimeType = "text";
            Bot = bot;
            ContentType = ContentType.Text | ContentType.Markup | ContentType.Rich;

            if (message.Attachments != null && 
                message.Attachments.Count > 0)
                ContentType |= ContentType.File;
        }
    }
}
