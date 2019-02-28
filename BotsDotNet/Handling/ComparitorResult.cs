namespace BotsDotNet.Handling
{
    public class ComparitorResult
    {
        public bool IsMatch { get; set; }
        public string CappedCommand { get; set; }

        public ComparitorResult() { }

        public ComparitorResult(bool match)
        {
            IsMatch = match;
        }
        
        public ComparitorResult(bool match, string capped)
        {
            IsMatch = match;
            CappedCommand = capped;
        }
    }
}
