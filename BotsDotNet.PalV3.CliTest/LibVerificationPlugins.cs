using System.Threading.Tasks;

namespace BotsDotNet.PalV3.CliTest
{
	using Microsoft.VisualBasic;
	using PalringoV3;
	using System.Collections.Generic;
	using System.Linq;

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
            if (!int.TryParse(cmd, out int groupId) && groupId > 0)
			{
                await msg.Reply("Invalid group Id! It must be a number!");
                return;
			}

            await ((PalBot)b).GroupJoin(groupId);
            await msg.Reply("Attempted to join group: " + groupId);
		}

        [Command("authorize", Restriction = "AuthOnly")]
        public async Task AuthorizeUser(IBot bot, IMessage msg, string cmd)
		{
            if (!int.TryParse(cmd, out int userid) && userid > 0)
            {
                await msg.Reply("Invalid user Id! It must be a number!");
                return;
            }

            if (AuthOnlyRestriction.AuthUsers.Contains(userid.ToString()))
                AuthOnlyRestriction.AuthUsers.Remove(userid.ToString());
            else
                AuthOnlyRestriction.AuthUsers.Add(userid.ToString());

            await msg.Reply("User was " + (AuthOnlyRestriction.AuthUsers.Contains(userid.ToString()) ? "authorized" : "deauthorized"));
        }
    }

	public class AuthOnlyRestriction : IRestriction
	{
        public static List<string> AuthUsers = new List<string>
        {
            "43681734"
        };

        public string Name => "AuthOnly";

        public string Platform => BotPlatform.PalringoV3;

		public Task OnRejected(IBot bot, IMessage message)
		{
            return message.Reply("You cannot use this feature.");
		}

		public Task<bool> Validate(IBot bot, IMessage message)
		{
            return Task.FromResult(AuthUsers.Contains(message.UserId));
		}
	}
}
