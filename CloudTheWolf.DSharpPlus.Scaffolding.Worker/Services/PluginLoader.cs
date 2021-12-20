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

        public void LoadPlugins()
        {
            try
            {
                Plugins = new List<IPlugin>();

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

                var interfaceType = typeof(IPlugin);
                //Fetch all types that implement the interface IPlugin and are a class
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var ts = new List<Type>();
                var tsLikePlugin = new List<Type>();

                foreach (var assembly in assemblies)
                {
                    var item = assembly.GetTypes();
                    ts.Add(item.FirstOrDefault());
                }

                foreach (var type in ts)
                {
                    if (interfaceType.IsAssignableFrom(type))
                    {
                        if (type.IsClass)
                        {
                            tsLikePlugin.Add(type);
                        }
                    }
                }

                var types = tsLikePlugin.ToArray();

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

    }
}
