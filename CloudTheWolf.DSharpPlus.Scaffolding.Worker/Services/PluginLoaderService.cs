using System.Collections.Generic;
using System.IO;
using System.Linq;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using McMaster.NETCore.Plugins;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services
{
    /// <summary>
    /// Handle loading Plugins on application startup.
    /// </summary>
    public class PluginLoaderService
    {
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
                if (File.Exists(pluginDll))
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        pluginDll,
                        isUnloadable: true,
                        sharedTypes: [typeof(IPlugin)],
                        configure: config => config.LoadInMemory = true);
                    _loaders.Add(loader);
                }
            }

            foreach (var loader in _loaders)
            {
                foreach (var pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract))
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
                        Logger.Log.LogError(e, "Error loading plugin type {PluginType}", pluginType.FullName);
                        continue;
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
