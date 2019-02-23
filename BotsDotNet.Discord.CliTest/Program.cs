using System;
using System.Threading.Tasks;

namespace BotsDotNet.Discord.CliTest
{
    public class Program
    {
        public DiscordBot Bot { get; set; }

        public Program()
        {
            Bot = DiscordBot.Create();
        }

        public async Task Start(string token)
        {
            var success = await Bot.Start(token);

            if (success)
            {
                Console.WriteLine("Login Success");
                return;
            }

            Console.WriteLine("Login Failed");
        }

        static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Please specify token");
                return;
            }

            string token = args[0];

            new Program().Start(token).Wait();

            while (Console.ReadKey().Key != ConsoleKey.E)
                Console.WriteLine("Press \"E\" to exit");
        }
    }
}
