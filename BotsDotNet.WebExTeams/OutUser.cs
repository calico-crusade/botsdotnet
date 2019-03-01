namespace BotsDotNet.WebExTeams
{
    using SparkDotNet;

    public class OutUser : BdnModel, IUser
    {
        public string Id { get; private set; }

        public string Nickname { get; private set; }

        public string Status { get; private set; }

        public string[] Attributes { get; private set; }

        public OutUser(Person person) : base(person)
        {
            Id = person.Id;
            Nickname = person.Nickname;
            Status = person.Status;
            Attributes = person.Roles;
        }

        public static implicit operator OutUser(Person person)
        {
            return new OutUser(person);
        }
    }
}
