using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace HCI_PROJEKA_1
{
    public class ReportModel
    {
        public int ReportID { get; set; }
        public string ReportName { get; set; }

        public static void SavePdfToDatabase(byte[] pdfBytes, string reportName)
        {
            try
            {
                
                int userId = UserModel.GetIDForUser(LoginViewModel.LoggedInUsername); 

                using (var conn = Database.GetConnection())
                {
                    string insertQuery = "INSERT INTO report (reportName, user_userID) VALUES (@reportName, @userId); SELECT LAST_INSERT_ID();";
                    conn.Open();

                    using (MySqlCommand insertCmd = new(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@reportName", reportName);
                        insertCmd.Parameters.AddWithValue("@userId", userId);

                        int reportId = Convert.ToInt32(insertCmd.ExecuteScalar());

                        string updateQuery = "UPDATE report SET pdfFile = @pdfFile WHERE reportID = @reportId";
                        using (MySqlCommand updateCmd = new(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@pdfFile", pdfBytes);
                            updateCmd.Parameters.AddWithValue("@reportId", reportId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }

      
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the report: {ex.Message}");
            }
        }

        public static void GetPdfFromDatabase(int reportId)
        {
            try
            {

                using (var conn = Database.GetConnection())
                {
                    string selectQuery = "SELECT pdfFile FROM report WHERE reportID = @reportId";
                    conn.Open();

                    using (MySqlCommand cmd = new(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@reportId", reportId);

                        byte[] pdfBytes = cmd.ExecuteScalar() as byte[];

                        if (pdfBytes != null)
                        {
                            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Report_{reportId}.pdf");
                            File.WriteAllBytes(filePath, pdfBytes);
                            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                        }
                        else
                        {
                            MessageBox.Show("No PDF found for the specified report.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while retrieving the report: {ex.Message}");
            }
        }

        public static List<ReportModel> GetAllReports(int userID)
        {
      
            List<ReportModel> reports = new();

            using (var conn = Database.GetConnection())
            {
                string selectQuery = "SELECT reportID, reportName, pdfFile FROM report WHERE user_userID = @userID";
                conn.Open();

                using (MySqlCommand cmd = new(selectQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var report = new ReportModel
                            {
                                ReportID = reader.GetInt32("reportID"),
                                ReportName = reader.GetString("reportName"),
                            };
                            reports.Add(report);
                        }
                    }
                }
            }

            return reports;

        }


        public static void DeleteReport(int reportId)
        {
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string deletePdfQuery = "UPDATE report SET pdfFile = NULL WHERE reportID = @reportId";
                        using (MySqlCommand deletePdfCmd = new(deletePdfQuery, conn, transaction))
                        {
                            deletePdfCmd.Parameters.AddWithValue("@reportId", reportId);
                            deletePdfCmd.ExecuteNonQuery();
                        }

                        string deleteReportQuery = "DELETE FROM report WHERE reportID = @reportId";
                        using (MySqlCommand deleteReportCmd = new(deleteReportQuery, conn, transaction))
                        {
                            deleteReportCmd.Parameters.AddWithValue("@reportId", reportId);
                            deleteReportCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the report: {ex.Message}");
            }
        }

    }
}



