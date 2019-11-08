using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        [Command("give roll", Description = "Testing", Platform = BotPlatform.Discord)]
        public async Task GiveRoll(IMessage msg, string msgrole, DiscordBot bot, IUser msgUsr)
        {
            try
            {
                var validUsers = new List<string>
                {
                    "Cardboard#0026",
                    "EB0LAxV1RUS#3573"
                };

                if (!validUsers.Contains(msgUsr.Nickname))
                {
                    await msg.Reply("You cannot use this command!");
                    return;
                }

                var id = ulong.Parse(msg.GroupId);
                var channel = bot.Connection.GetChannel(ulong.Parse(msg.GroupId));
                var guild = bot.Connection.Guilds.Where(t => t.Channels.Any(a => a.Id == channel.Id)).First();
                if (guild == null)
                {
                    Console.WriteLine("No guild?");
                    return;
                }

                var role = guild.Roles.FirstOrDefault(t => t.Name.Equals(msgrole, StringComparison.InvariantCultureIgnoreCase));

                if (role == null)
                {
                    await msg.Reply("Cannot find a role with that name!");
                    return;
                }

                await msg.Reply("Giving role to users. This might take a minute...");

                var users = new List<IGuildUser>();

                foreach (var user in guild.Users)
                {
                    if (user.Roles.Any(t => t.Name.Equals(msgrole, StringComparison.InvariantCultureIgnoreCase)))
                        continue;

                    await user.AddRoleAsync(role);
                    users.Add(user);
                    Console.WriteLine($"{user.Username} given {msgrole}");
                }

                await msg.Reply($"Given {users.Count} users the {msgrole} role.\r\n" + string.Join(", ", users.Select(t => t.Username)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
