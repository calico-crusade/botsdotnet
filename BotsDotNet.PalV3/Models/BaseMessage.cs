using Newtonsoft.Json;

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
        public DataPlaceholder Data { get; set; }
        public string FlightId { get; set; }

        public string Contents { get; set; }//Encoding.UTF8.GetString(Convert.FromBase64String(Data.ToString()));

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public class DataPlaceholder
        {
            [JsonProperty("_placeholder")]
            public bool Placeholder { get; set; }
            public int Num { get; set; }
        }
    }
}
