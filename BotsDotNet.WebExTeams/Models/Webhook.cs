using System;

namespace BotsDotNet.WebExTeams.Models
{
    public class Webhook : IWTModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TargetUrl { get; set; }
        public string Resource { get; set; }
        public string Event { get; set; }
        public string Filter { get; set; }
        public string Secret { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
    }
}
