using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Client.Models;

namespace BotsDotNet.Twitch
{
    public class Group : BdnModel, IGroup
    {
        public string Id { get; private set; }

        public string Name { get; private set; }
        
        public Group(ChatMessage message) : base(message)
        {
            Id = message.RoomId;
            Name = message.Channel;
        }

        public Group(Channel channel) : base(channel)
        {
            Id = channel.Id;
            Name = channel.Name;
        }
    }
}
