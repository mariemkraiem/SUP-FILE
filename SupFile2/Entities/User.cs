using MySql.Data.MySqlClient;
using SupFile2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupFile2.Entities
{
    public class User
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private String pseudo;
        public String Pseudo
        {
            get { return pseudo; }
            set { pseudo = value; }
        }

        private long stockage;
        public long Stockage
        {
            get { return stockage; }
            set { stockage = value; }
        }

        private long stockageMax;
        public long StockageMax
        {
            get { return stockageMax; }
            set { stockageMax = value; }
        }

        public User(int id)
        {
            string sql = "SELECT * FROM `user` WHERE `id` = @id";
            MySqlCommand cmd = new MySqlCommand(sql, Database.Instance.Connection);
            cmd.Parameters.AddWithValue("@id", id);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                this.id = reader.GetInt32("id");
                this.stockage = reader.GetInt32("stockage");
                this.stockageMax = reader.GetInt32("stockagemax");
            }
            
            reader.Close();
            cmd.Dispose();
        }

        public void libererStockage(long tailleAliberer, int user_id)
        {

            stockage -= tailleAliberer;
            Save();   
        }

        public void Save()
        {
            string sql = "UPDATE `user` SET `stockage` = @nouveautaille WHERE `id` = @id";
            MySqlCommand cmd = new MySqlCommand(sql, Database.Instance.Connection);
            cmd.Parameters.AddWithValue("@id", this.id);
            cmd.Parameters.AddWithValue("@nouveautaille", this.stockage);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

        }

        public  bool ajouterStockage(long tailleAajouter)
        {  
            if(this.stockage + tailleAajouter < this.stockageMax)
            {
                this.stockage += tailleAajouter;
                return true;
            }
            return false;
        }
    }
}