using System;
using Newtonsoft.Json;

namespace BotsDotNet.WebExTeams.SparkDotNet
{
    public class Person
    {
        public string Id { get; set; }
        public string[] Emails { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string OrgId { get; set; }
        public string[] Roles { get; set; }
        public string[] Licenses { get; set; }
        public DateTime Created { get; set; }
        public string TimeZone { get; set; }
        public string Status { get; set; }
        public DateTime LastActivity { get; set; }
        public string Type { get; set; }
        [JsonProperty("NickName")]
        public string Nickname { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}