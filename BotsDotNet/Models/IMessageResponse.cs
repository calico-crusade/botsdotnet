namespace BotsDotNet
{
    public interface IMessageResponse : IBdnModel
    {
        bool Success { get; }
    }
}
