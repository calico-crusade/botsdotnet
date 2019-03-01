namespace BotsDotNet.Twitch
{
    public class MessageResponse : BdnModel, IMessageResponse
    {
        public bool Success { get; private set; }

        public MessageResponse(bool worked) : base(worked)
        {
            Success = worked;
        }
    }
}
