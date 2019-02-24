using System.Threading.Tasks;

namespace BotsDotNet.Palringo.Restrictions
{
    public class AdminRestriction : IRestriction
    {
        public string Name => "Admin";

        public string Platform => PalBot.PLATFORM;

        public Task OnRejected(IBot bot, IMessage message)
        {
            return new Task(() => { });
        }

        public Task<bool> Validate(IBot bot, IMessage message)
        {
            return Task.FromResult(RestrictionUtil.ValidateAdmin(message));
        }
    }

    public class ModRestriction : IRestriction
    {
        public string Name => "Mod";

        public string Platform => PalBot.PLATFORM;

        public Task OnRejected(IBot bot, IMessage message)
        {
            return new Task(() => { });
        }

        public Task<bool> Validate(IBot bot, IMessage message)
        {
            return Task.FromResult(RestrictionUtil.ValidateMod(message));
        }
    }

    public class OwnerRestriction : IRestriction
    {
        public string Name => "Owner";

        public string Platform => PalBot.PLATFORM;

        public Task OnRejected(IBot bot, IMessage message)
        {
            return new Task(() => { });
        }

        public Task<bool> Validate(IBot bot, IMessage message)
        {
            return Task.FromResult(RestrictionUtil.ValidateOwner(message));
        }
    }
}
