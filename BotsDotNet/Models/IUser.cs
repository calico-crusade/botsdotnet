namespace BotsDotNet
{
    public interface IUser : IBdnModel
    {
        string Id { get; }
        string Nickname { get; }
        string Status { get; }
        string[] Attributes { get; }
    }
}
