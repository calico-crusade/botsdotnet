using System;
using System.Collections.Generic;
using System.Text;

namespace BotsDotNet.WebExTeams.Web
{
    public class Url
    {
        private string UrlPart;

        public Url(string url)
        {
            UrlPart = url;
        }

        public Url Add(string part)
        {
            if (!UrlPart.EndsWith("\\"))
                UrlPart += "\\";

            UrlPart += part;
            return this;
        }

        public override string ToString()
        {
            return UrlPart;
        }
    }
}
