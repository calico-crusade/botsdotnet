using System;
using Newtonsoft.Json;

namespace BotsDotNet.WebExTeams.SparkDotNet
{
    public class Team
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string CreatorId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}