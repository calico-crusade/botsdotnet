using System;

namespace BotsDotNet.WebExTeams.Models
{
    public class Organization : IWTModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public DateTime Created { get; set; }
    }
}
