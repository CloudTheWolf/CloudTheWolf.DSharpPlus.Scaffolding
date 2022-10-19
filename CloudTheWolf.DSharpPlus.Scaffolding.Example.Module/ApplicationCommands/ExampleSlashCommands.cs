using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.ApplicationCommands
{
    internal class ExampleSlashCommands : ApplicationCommandModule
    {
        [SlashCommand("example", "A slash command made to test the DSharpPlus Slash Commands extension!")]
        public async Task ExampleSlashCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder().WithContent("Success!"));
        }
    }
}
