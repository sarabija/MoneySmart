using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HCI_PROJEKA_1
{
    public class GoalModel
    {
        public int GoalID { get; set; }
        public string GoalName { get; set; }
        public decimal GoalValue { get; set; }
        public int Instalments { get; set; }
        public int BalanceID { get; set; }
        public string Doable { get; set; }

        public static void AddGoal(GoalModel goal)
        {
            (decimal averageRevenue, decimal averageSpending) = GetCurrentMonthAverages(goal.BalanceID);
            decimal difference = averageRevenue - averageSpending;
            goal.Doable = difference >= goal.GoalValue / goal.Instalments ? "Yes" : "No";

            using var connection = Database.GetConnection();
            connection.Open();
            string query = "INSERT INTO goal (goalName, goalValue, instalments, balance_balanceID, doable) " +
                           "VALUES (@GoalName, @GoalValue, @Instalments, @BalanceID, @Doable)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@GoalName", goal.GoalName);
            command.Parameters.AddWithValue("@GoalValue", goal.GoalValue);
            command.Parameters.AddWithValue("@Instalments", goal.Instalments);
            command.Parameters.AddWithValue("@BalanceID", goal.BalanceID);
            command.Parameters.AddWithValue("@Doable", goal.Doable);

            command.ExecuteNonQuery();
        }


        public static List<GoalModel> GetAllGoals(int balanceID)
        {
            List<GoalModel> goals = new();
            (decimal averageRevenue, decimal averageSpending) = GetCurrentMonthAverages(balanceID);
            decimal difference = averageRevenue - averageSpending;

         
            var yesNoDictionary = new Dictionary<string, string>
            {
                { "English", "Yes" },
                { "Serbian", "Da" }
            };

            var noNoDictionary = new Dictionary<string, string>
            {
                { "English", "No" },
                { "Serbian", "Ne" }
            };

            string doableYes = yesNoDictionary.ContainsKey(LoginViewModel.CurrentLanguage) ? yesNoDictionary[LoginViewModel.CurrentLanguage] : "Yes";
            string doableNo = noNoDictionary.ContainsKey(LoginViewModel.CurrentLanguage) ? noNoDictionary[LoginViewModel.CurrentLanguage] : "No";

            using (var connection = Database.GetConnection())
            {
                string query = "SELECT * FROM goal WHERE balance_balanceID = @BalanceID";
                MySqlCommand cmd = new(query, connection);
                cmd.Parameters.AddWithValue("@BalanceID", balanceID);

                connection.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GoalModel goal = new GoalModel
                    {
                        GoalID = reader.GetInt32("goalID"),
                        GoalName = reader.GetString("goalName"),
                        GoalValue = reader.GetDecimal("goalValue"),
                        Instalments = reader.GetInt32("instalments"),
                        BalanceID = reader.GetInt32("balance_balanceID")
                    };

                    goal.Doable = difference >= goal.GoalValue / goal.Instalments ? doableYes : doableNo;
                    goals.Add(goal);
                }
            }

            return goals;
        }


 
        public static void UpdateGoal(GoalModel goal)
        {
            (decimal averageRevenue, decimal averageSpending) = GetCurrentMonthAverages(goal.BalanceID);
            decimal difference = averageRevenue - averageSpending;
            goal.Doable = difference >= goal.GoalValue / goal.Instalments ? "Yes" : "No";

            using var connection = Database.GetConnection();
            connection.Open();
            string query = "UPDATE goal SET goalName = @GoalName, goalValue = @GoalValue, " +
                           "instalments = @Instalments, balance_balanceID = @BalanceID, doable = @Doable " +
                           "WHERE goalID = @GoalID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@GoalName", goal.GoalName);
            command.Parameters.AddWithValue("@GoalValue", goal.GoalValue);
            command.Parameters.AddWithValue("@Instalments", goal.Instalments);
            command.Parameters.AddWithValue("@BalanceID", goal.BalanceID);
            command.Parameters.AddWithValue("@Doable", goal.Doable);
            command.Parameters.AddWithValue("@GoalID", goal.GoalID);

            command.ExecuteNonQuery();
        }

     
        public static void DeleteGoal(int goalID)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            string query = "DELETE FROM goal WHERE goalID = @GoalID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@GoalID", goalID);
            command.ExecuteNonQuery();
        }

    
        public static decimal CalculateMonthlyInstalment(decimal goalValue, int instalments)
        {
            if (instalments <= 0)
                throw new ArgumentException("Instalments must be greater than zero.");
            return Math.Round(goalValue / instalments, 2);
        }

    
        private static (decimal averageRevenue, decimal averageSpending) GetCurrentMonthAverages(int balanceID)
        {
            decimal averageRevenue = 0, averageSpending = 0;
            int revenueMonthsCount = 0; 

            using (var connection = Database.GetConnection())
            {
                connection.Open();

                string revenueQuery = @"  SELECT revenueDate, revenueValue   FROM revenue WHERE balance_balanceID = @BalanceID AND revenueDate >= DATE_SUB(CURRENT_DATE, INTERVAL 6 MONTH) ORDER BY revenueDate DESC";

                using (var command = new MySqlCommand(revenueQuery, connection))
                {
                    command.Parameters.AddWithValue("@BalanceID", balanceID);
                    using (var reader = command.ExecuteReader())
                    {
                        decimal totalRevenue = 0;
                        while (reader.Read())
                        {
                            totalRevenue += reader.GetDecimal("revenueValue");
                            revenueMonthsCount++;
                        }
                        if (revenueMonthsCount > 0)
                            averageRevenue = totalRevenue / revenueMonthsCount;
                    }
                }
                string spendingQuery = @" SELECT spendingDate, spendingValue FROM spending WHERE balance_balanceID = @BalanceID AND spendingDate >= DATE_SUB(CURRENT_DATE, INTERVAL 6 MONTH)  ORDER BY spendingDate DESC";

                using (var command = new MySqlCommand(spendingQuery, connection))
                {
                    command.Parameters.AddWithValue("@BalanceID", balanceID);
                    using var reader = command.ExecuteReader();
                    decimal totalSpending = 0;
                    int spendingMonthsCount = 0;
                    while (reader.Read())
                    {
                        totalSpending += reader.GetDecimal("spendingValue");
                        spendingMonthsCount++;
                    }
                    if (spendingMonthsCount > 0)
                        averageSpending = totalSpending / spendingMonthsCount;
                }
            }

            return (averageRevenue, averageSpending);
        }

    }
}
