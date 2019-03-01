using Discord.WebSocket;

namespace BotsDotNet.Discord
{
    public class Group : BdnModel, IGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }
        
        public Group(ISocketMessageChannel channel) : base(channel)
        {
            Id = channel?.Id.ToString();
            Name = channel?.Name;
        }
    }
}
