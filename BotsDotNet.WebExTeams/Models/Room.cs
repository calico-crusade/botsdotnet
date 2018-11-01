using System;

namespace BotsDotNet.WebExTeams.Models
{
    public class Room : IWTModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public bool IsLocked { get; set; }
        public string TeamId { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime Created { get; set; }
    }
}
