namespace BotsDotNet.Handling
{
    public class PluginResult
    {
        public object ReturnObject { get; set; }
        public ReflectedPlugin Plugin { get; set; }
        public PluginResultType Result { get; set; }

        public PluginResult(PluginResultType result, object obj, ReflectedPlugin plug)
        {
            Result = result;
            ReturnObject = obj;
            Plugin = plug;
        }
    }
}
