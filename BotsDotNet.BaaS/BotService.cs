using System;
using System.ServiceProcess;

namespace BotsDotNet.BaaS
{
    public class BotService : ServiceBase
    {
        public Action<IBot> OnLogin { get; private set; }

        public BotService(Action<IBot> onlogin = null)
        {
            OnLogin = onlogin;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            base.OnStop();
        }
    }
}
