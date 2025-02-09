using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using Google.Protobuf.WellKnownTypes;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands.Choices
{
    internal class DaysOfWeekProvider : IChoiceProvider
    {
        private readonly IEnumerable<DiscordApplicationCommandOptionChoice> _commandOptions = new List<DiscordApplicationCommandOptionChoice>
        {
            new("Monday", "1"),
            new("Tuesday", "2"),
            new("Wednesday", "3"),
            new("Thursday", "4"),
            new("Friday", "5"),
            new("Saturday", "6"),
            new("Sunday", "7")
        };
        ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> IChoiceProvider.ProvideAsync(CommandParameter parameter)
        {
            return ValueTask.FromResult(_commandOptions);
        }
    }
}
