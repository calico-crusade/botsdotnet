using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotsDotNet.PalringoV3
{
    using Network;
    using Models;

    public interface ICacheUtility
    {
        Task<IUser> GetUser(string userid, PalBot bot);
        Task<IGroup> GetGroup(string groupid, PalBot bot);
    }

    public class CacheUtility : ICacheUtility
    {
        private Dictionary<string, IUser> _users { get; set; } = new Dictionary<string, IUser>();
        private Dictionary<string, IGroup> _groups { get; set; } = new Dictionary<string, IGroup>();

        private IPacketTemplate packetTemplate;

        public CacheUtility(IPacketTemplate packetTemplate)
        {
            this.packetTemplate = packetTemplate;
        }

        public async Task<IUser> GetUser(string userid, PalBot bot)
        {
            if (_users.ContainsKey(userid))
                return _users[userid];

            var user = (OutUser)await bot.WritePacket<User>(packetTemplate.UserProfile(userid));
            _users.Add(userid, user);
            return user;
        }

        public async Task<IGroup> GetGroup(string groupid, PalBot bot)
        {
            if (_groups.ContainsKey(groupid))
                return _groups[groupid];

            var group = (OutGroup)await bot.WritePacket<Group>(packetTemplate.GroupProfile(groupid));
            _groups.Add(groupid, group);
            return group;
        }
    }
}
