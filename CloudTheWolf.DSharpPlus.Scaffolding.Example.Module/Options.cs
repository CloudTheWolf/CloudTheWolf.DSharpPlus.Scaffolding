using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module
{
    public class Options
    {
        public static string MySqlHost {  get; set; }
        public static int MySqlPort { get; set; }
        public static string MySqlUsername { get; set; }
        public static string MySqlPassword { get; set; }
        public static string MySqlDatabase { get; set; }

    }
}
