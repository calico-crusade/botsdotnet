using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotsDotNet.PalringoV3
{
    using Network;
    using Models;
	using System.Linq;
	using System;
	using System.Net.Http.Headers;

	public interface ICacheUtility
    {
        Task<IUser> GetUser(string userid, PalBot bot);
        Task<IGroup> GetGroup(string groupid, PalBot bot);
        Task<ExtendedGroupUser> GetGroupUser(string groupid, string userid, PalBot bot);
        void UpdateGroupMember(GroupUserUpdate user, bool kicked = false);
        Task<IUser[]> GetGroupUsers(string groupid, PalBot bot);
    }

    public class CacheUtility : ICacheUtility
    {
        private readonly Dictionary<string, IUser> _users = new Dictionary<string, IUser>();
        private readonly Dictionary<string, IGroup> _groups = new Dictionary<string, IGroup>();
        private readonly Dictionary<string, Dictionary<string, GroupUser>> _groupUsers = new Dictionary<string, Dictionary<string, GroupUser>>();

        private readonly IPacketTemplate _packetTemplate;

        public CacheUtility(IPacketTemplate packetTemplate)
        {
            this._packetTemplate = packetTemplate;
        }

        public async Task<IUser[]> GetUsers(int[] ids, PalBot bot)
		{
            var users = new List<IUser>();
            var idList = new List<int>();

            foreach(var id in ids)
			{
                if (_users.ContainsKey(id.ToString()))
                {
                    users.Add(_users[id.ToString()]);
                    continue;
                }

                idList.Add(id);
			}

            var usersPacket = await bot.WritePacket<Dictionary<string, UserReturn>>(_packetTemplate.UsersProfile(false, true, idList.ToArray()));

            foreach(var user in usersPacket)
			{
                if (user.Value.Code >= 200 && user.Value.Code < 300)
                    users.Add((OutUser)user.Value.Body);
			}

            return users.ToArray();
		}

        public async Task<IUser> GetUser(string userid, PalBot bot)
        {
            if (_users.ContainsKey(userid))
                return _users[userid];

            var user = (OutUser)await bot.WritePacket<User>(_packetTemplate.UserProfile(userid));
            _users.Add(userid, user);
            return user;
        }

        public async Task<IGroup> GetGroup(string groupid, PalBot bot)
        {
            if (_groups.ContainsKey(groupid))
                return _groups[groupid];

            var group = (OutGroup)await bot.WritePacket<Group>(_packetTemplate.GroupProfile(groupid));
            _groups.Add(groupid, group);
            return group;
        }

        public async Task<IUser[]> GetGroupUsers(string groupid, PalBot bot)
		{
            var users = await GroupMemberList(bot, groupid);
            var ids = users.Keys.Select(t => int.Parse(t)).ToArray();
            return await GetUsers(ids, bot);
		}

        public void UpdateGroupMember(GroupUserUpdate user, bool kicked = false)
		{
            user.Id = user.SubscriberId;
            var userid = user.SubscriberId.ToString();
            var groupid = user.GroupId.ToString();

            if (!_groupUsers.ContainsKey(groupid))
                _groupUsers.Add(groupid, new Dictionary<string, GroupUser>());

            if (kicked)
            {
                if (_groupUsers[groupid].ContainsKey(userid))
				{
                    _groupUsers[groupid].Remove(userid);
                    return;
				}

                return;
			}

            if (!_groupUsers[groupid].ContainsKey(userid))
            {

                _groupUsers[groupid].Add(userid, user);
                return;
			}

            var trueUser = _groupUsers[groupid][userid];
            trueUser.Capabilities = user.Capabilities;

            _groupUsers[groupid][userid] = trueUser;
		}

        public async Task<Dictionary<string, GroupUser>> GroupMemberList(PalBot bot, string groupid, bool subscribe = true)
		{
            if (_groupUsers.ContainsKey(groupid))
                return _groupUsers[groupid];

            var group = await bot.WritePacket<GroupUser[]>(_packetTemplate.GroupMemberList(groupid, subscribe));

            if (!_groupUsers.ContainsKey(groupid))
                _groupUsers.Add(groupid, new Dictionary<string, GroupUser>());

            foreach (var member in group)
            {
                if (_groupUsers[groupid].ContainsKey(member.Id.ToString()))
                    _groupUsers[groupid][member.Id.ToString()] = member;
                else
                    _groupUsers[groupid].Add(member.Id.ToString(), member);
            }

            return _groupUsers[groupid];
        }

        public async Task<ExtendedGroupUser> GetGroupUser(string groupid, string userid, PalBot bot)
		{
            if (!int.TryParse(userid, out int id))
                return null;

            var users = await GroupMemberList(bot, groupid, true);

            var user = (User)(await GetUser(userid, bot)).Original.Original;

            if (!users.ContainsKey(userid) && user != null)
                return new ExtendedGroupUser
                {
                    Capabilities = GroupUserType.User,
                    Id = id,
                    Profile = user
                };

            if (users.ContainsKey(userid) && user != null)
                return new ExtendedGroupUser
                {
                    Capabilities = users[userid].Capabilities,
                    Id = id,
                    Profile = user
                };

            if (users.ContainsKey(userid))
                return new ExtendedGroupUser
                {
                    Profile = null,
                    Id = id,
                    Capabilities = users[userid].Capabilities
                };

            return null;
        }
    }
}
