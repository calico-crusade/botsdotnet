using Discord.WebSocket;
using System;

namespace BotsDotNet.Discord
{
    public class Message : IMessage
    {
        public MessageType MessageType { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }

        public string UserId { get; set; }

        public string GroupId { get; set; }

        public string ContentType { get; set; }

        public IUser User { get; set; }

        public IGroup Group { get; set; }

        public string ReturnAddress => MessageType == MessageType.Private ? UserId : GroupId;

        public SocketMessage Original { get; private set; }

        public ISocketMessageChannel ReturnChannel { get; private set; }

        public Message(SocketMessage message, IUser user, IGroup group)
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
        }
    }
}
