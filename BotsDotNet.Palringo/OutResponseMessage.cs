namespace BotsDotNet.Palringo
{
    using PacketTypes;

    public class OutResponseMessage : BdnModel, IMessageResponse
    {
        public bool Success { get; private set; }

        public OutResponseMessage(Response resp) : base(resp)
        {
            Success = resp.Code == Types.Code.OK;
        }

        public static implicit operator OutResponseMessage(Response resp)
        {
            return new OutResponseMessage(resp);
        }
    }
}
