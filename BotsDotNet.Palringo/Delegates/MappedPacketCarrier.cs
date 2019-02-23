namespace BotsDotNet.Palringo.Delegates
{
    using Networking.Mapping;

    public delegate void MappedPacketCarrier<T>(PalBot bot, T packet) where T: IPacketMap;
}
