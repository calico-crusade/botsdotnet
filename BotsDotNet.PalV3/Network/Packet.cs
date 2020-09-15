using System.Collections.Generic;

namespace BotsDotNet.PalringoV3.Network
{
    public class Packet<T>
    {
        public string Command { get; set; }

        public T Body { get; set; }

        public Dictionary<string, object> Headers { get; set; }

        public Packet() { }

        public Packet(string cmd, T body)
        {
            Command = cmd;
            Body = body;
        }

        public override string ToString()
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(Body, Newtonsoft.Json.Formatting.Indented);
            return $"CMD: {Command}\r\n{data}";
        }
    }

    public class Packet : Packet<object>
    {
        public Packet() { }

        public Packet(string cmd, object body) : base(cmd, body) { }
    }
}
