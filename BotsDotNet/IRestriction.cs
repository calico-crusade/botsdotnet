using System.Threading.Tasks;

namespace BotsDotNet
{
    public interface IRestriction
    {
        string Name { get; }
        string Platform { get; }

        Task<bool> Validate(IBot bot, IMessage message);

        Task OnRejected(IBot bot, IMessage message);
    }
}
