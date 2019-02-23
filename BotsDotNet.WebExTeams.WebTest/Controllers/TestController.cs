using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace BotsDotNet.WebExTeams.WebTest.Controllers
{
    using Handling;

    [Route("/test"), ApiController]
    public class TestController : ControllerBase
    {
        private readonly IPluginManager pluginManager;
        private readonly IBot bot;

        public TestController(IPluginManager pluginManager, IBot bot)
        {
            this.pluginManager = pluginManager;
            this.bot = bot;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(new
                {
                    Plugins = pluginManager.Plugins().Select(t => t.Command).ToArray(),
                    bot.Profile
                });
            }
            catch (Exception ex)
            {
                Extensions.Log(ex);
                return BadRequest(ex);
            }
        }
    }
}
