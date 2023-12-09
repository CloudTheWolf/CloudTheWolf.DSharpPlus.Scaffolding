using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.SystemCommands
{
    internal class PluginManagerCommands : BaseCommandModule
    {
        [Command("plugin.remove")]
        [RequireOwner]
        [Hidden]
        public async Task RemovePlugin(CommandContext ctx, [RemainingText]string pluginName)
        {
            await ctx.TriggerTypingAsync();
            var plugins = Bot.PluginLoader.Plugins;

            foreach (var plugin in plugins)
            {
                if (plugin.Value.Name != pluginName) return;

                if (plugin.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                var loadContext = PluginLoader.PluginLoadContexts[pluginName];
                PluginLoader.PluginLoadContexts.Remove(pluginName);

                loadContext.Unload();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                await ctx.RespondAsync($"Unloaded [{pluginName}]");
                return;
            }
            await ctx.RespondAsync($"Could not find [{pluginName}] to unload");
        }
    }
}
