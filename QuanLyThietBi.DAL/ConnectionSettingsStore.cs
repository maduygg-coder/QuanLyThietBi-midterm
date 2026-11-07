using System;
using System.Configuration;
using System.Data.SqlClient;
using QuanLyThietBi.Entities;
namespace QuanLyThietBi.DAL
{
    public sealed class ConnectionSettingsStore : IConnectionSettingsStore
    {
        private const string Key = "ThietBiDb";
        internal static string ReadConnectionString()
        {
            var entry = ConfigurationManager.ConnectionStrings[Key];
            if (entry == null || string.IsNullOrWhiteSpace(entry.ConnectionString))
                throw new InvalidOperationException("Không tìm thấy chuỗi kết nối ThietBiDb trong tệp cấu hình ứng dụng.");
            if (!string.Equals(entry.ProviderName, "System.Data.SqlClient", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Provider của ThietBiDb phải là System.Data.SqlClient.");
            
            var builder = new SqlConnectionStringBuilder(entry.ConnectionString)
            {
                ConnectTimeout = 5,
                ConnectRetryCount = 0
            };
            return builder.ConnectionString;
        }
        private static SqlConnectionStringBuilder Build(DatabaseConnectionOptions options)
        {
            return new SqlConnectionStringBuilder(ReadConnectionString())
            {
                DataSource = options.Server,
                InitialCatalog = options.Database
            };
        }
        public DatabaseConnectionOptions Read()
        {
            var builder = new SqlConnectionStringBuilder(ReadConnectionString());
            return new DatabaseConnectionOptions { Server = builder.DataSource, Database = builder.InitialCatalog };
        }
        public void TestServer(DatabaseConnectionOptions options)
        {
            var builder = Build(options);
       
            builder.InitialCatalog = "master";
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT 1";
                    command.CommandTimeout = 5;
                    command.ExecuteScalar();
                }
            }
        }
        public void Save(DatabaseConnectionOptions options)
        {
            var connectionString = Build(options).ConnectionString;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var entry = config.ConnectionStrings.ConnectionStrings[Key];
            if (entry == null) throw new InvalidOperationException("Thiếu ThietBiDb trong tệp cấu hình ứng dụng.");
            entry.ConnectionString = connectionString;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
        internal static void CheckConfiguredServer()
        {
            var store = new ConnectionSettingsStore();
            store.TestServer(store.Read());
        }
    }
}
