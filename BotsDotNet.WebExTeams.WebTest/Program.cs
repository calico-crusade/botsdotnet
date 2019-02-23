using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StructureMap.AspNetCore;

namespace BotsDotNet.WebExTeams.WebTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(l =>
                {
                    l.ClearProviders();

                    l.AddEventSourceLogger();
                    l.AddConsole();
                    l.SetMinimumLevel(LogLevel.Trace);
                })
                .UseStructureMap()
                .UseIISIntegration()
                .UseStartup<Startup>();
    }
}
