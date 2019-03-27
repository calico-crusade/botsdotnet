namespace BotsDotNet.Test.Core.Stubs
{
    public class GroupStub : BdnModel, IGroup
    {
        public string Id { private set; get; }

        public string Name { private set; get; }

        public GroupStub(string id, string name) : base(new { id, name })
        {
            Id = id;
            Name = name;
        }
    }
}
