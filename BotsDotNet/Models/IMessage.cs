using System;
using System.Drawing;
using System.Threading.Tasks;

namespace BotsDotNet
{
    /// <summary>
    /// Represents a message sent by any platform
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Whether the message was targeting a private or group chat.
        /// </summary>
        MessageType MessageType { get; }
        /// <summary>
        /// The contents of the message
        /// </summary>
        string Content { get; set;  }
        /// <summary>
        /// The time the message was received.
        /// </summary>
        DateTime TimeStamp { get; }
        /// <summary>
        /// The Id of the user who sent the message
        /// </summary>
        string UserId { get; }
        /// <summary>
        /// The Id of the group the message was sent to
        /// Can be null if the message was sent in a private chat
        /// </summary>
        string GroupId { get; }
        /// <summary>
        /// The Mimetype of the message
        /// </summary>
        string MimeType { get; }
        /// <summary>
        /// The content type derived from the MimeType
        /// </summary>
        ContentType ContentType { get; }
        /// <summary>
        /// The profile of the user who send the message
        /// </summary>
        IUser User { get; }
        /// <summary>
        /// The profile of the group the message was sent in
        /// Can be null if the message was sent in a private chat
        /// </summary>
        IGroup Group { get; }
        /// <summary>
        /// The bot that received the message
        /// </summary>
        IBot Bot { get; }
        /// <summary>
        /// Returns the User's Id or Group's Id depending on the <see cref="MessageType"/>
        /// </summary>
        string ReturnAddress { get; }

        /// <summary>
        /// Replies to the group or user with the specified message
        /// </summary>
        /// <param name="content">The message to send</param>
        /// <returns>The response from the platform</returns>
        Task<IMessageResponse> Reply(string content);

        /// <summary>
        /// Replies to the group or use with the specified image
        /// </summary>
        /// <param name="image">The image to send</param>
        /// <returns>The response from the platform</returns>
        Task<IMessageResponse> Reply(Bitmap image);

        /// <summary>
        /// Replies to the group or use with the specified file
        /// </summary>
        /// <param name="data">The file bytes to send</param>
        /// <param name="filename">The name of the file to send</param>
        /// <returns>The response from the platform</returns>
        Task<IMessageResponse> Reply(byte[] data, string filename);
        
        /// <summary>
        /// Replies to the group or use with the specified image and message
        /// </summary>
        /// <param name="content">The message to send with the image</param>
        /// <param name="image">The image to send</param>
        /// <returns>The response from the platform</returns>
        Task<IMessageResponse> Reply(string content, Bitmap image);

        /// <summary>
        /// Waits for the next message to be sent in the same context by the same user
        /// </summary>
        /// <returns>The message received</returns>
        Task<IMessage> NextMessage();
    }

    public abstract class MessageImpl : IMessage
    {
        public virtual MessageType MessageType { get; set; }

        public virtual string Content { get; set; }

        public virtual DateTime TimeStamp { get; set; }

        public virtual string UserId { get; set; }

        public virtual string GroupId { get; set; }

        public virtual string MimeType { get; set; }

        public virtual ContentType ContentType { get; set; }

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
