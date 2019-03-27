using System;

namespace BotsDotNet.Reddit.CliTest
{
    class Program
    {
        public RedditBot Bot { get; set; }

        static void Main(string[] args)
        {
            if (args == null || args.Length < 5)
            {
                Console.WriteLine("Please specify all arguments");
                return;
            }

            string clientid = args[0],
                   clientSecret = args[1],
                   clientUsername = args[2],
                   clientPassword = args[3],
                   redirectUrl = args[4];

            new Program().Start(clientid, clientSecret, clientUsername, clientPassword, redirectUrl);

            while (Console.ReadKey().Key != ConsoleKey.E)
                Console.WriteLine("Press \"E\" to exit");
        }

        public Program()
        {
            Bot = RedditBot.Create();

            Bot.Commands.PlatformSpecific(BotPlatform.Reddit)
                        .Command("$cardboard test", async (m) => await m.Reply("Hello!"))
                        .Command("$hello", async (m) => await m.Reply($"Hello there, {m.User.Nickname} in {m.Group.Name}"));
        }

        public async void Start(string clientid, string clientsecret, string clientusername, string clientpassword, string rediretUrl)
        {
            Bot.OnError += (e) => Console.WriteLine(e);
            var success = Bot.Start(clientid, clientsecret, clientusername, clientpassword, rediretUrl, "BotsDotNet", "!");

            if (success)
            {
                Console.WriteLine("Login Success");
                Console.WriteLine($"Username: {Bot.Profile.Nickname}({Bot.Profile.Id}) - {Bot.Profile.Status}");
                Bot.MonitorSubReddit("CardboardTestCom");
                return;
            }

            Console.WriteLine("Something went wrong...");
        }
    }
}
