using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.Twitch
{
    public partial class TwitchBot
    {
        public override async Task<IUser[]> GetGroupUsers(string groupid)
        {
            var users = await Api.Undocumented.GetChattersAsync(groupid);

            return users.Select(t => new User(t)).ToArray();
        }

        public override async Task<IUser> GetUser(string userid)
        {
            var users = await Api.Helix.Users.GetUsersAsync(new List<string> { userid }, accessToken: Token);
            var u = users.Users.FirstOrDefault();

            return u == null ? null : new User(u);
        }

        public override async Task<IGroup> GetGroup(string groupid)
        {
            var channel = await Api.V5.Channels.GetChannelByIDAsync(groupid);
            return channel == null ? null : new Group(channel);
        }

        private Task<IMessageResponse> Resp(bool res, bool image = false)
        {
            if (image)
                Error(new NotImplementedException("Images are not supported on Twitch!"));
            return Task.FromResult((IMessageResponse)new MessageResponse(res));
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, string contents)
        {
            if (int.TryParse(groupid, out int _))
            {
                var channel = await GetGroup(groupid);
                groupid = channel.Name;
            }

            Connection.SendMessage(groupid, contents);
            return await Resp(true);
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            return Resp(false, true);
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, byte[] data, string filename)
        {
            return Resp(false, true);
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, string contents, Bitmap image)
        {
            return Resp(false, true);
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, string contents)
        {
            if (int.TryParse(userid, out int u))
            {
                var user = await GetUser(userid);
                userid = user.Nickname;
            }

            Connection.SendWhisper(userid, contents);
            return await Resp(true);
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            return Resp(false, true);
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, byte[] data, string filename)
        {
            return Resp(false, true);
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, string contents, Bitmap image)
        {
            return Resp(false, true);
        }

        public override Task<IMessageResponse> Reply(IMessage message, string contents)
        {
            if (message.MessageType == MessageType.Group)
                return GroupMessage(message.Group.Name, contents);

            return PrivateMessage(message.User.Nickname, contents);
        }
    }
}
