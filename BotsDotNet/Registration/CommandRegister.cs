using System;
using System.Reflection;

namespace BotsDotNet.Registration
{
    using Handling;

    [NoDescp]
    public class CommandRegister
    {
        public class CommandInstance : IPlugin { }

        private PluginManager _pluginManager;

        private readonly string _restriction = null;
        private readonly string _pluginset = null;
        private readonly string _platform = null;

        public CommandRegister(IPluginManager pluginManager, string restriction = null, string platform = null, string pluginset = null)
        {
            if (!(pluginManager is PluginManager))
                throw new Exception("Invalid Plugin Manager loaded");

            _pluginManager = (PluginManager)pluginManager;
            _restriction = restriction;
            _platform = platform;
            _pluginset = pluginset;
        }

        #region Plugins

        public CommandRegister Command(ReflectedPlugin plugin)
        {
            plugin.Command.Restriction = plugin.Command.Restriction ?? _restriction;
            plugin.Command.PluginSet = plugin.Command.PluginSet ?? _pluginset;
            plugin.Command.Platform = plugin.Command.Platform ?? _platform;
            _pluginManager.AddPlugin(plugin);
            return this;
        }

        public CommandRegister Command(object target, ICommand command, MethodInfo method)
        {
            return Command(new ReflectedPlugin
            {
                Instance = target,
                Command = command,
                Method = method
            });
        }

        public CommandRegister Command(ICommand command, Action<IBot, IMessage, string> expression)
        {
            return Command(expression.Target, command, expression.Method);
        }

        public CommandRegister Command(ICommand command, Action<IMessage> expression)
        {
            return Command(expression.Target, command, expression.Method);
        }

        public CommandRegister Command(ICommand command, Action<IMessage, string> expression)
        {
            return Command(expression.Target, command, expression.Method);
        }

        public CommandRegister Command(string cmd, Action<IBot, IMessage, string> expression, Options opts)
        {
            opts = opts ?? new Options();
            return Command(opts.ToCommand(cmd), expression);
        }

        public CommandRegister Command(string cmd, Action<IMessage> expression, Options opts = null)
        {
            opts = opts ?? new Options();
            return Command(opts.ToCommand(cmd), expression);
        }

        public CommandRegister Command(string cmd, Action<IMessage, string> expression, Options opts = null)
        {
            opts = opts ?? new Options();
            return Command(opts.ToCommand(cmd), expression);
        }

        public CommandRegister Command(string cmd, Action<IBot, IMessage, string> expression, MessageType? messageType = null,
            string restriction = null, string description = null, string platform = null, string pluginset = null)
        {
            return Command(new Command(cmd)
            {
                MessageType = messageType,
                Restriction = restriction,
                Description = description,
                Platform = platform,
                PluginSet = pluginset
            }, expression);
        }

        #endregion
    }
}
