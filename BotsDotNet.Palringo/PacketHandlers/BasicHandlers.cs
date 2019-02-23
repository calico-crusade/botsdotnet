namespace BotsDotNet.Palringo.PacketHandlers
{
    using Networking;
    using Networking.Handling;
    using PacketTypes;
    using SubProfile;
    using SubProfile.Parsing;
    using Handling;
    using Types;
    using Util;

    public class BasicHandlers : IPacketHandler
    {
        private readonly IPacketTemplates templates;
        private readonly IPluginManager pluginManager;
        private readonly BroadcastUtility broadcast;

        public BasicHandlers(IPacketTemplates templates, IPluginManager pluginManager, IBroadcastUtility broadcast)
        {
            this.templates = templates;
            this.pluginManager = pluginManager;
            this.broadcast = (BroadcastUtility)broadcast;
        }

        public async void PingHandler(PingRequest ping, PalBot bot)
        {
            var sent = await bot.Write(templates.Ping());
        }

        public void SubProfileHandler(SubProfilePacket sub, PalBot bot)
        {
            bot.SubProfiling.Process(sub.Data, sub.IV, sub.RK);
        }

        public void BalanceQueryResultHandle(BalanceQueryResult res, PalBot bot)
        {
            bot.SubProfiling.Process(res.Payload, null, null);
        }
        
        public void SubProfileQueryResponse(SubProfileQueryResult result, PalBot bot)
        {
            var map = result.Data;
            if (!map.ContainsKey("sub-id"))
                return;

            var id = map.GetValueInt("sub-id").ToString();

            if (!((PalBot)bot).SubProfiling.Users.ContainsKey(id))
                ((PalBot)bot).SubProfiling.Users.Add(id, new User());

            foreach(var m in map.EnumerateMaps())
            {
                ((PalBot)bot).SubProfiling.Users[id].Process(m);
            }
        }

        public void GroupUpdateHandler(GroupUpdate update, PalBot bot)
        {
            var map = update.Payload;
            broadcast.BroadcastGroupUpdate(bot, update);

            if (!((PalBot)bot).SubProfiling.Users.ContainsKey(update.UserId))
                ((PalBot)bot).SubProfiling.Users.Add(update.UserId, new User(update.UserId));

            if (!((PalBot)bot).SubProfiling.Groups.ContainsKey(update.GroupId))
                return;

            if (update.Type == GroupUpdateType.Leave &&
                ((PalBot)bot).SubProfiling.Groups[update.GroupId].Members.ContainsKey(update.UserId))
            {
                ((PalBot)bot).SubProfiling.Groups[update.GroupId].Members.Remove(update.UserId);
            }

            if (update.Type == GroupUpdateType.Join)
            {
                var nextMap = new DataMap(map);

                if (nextMap.ContainsKey("contacts"))
                    nextMap = nextMap.GetValueMap("contacts");
                if (nextMap.ContainsKey(update.UserId.ToString()))
                    nextMap = nextMap.GetValueMap(update.UserId.ToString());

                ((PalBot)bot).SubProfiling.Users[update.UserId].Process(nextMap);

                if (!((PalBot)bot).SubProfiling.Groups[update.GroupId].Members.ContainsKey(update.UserId))
                    ((PalBot)bot).SubProfiling.Groups[update.GroupId].Members.Add(update.UserId, Role.User);
                return;
            }
        }

        public void AdminActionHandler(AdminAction action, PalBot oBot)
        {
            var bot = (PalBot)oBot;

            if (!bot.SubProfiling.Groups.ContainsKey(action.GroupId))
                return;

            var group = bot.SubProfiling.Groups[action.GroupId];

            if (!group.Members.ContainsKey(action.TargetId))
                return;

            group.Members[action.TargetId] = action.Action.ToRole();
            broadcast.BroadcastAdminAction(bot, action);
        }

        public void ThrottleHandler(Throttle throttle, PalBot bot)
        {
            broadcast.BroadcastThrottle(bot, throttle);
        }
    }
}
