using System;

namespace BotsDotNet.Utilities
{
    public static class Extensions
    {
        /// <summary>
        /// Nullable safe method of converting object between types.
        /// </summary>
        /// <param name="value">The object to convert</param>
        /// <param name="type">The type to convert to</param>
        /// <returns>The converted object</returns>
        public static object ChangeType(this object value, Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            if (t.IsEnum)
            {
                try
                {
                    return Enum.ToObject(t, Convert.ChangeType(value, typeof(int)));
                }
                catch
                {
                    return Enum.Parse(t, (string)Convert.ChangeType(value, typeof(string)), true);
                }
            }
            return Convert.ChangeType(value, t);
        }
    }
}
