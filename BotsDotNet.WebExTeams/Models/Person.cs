using System;

namespace BotsDotNet.WebExTeams.Models
{
    public class Person : IWTModel
    {
        public string Id { get; set; }
        public string[] Emails { get; set; }
        public string DisplayName { get; set; }
        public string NickName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string OrgId { get; set; }
        public string[] Roles { get; set; }
        public string[] Licenses { get; set; }
        public DateTime Created { get; set; }
        public string Timezone { get; set; }
        public DateTime LastActivity { get; set; }
        public string Status { get; set; }
        public bool InvitePending { get; set; }
        public bool LoginEnabled { get; set; }
        public string Type { get; set; }
    }
}
