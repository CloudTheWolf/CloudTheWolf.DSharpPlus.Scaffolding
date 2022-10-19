using CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Classes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands
{
    class ExampleCommands : BaseCommandModule
    {
        [Command("example")]
        public async Task GetConfigSettings(CommandContext ctx)
        {
            await ctx.RespondAsync("Example");
        }
    }
}
