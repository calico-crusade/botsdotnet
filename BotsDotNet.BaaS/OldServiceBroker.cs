using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using PeterKottas.DotNetCore.WindowsService;
using System;
using System.Runtime.InteropServices;

namespace BotsDotNet.BaaS
{
    using Conf;

    public class OldServiceBroker
    {
        private readonly Settings settings;

        private OldServiceBroker(Settings settings)
        {
            this.settings = settings ?? new Settings();
        }

        public void Run()
        {
            if (settings.Logger == null)
                settings.Logger = DefaultLogger();

            if (settings.Configuration == null)
                settings.Configuration = DefaultConfig();

            if (settings.Configuration == null)
            {
                settings.Logger.LogError("Could not get find configuration file, one will be generated. Please update the generated configuration");
                return;
            }

            if (settings.OnLoggedIn == null)
                settings.OnLoggedIn = (b) => { };

            if (settings.DependencyHandler == null)
                settings.DependencyHandler = (m) => { };

            if (settings.AsAService && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                settings.AsAService = false;
                settings.Logger.LogWarning("OSX Platform does not support running as a service. Default to console.");
            }

            if (settings.AsAService)
            {
                RunService();
                return;
            }

            RunCli();
        }

        private void RunService()
        {
            var logger = settings.Logger;
            var configuration = settings.Configuration;

            try
            {
                ServiceRunner<BotManager>.Run(config =>
                {
                    config.SetName(configuration.ServiceName);
                    config.SetDisplayName(configuration.ServiceDisplayName);
                    config.SetDescription(configuration.ServiceDescription);

                    config.Service(sc =>
                    {
                        sc.ServiceFactory((s, e) =>
                        {
                            return new BotManager(settings);
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

        private void RunCli()
        {
            Console.Title = settings.Configuration.ServiceDisplayName;

            var manager = new BotManager(settings);
            manager.Start();

            while (Console.ReadKey().Key != ConsoleKey.E)
                Console.WriteLine("Press \"E\" to exit");

            manager.Stop();
            Environment.Exit(0);
        }

        private ILogger DefaultLogger()
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
                .GetRequiredService<ILogger<OldServiceBroker>>();
        }

        private IConfiguration DefaultConfig()
        {
            if (string.IsNullOrEmpty(settings.SettingsPath))
                settings.SettingsPath = Settings.DEFAULT_SETTINGSFILE;

            return Configuration.FromJson(settings.SettingsPath);
        }

        public static OldServiceBroker Create(Settings settings = null)
        {
            return new OldServiceBroker(settings);
        }

        public static OldServiceBroker Create(Action<Settings> settings)
        {
            var set = new Settings();
            settings?.Invoke(set);
            return new OldServiceBroker(set);
        }
    }
}
