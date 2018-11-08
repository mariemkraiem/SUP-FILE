using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace SupFile2.Utilities
{
    public class Database
    {
        public static Database Instance;
        public static string StoragePath = "storage";

        public MySqlConnection Connection;

        public Database()
        {
            if (Instance != null)
                return;

            Instance = this;

            string connectionInitiator = "Database=" + "supfile" + "; ";
            connectionInitiator += "Data Source=" + "localhost" + "; ";

            connectionInitiator += "User Id=" + "supfileUser" + "; ";
            connectionInitiator += "Password=" + "ogdTdolLxsFDrrNwI3JB" + "; ";

            connectionInitiator += "SslMode=" + "none" + "; ";
            Connection = new MySqlConnection(connectionInitiator);
            Connection.Open();
        }

        public bool Query(string query)
        {
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = query;
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Debug.Log("query result : " + reader.ToString());
            }

            reader.Close();
            return true;
        }

        public string SeePreparedQuery(MySqlCommand cmd)
        {
            string query = cmd.CommandText;
            foreach (MySqlParameter p in cmd.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }
            return query;
        }
    }
}
