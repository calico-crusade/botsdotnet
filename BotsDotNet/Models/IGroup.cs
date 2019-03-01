namespace BotsDotNet
{
    public interface IGroup : IBdnModel
    {
        string Id { get; }
        string Name { get; }
    }
}
