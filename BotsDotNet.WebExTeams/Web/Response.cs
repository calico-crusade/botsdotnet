using System.Collections.Generic;

namespace BotsDotNet.WebExTeams.Web
{
    public class Response<T>
    {
        public int Code { get; set; }
        public string Status { get; set; }
        public Dictionary<string, object> Headers { get; set; }
        public T Data { get; set; }
    }
}
