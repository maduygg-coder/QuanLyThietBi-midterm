using System.Data.Entity.Migrations;
namespace QuanLyThietBi.DAL.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable("dbo.LoaiThietBi", c => new
            {
                MaLoai=c.String(nullable:false, maxLength:10),
                TenLoai=c.String(nullable:false, maxLength:100)
            }).PrimaryKey(t => t.MaLoai);
            CreateTable("dbo.ThietBi", c => new
            {
                MaTB=c.String(nullable:false, maxLength:20),
                TenTB=c.String(nullable:false, maxLength:150),
                NgayMua=c.DateTime(nullable:false),
                SoLuong=c.Int(nullable:false),
                DonGia=c.Decimal(nullable:false, precision:18, scale:2),
                MaLoai=c.String(nullable:false, maxLength:10)
            }).PrimaryKey(t => t.MaTB)
              .ForeignKey("dbo.LoaiThietBi", t => t.MaLoai, cascadeDelete:false)
              .Index(t => t.MaLoai);
        }
        public override void Down()
        {
            DropForeignKey("dbo.ThietBi", "MaLoai", "dbo.LoaiThietBi");
            DropIndex("dbo.ThietBi", new[] { "MaLoai" });
            DropTable("dbo.ThietBi");
            DropTable("dbo.LoaiThietBi");
        }
    }
}
