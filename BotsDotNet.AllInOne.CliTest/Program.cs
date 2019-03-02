namespace BotsDotNet.AllInOne.CliTest
{
    using BaaS;

    public class Program
    {
        public static void Main(string[] args)
        {
            //Default to a console application
            bool runAsAService = false;

            ServiceBroker.Get().Run(runAsAService, (b) =>
            {
                b.RegisterTestPlugin();
            });
        }
    }
}
