using System;

namespace BotsDotNet.Palringo
{
    public static class InstanceExtensions
    {
        public static IBot UseConsoleLogging(this IBot ibot, bool logUnhandled = false)
        {
            if (!(ibot is PalBot))
                return ibot;

            var bot = (PalBot)ibot;

            if (logUnhandled)
                bot.On.UnhandledPacket += (p) =>
                    Extensions.ColouredConsole($"^w[^g{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}^w] ^rUNHANDLED ^w{p.Command}");

            bot.On.PacketReceived += (p) =>
                Extensions.ColouredConsole($"^w[^g{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}^w] ^c<< ^w{p.Command} - ^b{p.ContentLength}");

            bot.On.PacketSent += (p) =>
                Extensions.ColouredConsole($"^w[^g{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}^w] ^e>> ^w{p.Command} - ^b{p.ContentLength}");

            bot.On.Disconnected += () =>
                Extensions.ColouredConsole($"^w[^g{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}^w] ^yDisconnected..");

            bot.On.Exception += (e, n) =>
                Extensions.ColouredConsole($"^w[^g{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}^w] ^rERROR {n} - {e.ToString()}");

            bot.On.LoginFailed += (b, r) =>
                Extensions.ColouredConsole($"^w[^g{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}^w] ^rLogin Failed: {r.Reason}");

            return bot;
        }
        public static IBot AddAuth(this IBot bot, params string[] ids)
        {
            Restrictions.AuthRestriction.AuthorizedUsers.AddRange(ids);
            return bot;
        }
        public static IBot RemoveAuth(this IBot bot, params string[] ids)
        {
            foreach (var id in ids)
                Restrictions.AuthRestriction.AuthorizedUsers.Remove(id);
            return bot;
        }
        public static IBot AddIgnoredUser(this IBot bot, params string[] ids)
        {
            Restrictions.IgnoreRestriction.IgnoreUsers.AddRange(ids);
            return bot;
        }
        public static IBot RemoveIgnoredUser(this IBot bot, params string[] ids)
        {
            foreach (var id in ids)
                Restrictions.IgnoreRestriction.IgnoreUsers.Remove(id);
            return bot;
        }
        public static IBot AutoReconnect(this IBot ibot)
        {
            if (!(ibot is PalBot))
                return ibot;

            var bot = (PalBot)ibot;

            bot.On.Disconnected += async () => await bot.Login(bot.Email, bot.Password, bot.Status, bot.Device, bot.SpamFilter);
            return bot;
        }
        public static IBot ReloginOnThrottle(this IBot ibot)
        {
            if (!(ibot is PalBot))
                return ibot;

            var bot = (PalBot)ibot;

            bot.On.Throttle += async (b, c) => await b.Login(b.Email, b.Password, b.Status, b.Device, b.SpamFilter, b.EnablePlugins);
            return bot;
        }
    }
}
