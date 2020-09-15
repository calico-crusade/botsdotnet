using BotsDotNet.PalringoV3;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BotsDotNet.PalV3.CliTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            new Program().Start(args[0], args[1]).GetAwaiter().GetResult();

            Console.ReadLine();
        }

        public IBot Bot { get; set; }

        public Program()
        {
            Bot = PalBot.Create()
                        .RegisterTestPlugin();
        }

        public async Task Start(string email, string password)
        {
            Bot.OnError += (e) => Console.WriteLine(e);
            var success = await ((PalBot)Bot).Login(email, password, null, "!");

            if (success)
            {
                Console.WriteLine("Login Success");
                return;
            }

            Console.WriteLine("Something went wrong...");
        }
    }
}
