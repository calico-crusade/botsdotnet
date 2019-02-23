using System;
using System.Drawing;
using System.Threading.Tasks;

namespace BotsDotNet
{
    public interface IBot
    {
        IUser Profile { get; }
        IGroup[] Groups { get; }
        string Platform { get; }

        Task<IMessage> NextMessage(Func<IMessage, bool> predicate);
        Task<IMessage> NextGroupMessage(string groupid);
        Task<IMessage> NextGroupMessage(string groupid, string userid);
        Task<IMessage> NextPrivateMessage(string userid);

        Task<IMessageResponse> Message(IMessage message);

        Task<IMessageResponse> PrivateMessage(string userid, string contents);
        Task<IMessageResponse> PrivateMessage(string userid, Bitmap image);
        Task<IMessageResponse> PrivateMessage(string userid, byte[] data);

        Task<IMessageResponse> GroupMessage(string groupid, string contents);
        Task<IMessageResponse> GroupMessage(string groupid, Bitmap image);
        Task<IMessageResponse> GroupMessage(string groupid, byte[] data);

        Task<IMessageResponse> Reply(IMessage message, string contents);
        Task<IMessageResponse> Reply(IMessage message, Bitmap image);
        Task<IMessageResponse> Reply(IMessage message, byte[] data);

        Task<IUser> GetUser(string userid);
        Task<IUser[]> GetUsers(params string[] userids);
        Task<IUser[]> GetGroupUsers(string groupid);
        Task<IGroup> GetGroup(string groupid);
    }
}
