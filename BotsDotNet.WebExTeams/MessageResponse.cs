namespace BotsDotNet.WebExTeams
{
    using SparkDotNet;

    public class MessageResponse : BdnModel, IMessageResponse
    {
        public bool Success => Message != null;

        public SparkMessage Message { get; private set; }

        public MessageResponse(SparkMessage message) : base(message)
        {
            Message = message;
        }
    }
}
