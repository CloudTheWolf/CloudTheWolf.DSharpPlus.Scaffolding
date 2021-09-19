using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using DSharpPlus.Lavalink;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces
{
    /// <summary>
    /// Bot Interface
    /// </summary>
    public interface IBot
    {
        /// <summary>
        ///<see cref="CommandsNextExtension"/>
        /// </summary>
        CommandsNextExtension Commands { get; set;}
        /// <summary>
        /// <see cref="InteractivityExtension"/>
        /// </summary>
        InteractivityExtension Interactivity { get; set; }

        /// <summary>
        /// <see cref="DiscordClient"/>
        /// </summary>
        DiscordClient Client { get; set; }

        /// <summary>
        /// <see cref="VoiceNextExtension"/>
        /// </summary>
        VoiceNextExtension Voice { get; set; }
        /// <summary>
        /// <see cref="DiscordRestClient"/>
        /// </summary>
        DiscordRestClient Rest { get; set; }

        /// <summary>
        /// <see cref="InteractivityExtension"/>
        /// </summary>
        LavalinkConfiguration LvalinkConfig { get; set; }
    }

}
