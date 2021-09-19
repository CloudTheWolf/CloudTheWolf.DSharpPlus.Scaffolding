using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Data
{
    public class SqlSvrDataAccess : DataAccess
    {
        private string _sqlConnectionString;
        private SqlConnection _sqlConnection;
        public override string LoadConnectionString(string connStr)
        {
            _sqlConnectionString = connStr;
            return _sqlConnectionString;
        }

        public override string Request(string sqlCommandString)
        {
            try
            {
                DataTable dt = new DataTable();
                using SqlDataAdapter sda = new SqlDataAdapter(sqlCommandString, _sqlConnection);
                sda.Fill(dt);

                return JsonConvert.SerializeObject(dt, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
