using System;
using System.Threading.Tasks;

namespace BotsDotNet.Registration
{
    using Handling;

    [NoDescp]
    public class Restriction : IRestriction
    {
        public string Name { get; private set; }

        public string Platform { get; private set; }

        public Func<IBot, IMessage, bool> Validation { get; private set; }
        public Action<IBot, IMessage> Rejection { get; private set; }
        
        public Task<bool> Validate(IBot bot, IMessage message)
        {
            var res = Validation?.Invoke(bot, message) ?? false;
            return Task.FromResult(res);
        }

        public Task OnRejected(IBot bot, IMessage message)
        {
            Rejection?.Invoke(bot, message);
            return Task.CompletedTask;
        }

        public static Restriction Get(string name, Func<IBot, IMessage, bool> val, string platform = null, Action<IBot, IMessage> rej = null)
        {
            return new Restriction
            {
                Name = name,
                Platform = platform,
                Validation = val,
                Rejection = rej
            };
        }
    }
}
