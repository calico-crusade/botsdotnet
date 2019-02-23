namespace BotsDotNet.WebExTeams
{
    using SparkDotNet;

    public class MessageResponse : IMessageResponse
    {
        public bool Success => Message != null;

        public SparkMessage Message { get; private set; }

        public MessageResponse(SparkMessage message)
        {
            Message = message;
        }
    }
}
