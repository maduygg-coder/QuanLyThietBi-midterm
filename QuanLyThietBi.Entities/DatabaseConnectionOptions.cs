namespace QuanLyThietBi.Entities
{
    public sealed class DatabaseConnectionOptions
    {
        public string Server { get; set; }
        public string Database { get; set; }
    }
    public interface IConnectionSettingsStore
    {
        DatabaseConnectionOptions Read();
        void TestServer(DatabaseConnectionOptions options);
        void Save(DatabaseConnectionOptions options);
    }
}
