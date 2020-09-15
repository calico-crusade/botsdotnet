using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace BotsDotNet.PalringoV3
{
    public static class Extensions
    {
        public static Random Rnd = new Random();
        static readonly DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0);

        static readonly DateTimeOffset epochDateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static ImmutableList<T> ToImmutable<T>(this IEnumerable<T> data)
        {
            return ImmutableList.Create(data.ToArray());
        }

        public static long GetTime(this DateTime timestamp)
        {
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (timestamp.ToUniversalTime() - st);
            return (long)(t.TotalMilliseconds + 0.5);
        }

        public static string GenerateDeviceToken(this IBot bot)
        {
            var d = DateTime.Now.GetTime();
            var uuid = "WExxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx".ToCharArray();
            var chars = "abcedf1234567890";

            for (var i = 2; i < uuid.Length; i++)
                uuid[i] = chars[Rnd.Next(0, chars.Length)];

            return string.Join("", uuid);
        }

        /// <summary>
        /// From pal unix to ours
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime FromPalUnix(this string ts)
        {
            return FromPalUnix(int.Parse(ts.Split('.')[0]), int.Parse(ts.Split('.')[1]));
        }

        /// <summary>
        /// from regular unix
        /// </summary>
        /// <param name="secondsSinceepoch"></param>
        /// <returns></returns>
        public static DateTime FromUnix(this long secondsSinceepoch)
        {
            return epochStart.AddSeconds(secondsSinceepoch).ToLocalTime();
        }

        /// <summary>
        /// From pal unix in intergers
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="nanoseconds"></param>
        /// <returns></returns>
        public static DateTime FromPalUnix(int seconds, int nanoseconds)
        {
            var dt = epochStart;
            dt = epochStart.AddSeconds(seconds);
            dt = dt.AddMilliseconds(nanoseconds / 1000);
            return dt.ToLocalTime();
        }

        /// <summary>
        /// Converts our timestamps to pal timestamps
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToPalUnix(this DateTime time)
        {
            var current = time.ToUniversalTime().ToUnix();
            return current + "." + ((int)(time.Ticks % TimeSpan.TicksPerMillisecond % 10) * 100);
        }

        /// <summary>
        /// Converts an offset from unix to windows
        /// </summary>
        /// <param name="secondsSinceEpoch"></param>
        /// <param name="timeZoneOffsetInMinutes"></param>
        /// <returns></returns>
        public static DateTimeOffset FromUnix(this int secondsSinceEpoch, int timeZoneOffsetInMinutes)
        {
            var utcDateTime = epochDateTimeOffset.AddSeconds(secondsSinceEpoch);
            var offset = TimeSpan.FromMinutes(timeZoneOffsetInMinutes);
            return new DateTimeOffset(utcDateTime.DateTime.Add(offset), offset);
        }

        /// <summary>
        /// Converts our timestamp to unix
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int ToUnix(this DateTime dateTime)
        {
            return (int)(dateTime - epochStart).TotalSeconds;
        }

        /// <summary>
        /// Gets now in unix epoch
        /// </summary>
        public static int Now
        {
            get
            {
                return (int)(DateTime.UtcNow - epochStart).TotalSeconds;
            }
        }

        public static Bitmap ToBitmap(this byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return new Bitmap(ms);
            }
        }

        public static byte[] ToByteArray(this Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            using (var b = new Bitmap(bitmap))
            {
                b.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static byte[] ToBuffer(this Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            using (var b = new Bitmap(bitmap))
            {
                b.Save(ms, ImageFormat.Jpeg);
                return ms.GetBuffer();
            }
        }
    }
}
