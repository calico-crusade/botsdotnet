using System.Threading.Tasks;

namespace BotsDotNet.Palringo.Restrictions
{
    public class BotAdminRestriction : IRestriction
    {
        public string Name => "BotIsAdmin";

        public string Platform => PalBot.PLATFORM;

        public Task OnRejected(IBot bot, IMessage message)
        {
            return new Task(() => { });
        }

        public Task<bool> Validate(IBot bot, IMessage message)
        {
            return Task.FromResult(RestrictionUtil.ValidateAdmin(message, bot.Profile.Id));
        }
    }

    public class BotModRestriction : IRestriction
    {
        public string Name => "BotIsMod";

        public string Platform => PalBot.PLATFORM;

        public Task OnRejected(IBot bot, IMessage message)
        {
            return new Task(() => { });
        }

        public Task<bool> Validate(IBot bot, IMessage message)
        {
            return Task.FromResult(RestrictionUtil.ValidateMod(message, bot.Profile.Id));
        }
    }

    public class BotOwnerRestriction : IRestriction
    {
        public string Name => "BotIsOwner";

        public string Platform => PalBot.PLATFORM;

        public Task OnRejected(IBot bot, IMessage message)
        {
            return new Task(() => { });
        }

        public Task<bool> Validate(IBot bot, IMessage message)
        {
            return Task.FromResult(RestrictionUtil.ValidateOwner(message, bot.Profile.Id));
        }
    }
}
