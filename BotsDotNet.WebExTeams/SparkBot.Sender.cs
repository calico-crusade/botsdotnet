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
        public override async Task<IGroup> GetGroup(string groupid)
        {
            return (OutGroup)await cacheUtility.GetRoom(this, groupid);
        }

        public override async Task<IUser[]> GetGroupUsers(string groupid)
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

        public override async Task<IUser> GetUser(string userid)
        {
            return await cacheUtility.GetUser(this, userid);
        }
        
        public async Task<IMessageResponse> GroupMessage(string id, string markdown, MemoryFile file)
        {
            var msg = await Connection.CreateFileMessageAsync(roomId: id, markdown: markdown, file: file);
            return new MessageResponse(msg);
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, string contents)
        {
            var msg = await Connection.CreateMessageAsync(roomId: groupid, markdown: contents);
            return new MessageResponse(msg);
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            return await GroupMessage(groupid, "", image.FileFromImage());
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, string content, Bitmap image)
        {
            return await GroupMessage(groupid, content, image.FileFromImage());
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, byte[] data, string filename)
        {
            var file = new MemoryFile
            {
                Content = data,
                FileName = filename
            };
            return await GroupMessage(groupid, "", file);
        }

        public async Task<IMessageResponse> PrivateMessage(string id, string markdown, MemoryFile file)
        {
            var msg = await Connection.CreateFileMessageAsync(toPersonId: id, markdown: markdown, file: file);
            return new MessageResponse(msg);
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, string contents)
        {
            var msg = await Connection.CreateMessageAsync(toPersonId: userid, markdown: contents);
            return new MessageResponse(msg);
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            return await PrivateMessage(userid, "", image.FileFromImage());
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, string content, Bitmap image)
        {
            return await PrivateMessage(userid, content, image.FileFromImage());
        }

        public override async Task<IMessageResponse> PrivateMessage(string userid, byte[] data, string filename)
        {
            var file = new MemoryFile
            {
                Content = data,
                FileName = filename
            };
            return await PrivateMessage(userid, "", file);
        }

        public async Task<IMessageResponse> Reply(IMessage message, string markdown, MemoryFile file)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, markdown, file);

            return await PrivateMessage(message.ReturnAddress, markdown, file);
        }

    }
}
