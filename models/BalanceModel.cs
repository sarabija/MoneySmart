using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HCI_PROJEKA_1
{
    public class BalanceModel
    {
        public int BalanceID { get; set; }
        public decimal Cash { get; set; }
        public decimal Credit { get; set; }

       
        public static BalanceModel GetBalanceByID(int balanceID)
        {
            BalanceModel balance = null;

            using (var connection = Database.GetConnection())
            {
                string query = "SELECT * FROM balance WHERE balanceID = @BalanceID";
                MySqlCommand cmd = new(query, connection);
                cmd.Parameters.AddWithValue("@BalanceID", balanceID);

                connection.Open();
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    balance = new BalanceModel
                    {
                        BalanceID = reader.GetInt32("balanceID"),
                        Cash = reader.GetDecimal("cash"),
                        Credit = reader.GetDecimal("credit")
                    };
                }
            }

            return balance;
        }

    }
}
