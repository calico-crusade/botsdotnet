using System;

namespace BotsDotNet.Handling
{
    public interface IComparitorProfile
    {
        Type AttributeType { get; }
        ComparitorResult IsMatch(IBot bot, IMessage message, ICommand command);
    }

    public class CommandComparitorProfile : IComparitorProfile
    {
        public Type AttributeType { get; } = typeof(Command);

        public ComparitorResult IsMatch(IBot bot, IMessage message, ICommand command)
        {
            var msg = message.Content.ToLower().Trim();
            var cmd = command.Comparitor.ToLower().Trim();

            if (!msg.StartsWith(cmd))
                return new ComparitorResult
                {
                    IsMatch = false,
                    CappedCommand = msg
                };

            msg = message.Content.Trim().Remove(0, cmd.Length);
            return new ComparitorResult
            {
                IsMatch = true,
                CappedCommand = msg
            };
        }
    }
}
