using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HCI_PROJEKA_1
{
    public class SpendingModel
    {
        public int SpendingID { get; set; }
        public string SpendingName { get; set; }
        public string SpendingCategory { get; set; }
        public decimal SpendingValue { get; set; }
        public int BalanceID { get; set; }
        public string SpendingType { get; set; } 
        public DateTime SpendingDate { get; set; }

        public static void AddSpending(SpendingModel spending)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            string query = "INSERT INTO spending (spendingName, spendingCategory, spendingValue, balance_balanceID, spendingType, spendingDate) " +
                           "VALUES (@SpendingName, @SpendingCategory, @SpendingValue, @BalanceID, @SpendingType, @SpendingDate)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SpendingName", spending.SpendingName);
                command.Parameters.AddWithValue("@SpendingCategory", spending.SpendingCategory);
                command.Parameters.AddWithValue("@SpendingValue", spending.SpendingValue);
                command.Parameters.AddWithValue("@BalanceID", spending.BalanceID);
                command.Parameters.AddWithValue("@SpendingType", spending.SpendingType);
                command.Parameters.AddWithValue("@SpendingDate", DateTime.Now);

                command.ExecuteNonQuery();
            }
        }

        public static List<SpendingModel> GetAllSpendings(int balanceID)
        {
            List<SpendingModel> spendings = new List<SpendingModel>();

            Dictionary<string, string> categoryMappingsToSerbian = new Dictionary<string, string>
            {
                { "Food", "Hrana" },
                { "Rent and utilities", "Kirija i računi" },
                { "Gifts", "Pokloni" },
                { "Entertainment", "Zabava" },
                { "Transportation", "Prevoz" },
                { "Shopping", "Kupovina" },
                { "Miscellaneous", "Razno" }
            };

            Dictionary<string, string> categoryMappingsToEnglish = new Dictionary<string, string>
            {
                { "Hrana", "Food" },
                { "Kirija i računi", "Rent and utilities" },
                { "Pokloni", "Gifts" },
                { "Zabava", "Entertainment" },
                { "Prevoz", "Transportation" },
                { "Kupovina", "Shopping" },
                { "Razno", "Miscellaneous" }
            };

            Dictionary<string, string> revenueModeMappingsToSerbian = new Dictionary<string, string>
            {
                { "Cash", "Gotovina" },
                { "Credit card", "Kartica" }
            };

            Dictionary<string, string> revenueModeMappingsToEnglish = new Dictionary<string, string>
            {
                { "Gotovina", "Cash" },
                { "Kartica", "Credit card" }
            };

            string currentLanguage = LoginViewModel.CurrentLanguage.ToLower(); 

            using (var connection = Database.GetConnection())
            {
                string query = "SELECT * FROM spending WHERE balance_balanceID = @BalanceID";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BalanceID", balanceID);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string spendingCategory = reader.GetString("spendingCategory");
                        if (currentLanguage == "serbian" && categoryMappingsToSerbian.ContainsKey(spendingCategory))
                        {
                            spendingCategory = categoryMappingsToSerbian[spendingCategory];
                        }
                        else if (currentLanguage == "english" && categoryMappingsToEnglish.ContainsKey(spendingCategory))
                        {
                            spendingCategory = categoryMappingsToEnglish[spendingCategory];
                        }

                        string spendingType = reader.GetString("spendingType");
                        if (currentLanguage == "serbian" && revenueModeMappingsToSerbian.ContainsKey(spendingType))
                        {
                            spendingType = revenueModeMappingsToSerbian[spendingType];
                        }
                        else if (currentLanguage == "english" && revenueModeMappingsToEnglish.ContainsKey(spendingType))
                        {
                            spendingType = revenueModeMappingsToEnglish[spendingType];
                        }

                        SpendingModel spending = new SpendingModel
                        {
                            SpendingID = reader.GetInt32("spendingID"),
                            SpendingName = reader.GetString("spendingName"),
                            SpendingCategory = spendingCategory, 
                            SpendingValue = reader.GetDecimal("spendingValue"),
                            BalanceID = reader.GetInt32("balance_balanceID"),
                            SpendingType = spendingType, 
                            SpendingDate = reader.GetDateTime("spendingDate")
                        };
                        spendings.Add(spending);
                    }
                }
            }

            return spendings;
        }


        public static void UpdateSpending(SpendingModel spending)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = "UPDATE spending SET spendingName = @SpendingName, spendingCategory = @SpendingCategory, " +
                               "spendingValue = @SpendingValue, balance_balanceID = @BalanceID, spendingType = @SpendingType, " +
                               "spendingDate = @SpendingDate WHERE spendingID = @SpendingID";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@SpendingName", spending.SpendingName);
                command.Parameters.AddWithValue("@SpendingCategory", spending.SpendingCategory);
                command.Parameters.AddWithValue("@SpendingValue", spending.SpendingValue);
                command.Parameters.AddWithValue("@BalanceID", spending.BalanceID);
                command.Parameters.AddWithValue("@SpendingType", spending.SpendingType);
                command.Parameters.AddWithValue("@SpendingDate", DateTime.Now);
                command.Parameters.AddWithValue("@SpendingID", spending.SpendingID);

                command.ExecuteNonQuery();
            }
        }

        public static void DeleteSpending(int spendingID)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM spending WHERE spendingID = @SpendingID";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@SpendingID", spendingID);
                command.ExecuteNonQuery();
            }
        }

        public static Dictionary<string, decimal> GetTotalSpendingByCategory(int balanceID)
        {
            Dictionary<string, decimal> categoryTotals = new Dictionary<string, decimal>();

            Dictionary<string, string> categoryMappingsToSerbian = new Dictionary<string, string>
            {
                { "Food", "Hrana" },
                { "Rent and utilities", "Kirija i računi" },
                { "Gifts", "Pokloni" },
                { "Entertainment", "Zabava" },
                { "Transportation", "Prevoz" },
                { "Shopping", "Kupovina" },
                { "Miscellaneous", "Razno" }
            };

            Dictionary<string, string> categoryMappingsToEnglish = new Dictionary<string, string>
            {
                { "Hrana", "Food" },
                { "Kirija i računi", "Rent and utilities" },
                { "Pokloni", "Gifts" },
                { "Zabava", "Entertainment" },
                { "Prevoz", "Transportation" },
                { "Kupovina", "Shopping" },
                { "Razno", "Miscellaneous" }
            };

            using (var connection = Database.GetConnection())
            {
                string query = "SELECT spendingCategory, SUM(spendingValue) AS totalSpending " +
                               "FROM spending WHERE balance_balanceID = @BalanceID GROUP BY spendingCategory";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BalanceID", balanceID);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string category = reader.GetString("spendingCategory");
                        decimal total = reader.GetDecimal("totalSpending");

               
                        if (LoginViewModel.CurrentLanguage == "Serbian" && categoryMappingsToSerbian.ContainsKey(category))
                        {
                            category = categoryMappingsToSerbian[category];
                        }
                        else if (LoginViewModel.CurrentLanguage == "English" && categoryMappingsToEnglish.ContainsKey(category))
                        {
                            category = categoryMappingsToEnglish[category]; 
                        }

                        if (categoryTotals.ContainsKey(category))
                        {
                            categoryTotals[category] += total;  
                        }
                        else
                        {
                            categoryTotals[category] = total;  
                        }
                    }
                }
            }

            return categoryTotals;
        }
    }
}
