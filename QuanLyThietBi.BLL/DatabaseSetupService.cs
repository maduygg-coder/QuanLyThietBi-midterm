using System;
using QuanLyThietBi.Entities;
namespace QuanLyThietBi.BLL
{
    public sealed class DatabaseSetupService
    {
        private readonly IConnectionSettingsStore settings;
        public DatabaseSetupService(IConnectionSettingsStore settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        public DatabaseConnectionOptions Read() => settings.Read();
        public void TestAndSave(DatabaseConnectionOptions input)
        {
            if (input == null) throw new BusinessException("Vui lòng nhập cấu hình kết nối.");
            var options = new DatabaseConnectionOptions
            {
                Server = (input.Server ?? "").Trim(),
                Database = (input.Database ?? "").Trim()
            };
            if (options.Server.Length == 0) throw new BusinessException("Vui lòng nhập tên máy chủ SQL Server.");
            if (options.Server.Length > 255) throw new BusinessException("Tên máy chủ SQL Server quá dài.");
            if (options.Database.Length == 0) throw new BusinessException("Vui lòng nhập tên database.");
            if (options.Database.Length > 128) throw new BusinessException("Tên database tối đa 128 ký tự.");
           
            settings.TestServer(options);
            settings.Save(options);
        }
    }
}
