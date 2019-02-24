using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.TestPlugin
{
    using Handling;

    public class Test : IPlugin
    {
        [Command("hello", Description = "Hello world!")]
        public async Task TestMethod(IMessage msg, IUser user)
        {
            await msg.Reply($"Hello there {user.Nickname}. How are you doing today?");

            var answer = await msg.NextMessage();

            await msg.Reply($"I'm glad you're {answer.Content}");
        }

        [Command("discord", Platform = "Discord", Description = "Hello discord!")]
        public async Task DiscordTest(IMessage msg)
        {
            await msg.Reply("Hello Discord!");
        }

        [Command("pal", Platform = "Palringo", Description = "Hello palringo!")]
        public async Task PalringoTest(IMessage msg)
        {
            await msg.Reply("Hello Palringo!");
        }

        [Command("spark", Platform = "Spark", Description = "Hello spark!")]
        public async Task WebExTeamsTest(IMessage msg)
        {
            await msg.Reply("Hello WebEx teams! Formally Spark!");
        }

        [Command("help", Description = "Displays a help message")]
        public async Task HelpMenu(IMessage msg, IPluginManager pluginManager, IBot bot)
        {
            var plugins = pluginManager.Plugins()
                                       .Where(t =>
                                            t.Command.Platform == null ||
                                            (t.Command.Platform == null && bot.Platform == null) ||
                                            (bot.Platform != null && t.Command.Platform == bot.Platform))
                                       .Select(t => t.Command)
                                       .ToArray();

            var omsg = "Plugins: ";
            foreach (var plug in plugins)
                omsg += $"\r\n{(bot.Prefix ?? "")}{plug.Comparitor} ~ {plug.Description}";

            await msg.Reply(omsg);
        }
    }
}
