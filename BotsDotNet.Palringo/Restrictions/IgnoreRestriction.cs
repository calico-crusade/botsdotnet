using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotsDotNet.Palringo.Restrictions
{
    public class IgnoreRestriction : IRestriction
    {
        public static List<string> IgnoreUsers = new List<string>();

        public string Name => "ignored";

        public string Platform => PalBot.PLATFORM;

        public Task OnRejected(IBot bot, IMessage message)
        {
            return new Task(() => { });
        }

        public Task<bool> Validate(IBot bot, IMessage message)
        {
            return Task.FromResult(!IgnoreUsers.Contains(message.UserId));
        }
    }
}
