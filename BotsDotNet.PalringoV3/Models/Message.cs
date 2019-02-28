using System;

namespace BotsDotNet.PalringoV3.Models
{
    public class Message : MessageImpl
    {
        public BaseMessage Original { get; set; }

        public Message(BaseMessage msg, IUser user, IGroup group, IBot bot)
        {
            User = user;
            Group = group;
            Bot = bot;
            Original = msg;

            MessageType = msg.IsGroup ? MessageType.Group : MessageType.Private;
            Content = msg.Contents;
            TimeStamp = new DateTime(msg.Timestamp);
            UserId = user.Id;
            GroupId = msg.IsGroup ? msg.Recipient : null;
            MimeType = msg.MimeType;

            ContentType = ContentType.Other;
            if (msg.MimeType == "text/plain")
                ContentType |= ContentType.Text | ContentType.Markup;
            if (msg.MimeType == "text/html")
                ContentType |= ContentType.Rich;

            if (msg.MimeType == "text/image_link")
                ContentType |= ContentType.Image;
        }
    }
}
