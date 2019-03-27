namespace BotsDotNet.Reddit
{
    public class Group : BdnModel, IGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Group(string subreddit, string postId = null) : base(subreddit)
        {
            Id = postId ?? subreddit;
            Name = subreddit;
        }
    }
}
