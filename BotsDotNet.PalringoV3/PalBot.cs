using System;
using System.Linq;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;

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

        public Socket Connection { get; private set; }

        private IPacketTemplate packetTemplate;
        private ICacheUtility cacheUtility;
        
        public PalBot(IPacketTemplate packetTemplate, IPluginManager pluginManager, ICacheUtility cacheUtility) : base(pluginManager)
        {
            this.packetTemplate = packetTemplate;
            this.cacheUtility = cacheUtility;
        }

        public async Task<bool> Login(string email, string password, string token = null)
        {
            try
            {
                Email = email;
                Password = password;
                Connection = CreateSocket(token);

                var welcome = await On<Welcome>("welcome");

                _user = (OutUser)(welcome.LoggedInUser ?? await WritePacket<User>(packetTemplate.Login(email, password)));

                OnLoginSuccess();
                return true;
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }
        }

        private async void OnLoginSuccess()
        {
            try
            {
                On<BaseMessage>("message send", HandleMessageRecieved);
                await WritePacket(packetTemplate.PrivateMessageSubscribe());

                _groups = (await WritePacket<Group[]>(packetTemplate.GroupList())).Cast<OutGroup>().ToArray();

                var ids = _groups.Select(t => t.Id).ToArray();
                await WritePacket(packetTemplate.GroupMessageSubscribe(ids));
            }
            catch(SocketException ex)
            {
                WriteObject(ex.ReturnData);
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
