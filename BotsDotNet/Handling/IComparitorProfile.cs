using System;
using System.Linq;

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

            if (!string.IsNullOrEmpty(bot.PluginSets) && 
                !string.IsNullOrEmpty(command.PluginSet))
            {
                var sets = bot.PluginSets.Split(new[] { PluginManager.RESTRICTION_SPLITTER }, StringSplitOptions.RemoveEmptyEntries);

                if (!sets.Any(t => t.ToLower().Trim() == bot.PluginSets.ToLower().Trim()))
                    return new ComparitorResult
                    {
                        IsMatch = false,
                        CappedCommand = msg
                    };
            }

            if (!msg.StartsWith(cmd))
                return new ComparitorResult
                {
                    IsMatch = false,
                    CappedCommand = msg
                };

            msg = message.Content.Trim().Remove(0, cmd.Length).Trim();
            return new ComparitorResult
            {
                IsMatch = true,
                CappedCommand = msg
            };
        }
    }
}
