using System.Threading.Tasks;

namespace BotsDotNet.Discord.CliTest
{
    public class TestPlugin : IPlugin
    {
        [Command("test", Description = "Testing")]
        public async Task Testing(IMessage msg, IUser user)
        {
            await msg.Reply($"Hello there {user.Nickname}. How are you doing today?");

            var answer = await msg.NextMessage();

            await msg.Reply($"I'm glad you're {answer.Content}");
        }
    }
}
