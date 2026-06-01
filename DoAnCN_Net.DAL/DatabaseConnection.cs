using System.Data.SqlClient;

namespace DoAnCN_Net.DAL
{
    public static class DatabaseConnection
    {
        private static string GetConnectionString()
        {
            return "Data Source=DESKTOP-IGQEI1S\\SQLEXPRESS;" +
                   "Initial Catalog=Adora;" +
                   "Integrated Security=True;" +
                   "TrustServerCertificate=True";
        }

        public static SqlConnection GetConnection()
        {
            try
            {
                var connection = new SqlConnection(GetConnectionString());
                connection.Open();
                return connection;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Không thể kết nối database: " + ex.Message);
            }
        }
    }
}