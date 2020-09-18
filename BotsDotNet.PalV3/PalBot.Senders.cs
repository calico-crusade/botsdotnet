using System.Drawing;
using System.Threading.Tasks;

namespace BotsDotNet.PalringoV3
{
    using Models;

    public partial class PalBot
    {
        public override Task<IGroup> GetGroup(string groupid)
        {
            return cacheUtility.GetGroup(groupid, this);
        }

        public override Task<IUser> GetUser(string userid)
        {
            return cacheUtility.GetUser(userid, this);
        }

        public async override Task<IUser[]> GetGroupUsers(string groupid)
        {
            return await cacheUtility.GetGroupUsers(groupid, this);
        }

        public Task<ExtendedGroupUser> GetGroupUser(string groupid, string userid)
		{
            return cacheUtility.GetGroupUser(groupid, userid, this);
		}

        private async Task<IMessageResponse> Message(string id, bool isGroup, object content, string mimetype = "text/plain")
        {
            return (OutMessageResp)await WritePacket<MessageResponse>(packetTemplate.Message(id, isGroup, content, mimetype));
        }

        //4239["group member add",{"body":{"groupId":18334086,"password":""}}]
        public async Task<Resp> GroupJoin(int groupId, string password = "")
		{
            var result = await WritePacket<Resp>(packetTemplate.GroupJoin(groupId, password));

            if (result.Code != 200)
                return result;

            return await WritePacket<Resp>(packetTemplate.GroupMessageSubscribe(groupId.ToString()));
		}

        public Task<Resp> GroupLeave(int groupId)
		{
            return WritePacket<Resp>(packetTemplate.GroupLeave(groupId));
		}

        public override Task<IMessageResponse> GroupMessage(string groupid, string contents)
        {
            return Message(groupid, true, contents);
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            return Message(groupid, true, image.ToBuffer(), "image/jpeg");
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, byte[] data, string filename)
        {
            return Message(groupid, true, data, "image/jpeg");
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, string contents, Bitmap image)
        {
            GroupMessage(groupid, image);
            return GroupMessage(groupid, contents);
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, string contents)
        {
            return Message(userid, false, contents);
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            return Message(userid, false, image.ToBuffer(), "image/jpeg");
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, byte[] data, string filename)
        {
            return Message(userid, false, data, "image/jpeg");
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, string contents, Bitmap image)
        {
            PrivateMessage(userid, image);
            return PrivateMessage(userid, contents);
        }
    }
}
