using System;

namespace BotsDotNet.WebExTeams.Models
{
    public class Event<T> : IWTModel
        where T: IWTModel
    {
        public string Id { get; set; }
        public string Resource { get; set; }
        public string Type { get; set; }
        public string ActorId { get; set; }
        public string OrgId { get; set; }
        public string AppId { get; set; }
        public DateTime Created { get; set; }
        public T Data { get; set; }
    }
}
