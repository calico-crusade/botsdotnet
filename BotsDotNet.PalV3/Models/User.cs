using Newtonsoft.Json;

namespace BotsDotNet.PalringoV3.Models
{
    public class User
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
        public GroupUserType? GroupMemberCapabilities { get; set; }

        [JsonProperty("contactListBlockedState")]
        public int? ContactListBlockedState { get; set; }

        [JsonProperty("contactListAuthState")]
        public int? ContactListAuthState { get; set; }

        [JsonProperty("charms")]
        public object Charms { get; set; }

        [JsonProperty("extended")]
        public Extended Extended { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        public string[] Attributes { get; set; }
    }

    public class OutUser : BdnModel, IUser
    {
        public string Id { get; private set; }

        public string Nickname { get; private set; }

        public string Status { get; private set; }

        public string[] Attributes { get; private set; }

        public OutUser(User user) : base(user)
        {
            Id = user.Id;
            Nickname = user.Nickname;
            Status = user.Status;
            Attributes = user.Attributes;
        }

        public static implicit operator OutUser(User user)
        {
            return new OutUser(user);
        }
    }
}
