namespace BotsDotNet.Palringo.PacketTypes
{
    using Networking.Mapping;
    using SubProfile.Parsing;

    public class SubProfileQueryResult : IPacketMap
    {
        public string Command => "SUB PROFILE QUERY RESULT";

        [Payload]
        public DataMap Data { get; set; }

        public string Id => (Data != null && Data.ContainsKey("sub-id") ? Data.GetValueInt("sub-id") : 0).ToString();
    }
}
