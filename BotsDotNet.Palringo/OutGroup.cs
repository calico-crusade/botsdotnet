namespace BotsDotNet.Palringo
{
    using SubProfile;

    public class OutGroup : BdnModel, IGroup
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public OutGroup(Group group) : base(group)
        {
            Id = group.Id;
            Name = group.Name;
        }

        public static implicit operator OutGroup(Group group)
        {
            return new OutGroup(group);
        }
    }
}
