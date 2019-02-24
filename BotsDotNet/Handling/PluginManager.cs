using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BotsDotNet.Handling
{
    using Utilities;

    public interface IPluginManager
    {
        Task<PluginResult> Process(IMessage message);

        Task<bool> IsInRole(string restricts, IMessage message, bool executeReject = false);

        IEnumerable<IExportedPlugin> Plugins();
    }

    public class PluginManager : IPluginManager
    {
        private const string RESTRICTION_SPLITTER = ",";
        private const string PLATFORM_AGNOSTIC_IND = "_";

        private static List<ReflectedPlugin> plugins = null;
        private static Dictionary<Type, IComparitorProfile> profiles = null;
        private static Dictionary<string, Dictionary<string, IRestriction>> restrictions = null;

        private readonly IReflectionUtility _reflectionUtility;

        public PluginManager(IReflectionUtility reflectionUtility)
        {
            _reflectionUtility = reflectionUtility;
        }

        public async Task<PluginResult> Process(IMessage message)
        {
            Init();
            var bot = message.Bot;

            foreach(var plugin in plugins)
            {
                try
                {
                    //Skip plugin if platforms don't match
                    if (!string.IsNullOrEmpty(plugin.Command.Platform) &&
                        bot.Platform != plugin.Command.Platform)
                        continue;

                    //Skip plugin if message types don't match
                    if (plugin.Command.MessageType != null &&
                        message.MessageType != plugin.Command.MessageType)
                        continue;

                    var msg = message.Content.Trim();
                    var typ = plugin.Command.GetType();

                    //Skip plugin if there is no comparitor profile
                    if (!profiles.ContainsKey(typ))
                        continue;
                    
                    var pro = profiles[typ];
                    var res = pro.IsMatch(bot, message, plugin.Command);

                    //Skip if comparitor doesn't match
                    if (!res.IsMatch)
                        continue;

                    //Stop processing plugins if user doesn't have the correct permissions
                    if (!await IsInRole(plugin.Command.Restriction, message, true))
                        return new PluginResult(PluginResultType.Restricted, null, null);

                    //Run the plugin
                    _reflectionUtility.ExecuteDynamicMethod(plugin.Method, plugin.Instance, out bool error, 
                        bot, message, res.CappedCommand, message.User, message.Group,
                        plugin.Instance, plugin.Command);
                    //Return the results of the plugin execution
                    return new PluginResult(error ? PluginResultType.Error : PluginResultType.Success, null, plugin);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return new PluginResult(PluginResultType.Error, ex, plugin);
                }                
            }

            return new PluginResult(PluginResultType.NotFound, null, null);
        }

        public async Task<bool> IsInRole(string restricts, IMessage message, bool executeReject = false)
        {
            if (string.IsNullOrEmpty(restricts))
                return true;
            var bot = message.Bot;

            Init();

            var parts = restricts.Split(new[] { RESTRICTION_SPLITTER }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(t => t.Trim().ToLower())
                                 .ToArray();

            foreach(var part in parts)
            {
                var res = GetRestriction(bot, part);
                if (res == null)
                    continue;

                if (await res.Validate(bot, message))
                    continue;
                
                if (executeReject)
                    await res.OnRejected(bot, message);

                return false;
            }

            return true;
        }

        public IRestriction GetRestriction(IBot bot, string name)
        {
            Init();
            var plat = bot.Platform ?? PLATFORM_AGNOSTIC_IND;

            if (restrictions.ContainsKey(plat) &&
                restrictions[plat].ContainsKey(name))
                return restrictions[plat][name];

            if (!restrictions.ContainsKey(PLATFORM_AGNOSTIC_IND))
                return null;

            if (!restrictions[PLATFORM_AGNOSTIC_IND].ContainsKey(name))
                return null;

            return restrictions[PLATFORM_AGNOSTIC_IND][name];
        }

        public IEnumerable<IExportedPlugin> Plugins()
        {
            Init();

            return plugins.ToArray();
        }

        public void Init()
        {
            LoadPlugins();
            LoadProfiles();
            LoadRestrictions();
        }

        public void LoadPlugins()
        {
            if (plugins != null)
                return;

            plugins = GetPlugins().OrderByDescending(t => t.Command.Comparitor.Length).ToList();
        }

        public void LoadProfiles()
        {
            if (profiles != null)
                return;

            profiles = new Dictionary<Type, IComparitorProfile>();

            var profs = _reflectionUtility.GetAllTypesOf<IComparitorProfile>();

            foreach(var profile in profs)
            {
                if (profiles.ContainsKey(profile.AttributeType))
                    throw new Exception($"Duplicate Comparitor Profile loaded: {profile.AttributeType.Name}");

                profiles.Add(profile.AttributeType, profile);
            }
        }

        public void LoadRestrictions()
        {
            if (restrictions != null)
                return;

            restrictions = new Dictionary<string, Dictionary<string, IRestriction>>();

            foreach(var restriction in _reflectionUtility.GetAllTypesOf<IRestriction>())
            {
                var plat = restriction.Platform?.Split(new[] { RESTRICTION_SPLITTER }, StringSplitOptions.RemoveEmptyEntries);

                if (plat == null || plat.Length <= 0)
                    plat = new string[] { PLATFORM_AGNOSTIC_IND };

                foreach (var platform in plat)
                {
                    if (!restrictions.ContainsKey(platform))
                        restrictions.Add(platform, new Dictionary<string, IRestriction>());

                    var name = restriction.Name.ToLower().Trim();
                    if (restrictions[platform].ContainsKey(name))
                        throw new Exception($"Duplicate restriction loaded: {name}");

                    restrictions[platform].Add(name, restriction);
                }
            }
        }

        public IEnumerable<ReflectedPlugin> GetPlugins()
        {
            var plugins = _reflectionUtility.GetAllTypesOf<IPlugin>();
            
            foreach (var plugin in plugins)
            {
                var type = plugin.GetType();

                foreach(var method in type.GetMethods())
                {
                    if (!Attribute.IsDefined(method, typeof(Command)))
                        continue;

                    var attributes = method.GetCustomAttributes<Command>();
                    foreach(var attribute in attributes)
                    {
                        var platforms = attribute.Platform?.Split(new[] { RESTRICTION_SPLITTER }, StringSplitOptions.RemoveEmptyEntries);

                        if (string.IsNullOrEmpty(attribute.Platform) || platforms == null || platforms.Length <= 0)
                        {
                            yield return new ReflectedPlugin
                            {
                                Command = attribute,
                                Instance = plugin,
                                Method = method
                            };
                            continue;
                        }

                        foreach (var plat in platforms)
                        {
                            var tmp = attribute.Clone();
                            tmp.Platform = plat;
                            yield return new ReflectedPlugin
                            {
                                Command = tmp,
                                Instance = plugin,
                                Method = method
                            };
                        }
                    }
                }
            }
        }
    }
}
