namespace BotsDotNet.Test.Core.Stubs
{
    public class UserStub : BdnModel, IUser
    {
        public string Id { get; set; }

        public string Nickname { get; set; }

        public string Status { get; set; }

        public string[] Attributes { get; set; }

        public UserStub(string id, string nickname, string status) : base(new { id, nickname, status })
        {
            Id = id;
            Nickname = nickname;
            Status = status;
            Attributes = new string[0];
        }
    }
}
