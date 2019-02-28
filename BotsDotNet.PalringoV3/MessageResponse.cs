using System;

namespace BotsDotNet.PalringoV3
{
    public class MessageResponse : IMessageResponse
    {
        public string Uuid { get; set; }
        public long Timestamp { get; set; }

        public DateTime ProperTimeStamp => new DateTime(Timestamp);

        public bool Success => !string.IsNullOrEmpty(Uuid);
    }
}
