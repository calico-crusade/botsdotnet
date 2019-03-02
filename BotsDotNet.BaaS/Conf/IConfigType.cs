namespace BotsDotNet.BaaS.Conf
{
    public interface IConfigType : IValidator
    {
        string Prefix { get; }
        string PluginSet { get; }
    }
}
