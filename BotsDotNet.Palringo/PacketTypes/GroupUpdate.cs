namespace BotsDotNet.Palringo.PacketTypes
{
    using Networking.Mapping;
    using SubProfile.Parsing;
    using Types;

    public class GroupUpdate : IPacketMap
    {
        public string Command => "GROUP UPDATE";

        [Payload]
        public DataMap Payload { get; set; }

        public string UserId => Payload.GetValueInt("contact-id").ToString();
        public string GroupId => Payload.GetValueInt("group-id").ToString();
        public GroupUpdateType Type => (GroupUpdateType)Payload.GetValueInt("type");
    }
}
