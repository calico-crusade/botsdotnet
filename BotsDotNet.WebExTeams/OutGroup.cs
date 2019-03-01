namespace BotsDotNet.WebExTeams
{
    using SparkDotNet;

    public class OutGroup : BdnModel, IGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public OutGroup(Room room) : base(room)
        {
            Id = room.Id;
            Name = room.Name;
        }

        public static implicit operator OutGroup(Room room)
        {
            return new OutGroup(room);
        }
    }
}
