using System;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Data
{
    public interface IDatabase
    {
        string LoadConnectionString(string name);

        string Request(string sql);

        
    }
}
