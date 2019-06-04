using System.Threading.Tasks;

namespace BotsDotNet.Palringo.CliTest
{
    public class TestPlugin : IPlugin
    {
        [Command("sample")]
        public async Task Testing(IBot bot, IMessage message, string cmd)
        {
            await bot.Reply(message, "Hello world! " + cmd);

            var msg = await message.NextMessage();

            await bot.Reply(message, "Well damn, " + message.User.Nickname + " - Nice " + msg.Content);
        }
    }
}
