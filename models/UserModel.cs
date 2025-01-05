using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace HCI_PROJEKA_1
{
    public class UserModel
    {
        public bool CheckUserCredentials(string username, string password)
        {
            using (var connection = Database.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM user WHERE username = @Username AND password = @Password";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                connection.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        
        }

        public bool UpdateTheme(string username, string newTheme)
        {

            using (var connection = Database.GetConnection())
            {
                string query = "UPDATE user SET theme = @NewTheme WHERE username = @Username";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@NewTheme", newTheme);

                connection.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0; 
            }
        }

        public bool UpdateLanguage(string username, string newLanguage)
        {
            using (var connection = Database.GetConnection())
            {
                string query = "UPDATE user SET language = @NewLanguage WHERE username = @Username";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@NewLanguage", newLanguage);

                connection.Open();
                int result = cmd.ExecuteNonQuery();

                return result > 0; 
            }
        }

        public bool UpdateExpertUser(string username, bool isExpertUser)
        {
            using var connection = Database.GetConnection();
            string query = "UPDATE user SET expertUser = @IsExpertUser WHERE username = @Username";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@IsExpertUser", isExpertUser ? 1 : 0);

            connection.Open();
            int result = cmd.ExecuteNonQuery();

            return result > 0;
        }

        public string GetTheme(string username)
        {
            using (var connection = Database.GetConnection())
            {
                string query = "SELECT theme FROM user WHERE username = @Username";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);

                connection.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }

      
        public string GetLanguage(string username)
        {
            using (var connection = Database.GetConnection())
            {
                string query = "SELECT language FROM user WHERE username = @Username";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);

                connection.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null; 
            }
        }

 
        public bool IsExpertUser(string username)
        {
            using var connection = Database.GetConnection();
            string query = "SELECT expertUser FROM user WHERE username = @Username";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", username);

            connection.Open();
            object result = cmd.ExecuteScalar();
            return result != null && Convert.ToInt32(result) == 1;
        }



        public bool RegisterUser(string username, string password)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string checkUserQuery = "SELECT COUNT(*) FROM user WHERE username = @Username";
                        MySqlCommand checkUserCmd = new MySqlCommand(checkUserQuery, connection, transaction);
                        checkUserCmd.Parameters.AddWithValue("@Username", username);

                        int userCount = Convert.ToInt32(checkUserCmd.ExecuteScalar());
                        if (userCount > 0)
                        {
                            return false; 
                        }

                        string balanceQuery = "INSERT INTO balance (cash, credit) VALUES (0, 0); SELECT LAST_INSERT_ID();";
                        MySqlCommand balanceCmd = new MySqlCommand(balanceQuery, connection, transaction);
                        int balanceID = Convert.ToInt32(balanceCmd.ExecuteScalar());

                        string userQuery = "INSERT INTO user (username, password, balance_balanceID, theme, language, expertUser) " +
                                           "VALUES (@Username, @Password, @BalanceID, 'custom', 'english', 0)";
                        MySqlCommand userCmd = new MySqlCommand(userQuery, connection, transaction);
                        userCmd.Parameters.AddWithValue("@Username", username);
                        userCmd.Parameters.AddWithValue("@Password", password);
                        userCmd.Parameters.AddWithValue("@BalanceID", balanceID);

                        int result = userCmd.ExecuteNonQuery();

                        transaction.Commit();

                        return result > 0;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static int GetBalanceIDForUser(string username)
        {
            using (var connection = Database.GetConnection())
            {
                string query = "SELECT balance_balanceID FROM user WHERE username = @Username";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);

                connection.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0; 
            }
        }

        public static int GetIDForUser(string username)
        {
            using (var connection = Database.GetConnection())
            {
                string query = "SELECT userID FROM user WHERE username = @Username";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);

                connection.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0; 
            }
        }


    }
}
