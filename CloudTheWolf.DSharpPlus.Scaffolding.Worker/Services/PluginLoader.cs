using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services
{
    public class PluginLoader
    {
        public List<IPlugin> Plugins { get; set; }
        public List<IShardPlugin> ShardPlugins { get; set; }

        public void LoadPlugins()
        {
            try
            {
                Plugins = new List<IPlugin>();

                //Load the DLLs from the Plugins directory
                if (Directory.Exists(Constants.PluginsFolder))
                {
                    var dirs = Directory.GetDirectories(Constants.PluginsFolder);
                    foreach (var dir in dirs)
                    {
                        var files = Directory.GetFiles(dir);
                        foreach (var file in files)
                        {
                            if (file.EndsWith("dll"))
                                Assembly.LoadFrom(Path.GetFullPath(file));
                        }
                    }
                }

                var interfaceType = typeof(IPlugin);
                //Fetch all types that implement the interface IPlugin and are a class
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var ts = assemblies.Select(assembly => assembly.GetTypes()).Select(item => item.FirstOrDefault()).ToList();

                var types = ts.Where(type => interfaceType.IsAssignableFrom(type)).Where(type => type.IsClass).ToArray();

                //Create a new instance of all found types
                foreach (var type in types)
                {
                    Plugins.Add((IPlugin)Activator.CreateInstance(type));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void LoadShardPlugins()
        {
            try
            {
                ShardPlugins = new List<IShardPlugin>();

                //Load the DLLs from the Plugins directory
                if (Directory.Exists(Constants.PluginsFolder))
                {
                        var files = Directory.GetFiles(Constants.PluginsFolder);
                        foreach (var file in files)
                        {
                            if (file.EndsWith("dll"))
                                Assembly.LoadFrom(Path.GetFullPath(file));
                        }
                }

                var interfaceType = typeof(IShardPlugin);
                //Fetch all types that implement the interface IPlugin and are a class
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var ts = assemblies.Select(assembly => assembly.GetTypes()).Select(item => item.FirstOrDefault()).ToList();

                var types = ts.Where(type => interfaceType.IsAssignableFrom(type)).Where(type => type.IsClass).ToArray();

                //Create a new instance of all found types
                foreach (var type in types)
                {
                    ShardPlugins.Add((IShardPlugin)Activator.CreateInstance(type));
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
