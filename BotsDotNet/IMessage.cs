using System;

namespace BotsDotNet
{
    public interface IMessage
    {
        MessageType MessageType { get; }
        string Content { get; }
        DateTime TimeStamp { get; }
        string UserId { get; }
        string GroupId { get; }
        string ContentType { get; }
        IUser User { get; }
        IGroup Group { get; }

        string ReturnAddress { get; }
    }
}
