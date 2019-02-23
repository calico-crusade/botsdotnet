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

        public Task<IGroup> GetGroup(string id)
        {
            if (!SubProfiling.Groups.ContainsKey(id))
                return null;

            return Task.FromResult((IGroup)SubProfiling.Groups[id]);
        }

        public async Task<IUser[]> GetGroupUsers(string id)
        {
            var group = (Group)await GetGroup(id);
            return group == null ? null : SubProfiling.GroupUsers[group].ToArray();
        }

        public async Task<IUser> GetUser(string id)
        {
            if (SubProfiling.Users.ContainsKey(id))
                return SubProfiling.Users[id];

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
                return SubProfiling.Users[id];

            return new User
            {
                Id = id,
                Nickname = ""
            };
        }

        public async Task<IUser[]> GetUsers(params string[] ids)
        {
            return await Task.WhenAll(ids.Select(t => GetUser(t)));
        }

        private async Task<Response> SendMessage(MessageType target, DataType type, string id, byte[] data)
        {
            var packet = packetTemplates.Message(target, type, id, data);

            return new Response(await Write(packet));
        }

        private async Task<Response> SendMessage(MessageType target, DataType type, string id, string data)
        {
            var packet = packetTemplates.Message(target, type, id, data);

            var send = await Write(packet, () => packetWatcher.Subscribe<Response>((t) => t.MessageId == packet.MessageId));

            if (send == null)
            {
                return new Response
                {
                    Type = Type.Code,
                    What = What.MESG,
                    Code = Code.NOT_DELIVERED
                };
            }

            return send;
        }

        public async Task<IMessageResponse> Message(IMessage message)
        {
            return await SendMessage(message.MessageType, message.ContentType.FromMimeType(), message.GroupId, message.Content);
        }

        public async Task<IMessageResponse> GroupMessage(string id, string message)
        {
            return await SendMessage(MessageType.Group, DataType.Text, id, message);
        }

        public async Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            return await GroupMessage(groupid, image.ToByteArray());
        }

        public async Task<IMessageResponse> GroupMessage(string groupid, byte[] data)
        {
            return await SendMessage(MessageType.Group, DataType.Image, groupid, data);
        }

        public async Task<IMessageResponse> PrivateMessage(string id, string message)
        {
            return await SendMessage(MessageType.Private, DataType.Text, id, message);
        }

        public async Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            return await PrivateMessage(userid, image.ToByteArray());
        }

        public async Task<IMessageResponse> PrivateMessage(string id, byte[] data)
        {
            return await SendMessage(MessageType.Private, DataType.Image, id, data);
        }

        public async Task<IMessageResponse> Reply(IMessage message, string contents)
        {
            return await SendMessage(message.MessageType, DataType.Text, message.ReturnAddress, contents);
        }

        public async Task<IMessageResponse> Reply(IMessage message, Bitmap image)
        {
            return await SendMessage(message.MessageType, DataType.Image, message.ReturnAddress, image.ToByteArray());
        }

        public async Task<IMessageResponse> Reply(IMessage message, byte[] data)
        {
            return await SendMessage(message.MessageType, DataType.Image, message.ReturnAddress, data);
        }

        public async Task<IMessage> NextGroupMessage(string groupid)
        {
            return await NextMessage(t => t.MessageType == MessageType.Group && t.GroupId == groupid);
        }

        public async Task<IMessage> NextGroupMessage(string groupid, string userid)
        {
            return await NextMessage(t => t.MessageType == MessageType.Group && t.UserId == userid && t.GroupId == groupid);
        }

        public async Task<IMessage> NextPrivateMessage(string userid)
        {
            return await NextMessage(t => t.MessageType == MessageType.Private && t.UserId == userid);
        }

        public async Task<IMessage> NextMessage(Func<IMessage, bool> predicate)
        {
            var packet = await packetWatcher.Subscribe<MessagePacket>(t =>
            {
                var p = OnPacketFound(t, predicate);
                p.Wait();
                return p.Result;
            });

            return await Message(packet);
        }

        private async Task<bool> OnPacketFound(MessagePacket packet, Func<IMessage, bool> pred)
        {
            return pred(await Message(packet));
        }

        public async Task<bool> Login(string email, string password,
            AuthStatus status = AuthStatus.Online,
            DeviceType device = DeviceType.PC,
            bool spamFilter = false,
            bool enablePlugins = true)
        {
            this.Email = email;
            this.Password = password;
            this.Status = status;
            this.Device = device;
            this.SpamFilter = spamFilter;
            this.EnablePlugins = enablePlugins;

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

        public async Task<Response> AdminAction(AdminActions action, string user, string group)
        {
            var packet = packetTemplates.AdminAction(action, user, group);

            return await SendAwaitResponse(packet);
        }

        public async Task<Response> AddContact(string user, string message = "I'd like to add you")
        {
            return await SendAwaitResponse(packetTemplates.AddContact(user, message));
        }

        public async Task<Response> AddContactResponse(bool accept, string user)
        {
            return await SendAwaitResponse(packetTemplates.AddContactResponse(accept, user));
        }

        public async Task<Response> CreateGroup(string name, string description, string password = null)
        {
            return await SendAwaitResponse(packetTemplates.CreateGroup(name, description, password));
        }

        public async Task<Response> JoinGroup(string name, string password = null)
        {
            return await SendAwaitResponse(packetTemplates.JoinGroup(name, password));
        }

        public async Task<Response> LeaveGroup(string group)
        {
            return await SendAwaitResponse(packetTemplates.LeaveGroup(group));
        }

        public async Task<Response> UpdateProfile(string nickname, string status)
        {
            return await SendAwaitResponse(packetTemplates.UpdateProfile(nickname, status));
        }

        public async Task<Response> UpdateProfile(ExtendedUser user)
        {
            return await SendAwaitResponse(packetTemplates.UpdateProfile(user));
        }

        public async Task Disconnect()
        {
            await Write(new Packet
            {
                Command = "BYE",
                Headers = new Dictionary<string, string>(),
                Payload = new byte[0]
            });
            _client.Stop();
        }

        public async Task<bool> UpdateAvatar(byte[] image)
        {
            var packet = packetTemplates.AvatarUpdate(image);
            return await Write(packet);
        }

        public async Task<IMessage> Message(MessagePacket packet)
        {
            var user = await GetUser(packet.UserId);
            var group = packet.MesgType == MessageType.Group ? (await GetGroup(packet.GroupId)) : null;
            return new Message(packet, user, group);
        }
    }
}
