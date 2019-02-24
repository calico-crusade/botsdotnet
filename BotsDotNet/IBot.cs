using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet
{
    using Handling;

    public delegate void ExceptionCarrier(Exception ex);

    public interface IBot
    {
        event ExceptionCarrier OnError;

        IUser Profile { get; }
        IGroup[] Groups { get; }
        string Platform { get; }

        Task<IMessage> NextMessage(Func<IMessage, bool> predicate);
        Task<IMessage> NextMessage(string groupid);
        Task<IMessage> NextMessage(string groupid, string userid);
        Task<IMessage> NextPrivateMessage(string userid);

        Task<IMessageResponse> PrivateMessage(string userid, string contents);
        Task<IMessageResponse> PrivateMessage(string userid, Bitmap image);
        Task<IMessageResponse> PrivateMessage(string userid, byte[] data, string filename);
        Task<IMessageResponse> PrivateMessage(string userid, string contents, Bitmap image);

        Task<IMessageResponse> GroupMessage(string groupid, string contents);
        Task<IMessageResponse> GroupMessage(string groupid, Bitmap image);
        Task<IMessageResponse> GroupMessage(string groupid, byte[] data, string filename);
        Task<IMessageResponse> GroupMessage(string groupid, string contents, Bitmap image);

        Task<IMessageResponse> Reply(IMessage message, string contents);
        Task<IMessageResponse> Reply(IMessage message, Bitmap image);
        Task<IMessageResponse> Reply(IMessage message, byte[] data, string filename);
        Task<IMessageResponse> Reply(IMessage message, string contents, Bitmap image);

        Task<IUser> GetUser(string userid);
        Task<IUser[]> GetUsers(params string[] userids);
        Task<IUser[]> GetGroupUsers(string groupid);
        Task<IGroup> GetGroup(string groupid);
    }

    public abstract class BotImpl : IBot
    {
        public event ExceptionCarrier OnError = delegate { };

        public abstract IUser Profile { get; }
        public abstract IGroup[] Groups { get; }
        public abstract string Platform { get; }

        private readonly IPluginManager pluginManager;
        private ConcurrentDictionary<Func<IMessage, bool>, TaskCompletionSource<IMessage>> awaitedMessages { get; set; }
            = new ConcurrentDictionary<Func<IMessage, bool>, TaskCompletionSource<IMessage>>();

        public BotImpl(IPluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
        }

        #region Getting Next Message
        public virtual async Task<IMessage> NextMessage(string groupid)
        {
            return await NextMessage(t => t.MessageType == MessageType.Group && t.GroupId == groupid);
        }

        public virtual async Task<IMessage> NextMessage(string groupid, string userid)
        {
            return await NextMessage(t => t.MessageType == MessageType.Group && t.UserId == userid && t.GroupId == groupid);
        }

        public virtual async Task<IMessage> NextMessage(Func<IMessage, bool> predicate)
        {
            var task = new TaskCompletionSource<IMessage>();

            if (!awaitedMessages.TryAdd(predicate, task))
                return null;

            return await awaitedMessages[predicate].Task;
        }

        public virtual async Task<IMessage> NextPrivateMessage(string userid)
        {
            return await NextMessage(t => t.MessageType == MessageType.Private && t.UserId == userid);
        }

        public virtual bool CheckMessage(IMessage msg)
        {
            var tests = awaitedMessages.ToArray();

            foreach (var t in tests)
            {
                if (!t.Key(msg))
                    continue;

                awaitedMessages.TryRemove(t.Key, out TaskCompletionSource<IMessage> output);
                output.SetResult(msg);
                return true;
            }

            return false;
        }
        #endregion

        #region Information Requests

        public virtual Task<IGroup> GetGroup(string groupid)
        {
            var group = Groups.FirstOrDefault(t => t.Id == groupid);

            return Task.FromResult(group);
        }

        public virtual Task<IUser[]> GetUsers(params string[] userids)
        {
            return Task.WhenAll(userids.Select(t => GetUser(t)));
        }

        public abstract Task<IUser> GetUser(string userid);

        public abstract Task<IUser[]> GetGroupUsers(string groupid);

        #endregion

        #region Group Messaging

        public abstract Task<IMessageResponse> GroupMessage(string groupid, string contents);
        public abstract Task<IMessageResponse> GroupMessage(string groupid, Bitmap image);
        public abstract Task<IMessageResponse> GroupMessage(string groupid, byte[] data, string filename);
        public abstract Task<IMessageResponse> GroupMessage(string groupid, string contents, Bitmap image);

        #endregion

        #region Private Messaging

        public abstract Task<IMessageResponse> PrivateMessage(string userid, string contents);
        public abstract Task<IMessageResponse> PrivateMessage(string userid, Bitmap image);
        public abstract Task<IMessageResponse> PrivateMessage(string userid, byte[] data, string filename);
        public abstract Task<IMessageResponse> PrivateMessage(string userid, string contents, Bitmap image);

        #endregion

        #region Reply

        public virtual async Task<IMessageResponse> Reply(IMessage message, string contents)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, contents);

            return await PrivateMessage(message.ReturnAddress, contents);
        }

        public virtual async Task<IMessageResponse> Reply(IMessage message, Bitmap image)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, image);

            return await PrivateMessage(message.ReturnAddress, image);
        }

        public virtual async Task<IMessageResponse> Reply(IMessage message, string content, Bitmap image)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, content, image);

            return await PrivateMessage(message.ReturnAddress, content, image);
        }

        public virtual async Task<IMessageResponse> Reply(IMessage message, byte[] data, string filename)
        {
            if (message.MessageType == MessageType.Group)
                return await GroupMessage(message.ReturnAddress, data, filename);

            return await PrivateMessage(message.ReturnAddress, data, filename);
        }

        #endregion

        #region Misc

        public virtual async Task MessageReceived(IMessage message)
        {
            try
            {
                if (message.UserId == Profile.Id)
                    return;

                if (CheckMessage(message))
                    return;

                var res = await pluginManager.Process(message);

                if (res.Result == PluginResultType.Error && res.ReturnObject is Exception)
                    OnError(res.ReturnObject as Exception);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public void Error(Exception ex)
        {
            OnError(ex);
        }

        #endregion
    }
}
