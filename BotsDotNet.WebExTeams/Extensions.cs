using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using StructureMap;
using System;
using System.IO;

namespace BotsDotNet.WebExTeams
{
    using Handling;
    using Utilities;
    using Util;

    public static class Extensions
    {
        public const string SETTINGS_FILE = "appsettings.json";
        public const string SETTINGS_API_KEY = "SPARK_APIKEY";
        public const string SETTINGS_BASE_URI = "SPARK_BASE_URI";
        public const string SETTINGS_PART = "SparkConfig";
        public const string OUTPUT_LOG = @"logs\\log-{0:yyyy-MM-dd}.txt";

        public static IServiceProvider ConfigureSparkBot(this IServiceCollection services, 
            Action<MapHandler> handler = null, 
            IConfiguration config = null)
        {
            try
            {
                services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                });

                var pf = new PhysicalFileProvider(AppContext.BaseDirectory);
                config = config ?? new ConfigurationBuilder()
                                    .SetFileProvider(pf)
                                    .AddJsonFile(SETTINGS_FILE, false, true)
                                    .AddEnvironmentVariables()
                                    .Build();

                var configUtil = new ConfigUtility();
                config.GetSection(SETTINGS_PART).Bind(configUtil);

                if (configUtil == null)
                    configUtil = new ConfigUtility();

                if (string.IsNullOrEmpty(configUtil.ApiKey))
                    configUtil.ApiKey = config[SETTINGS_API_KEY];

                if (string.IsNullOrEmpty(configUtil.BaseUri))
                    configUtil.BaseUri = config[SETTINGS_BASE_URI];

                Log(configUtil);

                var map = ReflectionUtility.DependencyInjection()
                                           .Use<IBot, SparkBot>()
                                           .Config(c =>
                                           {
                                               c.ForSingletonOf<IBot>();
                                           })
                                           .Use<IPluginManager, PluginManager>()
                                           .Use<IConfigUtility, ConfigUtility>(configUtil);

                handler?.Invoke(map);

                var container = map.Create();

                container.Populate(services);

                return container.GetInstance<IServiceProvider>();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        public static IApplicationBuilder StartSparkBot(this IApplicationBuilder app, IBot bot)
        {
            try
            {
                if (!(bot is SparkBot))
                    return app;

                var b = (SparkBot)bot;

                b.Initialize().Wait();

                return app;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        public static void Log(object item)
        {
            var filename = string.Format(OUTPUT_LOG, DateTime.Now);
            var path = Path.Combine(AppContext.BaseDirectory, filename);
            File.AppendAllText(path, $"\r\n\r\n[{DateTime.Now}] LOG:\r\n{JsonConvert.SerializeObject(item, Formatting.Indented)}");
        }
    }
}
