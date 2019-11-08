using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.Palringo
{
    using Networking;
    using Networking.Watcher;
    using PacketTypes;
    using Types;
    using SubProfile;

    public partial class PalBot
    {
        private async Task<Response> SendAwaitResponse(IPacket packet)
        {
            var send = await Write(packet);

            if (!send)
            {
                return new Response
                {
                    Type = Type.Code,
                    What = What.MESG,
                    Code = Code.NOT_DELIVERED
                };
            }

            return await packetWatcher.Subscribe<Response>((t) => t.MessageId == packet.MessageId);
        }

        public async override Task<IGroup> GetGroup(string id)
        {
            if (SubProfiling.Groups.ContainsKey(id))
                return (OutGroup)SubProfiling.Groups[id];

            var pack = packetTemplates.GroupInfo(id);

            if (!await Write(pack))
                return null;

            var watcher = await packetWatcher.Subscribe<Response>(t => t.MessageId == pack.MessageId);

            Console.WriteLine(watcher.ToString());

            return null;
        }

        public override async Task<IUser[]> GetGroupUsers(string id)
        {
            var group = (Group)await GetGroup(id);
            return group == null ? null : SubProfiling.GroupUsers[group].Cast<OutUser>().ToArray();
        }

        public override async Task<IUser> GetUser(string id)
        {
            if (SubProfiling.Users.ContainsKey(id))
                return (OutUser)SubProfiling.Users[id];

            var pack = packetTemplates.UserInfo(id);

            if (!await Write(pack))
                return null;

            try
            {
                await packetWatcher.Subscribe<SubProfileQueryResult, Response>(t => t.Id.ToString() == id, t => t.MessageId == pack.MessageId && t.Code != Code.OK);
            }
            catch (PacketException ex)
            {
                Console.WriteLine("Issue with thing: " + ex.Packet.Command + " - " + ex.Packet.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Issue with thing: " + ex.ToString());
            }

            if (SubProfiling.Users.ContainsKey(id))
                return (OutUser)SubProfiling.Users[id];

            return null;
        }
         
        private async Task<IMessageResponse> SendMessage(MessageType target, DataType type, string id, byte[] data)
        {
            var packet = packetTemplates.Message(target, type, id, data);

            return (OutResponseMessage)new Response(await Write(packet));
        }

        private async Task<IMessageResponse> SendMessage(MessageType target, DataType type, string id, string data)
        {
            var packet = packetTemplates.Message(target, type, id, data);

            var send = await Write(packet, () => packetWatcher.Subscribe<Response>((t) => t.MessageId == packet.MessageId));

            if (send == null)
            {
                return (OutResponseMessage)new Response
                {
                    Type = Type.Code,
                    What = What.MESG,
                    Code = Code.NOT_DELIVERED
                };
            }

            return (OutResponseMessage)send;
        }

        public override async Task<IMessageResponse> GroupMessage(string id, string message)
        {
            return await SendMessage(MessageType.Group, DataType.Text, id, message);
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            return await GroupMessage(groupid, image.ToByteArray(), "");
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, string content, Bitmap image)
        {
            await GroupMessage(groupid, image.ToByteArray(), "");
            return await GroupMessage(groupid, content);
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, byte[] data, string filename)
        {
            return await SendMessage(MessageType.Group, DataType.Image, groupid, data);
        }

        public override async Task<IMessageResponse> PrivateMessage(string id, string message)
        {
            return await SendMessage(MessageType.Private, DataType.Text, id, message);
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            return await PrivateMessage(userid, image.ToByteArray(), "");
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, string content, Bitmap image)
        {
            await PrivateMessage(userid, image.ToByteArray(), "");
            return await PrivateMessage(userid, content);
        }

        public override async Task<IMessageResponse> PrivateMessage(string id, byte[] data, string filename)
        {
            return await SendMessage(MessageType.Private, DataType.Image, id, data);
        }
        
        public virtual async Task<bool> Login(string email, string password,
            string prefix = null,
            AuthStatus status = AuthStatus.Online,
            DeviceType device = DeviceType.PC,
            bool spamFilter = false)
        {
            this.Email = email;
            this.Password = password;
            this.Status = status;
            this.Device = device;
            this.SpamFilter = spamFilter;
            this.Prefix = prefix;

            var connected = await _client.Start();

            if (!connected)
            {
                _couldntConnect?.Invoke();
                return false;
            }

            if (!await Write(packetTemplates.Login(email, device, spamFilter)))
            {
                _couldntConnect?.Invoke();
                return false;
            }

            var resp = await packetWatcher.Subscribe(new LoginFailed(), new AuthRequest());

            if (resp.Packet is LoginFailed)
            {
                var pk = (LoginFailed)resp.Packet;
                _loginFailed?.Invoke(pk.Reason);
                broadcast.BroadcastLoginFailed(this, pk);
                return false;
            }

            var auth = (AuthRequest)resp.Packet;

            var pwd = PacketSerializer.Outbound.GetBytes(password);
            pwd = authentication.GenerateAuth(auth.Key, pwd);

            if (!await Write(packetTemplates.Auth(pwd, status)))
            {
                _couldntConnect?.Invoke();
                return false;
            }

            var balanceQuery = await packetWatcher.Subscribe(new LoginFailed(), new BalanceQueryResult());

            if (balanceQuery.Packet is LoginFailed)
            {
                var pk = (LoginFailed)resp.Packet;
                _loginFailed?.Invoke(pk.Reason);
                broadcast.BroadcastLoginFailed(this, pk);
                return false;
            }

            return true;
        }

        public virtual async Task<Response> AdminAction(AdminActions action, string user, string group)
        {
            var packet = packetTemplates.AdminAction(action, user, group);

            return await SendAwaitResponse(packet);
        }

        public virtual async Task<Response> AddContact(string user, string message = "I'd like to add you")
        {
            return await SendAwaitResponse(packetTemplates.AddContact(user, message));
        }

        public virtual async Task<Response> AddContactResponse(bool accept, string user)
        {
            return await SendAwaitResponse(packetTemplates.AddContactResponse(accept, user));
        }

        public virtual async Task<Response> CreateGroup(string name, string description, string password = null)
        {
            return await SendAwaitResponse(packetTemplates.CreateGroup(name, description, password));
        }

        public virtual async Task<Response> JoinGroup(string name, string password = null)
        {
            return await SendAwaitResponse(packetTemplates.JoinGroup(name, password));
        }

        public virtual async Task<Response> LeaveGroup(string group)
        {
            return await SendAwaitResponse(packetTemplates.LeaveGroup(group));
        }

        public virtual async Task<Response> UpdateProfile(string nickname, string status)
        {
            return await SendAwaitResponse(packetTemplates.UpdateProfile(nickname, status));
        }

        public virtual async Task<Response> UpdateProfile(ExtendedUser user)
        {
            return await SendAwaitResponse(packetTemplates.UpdateProfile(user));
        }

        public virtual async Task Disconnect()
        {
            await Write(new Packet
            {
                Command = "BYE",
                Headers = new Dictionary<string, string>(),
                Payload = new byte[0]
            });
            _client.Stop();
        }

        public virtual async Task<bool> UpdateAvatar(byte[] image)
        {
            var packet = packetTemplates.AvatarUpdate(image);
            return await Write(packet);
        }

        public virtual async Task<IMessage> Message(MessagePacket packet)
        {
            var user = await GetUser(packet.UserId);
            var group = packet.MesgType == MessageType.Group ? (await GetGroup(packet.GroupId)) : null;
            return new Message(packet, user, group, this);
        }
    }
}
