using MySql.Data.MySqlClient;

namespace PropLytics
{
    public static class DatabaseConnection
    {
        private static string connectionString =
            "server=localhost;user=root;password=;database=info_sys;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}