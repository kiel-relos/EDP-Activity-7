using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace PropLytics
{
    public class DatabaseConnection
    {
        // UPDATE THESE with your actual MySQL credentials
       private string connectionString = "Server=localhost;Database=info_sys;Uid=root;Pwd=;";
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Connection Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}