using CloudTheWolf.DSharpPlus.Scaffolding.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Classes
{
    class Database {

        private DataAccess sda;

        internal Database()
        {
            var MySql = true;
            sda = MySql ? new MySqlDataAccess() : new SqlSvrDataAccess();

            sda.LoadConnectionString($"Server={Options.MySqlHost};Port={Options.MySqlPort};Uid={Options.MySqlUsername};Pwd={Options.MySqlPassword};Database={Options.MySqlDatabase};",Example.Logger);

        }


        internal JArray GetConfig()
        {
            
            var json = sda.Request("SELECT * FROM config;", Example.Logger);
            return JArray.Parse(json);
        }


        
           
    }
}
