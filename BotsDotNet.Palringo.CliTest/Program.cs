using System;
using System.Threading.Tasks;

namespace BotsDotNet.Palringo.CliTest
{
    public class Program
    {
        public PalBot Bot { get; set; }

        static void Main(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Please specify email and password");
                return;
            }

            string email = args[0],
                   password = args[1];

            new Program().Start(email, password).Wait();

            while (Console.ReadKey().Key != ConsoleKey.E)
                Console.WriteLine("Press \"E\" to exit");
        }

        public Program()
        {
            Bot = PalBot.Create();
            Bot
                .UseConsoleLogging()
                .RegisterTestPlugin();
        }

        public async Task Start(string email, string password)
        {
            Bot.OnError += (e) => Console.WriteLine(e);
            var success = await ((PalBot)Bot).Login(email, password, "!");

            if (success)
            {
                Console.WriteLine("Login Success");
                return;
            }

            Console.WriteLine("Something went wrong...");
        }
    }
}
