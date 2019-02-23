using System;

namespace BotsDotNet.Palringo
{
    using Networking.Mapping;
    using Types;

    public class MessagePacket : IPacketMap
    {
        public virtual string Command => "MESG";

        /// <summary>
        /// The user who sent the message
        /// </summary>
        [Header("SOURCE-ID")]
        public string UserId { get; set; }

        /// <summary>
        /// The target group of the message
        /// </summary>
        [Header("TARGET-ID")]
        public string GroupId { get; set; }

        /// <summary>
        /// The message contents (payload)
        /// </summary>
        [Payload]
        public string Content { get; set; }

        /// <summary>
        /// The string representation of the type of message
        /// </summary>
        [Header("CONTENT-TYPE")]
        public string MimeType { get; set; }

        /// <summary>
        /// The string representation of the time stamp.
        /// </summary>
        [Header("TIMESTAMP")]
        public string UnsortedTimestamp { get; set; }

        /// <summary>
        /// The type of message (Group or otherwise)
        /// </summary>
        public MessageType MesgType => GroupId == null ? MessageType.Private : MessageType.Group;

        /// <summary>
        /// The DateTime representation of the time stamp.
        /// </summary>
        public DateTime Timestamp => UnsortedTimestamp.FromPalUnix();

        /// <summary>
        /// The Content Type of the message.
        /// </summary>
        public DataType ContentType
        {
            get { return MimeType.FromMimeType(); }
            set { MimeType = value.FromDataType(); }
        }
        
        /// <summary>
        /// The group ID or user ID depending on the message type.
        /// </summary>
        public string ReturnAddress => MesgType == MessageType.Private ? UserId : GroupId;

        public MessagePacket Clone()
        {
            return new MessagePacket
            {
                UserId = UserId,
                GroupId = GroupId,
                Content = Content,
                MimeType = MimeType,
                UnsortedTimestamp = UnsortedTimestamp
            };
        }
    }
}
