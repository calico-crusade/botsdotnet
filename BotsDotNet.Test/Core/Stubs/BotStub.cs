using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using BotsDotNet.Handling;

namespace BotsDotNet.Test.Core.Stubs
{
    public class BotStub : BotImpl
    {
        public BotStub(IPluginManager pluginManager) : base(pluginManager) { }

        public override IUser Profile => null;

        public override IGroup[] Groups => null;

        public override string Platform => "Test";

        public override Task<IUser[]> GetGroupUsers(string groupid)
        {
            throw new NotImplementedException();
        }

        public override Task<IUser> GetUser(string userid)
        {
            throw new NotImplementedException();
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, string contents)
        {
            throw new NotImplementedException();
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, Bitmap image)
        {
            throw new NotImplementedException();
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, byte[] data, string filename)
        {
            throw new NotImplementedException();
        }

        public override Task<IMessageResponse> GroupMessage(string groupid, string contents, Bitmap image)
        {
            throw new NotImplementedException();
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, string contents)
        {
            throw new NotImplementedException();
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, Bitmap image)
        {
            throw new NotImplementedException();
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, byte[] data, string filename)
        {
            throw new NotImplementedException();
        }

        public override Task<IMessageResponse> PrivateMessage(string userid, string contents, Bitmap image)
        {
            throw new NotImplementedException();
        }
    }
}
