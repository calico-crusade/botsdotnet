namespace BotsDotNet.WebExTeams.Models
{
    public class License : IWTModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int TotalUnits { get; set; }
        public int ConsumedUnits { get; set; }
    }
}
