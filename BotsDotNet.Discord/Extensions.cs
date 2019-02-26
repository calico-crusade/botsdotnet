using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BotsDotNet.Discord
{
    public static class Extensions
    {
        public static MemoryStream ToStream(this Bitmap image)
        {
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return ms;
        }
    }
}
