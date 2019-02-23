using System;
using Newtonsoft.Json;

namespace BotsDotNet.WebExTeams.SparkDotNet
{
    public class Membership
    {
        public string Id { get; set; }
        public string RoomId { get; set; }
        public string PersonId { get; set; }
        public string PersonEmail { get; set; }
        public string PersonDisplayName { get; set; }
        public bool IsModerator { get; set; }
        public bool IsMonitor { get; set; }
        public DateTime Created { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}