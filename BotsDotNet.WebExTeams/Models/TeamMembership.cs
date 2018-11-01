using System;

namespace BotsDotNet.WebExTeams.Models
{
    public class TeamMembership : IWTModel
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
        public string PersonId { get; set; }
        public string PersonEmail { get; set; }
        public string PersonDisplayName { get; set; }
        public string PersonOrgId { get; set; }
        public bool IsModerator { get; set; }
        public DateTime Created { get; set; }
    }
}
