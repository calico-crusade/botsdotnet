using System;
using System.Collections.Generic;
using System.Text;

namespace BotsDotNet.Test.Core
{
    using Stubs;
    using Utilities;

    public abstract class TestBase
    {
        public virtual string PluginSet { get; set; } = "test";
        public virtual string Prefix { get; set; } = "!";

        public virtual IBot GenerateBot()
        {
            var dih = ReflectionUtility.DependencyInjection()
                                       .Use<IBot, BotStub>();

            var bot = (BotStub)dih.Build<IBot>();
            bot.PluginSets = PluginSet;
            bot.Prefix = Prefix;

            return bot;
        }

        public virtual IMessage GenerateMessage(IBot bot)
        {
            return new MessageStub(null)
            {
                Bot = bot,
                Content = "! do something",
                ContentType = ContentType.Text,
                Group = new GroupStub("test-group", "Some Test Group"),
                GroupId = "test-group",
                MessageType = MessageType.Group,
                MimeType = "text/plain",
                TimeStamp = DateTime.Now,
                User = new UserStub("test-user", "Some Test User", "status"),
                UserId = "test-user"
            };
        }
    }
}
