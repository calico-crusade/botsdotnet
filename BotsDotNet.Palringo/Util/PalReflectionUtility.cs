using StructureMap;
using System;

namespace BotsDotNet.Palringo.Util
{
    using SubProfile.Parsing;
    using Utilities;

    public class PalReflectionUtility : ReflectionUtility
    {
        public PalReflectionUtility(Container container) : base(container) { }

        public override object ChangeType(object obj, Type toType)
        {
            if (obj == null)
                return null;

            var fromType = obj.GetType();

            var to = Nullable.GetUnderlyingType(toType) ?? toType;
            var from = Nullable.GetUnderlyingType(fromType) ?? fromType;

            if (from == typeof(byte[]) && to == typeof(DataMap))
            {
                return new DataMap((byte[])obj);
            }

            return base.ChangeType(obj, toType);
        }
    }
}
