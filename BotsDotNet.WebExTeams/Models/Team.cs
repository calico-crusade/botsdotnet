using System;

namespace BotsDotNet.WebExTeams.Models
{
    public class Team : IWTModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
    }
}
