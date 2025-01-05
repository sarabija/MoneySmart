using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HCI_PROJEKA_1
{
    public class RevenueModel
    {
        public int RevenueID { get; set; }
        public string RevenueName { get; set; }
        public decimal RevenueValue { get; set; }
        public string RevenueType { get; set; }
        public string RevenueMode { get; set; }
        public int BalanceID { get; set; }
        public DateTime RevenueDate { get; set; } 

        public static void AddRevenue(RevenueModel revenue)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            string query = "INSERT INTO revenue (revenueName, revenueValue, revenueType, revenueMode, balance_balanceID, revenueDate) " +
                           "VALUES (@RevenueName, @RevenueValue, @RevenueType, @RevenueMode, @BalanceID, @RevenueDate)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@RevenueName", revenue.RevenueName);
                command.Parameters.AddWithValue("@RevenueValue", revenue.RevenueValue);
                command.Parameters.AddWithValue("@RevenueType", revenue.RevenueType);
                command.Parameters.AddWithValue("@RevenueMode", revenue.RevenueMode);
                command.Parameters.AddWithValue("@BalanceID", revenue.BalanceID);
                command.Parameters.AddWithValue("@RevenueDate", DateTime.Now);

                command.ExecuteNonQuery();
            }
        }

        public static List<RevenueModel> GetAllRevenues(int balanceID)
        {
            List<RevenueModel> revenues = new List<RevenueModel>();

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

            Dictionary<string, string> revenueTypeMappingsToSerbian = new Dictionary<string, string>
            {
                { "Annual", "Godišnji" },
                { "Monthly", "Mesečni" },
                { "One-Time", "Jednokratni" }
            };

            Dictionary<string, string> revenueTypeMappingsToEnglish = new Dictionary<string, string>
            {
                { "Godišnji", "Annual" },
                { "Mesečni", "Monthly" },
                { "Jednokratni", "One-Time" }
            };

            using (var connection = Database.GetConnection())
            {
                string query = "SELECT * FROM revenue WHERE balance_balanceID = @BalanceID";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BalanceID", balanceID);

                connection.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RevenueModel revenue = new RevenueModel
                    {
                        RevenueID = reader.GetInt32("revenueID"),
                        RevenueName = reader.GetString("revenueName"),
                        RevenueValue = reader.GetDecimal("revenueValue"),
                        BalanceID = reader.GetInt32("balance_balanceID"),
                        RevenueDate = reader.GetDateTime("revenueDate")
                    };


                    string revenueType = reader.GetString("revenueType");
                    if (LoginViewModel.CurrentLanguage == "Serbian" && revenueTypeMappingsToSerbian.ContainsKey(revenueType))
                    {
                        revenue.RevenueType = revenueTypeMappingsToSerbian[revenueType];
                    }
                    else if (LoginViewModel.CurrentLanguage == "English" && revenueTypeMappingsToEnglish.ContainsKey(revenueType))
                    {
                        revenue.RevenueType = revenueTypeMappingsToEnglish[revenueType];
                    }
                    else
                    {
                        revenue.RevenueType = revenueType;
                    }

                    string revenueMode = reader.GetString("revenueMode");
                    if (LoginViewModel.CurrentLanguage == "Serbian" && revenueModeMappingsToSerbian.ContainsKey(revenueMode))
                    {
                        revenue.RevenueMode = revenueModeMappingsToSerbian[revenueMode];
                    }
                    else if (LoginViewModel.CurrentLanguage == "English" && revenueModeMappingsToEnglish.ContainsKey(revenueMode))
                    {
                        revenue.RevenueMode = revenueModeMappingsToEnglish[revenueMode];
                    }
                    else
                    {
                        revenue.RevenueMode = revenueMode;
                    }

                    revenues.Add(revenue);
                }
            }

            return revenues;
        }



        public static void UpdateRevenue(RevenueModel revenue)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = "UPDATE revenue SET revenueName = @RevenueName, revenueValue = @RevenueValue, " +
                               "revenueType = @RevenueType, revenueMode = @RevenueMode, balance_balanceID = @BalanceID, " +
                               "revenueDate = @RevenueDate " +
                               "WHERE revenueID = @RevenueID";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@RevenueName", revenue.RevenueName);
                command.Parameters.AddWithValue("@RevenueValue", revenue.RevenueValue);
                command.Parameters.AddWithValue("@RevenueType", revenue.RevenueType);
                command.Parameters.AddWithValue("@RevenueMode", revenue.RevenueMode);
                command.Parameters.AddWithValue("@BalanceID", revenue.BalanceID);
                command.Parameters.AddWithValue("@RevenueDate", DateTime.Now);
                command.Parameters.AddWithValue("@RevenueID", revenue.RevenueID);

                command.ExecuteNonQuery();
            }
        }


        public static void DeleteRevenue(int revenueID)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM revenue WHERE revenueID = @RevenueID";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@RevenueID", revenueID);
                command.ExecuteNonQuery();
            }
        }

        public static List<MonthlyRevenue> GetMonthlyRevenues(int balanceID, int year)
        {
            List<MonthlyRevenue> monthlyRevenues = new List<MonthlyRevenue>();

            using (var connection = Database.GetConnection())
            {
                string query = @" SELECT MONTH(revenueDate) AS Month,  SUM(CASE 
                                WHEN (revenueType = 'One-Time' OR revenueType = 'Jednokratni') 
                                THEN revenueValue 
                                ELSE 0 
                                END) AS OneTimeRevenue, 
                                SUM(CASE 
                                WHEN (revenueType = 'Monthly' OR revenueType = 'Mesečni') 
                                THEN revenueValue 
                                ELSE 0 
                                END) AS RegularRevenue,
                                SUM(CASE 
                                WHEN (revenueType = 'Annual' OR revenueType = 'Godišnji') 
                                THEN revenueValue / 12 
                                ELSE 0 
                                END) AS AnnualAverageRevenue
                                FROM revenue
                                WHERE balance_balanceID = @BalanceID
                                AND YEAR(revenueDate) = @Year
                                GROUP BY MONTH(revenueDate)
                                ORDER BY MONTH(revenueDate)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BalanceID", balanceID);
                cmd.Parameters.AddWithValue("@Year", year);

                connection.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var monthlyRevenue = new MonthlyRevenue
                    {
                        Month = reader.GetInt32("Month"),
                        OneTimeRevenue = reader.GetDecimal("OneTimeRevenue"),
                        RegularRevenue = reader.GetDecimal("RegularRevenue"),
                        AnnualAverageRevenue = reader.GetDecimal("AnnualAverageRevenue")
                    };
                    monthlyRevenues.Add(monthlyRevenue);
                }
            }

            return monthlyRevenues;
        }
    }

    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public decimal OneTimeRevenue { get; set; }
        public decimal RegularRevenue { get; set; }
        public decimal AnnualAverageRevenue { get; set; }
    }
}
