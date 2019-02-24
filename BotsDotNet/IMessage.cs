using System;
using System.Drawing;
using System.Threading.Tasks;

namespace BotsDotNet
{
    public interface IMessage
    {
        MessageType MessageType { get; }
        string Content { get; set;  }
        DateTime TimeStamp { get; }
        string UserId { get; }
        string GroupId { get; }
        string ContentType { get; }
        IUser User { get; }
        IGroup Group { get; }
        IBot Bot { get; }

        string ReturnAddress { get; }

        Task<IMessageResponse> Reply(string content);

        Task<IMessageResponse> Reply(Bitmap image);

        Task<IMessageResponse> Reply(byte[] data, string filename);

        Task<IMessageResponse> Reply(string content, Bitmap image);

        Task<IMessage> NextMessage();
    }

    public abstract class MessageImpl : IMessage
    {
        public virtual MessageType MessageType { get; set; }

        public virtual string Content { get; set; }

        public virtual DateTime TimeStamp { get; set; }

        public virtual string UserId { get; set; }

        public virtual string GroupId { get; set; }

        public virtual string ContentType { get; set; }

        public virtual IUser User { get; set; }

        public virtual IGroup Group { get; set; }

        public virtual IBot Bot { get; set; }

        public virtual string ReturnAddress => MessageType == MessageType.Group ? GroupId : UserId;

        public virtual Task<IMessageResponse> Reply(string content) => Bot.Reply(this, content);

        public virtual Task<IMessageResponse> Reply(Bitmap image) => Bot.Reply(this, image);

        public virtual Task<IMessageResponse> Reply(byte[] data, string filename) => Bot.Reply(this, data, filename);

        public virtual Task<IMessageResponse> Reply(string content, Bitmap image) => Bot.Reply(this, content, image);

        public virtual Task<IMessage> NextMessage() => Bot.NextMessage((t) => t.MessageType == MessageType && t.UserId == UserId && t.GroupId == GroupId);
    }
}
