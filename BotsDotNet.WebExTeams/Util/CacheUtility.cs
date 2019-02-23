using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotsDotNet.WebExTeams.Util
{
    using SparkDotNet;

    public interface ICacheUtility
    {
        Task<IUser> GetUser(SparkBot bot, string id);
        Task<Room> GetRoom(SparkBot bot, string id);
        Task<Message> MessageFromId(SparkBot bot, string id);
    }

    public class CacheUtility : ICacheUtility
    {
        public static Dictionary<string, IUser> Users { get; set; } = new Dictionary<string, IUser>();
        public static Dictionary<string, Room> Rooms { get; set; } = new Dictionary<string, Room>();

        public CacheUtility() { }

        public async Task<IUser> GetUser(SparkBot bot, string id)
        {
            if (Users.ContainsKey(id))
                return Users[id];

            var prsn = await bot.Connection.GetPersonAsync(id);

            if (prsn == null)
                return null;
            
            Users.Add(prsn.Id, prsn);
            return prsn;
        }

        public async Task<Room> GetRoom(SparkBot bot, string id)
        {
            if (Rooms.ContainsKey(id))
                return Rooms[id];

            var room = await bot.Connection.GetRoomAsync(id);

            if (room == null)
                return null;

            Rooms.Add(room.Id, room);
            return room;
        }

        public async Task<Message> MessageFromId(SparkBot bot, string id)
        {
            var msg = await bot.Connection.GetMessageAsync(id);

            if (msg == null)
                return null;

            var type = msg.RoomType == "direct" ? MessageType.Private : MessageType.Group;
            var user = await GetUser(bot, msg.PersonId);

            var room = type == MessageType.Group ? await GetRoom(bot, msg.RoomId) : null;

            var mesg = new Message(msg, user, room);


            if (msg.MentionedPeople != null && msg.MentionedPeople.Length > 0)
            {
                var users = new IUser[msg.MentionedPeople.Length];

                for(var i = 0; i < msg.MentionedPeople.Length; i++)
                {
                    users[i] = await GetUser(bot, msg.MentionedPeople[i]);
                }

                mesg.Mentions = users;
            }

            return mesg;
        }
    }
}
