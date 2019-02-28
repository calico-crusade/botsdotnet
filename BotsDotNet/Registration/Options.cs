namespace BotsDotNet.Registration
{
    public class Options
    {
        public virtual MessageType? MessageType { get; set; } = null;
        public virtual string Restriction { get; set; } = null;
        public virtual string Description { get; set; } = null;
        public virtual string Platform { get; set; } = null;
        public virtual string PluginSet { get; set; } = null;

        public Options() { }

        public Command ToCommand(string cmd)
        {
            return new Command(cmd)
            {
                MessageType = MessageType,
                Restriction = Restriction,
                Description = Description,
                Platform = Platform,
                PluginSet = PluginSet
            };
        }
    }
}
