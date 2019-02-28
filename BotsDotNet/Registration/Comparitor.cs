using System;

namespace BotsDotNet.Registration
{
    using Handling;

    [NoDescp]
    public class Comparitor : IComparitorProfile
    {
        public Func<IBot, IMessage, ICommand, ComparitorResult> Matcher { get; private set; }

        public Type AttributeType { get; private set; }
        
        public ComparitorResult IsMatch(IBot bot, IMessage message, ICommand command)
        {
            return Matcher(bot, message, command);
        }

        public static Comparitor Get(Type type, Func<IBot, IMessage, ICommand, ComparitorResult> matcher)
        {
            return new Comparitor
            {
                AttributeType = type,
                Matcher = matcher
            };
        }
    }
}
