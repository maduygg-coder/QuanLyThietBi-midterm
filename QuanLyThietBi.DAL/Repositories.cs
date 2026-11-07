using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using QuanLyThietBi.Entities;
namespace QuanLyThietBi.DAL
{
    public sealed class ThietBiRepository : IThietBiRepository
    {
        public List<ThietBiRow> LayTatCa()
        {
            using (var db = new ThietBiContext())
                return db.ThietBis.AsNoTracking().OrderBy(x => x.MaTB).Select(x => new ThietBiRow
                {
                    MaTB=x.MaTB, TenTB=x.TenTB, NgayMua=x.NgayMua, SoLuong=x.SoLuong,
                    DonGia=x.DonGia, MaLoai=x.MaLoai, TenLoai=x.LoaiThietBi.TenLoai
                }).ToList();
        }
        public bool TonTai(string maTB)
        {
            using (var db = new ThietBiContext()) return db.ThietBis.Any(x => x.MaTB == maTB);
        }
        public void Them(ThietBi entity)
        {
            using (var db = new ThietBiContext()) { db.ThietBis.Add(entity); db.SaveChanges(); }
        }
        public void Sua(string maGoc, ThietBi entity)
        {
            using (var db = new ThietBiContext())
            {
                var current = db.ThietBis.Find(maGoc);
                if (current == null) throw new InvalidOperationException("Thiết bị đã bị xóa. Hãy tải lại danh sách.");
                current.TenTB=entity.TenTB; current.NgayMua=entity.NgayMua;
                current.SoLuong=entity.SoLuong; current.DonGia=entity.DonGia; current.MaLoai=entity.MaLoai;
                db.SaveChanges();
            }
        }
        public void Xoa(string maTB)
        {
            using (var db = new ThietBiContext())
            {
                var entity=db.ThietBis.Find(maTB);
                if (entity == null) throw new InvalidOperationException("Thiết bị không còn tồn tại.");
                db.ThietBis.Remove(entity); db.SaveChanges();
            }
        }
    }
    public sealed class LoaiThietBiRepository : ILoaiThietBiRepository
    {
        public List<LoaiThietBi> LayTatCa()
        {
            using (var db = new ThietBiContext()) return db.LoaiThietBis.AsNoTracking().OrderBy(x => x.TenLoai).ToList();
        }
        public bool TonTai(string maLoai)
        {
            using (var db = new ThietBiContext()) return db.LoaiThietBis.Any(x => x.MaLoai == maLoai);
        }
    }
}
