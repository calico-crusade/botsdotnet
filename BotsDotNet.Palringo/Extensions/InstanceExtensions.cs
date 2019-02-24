using System;

namespace BotsDotNet.Palringo
{
    public static class InstanceExtensions
    {
        public static PalBot UseConsoleLogging(this PalBot bot, bool logUnhandled = false)
        {
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
        public static PalBot AddAuth(this PalBot bot, params string[] ids)
        {
            Restrictions.AuthRestriction.AuthorizedUsers.AddRange(ids);
            return bot;
        }
        public static PalBot RemoveAuth(this PalBot bot, params string[] ids)
        {
            foreach (var id in ids)
                Restrictions.AuthRestriction.AuthorizedUsers.Remove(id);
            return bot;
        }
        public static PalBot AddIgnoredUser(this PalBot bot, params string[] ids)
        {
            Restrictions.IgnoreRestriction.IgnoreUsers.AddRange(ids);
            return bot;
        }
        public static PalBot RemoveIgnoredUser(this PalBot bot, params string[] ids)
        {
            foreach (var id in ids)
                Restrictions.IgnoreRestriction.IgnoreUsers.Remove(id);
            return bot;
        }
        public static PalBot AutoReconnect(this PalBot bot)
        {
            bot.On.Disconnected += async () => await bot.Login(bot.Email, bot.Password, bot.Prefix, bot.Status, bot.Device, bot.SpamFilter);
            return bot;
        }
        public static PalBot ReloginOnThrottle(this PalBot bot)
        {
            bot.On.Throttle += async (b, c) => await b.Login(b.Email, b.Password, b.Prefix, b.Status, b.Device, b.SpamFilter);
            return bot;
        }
    }
}
