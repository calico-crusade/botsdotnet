using System;
using System.Threading.Tasks;

namespace BotsDotNet.Twitch.CliTest
{
    public class Program
    {
        public IBot Bot { get; set; }

        static void Main(string[] args)
        {
            if (args == null || args.Length < 3)
            {
                Console.WriteLine("Please specify username, client_id and token");
                return;
            }

            string username = args[0],
                   clientid = args[1],
                   token = args[2];

            new Program().Start(username, clientid, token).Wait();

            while (Console.ReadKey().Key != ConsoleKey.E)
                Console.WriteLine("Press \"E\" to exit");
        }

        public Program()
        {
            Bot = TwitchBot.Create()
                           .RegisterTestPlugin();
        }

        public async Task Start(string username, string clientid, string token)
        {
            Bot.OnError += (e) => Console.WriteLine(e);
            var success = await ((TwitchBot)Bot).Login(username, clientid, token);

            if (success)
            {
                Console.WriteLine("Login Success");
                return;
            }

            Console.WriteLine("Something went wrong...");
        }
    }
}
