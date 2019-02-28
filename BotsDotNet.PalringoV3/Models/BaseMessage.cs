using System;
using System.Text;

namespace BotsDotNet.PalringoV3.Models
{
    public class BaseMessage
    {
        public string Id { get; set; }
        public string Recipient { get; set; }
        public string Originator { get; set; }
        public bool IsGroup { get; set; }
        public long Timestamp { get; set; }
        public string MimeType { get; set; }
        public string Data { get; set; }
        public string FlightId { get; set; }

        public string Contents => Encoding.UTF8.GetString(Convert.FromBase64String(Data));
    }
}
