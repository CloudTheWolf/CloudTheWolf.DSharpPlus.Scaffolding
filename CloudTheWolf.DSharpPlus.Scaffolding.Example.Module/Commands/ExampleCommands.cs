using CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Classes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands
{
    class ExampleCommands : BaseCommandModule
    {

        private Database _dba = new Database();

        [Command("configs")]
        public async Task GetConfigSettings(CommandContext ctx)
        {
            var exampleArray = _dba.GetConfig();
            foreach(JObject jObject in exampleArray )
            {
                await ctx.Channel.SendMessageAsync($"Setting {jObject["name"]} [bValue = {jObject["bValue"]}, iValue = {jObject["iValue"]}]");            
            };
        }

    }
}
