using Microsoft.Data.Sqlite;

namespace WinFormsApp1.Database
{
    public class ScuolaDatabase
    {

        static string folder = Path.Combine(
            Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
            "Database");

        static string dbPath = Path.Combine(folder, "scuola.db3");

        static string connectionString = $"Data Source={dbPath}";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }

        public static void InitializeDatabase()
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (var conn = GetConnection())
            {
                conn.Open();

                string query = @"CREATE TABLE IF NOT EXISTS Users (
                                Id INTEGER NOT NULL,
                                UserName TEXT UNIQUE,
                                Password TEXT,
                                Email TEXT,
                                Ruolo TEXT,
                                PRIMARY KEY(""Id"" AUTOINCREMENT))";

                var cmd = new SqliteCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool UserExists(string username)
        {
            using (SqliteConnection conn = GetConnection())
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM Users WHERE UserName=@user";

                SqliteCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@user", username);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
        }
    }
}
