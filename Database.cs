using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;


namespace HCI_PROJEKA_1
{
  

    public class Database
    {
        public static MySqlConnection GetConnection()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["MoneySmartDataBase"].ConnectionString;

            MySqlConnection connection = new(connectionString);
            return connection;
        }
    }

}
