using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Load and <see cref="Assembly"/> using out custom context
        /// </summary>
        public CustomLoadContext() : base(isCollectible: true)
        {
            // Nothing to do here, we just need to have isCollectible set
        }

        /// <summary>
        /// Load and <see cref="Assembly"/>
        /// </summary>
        /// <param name="assemblyName">Name of the assembly to load</param>
        /// <returns>null</returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            // We don't actually need to do anything here so let's just return null
            return null;
        }
    }
}
