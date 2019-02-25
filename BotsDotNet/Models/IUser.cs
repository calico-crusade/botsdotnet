namespace BotsDotNet
{
    public interface IUser
    {
        string Id { get; }
        string Nickname { get; }
        string Status { get; }
        string[] Attributes { get; }
    }
}
