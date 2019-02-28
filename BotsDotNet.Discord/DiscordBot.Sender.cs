using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.Discord
{
    public partial class DiscordBot
    {
        public override Task<IUser[]> GetGroupUsers(string groupid)
        {
            var group = Connection.GroupChannels.FirstOrDefault(t => t.Id.ToString() == groupid);

            if (group == null)
                return Task.FromResult<IUser[]>(null);

            var users = group.Users.Select(t => (IUser)new User(t)).ToArray();
            return Task.FromResult(users);
        }

        public override Task<IUser> GetUser(string userid)
        {
            if (ulong.TryParse(userid, out ulong id))
            {
                var u = Connection.GetUser(id);
                return Task.FromResult((IUser)new User(u));
            }

            if (userid.Contains("#"))
            {
                var parts = userid.Split('#');
                var name = string.Join("#", parts.Take(parts.Length - 2));
                var desc = parts.Last();
                var u = Connection.GetUser(name, desc);
                return Task.FromResult((IUser)new User(u));
            }

            return Task.FromResult<IUser>(null);
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, string contents)
        {
            var group = Connection.GroupChannels.FirstOrDefault(t => t.Id.ToString() == groupid);
            var result = await group.SendMessageAsync(contents);
            return new MessageResponse(result);
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, string content, Bitmap image)
        {
            var group = Connection.GroupChannels.FirstOrDefault(t => t.Id.ToString() == groupid);
            var result = await group.SendFileAsync(image.ToStream(), "image.png", content);
            return new MessageResponse(result);
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            var group = Connection.GroupChannels.FirstOrDefault(t => t.Id.ToString() == groupid);
            var result = await group.SendFileAsync(image.ToStream(), "image.png", "");
            return new MessageResponse(result);
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, byte[] data, string filename)
        {
            var group = Connection.GroupChannels.FirstOrDefault(t => t.Id.ToString() == groupid);
            var result = await group.SendFileAsync(new MemoryStream(data), filename, "");
            return new MessageResponse(result);
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, string contents)
        {
            var user = Connection.GetUser(ulong.Parse(userid));
            var channel = await user.GetOrCreateDMChannelAsync();
            var resp = await channel.SendMessageAsync(contents);
            return new MessageResponse(resp);
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, string content, Bitmap image)
        {
            var user = Connection.GetUser(ulong.Parse(userid));
            var channel = await user.GetOrCreateDMChannelAsync();
            var resp = await channel.SendFileAsync(image.ToStream(), "image.png", content);
            return new MessageResponse(resp);
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            var user = Connection.GetUser(ulong.Parse(userid));
            var channel = await user.GetOrCreateDMChannelAsync();
            var resp = await channel.SendFileAsync(image.ToStream(), "image.png");
            return new MessageResponse(resp);
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, byte[] data, string filename)
        {
            var user = Connection.GetUser(ulong.Parse(userid));
            var channel = await user.GetOrCreateDMChannelAsync();
            var resp = await channel.SendFileAsync(new MemoryStream(data), filename);
            return new MessageResponse(resp);
        }

        public override async Task<IMessageResponse> Reply(IMessage message, string contents)
        {
            var msg = (Message)message;

            var res = await msg.ReturnChannel.SendMessageAsync(contents);
            return new MessageResponse(res);
        }

        public override async Task<IMessageResponse> Reply(IMessage message, string content, Bitmap image)
        {
            var msg = (Message)message;

            var res = await msg.ReturnChannel.SendFileAsync(image.ToStream(), "image.png", content);
            return new MessageResponse(res);
        }

        public override async Task<IMessageResponse> Reply(IMessage message,Bitmap image)
        {
            var msg = (Message)message;

            var res = await msg.ReturnChannel.SendFileAsync(image.ToStream(), "image.png");
            return new MessageResponse(res);
        }

        public override async Task<IMessageResponse> Reply(IMessage message, byte[] data, string filename)
        {
            var msg = (Message)message;

            var res = await msg.ReturnChannel.SendFileAsync(new MemoryStream(data), filename);
            return new MessageResponse(res);
        }
    }
}
