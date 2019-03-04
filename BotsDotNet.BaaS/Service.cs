using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;

namespace BotsDotNet.BaaS
{
    public class Service<T> : IMicroService
    {
        private BotManager<T> manager;
        private Action<IBot> onloggedin = null;

        public Service(BotManager<T> manager, Action<IBot> onloggedin = null)
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
