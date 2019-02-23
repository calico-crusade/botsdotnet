namespace BotsDotNet.Palringo.Networking.Watcher
{
    using Mapping;

    public interface IWatch
    {
        bool Validate(IPacketMap packet);
    }
}
