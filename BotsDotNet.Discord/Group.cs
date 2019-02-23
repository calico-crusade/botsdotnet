using Discord.WebSocket;

namespace BotsDotNet.Discord
{
    public class Group : IGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ISocketMessageChannel Original { get; set; }

        public Group(ISocketMessageChannel channel)
        {
            Original = channel;
            Id = channel?.Id.ToString();
            Name = channel?.Name;
        }
    }
}
