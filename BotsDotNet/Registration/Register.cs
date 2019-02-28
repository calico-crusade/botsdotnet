using System;

namespace BotsDotNet.Registration
{
    using Handling;

    [NoDescp]
    public class Register : CommandRegister
    {
        private PluginManager _pluginManager;

        public Register(IPluginManager pluginManager) : base(pluginManager)
        {
            if (!(pluginManager is PluginManager))
                throw new Exception("Invalid Plugin Manager loaded");

            _pluginManager = (PluginManager)pluginManager;
        }

        #region Restrictions
        public Register Restriction(IRestriction restriction)
        {
            _pluginManager.AddRestriction(restriction);
            return this;
        }

        public Register Restriction(Restriction restriction)
        {
            return Restriction((IRestriction)restriction);
        }

        public Register Restriction(string name, Func<IBot, IMessage, bool> validation, string platform = null, Action<IBot, IMessage> rejection = null)
        {
            return Restriction(Registration.Restriction.Get(name, validation, platform, rejection));
        }

        public Register UserRestriction(string name, Func<IUser, bool> validation, string platform = null, Action<IBot, IMessage> rejection = null)
        {
            return Restriction(name, (b, m) => validation(m.User), platform, rejection);
        }

        public Register GroupRestriction(string name, Func<IGroup, bool> validation, string platform = null, Action<IBot, IMessage> rejection = null)
        {
            return Restriction(name, (b, m) => validation(m.Group), platform, rejection);
        }

        public Register GroupUserRestriction(string name, Func<IGroup, IUser, bool> validation, string platform = null, Action<IBot, IMessage> rejection = null)
        {
            return Restriction(name, (b, m) => validation(m.Group, m.User), platform, rejection);
        }

        public Register MessageRestriction(string name, Func<IMessage, bool> validation, string platform = null, Action<IBot, IMessage> rejection = null)
        {
            return Restriction(name, (b, m) => validation(m), platform, rejection);
        }

        public Register Restriction(string name, Func<IBot, bool> validation, string platform = null, Action<IBot, IMessage> rejection = null)
        {
            return Restriction(name, (b, m) => validation(b), platform, rejection);
        }
        #endregion

        #region Comparitors
        public Register Comparitor(IComparitorProfile comparitor)
        {
            _pluginManager.AddComparitor(comparitor);
            return this;
        }

        public Register Comparitor(Comparitor comparitor)
        {
            return Comparitor((IComparitorProfile)comparitor);
        }

        public Register Comparitor(Type type, Func<IBot, IMessage, ICommand, ComparitorResult> matcher)
        {
            return Comparitor(Registration.Comparitor.Get(type, matcher));
        }

        public Register Comparitor(Type type, Func<string, string, bool> matcher)
        {
            return Comparitor(type, (b, m, c) => new ComparitorResult(matcher(m.Content, c.Comparitor)));
        }
        #endregion

        public CommandRegister With(string pluginSet, string restriction, string platform)
        {
            return new CommandRegister(_pluginManager, restriction, platform, pluginSet);
        }

        public CommandRegister PluginSet(string set)
        {
            return new CommandRegister(_pluginManager, pluginset: set);
        }

        public CommandRegister Restricted(string restriction)
        {
            return new CommandRegister(_pluginManager, restriction: restriction);
        }

        public CommandRegister Restricted(Restriction restriction)
        {
            return new CommandRegister(_pluginManager, restriction: restriction.Name, platform: restriction.Platform);
        }

        public CommandRegister PlatformSpecific(string platform)
        {
            return new CommandRegister(_pluginManager, platform: platform);
        }
    }
}
