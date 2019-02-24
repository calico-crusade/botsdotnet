namespace BotsDotNet
{
    public static class Extensions
    {
        /// <summary>
        /// This is required because if C# detects that none of the code in an assembly has a hard reference 
        /// it will remove the assembly from the outputs. So use this somewhere in the application. 
        /// This method doesn't actually do anything.
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public static IBot RegisterTestPlugin(this IBot test)
        {
            return test;
        }
    }
}
