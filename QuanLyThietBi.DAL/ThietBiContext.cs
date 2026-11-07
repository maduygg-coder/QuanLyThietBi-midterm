using System.Data.Entity;
using System.Data.SqlClient;
using QuanLyThietBi.Entities;
namespace QuanLyThietBi.DAL
{
    public class ThietBiContext : DbContext
    {
        static ThietBiContext() { Database.SetInitializer<ThietBiContext>(null); }
        public ThietBiContext() : base(new SqlConnection(ConnectionSettingsStore.ReadConnectionString()), true)
        {
            Database.CommandTimeout = 30;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
        public DbSet<LoaiThietBi> LoaiThietBis { get; set; }
        public DbSet<ThietBi> ThietBis { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<ThietBi>().Property(x => x.DonGia).HasPrecision(18, 2);
            modelBuilder.Entity<ThietBi>().HasRequired(x => x.LoaiThietBi)
                .WithMany(x => x.ThietBis).HasForeignKey(x => x.MaLoai).WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }
    }
}
