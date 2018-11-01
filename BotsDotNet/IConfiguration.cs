using System;
using System.Collections.Generic;
using System.Text;

namespace BotsDotNet
{
    public interface IConfiguration
    {
        string WebhookBase { get; }
        string Secret { get; }
    }
}
