using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using CloudTheWolf.DSharpPlus.Scaffolding.Worker.Context;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services
{
    public class PluginLoader
    {
        /// <summary>
        /// Dictionary of <see cref="IPlugin"/>s
        /// </summary>
        public Dictionary<string, IPlugin> Plugins = new();
        /// <summary>
        /// Dictionary of <see cref="IShardPlugin"/>s
        /// </summary>
        public Dictionary<string, IShardPlugin> ShardPlugins = new();
        private Dictionary<string, CustomLoadContext> PluginLoadContexts = new();


        /// <summary>
        /// Load a Dictionary of Plugins as <see cref="IPlugin"/>
        /// </summary>
        public void LoadPlugins()
        {
            try
            {
                //Load the DLLs from the Plugins directory
                if (!Directory.Exists(Constants.PluginsFolder)) return;
                var plugins = Directory.GetDirectories(Constants.PluginsFolder);
                foreach (var plugin in plugins)
                {
                    var files = Directory.GetFiles(plugin);
                    foreach (var file in files)
                    {
                        if (!file.EndsWith("dll")) continue;
                        var loadContext = new CustomLoadContext();
                        var assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(file));

                        // Get types that implement IPlugin
                        var pluginTypes = assembly.GetTypes().Where(t =>
                            typeof(IPlugin).IsAssignableFrom(t) && t.IsClass);

                        // Create instance of each type and add to plugins list
                        foreach (var type in pluginTypes)
                        {
                            var pluginInstance = (IPlugin)Activator.CreateInstance(type);
                            Plugins[pluginInstance.Name] = pluginInstance;
                            PluginLoadContexts[pluginInstance.Name] = loadContext;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Load a Dictionary of Plugins supporting Shard Mode as <see cref="IShardPlugin"/>
        /// </summary>
        public void LoadShardPlugins()
        {
            try
            {
                //Load the DLLs from the Plugins directory
                if (!Directory.Exists(Constants.PluginsFolder)) return;
                var plugins = Directory.GetDirectories(Constants.PluginsFolder);
                foreach (var plugin in plugins)
                {
                    var files = Directory.GetFiles(plugin);
                    foreach (var file in files)
                    {
                        if (!file.EndsWith("dll")) continue;
                        var loadContext = new CustomLoadContext();
                        var assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(file));

                        // Get types that implement IShardPlugin
                        var pluginTypes = assembly.GetTypes().Where(t =>
                            typeof(IShardPlugin).IsAssignableFrom(t) && t.IsClass);

                        // Create instance of each type and add to plugins list
                        foreach (var type in pluginTypes)
                        {
                            var pluginInstance = (IShardPlugin)Activator.CreateInstance(type);
                            ShardPlugins[pluginInstance.Name] = pluginInstance;
                            PluginLoadContexts[pluginInstance.Name] = loadContext;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
