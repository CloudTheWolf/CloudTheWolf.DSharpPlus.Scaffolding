using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands.Choices
{
    internal class DaysOfWeekProvider : IChoiceProvider
    {
        private static readonly IReadOnlyDictionary<string, object> daysOfTheWeek = new Dictionary<string, object>
        {
            ["Monday"] = 1,
            ["Tuesday"] = 2,
            ["Wednesday"] = 3,
            ["Thursday"] = 4,
            ["Friday"] = 5,
            ["Saturday"] = 6,
            ["Sunday"] = 7
        };

        public ValueTask<IReadOnlyDictionary<string, object>> ProvideAsync(CommandParameter parameter) => ValueTask.FromResult(daysOfTheWeek);
    }
}
