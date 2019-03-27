using RedditSharp.Things;

namespace BotsDotNet.Reddit
{
    public class MessageResponse : BdnModel, IMessageResponse
    {
        public bool Success { get; private set; }

        public MessageResponse(Comment comment) : base(comment)
        {
            Success = comment.IsRemoved == false;
        }

        public MessageResponse(bool worked) : base(worked)
        {
            Success = worked;
        }
    }
}
