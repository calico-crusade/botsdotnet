using Discord.WebSocket;

namespace BotsDotNet.Discord
{
    public class Message : MessageImpl
    {
        public SocketMessage Original { get; private set; }

        public ISocketMessageChannel ReturnChannel { get; private set; }

        public Message(SocketMessage message, IUser user, IGroup group, IBot bot)
        {
            User = user;
            Group = group;
            Original = message;

            MessageType = message.Channel is ISocketPrivateChannel ? MessageType.Private : MessageType.Group;
            ReturnChannel = message.Channel;
            Content = message.Content;
            TimeStamp = message.CreatedAt.DateTime;
            UserId = message.Author.Id.ToString();
            GroupId = message?.Channel?.Id.ToString();
            ContentType = "text";
            Bot = bot;
        }
    }
}
