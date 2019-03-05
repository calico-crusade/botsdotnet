using System;

namespace BotsDotNet.AllInOne.CliTest
{
    using BaaS;
    using Palringo;

    public class Program
    {
        public static void Main(string[] args)
        {
            //Default to a console application
            bool runAsAService = false;

            ServiceBroker.Create(_ =>
            {
                _.AsAService = runAsAService;
                _.OnLoggedIn = (b) =>
                {
                    b.RegisterTestPlugin();
                    if (b is PalBot bot)
                    {
                        bot.On.GroupUpdate += (c, u) => Console.WriteLine($"Group Update: {u.UserId} {u.Type}ed {u.GroupId}");
                        bot.On.AdminAction += (c, u) => Console.WriteLine($"Admin Action: {u.SourceId} {u.Action}ed {u.TargetId} in {u.GroupId}");
                    }
                };
            }).Run();
        }
    }
}
