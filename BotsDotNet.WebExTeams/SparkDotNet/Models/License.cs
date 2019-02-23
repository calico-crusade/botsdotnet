using Newtonsoft.Json;

namespace BotsDotNet.WebExTeams.SparkDotNet
{
    public class License
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string TotalUnits { get; set; }
        public string ConsumedUnits { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}