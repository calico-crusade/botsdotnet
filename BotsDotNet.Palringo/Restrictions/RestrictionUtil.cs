namespace BotsDotNet.Palringo.Restrictions
{
    using SubProfile;
    using Types;

    public static class RestrictionUtil
    {
        public static bool TryGetRole(IMessage message, out Role role, bool checkOwner = true, string userid = null)
        {
            role = Role.NotMember;
            userid = userid ?? message.UserId;

            if (message.MessageType != MessageType.Group)
                return false;

            if (!(message.Group is Group))
                return false;

            var grp = message.Group as Group;

            if (checkOwner && grp.OwnerId == userid)
                return true;

            if (!grp.Members.ContainsKey(userid))
                return false;

            role = grp.Members[userid];
            return true;
        }

        public static bool ValidateAdmin(IMessage message, string userid = null)
        {
            if (!TryGetRole(message, out Role role, userid: userid))
                return false;

            return role == Role.Admin || role == Role.Owner;
        }

        public static bool ValidateOwner(IMessage message, string userid = null)
        {
            if (!TryGetRole(message, out Role role, userid: userid))
                return false;

            return role == Role.Owner;
        }

        public static bool ValidateMod(IMessage message, string userid = null)
        {
            if (!TryGetRole(message, out Role role, userid: userid))
                return false;

            return role == Role.Admin || role == Role.Owner || role == Role.Mod;
        }

        public static bool ValidateUser(IMessage message, string userid = null)
        {
            if (!TryGetRole(message, out Role role, userid: userid))
                return false;

            return role == Role.Admin || role == Role.Owner || role == Role.Mod || role == Role.User;
        }
    }
}
