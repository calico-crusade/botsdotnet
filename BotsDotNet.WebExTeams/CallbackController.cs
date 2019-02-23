using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BotsDotNet.WebExTeams
{
    using Handling;

    [Produces("application/json")]
    [Route("/spark")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private readonly IPluginManager pluginManager;
        private readonly IBot bot;

        public CallbackController(IPluginManager pluginManager, IBot bot)
        {
            this.pluginManager = pluginManager;
            this.bot = bot;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Callback result)
        {
            try
            {
                Extensions.Log(result);

                if (!(this.bot is SparkBot))
                    return NotFound();

                var bot = this.bot as SparkBot;

                await bot.HandleCallback(result);
                
                return Ok();
            }
            catch (Exception ex)
            {
                Extensions.Log(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                if (!(this.bot is SparkBot))
                    return NotFound();

                var bot = this.bot as SparkBot;

                var rooms = await bot.Connection.GetRoomsAsync();

                return Ok(new
                {
                    Rooms = rooms,
                    BotProfile = bot.Profile,
                    BotId = bot.MyId,
                    bot.HookId,
                    Plugins = pluginManager.Plugins().Select(t => t.Command).ToArray()
                });
            }
            catch (Exception ex)
            {
                //Bot.Log(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("/spark/clear")]
        public async Task<ActionResult> Clear()
        {
            try
            {
                var bot = this.bot as SparkBot;

                var hooks = await bot.Connection.GetWebhooksAsync();

                foreach(var hook in hooks)
                {
                    await bot.Connection.DeleteWebhookAsync(hook.Id);
                }

                await bot.HandleHooks();

                return Ok();
            }
            catch (Exception ex)
            {
                Extensions.Log(ex);
                return BadRequest(ex);
            }
        }
    }
}
