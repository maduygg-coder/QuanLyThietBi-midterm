using System;
using System.Data.Entity.Migrations;
using System.Linq;
using QuanLyThietBi.Entities;
namespace QuanLyThietBi.DAL.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<ThietBiContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled=false;
            AutomaticMigrationDataLossAllowed=false;
            CommandTimeout=30;
            ContextKey="QuanLyThietBi.DAL.ThietBiContext";
            MigrationsNamespace="QuanLyThietBi.DAL.Migrations";
        }
        protected override void Seed(ThietBiContext db)
        {
         
            bool taoMau = !db.LoaiThietBis.Any() && !db.ThietBis.Any();
            using (var transaction=db.Database.BeginTransaction())
            {
                foreach (var item in new[]
                {
                    new LoaiThietBi { MaLoai="TRINHCHIEU", TenLoai="Thiết bị trình chiếu" },
                    new LoaiThietBi { MaLoai="MAYTINH", TenLoai="Thiết bị máy tính" },
                    new LoaiThietBi { MaLoai="AMTHANH", TenLoai="Thiết bị âm thanh" },
                    new LoaiThietBi { MaLoai="MANG", TenLoai="Thiết bị mạng" },
                })
                    if (!db.LoaiThietBis.Any(x => x.MaLoai == item.MaLoai)) db.LoaiThietBis.Add(item);
                db.SaveChanges();
                if (taoMau)
                {
                    var today=DateTime.Today;
                    db.ThietBis.AddRange(new[]
                    {
                        new ThietBi { MaTB="TB001", TenTB="Máy chiếu Epson EB-X06", MaLoai="TRINHCHIEU", NgayMua=today.AddDays(-20), SoLuong=10, DonGia=18500000m },
                        new ThietBi { MaTB="TB002", TenTB="Máy tính Dell OptiPlex", MaLoai="MAYTINH", NgayMua=today.AddDays(-15), SoLuong=25, DonGia=15900000m },
                        new ThietBi { MaTB="TB003", TenTB="Bộ phát Wi-Fi TP-Link", MaLoai="MAYTINH", NgayMua=today.AddDays(-10), SoLuong=12, DonGia=1450000m },
                        new ThietBi { MaTB="TB004", TenTB="Loa trợ giảng", MaLoai="AMTHANH", NgayMua=today.AddDays(-5), SoLuong=8, DonGia=2800000m },
                        new ThietBi { MaTB="TB005", TenTB="Màn chiếu treo tường", MaLoai="TRINHCHIEU", NgayMua=today, SoLuong=5, DonGia=4200000m }
                    });
                    db.SaveChanges();
                }
                transaction.Commit();
            }
        }
    }
}
