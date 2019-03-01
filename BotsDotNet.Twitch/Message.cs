using System;
using TwitchLib.Client.Models;

namespace BotsDotNet.Twitch
{
    public class Message : MessageImpl
    {
        public Message(IBot bot, TwitchLibMessage msg) : base(msg)
        {
            Bot = bot;

            UserId = msg.Username;
            MimeType = "text/plain";
            ContentType = ContentType.Text;
            TimeStamp = DateTime.Now;
            User = new User(msg);

            if (msg is ChatMessage)
            {
                var chatmsg = (ChatMessage)msg;
                MessageType = MessageType.Group;
                Content = chatmsg.Message;
                GroupId = chatmsg.Channel;
                Group = new Group(chatmsg);
            }

            if (msg is WhisperMessage)
            {
                var whisper = (WhisperMessage)msg;
                MessageType = MessageType.Private;
                Content = whisper.Message;
                GroupId = null;
            }
        }
    }
}
