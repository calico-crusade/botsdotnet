using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.Discord
{
    public partial class DiscordBot
    {
        public Task<IGroup> GetGroup(string groupid)
        {
            var group = Groups.FirstOrDefault(t => t.Id == groupid);

            return Task.FromResult(group);
        }

        public Task<IUser[]> GetGroupUsers(string groupid)
        {
            var group = Connection.GroupChannels.FirstOrDefault(t => t.Id.ToString() == groupid);

            if (group == null)
                return Task.FromResult<IUser[]>(null);

            var users = group.Users.Select(t => (IUser)new User(t)).ToArray();
            return Task.FromResult(users);
        }

        public Task<IUser> GetUser(string userid)
        {
            var user = Connection.GetUser(ulong.Parse(userid));
            return Task.FromResult((IUser)new User(user));
        }

        public Task<IUser[]> GetUsers(params string[] userids)
        {
            return Task.WhenAll(userids.Select(t => GetUser(t)));
        }

        public async Task<IMessageResponse> GroupMessage(string groupid, string contents)
        {
            var group = Connection.GroupChannels.FirstOrDefault(t => t.Id.ToString() == groupid);
            var result = await group.SendMessageAsync(contents);
            return new MessageResponse(result);
        }

        public async Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            var group = Connection.GroupChannels.FirstOrDefault(t => t.Id.ToString() == groupid);
            var result = await group.SendFileAsync(image.ToStream(), "image.png", "");
            return new MessageResponse(result);
        }

        public async Task<IMessageResponse> GroupMessage(string groupid, byte[] data)
        {
            var group = Connection.GroupChannels.FirstOrDefault(t => t.Id.ToString() == groupid);
            var result = await group.SendFileAsync(new MemoryStream(data), "image.png", "");
            return new MessageResponse(result);
        }

        public Task<IMessageResponse> Message(IMessage message)
        {
            return Reply(message, message.Content);
        }

        public async Task<IMessage> NextGroupMessage(string groupid)
        {
            return await NextMessage(t => t.MessageType == MessageType.Group && t.GroupId == groupid);
        }

        public async Task<IMessage> NextGroupMessage(string groupid, string userid)
        {
            return await NextMessage(t => t.MessageType == MessageType.Group && t.UserId == userid && t.GroupId == groupid);
        }

        public async Task<IMessage> NextMessage(Func<IMessage, bool> predicate)
        {
            var task = new TaskCompletionSource<Message>();

            if (!awaitedMessages.TryAdd(predicate, task))
                return null;

            return await awaitedMessages[predicate].Task;
        }

        public async Task<IMessage> NextPrivateMessage(string userid)
        {
            return await NextMessage(t => t.MessageType == MessageType.Private && t.UserId == userid);
        }

        public async Task<IMessageResponse> PrivateMessage(string userid, string contents)
        {
            var user = Connection.GetUser(ulong.Parse(userid));
            var channel = await user.GetOrCreateDMChannelAsync();
            var resp = await channel.SendMessageAsync(contents);
            return new MessageResponse(resp);
        }

        public async Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            var user = Connection.GetUser(ulong.Parse(userid));
            var channel = await user.GetOrCreateDMChannelAsync();
            var resp = await channel.SendFileAsync(image.ToStream(), "image.png");
            return new MessageResponse(resp);
        }

        public async Task<IMessageResponse> PrivateMessage(string userid, byte[] data)
        {
            var user = Connection.GetUser(ulong.Parse(userid));
            var channel = await user.GetOrCreateDMChannelAsync();
            var resp = await channel.SendFileAsync(new MemoryStream(data), "image.png");
            return new MessageResponse(resp);
        }

        public async Task<IMessageResponse> Reply(IMessage message, string contents)
        {
            var msg = (Message)message;

            var res = await msg.ReturnChannel.SendMessageAsync(contents);
            return new MessageResponse(res);
        }

        public async Task<IMessageResponse> Reply(IMessage message, Bitmap image)
        {
            var msg = (Message)message;

            var res = await msg.ReturnChannel.SendFileAsync(image.ToStream(), "image.png");
            return new MessageResponse(res);
        }

        public async Task<IMessageResponse> Reply(IMessage message, byte[] data)
        {
            var msg = (Message)message;

            var res = await msg.ReturnChannel.SendFileAsync(new MemoryStream(data), "image.png");
            return new MessageResponse(res);
        }
    }
}
