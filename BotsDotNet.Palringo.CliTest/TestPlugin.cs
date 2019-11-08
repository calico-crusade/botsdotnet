using BotsDotNet.Handling;
using System;
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

        [Command("help", Description = "Displays this menu")]
        public async Task Help(IMessage msg, IBot bot, IPluginManager manager)
        {
            try
            {
                var m = $"Connect 4 Bot help ({bot.Prefix})";

                foreach (var plug in manager.Plugins())
                {
                    var res = await manager.IsInRole(plug.Command.Restriction, msg);
                    if (!res)
                        continue;

                    m += $"\r\n{bot.Prefix} {plug.Command.Comparitor} ~ {plug.Command.Description}";
                }

                await msg.Reply(m);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred during Connect4 Help Menu: " + ex);
            }
        }
    }
}
