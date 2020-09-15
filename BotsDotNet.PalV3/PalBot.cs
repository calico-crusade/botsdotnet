using SocketIOClient;
using System;
using System.Linq;
using System.Threading.Tasks;
//using Quobject.SocketIoClientDotNet.Client;

namespace BotsDotNet.PalringoV3
{
    using Handling;
    using Network;
    using Models;
    using Utilities;

    public partial class PalBot : BotImpl
    {
        public const string PLATFORM = BotPlatform.PalringoV3;

        private IUser _user;
        private IGroup[] _groups;

        public override string Platform => PLATFORM;
        public override IUser Profile => _user;
        public override IGroup[] Groups => _groups;

        public string Email { get; private set; }
        public string Password { get; private set; }
        public bool AutoReconnect { get; set; } = true;

        public SocketIO Connection { get; private set; }

        private IPacketTemplate packetTemplate;
        private ICacheUtility cacheUtility;
        
        public PalBot(IPacketTemplate packetTemplate, IPluginManager pluginManager, ICacheUtility cacheUtility) : base(pluginManager)
        {
            this.packetTemplate = packetTemplate;
            this.cacheUtility = cacheUtility;
        }

        public async Task<bool> Login(string email, string password, string token = null, string prefix = null)
        {
            try
            {
                Email = email;
                Password = password;
                Connection = CreateSocket(token);
                Prefix = prefix;

                
                MessageOn(HandleMessageRecieved);

                await DoLogin();

                OnLoginSuccess();
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }
        
        private async Task DoLogin()
        {
            var wt = On<Welcome>("welcome");

            await Connection.ConnectAsync();

            var welcome = await wt;

            _user = (OutUser)(welcome.LoggedInUser ?? await WritePacket<User>(packetTemplate.Login(Email, Password)));
        }

        private async void OnLoginSuccess()
        {
            try
            {
                var res = await WritePacket<Resp>(packetTemplate.PrivateMessageSubscribe());
                _groups = (await WritePacket<Group[]>(packetTemplate.GroupList())).Select(t => (OutGroup)t).ToArray();
                res = await WritePacket<Resp>(packetTemplate.GroupMessageSubscribe());
            }
            catch(SocketException ex)
            {
                WriteObject(ex?.ReturnData);
                Error(ex);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        private async void HandleMessageRecieved(BaseMessage msg)
        {
            try
            {
                var user = await GetUser(msg.Originator);
                var group = msg.IsGroup ? await GetGroup(msg.Recipient) : null;

                var m = new Message(msg, user, group, this);
                await base.MessageReceived(m);
            }
            catch(Exception ex)
            {
                Error(ex);
            }
        }

        public static MapHandler DependencyInjection()
        {
            return ReflectionUtility.DependencyInjection()
                                    .Use<IPacketTemplate, PacketTemplate>()
                                    .Use<ICacheUtility, CacheUtility>()
                                    .Use<IBot, PalBot>();
        }

        public static PalBot Create()
        {
            return (PalBot)DependencyInjection().Build<IBot>();
        }
    }
}
