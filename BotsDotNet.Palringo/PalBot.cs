using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.Palringo
{
    using Handling;
    using Networking;
    using Networking.Handling;
    using Networking.Mapping;
    using Networking.Watcher;
    using SubProfile;
    using Types;
    using Util;
    using Utilities;

    public partial class PalBot : BotImpl
    {
        public static string DefaultHost = "im.palringo.com";
        public static int DefaultPort = 12345;
        public const string PLATFORM = "Palringo";

        public override IUser Profile => SubProfiling.Profile;
        public override IGroup[] Groups => SubProfiling.Groups.Values.ToArray();
        public override string Platform => PLATFORM;

        public string Email { get; private set; }
        public string Password { get; private set; }
        public AuthStatus Status { get; private set; }
        public DeviceType Device { get; private set; }
        public bool SpamFilter { get; private set; }
        public bool EnablePlugins { get; private set; }
        public string[] Groupings { get; private set; }
        public IBroadcastUtility On { get; private set; }
        public ISubProfiling SubProfiling { get; }
        public bool Connected => _client?.Connected ?? false;

        private IPacketSerializer packetSerializer;
        private IPacketDeserializer packetDeserializer;
        private IPacketMapper packetMapper;
        private IPacketWatcher packetWatcher;
        private IPacketTemplates packetTemplates;
        private IZLibCompression compression;
        private IAuthenticationUtility authentication;
        private IPacketHandlerHub handlerHub;
        private IPluginManager pluginManager;
        private NetworkClient _client;
        private BroadcastUtility broadcast;

        private Action<string> _loginFailed;
        private Action _disconnected;
        private Action _couldntConnect;
        private Action<Exception, string> _error;
        private Action<PalBot, MessagePacket> _message;

        public PalBot(IPacketSerializer packetSerializer,
            IPacketDeserializer packetDeserializer,
            IPacketMapper packetMapper,
            IPacketWatcher packetWatcher,
            IPacketTemplates packetTemplates,
            IZLibCompression compression,
            IAuthenticationUtility authentication,
            IPacketHandlerHub handlerHub,
            ISubProfiling subProfiling,
            IPluginManager pluginManager,
            IBroadcastUtility broadcast) : base(pluginManager)
        {
            this.packetSerializer = packetSerializer;
            this.packetDeserializer = packetDeserializer;
            this.packetMapper = packetMapper;
            this.packetWatcher = packetWatcher;
            this.packetTemplates = packetTemplates;
            this.compression = compression;
            this.authentication = authentication;
            this.handlerHub = handlerHub;
            this.SubProfiling = subProfiling;
            this.pluginManager = pluginManager;
            this.On = broadcast;
            this.broadcast = (BroadcastUtility)broadcast;

            _client = new NetworkClient(DefaultHost, DefaultPort);
            _client.OnDisconnected += (c) => _disconnected?.Invoke();
            _client.OnDisconnected += (c) => ((BroadcastUtility)On).BroadcastDisconnected();
            _client.OnException += (e, n) => _error?.Invoke(e, n);
            _client.OnException += (e, n) => ((BroadcastUtility)On).BroadcastException(e, n);
            _client.OnDataReceived += (c, b) => this.packetDeserializer.ReadPacket(c, b);

            On.Exception += (e, n) => _error?.Invoke(e, n);
            On.Exception += (e, n) => Error(e);
            On.Message += (b, m) => _message?.Invoke(b, m);
            this.broadcast.PacketParsed += (c, p) => PacketReceived(p);
        }

        public async Task<bool> Write(IPacket packet)
        {
            if (!_client?.Connected ?? false)
                return false;

            var serialized = packetSerializer.Serialize(packet);
            bool worked = false;

            foreach (var data in serialized)
            {
                worked = await _client.WriteData(data);
            }

            if (worked)
                ((BroadcastUtility)On).BroadcastPacketSent(packet);

            return worked;
        }

        public async Task<T> Write<T>(IPacket packet, Func<Task<T>> onSerailized)
        {
            if (!_client?.Connected ?? false)
                return default(T);

            var serialized = packetSerializer.Serialize(packet);
            bool worked = false;

            var output = onSerailized.Invoke();

            foreach (var data in serialized)
            {
                worked = await _client.WriteData(data);
            }

            if (worked)
                ((BroadcastUtility)On).BroadcastPacketSent(packet);

            return worked ? await output : default(T);
        }

        public async Task<bool> Write(IPacketMap map)
        {
            var packet = packetMapper.Unmap(map);
            if (packet == null)
                return false;

            return await Write(packet);
        }

        private async void PacketReceived(IPacket packet)
        {
            if (packet["COMPRESSION"] == "1" && packet.Payload != null)
            {
                packet.Payload = compression.Decompress(packet.Payload);
            }

            var map = packetMapper.Map(packet);
            if (map == null) //Unhandled Packet
            {
                ((BroadcastUtility)On).BroadcastUnhandledPacket(packet);
                return;
            }


            //Notify any attached processes that a packet has been received.
            ((BroadcastUtility)On).BroadcastPacketReceived(packet.Clone());

            //Process any packet handlers on the packet
            handlerHub.ProcessPacket(this, map);

            //Process any watchs that are on the packet
            packetWatcher.Process(map);

            if (!(map is MessagePacket))
                return;
            
            var om = (MessagePacket)map;

            if (om.ContentType != DataType.Text)
                return;
            
            var msg = await Message(om);

            await MessageReceived(msg);
        }

        public PalBot Disconnected(Action action)
        {
            this._disconnected = action;
            return this;
        }

        public PalBot LoginFailed(Action<string> action)
        {
            this._loginFailed = action;
            return this;
        }

        public PalBot Error(Action<Exception, string> action)
        {
            this._error = action;
            return this;
        }

        public PalBot MessageReceived(Action<PalBot, MessagePacket> action)
        {
            this._message = action;
            return this;
        }

        public PalBot CouldNotConnect(Action action)
        {
            _couldntConnect = action;
            return this;
        }

        public static PalBot Create()
        {
            return (PalBot)ReflectionUtility.DependencyInjection()
                                            .Use<IBot, PalBot>()
                                            .Build<IBot>();
        }
    }
}
