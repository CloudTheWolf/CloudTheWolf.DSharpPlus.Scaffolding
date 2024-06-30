using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Context
{
    /// <summary>
    /// Custom Assembly Loading Class
    /// </summary>
    public class CustomLoadContext : AssemblyLoadContext
    {
        private string PluginDirectory;

        public CustomLoadContext(string pluginDirectory) : base(isCollectible: true)
        {
            PluginDirectory = pluginDirectory;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {

            // Check if the assembly is already loaded
            var assembly = Default.LoadFromAssemblyName(assemblyName);
            if (assembly != null)
            {
                return assembly;
            }

            // Try to load from the main application directory
            string assemblyPath = Path.Combine(AppContext.BaseDirectory, $"{assemblyName.Name}.dll");
            if (File.Exists(assemblyPath))
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            // Try to load from the plugin directory
            assemblyPath = Path.Combine(PluginDirectory, $"{assemblyName.Name}.dll");
            if (File.Exists(assemblyPath))
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            Console.WriteLine($"Failed to load assembly: {assemblyName.FullName}");
            return null;
        }
    }
}
