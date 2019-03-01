using System;
using Newtonsoft.Json;

namespace BotsDotNet.WebExTeams.SparkDotNet
{
    public class Room
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TeamId { get; set; }
        public bool IsLocked { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActivity { get; set; }
        public string Type { get; set; }
        public string CreatorId { get; set; }

        public string Name => Title;
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}