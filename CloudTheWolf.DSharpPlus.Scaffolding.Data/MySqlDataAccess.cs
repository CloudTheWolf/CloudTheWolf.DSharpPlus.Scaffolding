using System;
using System.Data;
using MySqlConnector;
using Newtonsoft.Json;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Data
{
    public class MySqlDataAccess : DataAccess
    {
        private MySqlConnection _sqlConnection;

        public override string LoadConnectionString(string connStr)
        {
            _sqlConnection = new MySqlConnection(connStr);
            return _sqlConnection.ConnectionString;
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
