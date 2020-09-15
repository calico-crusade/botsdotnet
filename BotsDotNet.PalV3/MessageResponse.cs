using System;

namespace BotsDotNet.PalringoV3
{
    public class MessageResponse
    {
        public string Uuid { get; set; }
        public long Timestamp { get; set; }

        public DateTime ProperTimeStamp => new DateTime(Timestamp);
    }

    public class OutMessageResp : BdnModel, IMessageResponse
    {
        public bool Success { get; private set; }

        public OutMessageResp(MessageResponse resp) : base(resp)
        {
            Success = !string.IsNullOrEmpty(resp?.Uuid);
        }

        public static implicit operator OutMessageResp(MessageResponse resp)
        {
            return new OutMessageResp(resp);
        }
    }
}
