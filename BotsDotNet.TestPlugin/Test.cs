using System.Threading.Tasks;

namespace BotsDotNet.TestPlugin
{
    public class Test : IPlugin
    {
        [Command("$test")]
        public async Task TestMethod(IMessage msg, IUser user)
        {
            await msg.Reply($"Hello there {user.Nickname}. How are you doing today?");

            var answer = await msg.NextMessage();

            await msg.Reply($"I'm glad you're {answer.Content}");
        }

        [Command("$discord", Platform = "Discord")]
        public async Task DiscordTest(IMessage msg)
        {
            await msg.Reply("Hello Discord!");
        }

        [Command("!pal", Platform = "Palringo")]
        public async Task PalringoTest(IMessage msg)
        {
            await msg.Reply("Hello Palringo!");
        }

        [Command("spark", Platform = "Spark")]
        public async Task WebExTeamsTest(IMessage msg)
        {
            await msg.Reply("Hello WebEx teams! Formally Spark!");
        }
    }
}
