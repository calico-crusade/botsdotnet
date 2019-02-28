using Newtonsoft.Json;
using System;

namespace BotsDotNet
{
    public static class Extensions
    {
        /// <summary>
        /// This is required because if C# detects that none of the code in an assembly has a hard reference 
        /// it will remove the assembly from the outputs. So use this somewhere in the application. 
        /// This method doesn't actually do anything.
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public static IBot RegisterTestPlugin(this IBot test)
        {
            return test.RegisterDynamicPlugins();
        }

        public static IBot RegisterDynamicPlugins(this IBot bot)
        {
            bot.Commands
               .UserRestriction("MeOnly", (u) => u.Id == "43681734", BotPlatform.Palringo, async (b,m) => await m.Reply("You can't use this"))
               .UserRestriction("MeOnly", (u) => u.Id == "43681734", BotPlatform.PalringoV3)
               .Restricted("MeOnly")
                    .Command("env info", async (m) => await m.Reply($"User: {Environment.UserName}\r\nOS: {Environment.OSVersion}"))
                    .Command("env time", async (m) => await m.Reply($"Time: {DateTime.Now.ToString()}\r\nUp Time: {Environment.TickCount}"));

            bot.Commands
                .Command("thing", async (m) => await m.Reply("hello"));

            bot.Commands
               .PlatformSpecific(BotPlatform.Discord)
                    .Command("profile", async (b, m, cmd) =>
                    {
                        if (!cmd.Contains("#"))
                        {
                            await m.Reply("Invalid user id!");
                            return;
                        }

                        var user = await b.GetUser(cmd.Trim());
                        if (user == null)
                        {
                            await m.Reply("User doesn't exist!");
                            return;
                        }

                        var json = JsonConvert.SerializeObject(user);
                        await m.Reply("User Profile: " + json);
                    })
                    .Command("test discord", async (m) => await m.Reply("Discord test"));

            return bot;
        }
    }
}
