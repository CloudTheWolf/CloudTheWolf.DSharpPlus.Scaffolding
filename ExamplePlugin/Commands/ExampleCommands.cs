
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands.Choices;
using System.ComponentModel;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;


namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands
{

    class ExampleCommands
    {
        [
            Command("example"),
            Description("An Example Command"),
            RequirePermissions(botPermissions: DiscordPermissions.None, userPermissions: DiscordPermissions.UseApplicationCommands),            
        ]
        
        public async Task GetConfigSettings(
            CommandContext ctx,
            [Parameter("dayofweek")]
            [Description("Day Of Week")]
            [SlashChoiceProvider<DaysOfWeekProvider>]
            int day)
        {
            _ = ctx.DeferResponseAsync();
            
            await ctx.EditResponseAsync($"You Selected {day}");

        }
    }
}
