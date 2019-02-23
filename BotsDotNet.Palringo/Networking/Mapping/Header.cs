using System;

namespace BotsDotNet.Palringo.Networking.Mapping
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Header : Attribute
    {
        public string Name { get; private set; }

        public Header(string name)
        {
            this.Name = name;
        }
    }
}
