namespace BotsDotNet
{
    public interface IBdnModel
    {
        Attribution Original { get; }
    }

    public abstract class BdnModel : IBdnModel
    {
        public Attribution Original { get; private set; }

        public BdnModel(object attribution)
        {
            Original = new Attribution(attribution);
        }
    }
}
