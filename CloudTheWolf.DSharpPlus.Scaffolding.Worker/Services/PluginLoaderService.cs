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
        public List<PluginLoader> Loaders = new();

        /// <summary>
        /// List of loaded plugins
        /// </summary>
        public List<IPlugin> Plugins = new();


        /// <summary>
        /// Load a Dictionary of Plugins as <see cref="IPlugin"/>
        /// </summary>
        public void LoadPlugins()
        {
            //Load the DLLs from the Plugins directory
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
                        sharedTypes: [typeof(IPlugin)]);
                    Loaders.Add(loader);
                }
            }

            foreach (var loader in Loaders)
            {
                foreach (var pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    try
                    {
                        var plugin = Activator.CreateInstance(pluginType) as IPlugin;
                        Plugins.Add(plugin);
                        Logger.Log.LogInformation($"Loaded plugin: {plugin?.Name}");
                    }
                    catch (Exception e)
                    {
                        Logger.Log.LogError($"Error Loading Plugin... \n {e.Message}");
                        continue;
                    }
                }
            }
        }

    }
}
