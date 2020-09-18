using System.Collections.Generic;
using System.Linq;

namespace BotsDotNet.PalringoV3.Network
{
    public interface IPacketTemplate
    {
        Packet Login(string email, string password);
        Packet PrivateMessageSubscribe();
        Packet GroupMessageSubscribe(params string[] ids);
        Packet Message(string userid, bool isGroup, object msg, string mimetype = "text/plain");
        Packet GroupList();
        Packet GroupJoin(int groupId, string password = "");
        Packet GroupLeave(int groupId);
        Packet GroupProfile(string gid, bool extended = true);
        Packet UserProfile(string uid, bool extended = true);
        Packet UsersProfile(bool extended, bool subscribe, int[] idList);
        Packet GroupMemberList(string gid, bool subscribe = true);
        string FlightId();
    }

    public class PacketTemplate : IPacketTemplate
    {
        public Packet Login(string email, string password)
        {
            return new Packet("security login", new
            {
                username = email,
                password = password
            });
        }

        public Packet Message(string userid, bool isGroup, object msg, string mimetype = "text/plain")
        {
            return new Packet("message send", new
            {
                recipient = int.Parse(userid),
                data = msg,
                mimeType = mimetype,
                isGroup,
                flightId = FlightId()
            });
        }

        public string FlightId()
        {
            var chars = "abcdefg1234567980";
            var outp = "0000000".ToCharArray();
            for (var i = 0; i < outp.Length; i++)
                outp[i] = chars[Extensions.Rnd.Next(0, chars.Length)];

            return new string(outp);
        }

        public Packet GroupJoin(int groupId, string password = "")
		{
            return new Packet("group member add", new
            {
                groupId,
                password
            });
		}

        public Packet GroupLeave(int groupId)
		{
            return new Packet("group member delete", new
            {
                groupId
            });
		}

        public Packet PrivateMessageSubscribe()
        {
            return new Packet("message private subscribe", new { })
            {
                Headers = new Dictionary<string, object>()
                {
                    ["version"] = 2
                }
            };
        }

        public Packet GroupMessageSubscribe(params string[] ids)
        {
            if (ids == null || ids.Length <= 0)
                return new Packet("message group subscribe", new { })
                {
                    Headers = new Dictionary<string, object>()
                    {
                        ["version"] = 4
                    }
                };

            if (ids.Length == 1)
                return new Packet("message group subscribe", new
                {
                    id = int.Parse(ids[0])
                })
                {
                    Headers = new Dictionary<string, object>()
                    {
                        ["version"] = 4
                    }
                };

            return new Packet("message group subscribe", new
            {
                idList = ids.Select(t => int.Parse(t)).ToArray()
            })
            {
                Headers= new Dictionary<string, object>()
				{
                    ["version"] = 4
				}
            };
        }

        public Packet GroupList()
        {
            return new Packet("group list", new { });
        }

        public Packet GroupProfile(string gid, bool extended = true)
        {
            return new Packet("group profile", new
            {
                id = int.Parse(gid),
                extended
            });
        }

        public Packet UserProfile(string uid, bool extended = true)
        {
            return new Packet("subscriber profile", new
            {
                id = int.Parse(uid),
                extended
            });
        }

        public Packet UsersProfile(bool extended, bool subscribe, int[] idList)
		{
            return new Packet("subscriber profile", new
            {
                extended,
                idList,
                subscribe
            })
            {
                Headers = new Dictionary<string, object>
				{
                    ["version"] = 4
				}
            };
		}

        public Packet GroupMemberList(string gid, bool subscribe = true)
        {
            return new Packet("group member list", new
            {
                id = int.Parse(gid),
                subscribe
            })
            {
                Headers = new Dictionary<string, object>()
                {
                    ["version"] = 3
                }
            };
        }
    }
}
