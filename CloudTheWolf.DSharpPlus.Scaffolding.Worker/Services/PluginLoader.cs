using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using CloudTheWolf.DSharpPlus.Scaffolding.Worker.Context;
using Microsoft.Extensions.Logging;

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
        public static Dictionary<string, CustomLoadContext> PluginLoadContexts = new();


        /// <summary>
        /// Load a Dictionary of Plugins as <see cref="IPlugin"/>
        /// </summary>
        public void LoadPlugins()
        {
            //Load the DLLs from the Plugins directory
            if (!Directory.Exists(Constants.PluginsFolder)) return;
            var pluginDirectories = Directory.GetDirectories(Constants.PluginsFolder);
            foreach (var pluginDir in pluginDirectories)
            {
                var files = Directory.GetFiles(pluginDir,"*.dll");
                foreach (var file in files)
                {
                    try
                    {
                        var loadContext = new CustomLoadContext(pluginDir);
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
                            Bot.Logger.LogInformation($"Loaded plugin: {pluginInstance.Name}");
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Load a Dictionary of Plugins supporting Shard Mode as <see cref="IShardPlugin"/>
        /// </summary>
        public void LoadShardPlugins()
        {

            //Load the DLLs from the Plugins directory
            if (!Directory.Exists(Constants.PluginsFolder)) return;
            var plugins = Directory.GetDirectories(Constants.PluginsFolder);
            foreach (var plugin in plugins)
            {
                var files = Directory.GetFiles(plugin);
                foreach (var file in files)
                {
                    try
                    {
                        if (!file.EndsWith("dll")) continue;
                        var loadContext = new CustomLoadContext(plugin);
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
                    catch (Exception e)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
