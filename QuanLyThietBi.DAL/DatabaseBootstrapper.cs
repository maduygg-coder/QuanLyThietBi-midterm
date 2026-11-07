using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
namespace QuanLyThietBi.DAL
{
    public static class DatabaseBootstrapper
    {
        public static void Initialize(Action<string> report)
        {
            report?.Invoke("Bước 1/3: Kiểm tra SQL Server…");
            try { ConnectionSettingsStore.CheckConfiguredServer(); }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không kết nối được SQL Server. Bấm Cấu hình CSDL để chọn máy chủ. " +
                    "Nếu chưa có LocalDB, cài SQL Server Express LocalDB trong Visual Studio Installer. " + ex.GetBaseException().Message, ex);
            }
            report?.Invoke("Bước 2/3: Áp dụng Migration và dữ liệu mẫu…");
            var configuration = new Migrations.Configuration
            {
                TargetDatabase = new DbConnectionInfo(ConnectionSettingsStore.ReadConnectionString(), "System.Data.SqlClient")
            };
            new DbMigrator(configuration).Update();
            report?.Invoke("Bước 3/3: Nạp loại thiết bị và danh sách…");
        }
    }
}
