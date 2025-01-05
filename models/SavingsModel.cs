using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;

namespace HCI_PROJEKA_1
{
    public class SavingModel
    {
        public int SavingID { get; set; }
        public string SavingName { get; set; }
        public decimal SavingDeposit { get; set; }
        public decimal SavingGoal { get; set; }
        public int BalanceID { get; set; }
        public decimal Percentage => SavingGoal > 0 ? (SavingDeposit / SavingGoal) * 100 : 0;

        public static void AddSaving(SavingModel saving)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            string query = "INSERT INTO saving (savingName, savingDeposit, savingGoal, balance_balanceID) " +
                           "VALUES (@SavingName, @SavingDeposit, @SavingGoal, @BalanceID)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@SavingName", saving.SavingName);
            command.Parameters.AddWithValue("@SavingDeposit", saving.SavingDeposit);
            command.Parameters.AddWithValue("@SavingGoal", saving.SavingGoal);
            command.Parameters.AddWithValue("@BalanceID", saving.BalanceID);

            command.ExecuteNonQuery();
        }

        public static List<SavingModel> GetAllSavings(int balanceID)
        {
            List<SavingModel> savings = new List<SavingModel>();

            using (var connection = Database.GetConnection())
            {
                string query = "SELECT * FROM saving WHERE balance_balanceID = @BalanceID";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BalanceID", balanceID);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SavingModel saving = new SavingModel
                        {
                            SavingID = reader.GetInt32("savingID"),
                            SavingName = reader.GetString("savingName"),
                            SavingDeposit = reader.GetDecimal("savingDeposit"),
                            SavingGoal = reader.GetDecimal("savingGoal"),
                            BalanceID = reader.GetInt32("balance_balanceID")
                        };
                        savings.Add(saving);
                    }
                }
            }

            return savings;
        }

        public static void UpdateSaving(SavingModel saving)
        {

            using var connection = Database.GetConnection();
            connection.Open();
            string query = "UPDATE saving SET savingName = @SavingName, savingDeposit = @SavingDeposit, " +
                           "savingGoal = @SavingGoal, balance_balanceID = @BalanceID " +
                           "WHERE savingID = @SavingID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@SavingName", saving.SavingName);
            command.Parameters.AddWithValue("@SavingDeposit", saving.SavingDeposit);
            command.Parameters.AddWithValue("@SavingGoal", saving.SavingGoal);
            command.Parameters.AddWithValue("@BalanceID", saving.BalanceID);
            command.Parameters.AddWithValue("@SavingID", saving.SavingID);

            command.ExecuteNonQuery();
        }

        public static void DeleteSaving(int savingID)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            string query = "DELETE FROM saving WHERE savingID = @SavingID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@SavingID", savingID);
            command.ExecuteNonQuery();
        }
    }
}
