namespace BotsDotNet.PalringoV3.Models
{
    public class GroupUser
	{
        public int Id { get; set; }
        public GroupUserType? Capabilities { get; set; }
    }

    public class ExtendedGroupUser : GroupUser
	{
        public User Profile { get; set; }
	}

    public class GroupUserUpdate : GroupUser
    {
        public int GroupId { get; set; }
        public int SubscriberId { get; set; }
    }

    public class UserReturn
	{
        public int Code { get; set; }
        public User Body { get; set; }
	}
}
