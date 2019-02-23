using System;
using System.Threading.Tasks;

namespace BotsDotNet.WebExTeams.WebTest.Plugins
{
    public class TestPlugin : IPlugin
    {
        [Command("test", Description = "Hello world")]
        public async Task Test(IBot bot, IMessage message, string cmd)
        {
            try
            {
                await bot.Reply(message, "Hello world! " + cmd);
            }
            catch (Exception ex)
            {
                Extensions.Log(ex);
            }
        }
    }
}
