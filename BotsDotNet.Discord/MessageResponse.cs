using System;
using Discord;

namespace BotsDotNet.Discord
{
    public class MessageResponse : BdnModel, IMessageResponse
    {
        public IUserMessage Message { get; set; }

        public bool Success => Message.CreatedAt.DateTime < DateTime.Now;

        public MessageResponse(IUserMessage message) : base(message)
        {
            Message = message;
        }
    }
}
