using Newtonsoft.Json;
using System;


namespace BotsDotNet.PalringoV3.Models
{
    public class Welcome
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("endpointConfig")]
        public EndpointConfig EndpointConfig { get; set; }

        [JsonProperty("loggedInUser")]
        public User LoggedInUser { get; set; }
    }

    public class EndpointConfig
    {
        [JsonProperty("avatarEndpoint")]
        public Uri AvatarEndpoint { get; set; }

        [JsonProperty("mmsUploadEndpoint")]
        public Uri MmsUploadEndpoint { get; set; }

        [JsonProperty("banner")]
        public Banner Banner { get; set; }
    }

    public class Banner
    {
        [JsonProperty("notification")]
        public Tion Notification { get; set; }

        [JsonProperty("promotion")]
        public Tion Promotion { get; set; }
    }

    public class Tion
    {
        [JsonProperty("en")]
        public Uri En { get; set; }

        [JsonProperty("ar")]
        public Uri Ar { get; set; }
    }

    public class Extended
    {
        [JsonProperty("language")]
        public int? Language { get; set; }

        [JsonProperty("urls")]
        public object Urls { get; set; }

        [JsonProperty("lookingFor")]
        public int? LookingFor { get; set; }

        [JsonProperty("dateOfBirth")]
        public object DateOfBirth { get; set; }

        [JsonProperty("relationship")]
        public int? Relationship { get; set; }

        [JsonProperty("gender")]
        public int? Gender { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("optOut")]
        public object OptOut { get; set; }

        [JsonProperty("utcOffset")]
        public object UtcOffset { get; set; }

        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("name1")]
        public string Name1 { get; set; }

        [JsonProperty("after")]
        public object After { get; set; }

        [JsonProperty("dobD")]
        public int? DobD { get; set; }

        [JsonProperty("dobM")]
        public int? DobM { get; set; }

        [JsonProperty("dobY")]
        public int? DobY { get; set; }

        [JsonProperty("relationshipStatus")]
        public int? RelationshipStatus { get; set; }

        [JsonProperty("sex")]
        public int? Sex { get; set; }
    }

}
