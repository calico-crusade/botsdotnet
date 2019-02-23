using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.WebExTeams
{
    using SparkDotNet;
    using SparkDotNet.Models;
    using Util;

    public partial class SparkBot
    {
        public async Task<IGroup> GetGroup(string groupid)
        {
            return await cacheUtility.GetRoom(this, groupid);
        }

        public async Task<IUser[]> GetGroupUsers(string groupid)
        {
            return (await GetMembership(groupid)).Select(t => t.Key).ToArray();
        }

        public async Task<KeyValuePair<IUser, Membership>[]> GetMembership(string groupid)
        {
            var room = await cacheUtility.GetRoom(this, groupid);
            var memberships = await Connection.GetMembershipsAsync(roomId: groupid);

            var userids = memberships.Select(t => t.PersonId).ToArray();
            var users = await GetUsers(userids);

            return users.Select(t => new KeyValuePair<IUser, Membership>(t, memberships.FirstOrDefault(a => a.PersonId == t.Id))).ToArray();
        }

        public async Task<IUser> GetUser(string userid)
        {
            return await cacheUtility.GetUser(this, userid);
        }

        public Task<IUser[]> GetUsers(params string[] userids)
        {
            return Task.WhenAll(userids.Select(t => GetUser(t)));
        }

        public async Task<IMessageResponse> GroupMessage(string id, string markdown, MemoryFile file)
        {
            var msg = await Connection.CreateFileMessageAsync(roomId: id, markdown: markdown, file: file);
            return new MessageResponse(msg);
        }

        public async Task<IMessageResponse> GroupMessage(string groupid, string contents)
        {
            var msg = await Connection.CreateMessageAsync(roomId: groupid, markdown: contents);
            return new MessageResponse(msg);
        }

        public async Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            return await GroupMessage(groupid, "", image.FileFromImage());
        }

        public async Task<IMessageResponse> GroupMessage(string groupid, byte[] data)
        {
            var file = new MemoryFile
            {
                Content = data,
                FileName = "somefile.dat"
            };
            return await GroupMessage(groupid, "", file);
        }

        public async Task<IMessageResponse> Message(IMessage message)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, message.Content);

            return await PrivateMessage(message.ReturnAddress, message.Content);
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

        public async Task<IMessageResponse> PrivateMessage(string id, string markdown, MemoryFile file)
        {
            var msg = await Connection.CreateFileMessageAsync(toPersonId: id, markdown: markdown, file: file);
            return new MessageResponse(msg);
        }

        public async Task<IMessageResponse> PrivateMessage(string userid, string contents)
        {
            var msg = await Connection.CreateMessageAsync(toPersonId: userid, markdown: contents);
            return new MessageResponse(msg);
        }

        public async Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            return await PrivateMessage(userid, "", image.FileFromImage());
        }

        public async Task<IMessageResponse> PrivateMessage(string userid, byte[] data)
        {
            var file = new MemoryFile
            {
                Content = data,
                FileName = "somefile.dat"
            };
            return await PrivateMessage(userid, "", file);
        }

        public async Task<IMessageResponse> Reply(IMessage message, string markdown, MemoryFile file)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, markdown, file);

            return await PrivateMessage(message.ReturnAddress, markdown, file);
        }

        public async Task<IMessageResponse> Reply(IMessage message, string contents)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, contents);

            return await PrivateMessage(message.ReturnAddress, contents);
        }

        public async Task<IMessageResponse> Reply(IMessage message, Bitmap image)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, image);

            return await PrivateMessage(message.ReturnAddress, image);
        }

        public async Task<IMessageResponse> Reply(IMessage message, byte[] data)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, data);

            return await PrivateMessage(message.ReturnAddress, data);
        }
    }
}
