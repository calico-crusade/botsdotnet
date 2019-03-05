using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using PeterKottas.DotNetCore.WindowsService;
using System;
using System.Runtime.InteropServices;

namespace BotsDotNet.BaaS
{
    using Conf;

    public class ServiceBroker<T>
    {
        public const string DEFAULT_SETTINGSFILE = "bot.settings.json";

        internal readonly BotManager<T> botManager;
        internal readonly ILogger logger;
        internal readonly IConfiguration<T> configuration;

        internal ServiceBroker(ILogger logger, IConfiguration<T> configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            botManager = new BotManager<T>(logger, configuration);
        }

        public void RunAsAService(Action<IBot> onloggedIn = null)
        {
            if (configuration == null)
            {
                logger.LogError("Could not get configuration file.");
                return;
            }

            try
            {
                ServiceRunner<Service<T>>.Run(config =>
                {
                    config.SetName(configuration.ServiceName);
                    config.SetDisplayName(configuration.ServiceDisplayName);
                    config.SetDescription(configuration.ServiceDescription);

                    config.Service(sc =>
                    {
                        sc.ServiceFactory((s, e) =>
                        {
                            return new Service<T>(botManager, onloggedIn);
                        });

                        sc.OnStart((s, e) =>
                        {
                            s.Start();
                        });

                        sc.OnStop(s =>
                        {
                            s.Stop();
                        });

                        sc.OnShutdown(s =>
                        {
                            s.Stop();
                        });

                        sc.OnError(e =>
                        {
                            logger.LogError(e, "Error occurred during service life-time");
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during service creation.");
            }
        }

        public void RunAsConsole(Action<IBot> onloggedIn = null)
        {
            if (configuration == null)
            {
                logger.LogError("Could not get configuration file.");
                return;
            }

            Console.Title = configuration.ServiceDisplayName;
            botManager.Start(onloggedIn);
        }

        public void Run(bool service = true, Action<IBot> onloggedIn = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && service)
            {
                logger.LogWarning("OSX Platform does not support running as a service. Defaulting to Console.");
                service = false;
            }

            if (!service)
            {
                RunAsConsole(onloggedIn);
                while (Console.ReadKey().Key != ConsoleKey.E)
                    Console.WriteLine("Hit the \"E\" key to exit.");

                return;
            }

            RunAsAService(onloggedIn);
        }

        private static ILogger NLogLogger()
        {
            return new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                })
                .BuildServiceProvider()
                .GetRequiredService<ILogger<ServiceBroker>>();
        }
        
        public static ServiceBroker<T> Get(string filename = DEFAULT_SETTINGSFILE)
        {
            var config = Configuration<T>.FromJson(filename);
            return new ServiceBroker<T>(NLogLogger(), config);
        }
        
        public static ServiceBroker<T> Get(ILogger logger, string filename = DEFAULT_SETTINGSFILE)
        {
            var config = Configuration<T>.FromJson(filename);
            return new ServiceBroker<T>(logger, config);
        }

        public static ServiceBroker<T> Get(IConfiguration<T> config)
        {
            return new ServiceBroker<T>(NLogLogger(), config);
        }

        public static ServiceBroker<T> Get(ILogger logger, IConfiguration<T> config)
        {
            return new ServiceBroker<T>(logger, config);
        }
    }

    public class ServiceBroker : ServiceBroker<ServiceBroker.SettingsFake>
    {
        public class SettingsFake
        {

        }

        internal ServiceBroker(ILogger logger, IConfiguration<SettingsFake> configuration) : base(logger, configuration) { }
    }
}
