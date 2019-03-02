using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;

namespace BotsDotNet.BaaS
{
    public class Service : IMicroService
    {
        private BotManager manager;
        private Action<IBot> onloggedin = null;

        public Service(BotManager manager, Action<IBot> onloggedin = null)
        {
            this.manager = manager;
            this.onloggedin = onloggedin;
        }

        public void Start()
        {
            manager.Start(onloggedin);
        }

        public void Stop()
        {
            manager.Stop().Wait();
        }
    }
}
