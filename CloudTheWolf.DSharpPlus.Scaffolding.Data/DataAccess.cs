using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Data
{
    public abstract class DataAccess
    {
        public abstract string LoadConnectionString(string name);

        public abstract string Request(string sqlCommandString);
        
    }
}
