using Newtonsoft.Json;

namespace BotsDotNet.PalringoV3.Models
{
    public class User : IUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("privileges")]
        public long? Privileges { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("reputation")]
        public double? Reputation { get; set; }

        [JsonProperty("icon")]
        public int? Icon { get; set; }

        [JsonProperty("onlineState")]
        public int? OnlineState { get; set; }

        [JsonProperty("deviceType")]
        public int? DeviceType { get; set; }

        [JsonProperty("groupMemberCapabilities")]
        public object GroupMemberCapabilities { get; set; }

        [JsonProperty("contactListBlockedState")]
        public object ContactListBlockedState { get; set; }

        [JsonProperty("contactListAuthState")]
        public object ContactListAuthState { get; set; }

        [JsonProperty("charms")]
        public object Charms { get; set; }

        [JsonProperty("extended")]
        public Extended Extended { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        public string[] Attributes { get; set; }
    }
}
