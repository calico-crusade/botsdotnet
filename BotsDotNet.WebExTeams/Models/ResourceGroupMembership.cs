namespace BotsDotNet.WebExTeams.Models
{
    public class ResourceGroupMembership : IWTModel
    {
        public string Id { get; set; }
        public string ResourceGroupId { get; set; }
        public string LicenseId { get; set; }
        public string PersonId { get; set; }
        public string PersonOrgId { get; set; }
        public string Status { get; set; }
    }
}
