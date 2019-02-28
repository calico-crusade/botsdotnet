namespace BotsDotNet.PalringoV3.Network
{
    public class Packet
    {
        public string Command { get; set; }

        public object Body { get; set; }

        public Packet() { }

        public Packet(string cmd, object body)
        {
            Command = cmd;
            Body = body;
        }
    }
}
