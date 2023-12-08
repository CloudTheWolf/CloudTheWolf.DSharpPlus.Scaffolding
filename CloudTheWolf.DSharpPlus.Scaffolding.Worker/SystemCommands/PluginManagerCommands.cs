using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.SystemCommands
{
    internal class PluginManagerCommands : BaseCommandModule
    {
        [Command("plugin.remove")]
        [RequireOwner]
        public async Task RemovePlugin(CommandContext ctx, [RemainingText]string pluginName)
        {
            await ctx.TriggerTypingAsync();

        }
    }
}
