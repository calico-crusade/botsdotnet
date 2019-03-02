namespace BotsDotNet.BaaS.Conf
{
    public interface IValidator
    {
        bool Validate(out string[] issues);
    }
}
