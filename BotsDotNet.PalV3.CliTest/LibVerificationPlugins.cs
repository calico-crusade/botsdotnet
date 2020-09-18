using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotsDotNet.PalV3.CliTest
{
	using PalringoV3;
    using PalringoV3.Network;
    using PalringoV3.Models;

	public class LibVerificationPlugins : IPlugin
    {
        [Command("dosomethingplz", Restriction = "AuthOnly")]
        public async Task Validate(IBot b, IMessage msg, string cmd)
        {
            var bot = (PalBot)b;

            if (msg.MessageType == MessageType.Group)
            {
                var users = await bot.GetGroupUsers(msg.GroupId);
                System.Console.WriteLine(string.Join(", ", users.Select(t => t.Nickname)));
                System.Console.WriteLine("Written users...");
            }

            await msg.Reply("Working!");
        }

        [Command("join", Restriction = "AuthOnly")]
        public async Task JoinGroup(IBot b, IMessage msg, string cmd)
		{
            if (!int.TryParse(cmd, out int groupId) || groupId <= 0)
			{
                await msg.Reply("Invalid group Id! It must be a number!");
                return;
			}

            await ((PalBot)b).GroupJoin(groupId);
            await msg.Reply("Attempted to join group: " + groupId);
		}

        [Command("questions")]
        public async Task Questions(IMessage msg, IBot bot)
		{
            await msg.Reply("What is your favourite colour?");

            var color = (await msg.NextMessage()).Content;

            await msg.Reply("What is your favourite food?");

            var food = (await msg.NextMessage()).Content;

            await msg.Reply($"Your taste in food sucks, why do you like {food}? However, I too like {color}, want me to paint your ass this color? Who else likes {color} {food}s? (Say \"me\")");

            var douche = await bot.NextMessage(m => m.GroupId == msg.GroupId && m.Content.ToLower().Trim() == "me");

            await msg.Reply($"You sir, {douche.User.Nickname}, are an idiot for liking {color} {food}s.");
		}

        [Command("leave", Restriction = "AuthOnly")]
        public async Task LeaveGroup(IBot b, IMessage msg, string cmd)
		{
            if (!int.TryParse(cmd, out int groupId) || groupId <= 0)
                groupId = int.Parse(msg.GroupId);

            var bot = (PalBot)b;

            var res = await bot.WritePacket<Resp>(new Packet("group member delete", new
            {
                groupId = groupId
            }));
            await b.PrivateMessage(msg.UserId, "Left group result: " + res.Code);
        }

        [Command("authorize", Restriction = "AuthOnly")]
        public async Task AuthorizeUser(IBot bot, IMessage msg, string cmd)
		{
            if (!int.TryParse(cmd, out int userid) && userid > 0)
            {
                await msg.Reply("Invalid user Id! It must be a number!");
                return;
            }

            var authed = AuthOnlyRestriction.Toggle(msg.UserId);
            if (AuthOnlyRestriction.AuthUsers.Contains(userid.ToString()))
                AuthOnlyRestriction.AuthUsers.Remove(userid.ToString());
            else
                AuthOnlyRestriction.AuthUsers.Add(userid.ToString());

            await msg.Reply("User was " + (authed ? "authorized" : "deauthorized"));
        }

        [Command("gu")]
        public async Task GetGroupUser(IBot bot, IMessage msg, string cmd)
		{
            var b = (PalBot)bot;

            var gu = await b.GetGroupUser(msg.GroupId, cmd);
            if (gu == null)
			{
                await msg.Reply($"The user {cmd} is not in this group!");
                return;
			}

            await msg.Reply($"User has {gu.Capabilities?.ToString()} ({(int)(gu.Capabilities ?? 0)})  in this group!");
		}
    }

    public class AuthOnlyRestriction : IRestriction
    {
        private const string USERS_FILE = "authusers.json";
        private static List<string> _authUsers;

        public static List<string> AuthUsers => _authUsers ??= GetUsersFromFile();

        public string Name => "AuthOnly";

        public string Platform => BotPlatform.PalringoV3;

        public Task OnRejected(IBot bot, IMessage message)
        {
            return Task.CompletedTask;
        }

        public Task<bool> Validate(IBot bot, IMessage message)
        {
            return Task.FromResult(AuthUsers.Contains(message.UserId));
        }

        public static bool IsAuth(int userid)
		{
            return IsAuth(userid.ToString());
		}

        public static bool IsAuth(string userid)
		{
            return AuthUsers.Contains(userid);
		}

        public static bool Toggle(string userid)
        {
            if (AuthUsers.Contains(userid))
            {
                AuthUsers.Remove(userid);
                SaveFile();
                return false;
            }

            AuthUsers.Add(userid);
            SaveFile();
            return true;
        }

        public static bool Toggle(int userid)
		{
            return Toggle(userid.ToString());
		}

        public static List<string> GetUsersFromFile()
		{
            if (!File.Exists(USERS_FILE))
                return new List<string>
                {
                    "43681734"
                };

            var data = File.ReadAllText(USERS_FILE);
            return JsonConvert.DeserializeObject<List<string>>(data);
		}

        public static void SaveFile()
		{
            var data = JsonConvert.SerializeObject(AuthUsers, Formatting.Indented);
            File.WriteAllText(USERS_FILE, data);
		}
	}
}
