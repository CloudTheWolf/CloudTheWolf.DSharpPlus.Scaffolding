using System;
using System.Data;
using MySqlConnector;
using Newtonsoft.Json;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Data
{
    public class MySqlDataAccess : DataAccess
    {
        private string _sqlConnectionString;
        private MySqlConnection _sqlConnection;

        public override string LoadConnectionString(string connStr)
        {
            _sqlConnectionString = connStr;
            return _sqlConnectionString;
        }

        public override string Request(string sqlCommandString)
        {            
            try
            {
                DbConnect();
                DataTable dt = new DataTable();
                using MySqlDataAdapter sda = new MySqlDataAdapter(sqlCommandString, _sqlConnection);
                sda.Fill(dt);

                return JsonConvert.SerializeObject(dt, Formatting.Indented);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void DbConnect()
        {
            _sqlConnection.ConnectionString = _sqlConnectionString;
            try
            {
                _sqlConnection.Open();
                Console.Write("SQL Connection Opened");
                _sqlConnection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


        }

    }
}
