using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    /// <summary>
    /// Application config 
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Load Discord Config from appsettings.json instead of from a Database
        /// </summary>
        public static bool LoadDiscordConfigFromFile { get; set; } = false;
        /// <summary>
        /// Prefix for commands
        /// </summary>
        public static IEnumerable<string> Prefix { get; set; }
        /// <summary>
        /// Discord Auth Token
        /// </summary>
        public static string Token {  get; set; }
        /// <summary>
        /// Enable DMs to the bot
        /// </summary>
        public static bool EnableDms { get; set; }
        /// <summary>
        /// Enable mention prefix
        /// </summary>
        public static bool EnableMentionPrefix { get; set; }
        /// <summary>
        /// DM the help to user instead of in the channel
        /// </summary>
        public static bool DmHelp { get; set; }
        /// <summary>
        /// Use DSharpPlus build in Help command
        /// </summary>
        public static bool DefaultHelp { get; set; }
        /// <summary>
        /// Run the bot in Shard Mode
        /// </summary>
        public static bool RunInShardMode { get; internal set; }

        /// <summary>
        /// Enable the GuildMembers Privileged Intent 
        /// </summary>
        public static bool GuildMembers { get; internal set; }
        /// <summary>
        /// Enable the GuildPresences Privileged Intent 
        /// </summary>
        public static bool GuildPresences { get; internal set; }
        /// <summary>
        /// Enable the MessageContents Privileged Intent 
        /// </summary>
        public static bool MessageContents { get; internal set; }

        /// <summary>
        /// Enable Debug Guild (Set to 0 to disable)
        /// </summary>
        public static ulong DebugGuildId { get; internal set; } = 0;
    }
}
