using RedditSharp.Things;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace BotsDotNet.Reddit
{
    public partial class RedditBot
    {

        public override Task<IUser[]> GetGroupUsers(string groupid)
        {
            throw new NotSupportedException("Getting users is not supported on reddit.");
        }

        public override async Task<IUser> GetUser(string userid)
        {
            var user = await Connection.GetUserAsync(userid);
            return new User(user);
        }

        public override async Task<IMessageResponse> GroupMessage(string groupid, string contents)
        {
            try
            {
                var post = await Connection.GetPostAsync(new Uri(groupid));

                return new MessageResponse(await post.CommentAsync(contents));

            }
            catch (Exception ex)
            {
                Error(ex);
                return new MessageResponse(false);
            }

        }

        public override Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            throw new NotSupportedException("Reddit does not support images");
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, byte[] data, string filename)
        {
            throw new NotSupportedException("Reddit does not support images");
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, string contents, Bitmap image)
        {
            throw new NotSupportedException("Reddit does not support images");
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, string contents)
        {
            throw new NotSupportedException("Reddit does not support direct messages at this time.");
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            throw new NotSupportedException("Reddit does not support images");
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, byte[] data, string filename)
        {
            throw new NotSupportedException("Reddit does not support images");
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, string contents, Bitmap image)
        {
            throw new NotSupportedException("Reddit does not support images");
        }

        public override async Task<IMessageResponse> Reply(IMessage message, string contents)
        {
            try
            {
                if (!(message.Original.Original is Comment comment))
                    return new MessageResponse(false);

                var c = await comment.ReplyAsync(contents);
                return new MessageResponse(c);
            }
            catch (Exception ex)
            {
                Error(ex);
                return new MessageResponse(false);
            }
        }
    }
}
