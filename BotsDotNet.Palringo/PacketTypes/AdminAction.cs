namespace BotsDotNet.Palringo.PacketTypes
{
    using Networking.Mapping;
    using Types;

    public class AdminAction : IPacketMap
    {
        public string Command => "GROUP ADMIN";

        [Header("SOURCE-ID")]
        public string SourceId { get; set; }

        [Header("TARGET-ID")]
        public string TargetId { get; set; }

        [Header("GROUP-ID")]
        public string GroupId { get; set; }

        [Header("ACTION")]
        public AdminActions Action { get; set; }

        public override string ToString()
        {
            return $"{Action.ToString()} - {SourceId} => {TargetId} ({GroupId})";
        }
    }
}
