using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotsDotNet.Palringo.Restrictions
{
    public class AuthRestriction : IRestriction
    {
        public static List<string> AuthorizedUsers = new List<string>();

        public string Name => "auth";

        public string Platform => PalBot.PLATFORM;

        public Task OnRejected(IBot bot, IMessage message)
        {
            return new Task(() => { });
        }

        public Task<bool> Validate(IBot bot, IMessage message)
        {
            return Task.FromResult(AuthorizedUsers.Contains(message.UserId));
        }
    }
}
