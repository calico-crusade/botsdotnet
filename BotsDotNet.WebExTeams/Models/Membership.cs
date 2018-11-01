using System;

namespace BotsDotNet.WebExTeams.Models
{
    public class Membership : IWTModel
    {
        public string Id { get; set; }
        public string RoomId { get; set; }
        public string PersonId { get; set; }
        public string PersonEmail { get; set; }
        public string PersonDisplayName { get; set; }
        public string PersonOrgId { get; set; }
        public bool IsModerator { get; set; }
        public bool IsMonitor { get; set; }
        public DateTime Created { get; set; }
    }
}
