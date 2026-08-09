using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using Lavalink4NET.Players;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Configuration;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services
{
    /// <summary>
    /// Handle loading Plugins on application startup.
    /// </summary>
    public class PluginLoaderService
    {
        private static readonly Type[] SharedPluginTypes =
        [
            typeof(IPlugin),
            typeof(DiscordClient),
            typeof(CommandBuilder),
            typeof(InteractivityExtension),
            typeof(VoiceNextExtension),
            typeof(LavalinkPlayerOptions),
            typeof(IConfigurationRoot),
            typeof(Serilog.ILogger)
        ];

        /// <summary>
        /// List of plugin loaders
        /// </summary>
        private readonly List<PluginLoader> _loaders = [];

        /// <summary>Plugin assembly loaders owned by this service.</summary>
        public IReadOnlyList<PluginLoader> Loaders => _loaders;

        /// <summary>
        /// List of loaded plugins
        /// </summary>
        private readonly List<IPlugin> _plugins = [];

        /// <summary>Successfully loaded plugin instances.</summary>
        public IReadOnlyList<IPlugin> Plugins => _plugins;


        /// <summary>
        /// Load a Dictionary of Plugins as <see cref="IPlugin"/>
        /// </summary>
        public void LoadPlugins()
        {
            if (_loaders.Count > 0)
            {
                throw new InvalidOperationException("Plugins have already been loaded.");
            }

            if (!Directory.Exists(Constants.PluginsFolder)) return;
            var pluginPath = Directory.GetDirectories(Constants.PluginsFolder);
            foreach (var dir in pluginPath)
            {
                var dirName = Path.GetFileName(dir);
                var dirPath = Path.GetFullPath(dir);
                var pluginDll = Path.Combine(dirPath, dirName + ".dll");
                if (!File.Exists(pluginDll))
                {
                    continue;
                }

                PluginLoader loader = null;
                try
                {
                    loader = PluginLoader.CreateFromAssemblyFile(
                        pluginDll,
                        isUnloadable: true,
                        sharedTypes: SharedPluginTypes,
                        configure: config => config.LoadInMemory = true);

                    var pluginTypes = loader
                        .LoadDefaultAssembly()
                        .GetTypes()
                        .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract)
                        .ToArray();

                    _loaders.Add(loader);
                    loader = null; // Ownership has transferred to _loaders.

                    foreach (var pluginType in pluginTypes)
                    {
                        try
                        {
                            var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;
                            if (_plugins.Any(existing =>
                                    string.Equals(existing.Id, plugin.Id, StringComparison.OrdinalIgnoreCase)))
                            {
                                throw new InvalidOperationException(
                                    $"A plugin with the ID '{plugin.Id}' is already loaded.");
                            }

                            _plugins.Add(plugin);
                            Logger.Log.LogInformation(
                                "Loaded plugin {PluginName} ({PluginId}) version {PluginVersion}",
                                plugin.Name, plugin.Id, plugin.PluginVersion);
                        }
                        catch (Exception e)
                        {
                            Logger.Log.LogError(e,
                                "Error loading plugin type {PluginType} from {PluginAssembly}",
                                pluginType.FullName, pluginDll);
                        }
                    }
                }
                catch (ReflectionTypeLoadException exception)
                {
                    Logger.Log.LogError(exception,
                        "Plugin assembly {PluginAssembly} is incompatible with the Worker runtime and was skipped",
                        pluginDll);

                    foreach (var loaderException in exception.LoaderExceptions.Where(e => e is not null))
                    {
                        Logger.Log.LogError(loaderException,
                            "Type loader error for plugin assembly {PluginAssembly}: {LoaderError}",
                            pluginDll, loaderException!.Message);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log.LogError(exception,
                        "Failed to load plugin assembly {PluginAssembly}; the plugin was skipped",
                        pluginDll);
                }
                finally
                {
                    if (loader is not null)
                    {
                        try
                        {
                            loader.Dispose();
                        }
                        catch (Exception exception)
                        {
                            Logger.Log.LogError(exception,
                                "Failed to release plugin assembly context for {PluginAssembly}", pluginDll);
                        }
                    }
                }
            }
        }

        /// <summary>Releases all collectible plugin load contexts.</summary>
        public void UnloadPlugins()
        {
            _plugins.Clear();
            for (var index = _loaders.Count - 1; index >= 0; index--)
            {
                try
                {
                    _loaders[index].Dispose();
                }
                catch (Exception exception)
                {
                    Logger.Log.LogError(exception, "Failed to unload plugin assembly context");
                }
            }

            _loaders.Clear();
        }

    }
}
