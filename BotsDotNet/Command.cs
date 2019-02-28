using System;

namespace BotsDotNet
{
    public interface ICommand
    {
        string Comparitor { get; }
        MessageType? MessageType { get; set; }
        string Restriction { get; set; }
        string Description { get; set; }
        string Platform { get; set; }
        string PluginSet { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class Command : Attribute, ICommand
    {
        public virtual string Comparitor { get; private set; }
        public virtual MessageType? MessageType { get; set; } = null;
        public virtual string Restriction { get; set; } = null;
        public virtual string Description { get; set; } = null;
        public virtual string Platform { get; set; } = null;
        public virtual string PluginSet { get; set; } = null;

        public Command(string comparitor)
        {
            Comparitor = comparitor;
        }

        public Command(string comparitor, string restriction)
        {
            Comparitor = comparitor;
            Restriction = restriction;
        }

        public Command(string comparitor, MessageType messageType)
        {
            Comparitor = comparitor;
            MessageType = messageType;
        }

        public Command(string comparitor, string restriction, MessageType messageType)
        {
            Comparitor = comparitor;
            Restriction = restriction;
            MessageType = messageType;
        }

        public Command Clone()
        {
            return new Command(Comparitor)
            {
                MessageType = MessageType,
                Restriction = Restriction,
                Description = Description,
                Platform = Platform
            };
        }
    }
}
