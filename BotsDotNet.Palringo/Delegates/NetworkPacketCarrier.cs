namespace BotsDotNet.Palringo.Delegates
{
    using Networking;

    public delegate void NetworkPacketCarrier(INetworkClient client, IPacket packet);
}
