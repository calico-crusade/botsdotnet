using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BotsDotNet.Test.Core
{
    using Handling;

    [TestClass]
    public class CommandComparitorTest : TestBase
    {
        [TestMethod]
        public void BasicPositiveTest()
        {
            var bot = GenerateBot();
            var com = new CommandComparitorProfile();
            var cmd = new Command("do something");
            var msg = GenerateMessage(bot);

            msg.Content = "do something impressive";

            var res = com.IsMatch(bot, msg, cmd);
            Assert.AreEqual(true, res.IsMatch, "Is Match Result");
            Assert.AreEqual("impressive", res.CappedCommand, "Command output");
        }
    }
}
