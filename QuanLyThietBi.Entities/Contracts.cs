using System;
using System.Collections.Generic;
namespace QuanLyThietBi.Entities
{
    public sealed class ThietBiInput
    {
        public string MaTB { get; set; }
        public string TenTB { get; set; }
        public DateTime NgayMua { get; set; }
        public string SoLuong { get; set; }
        public string DonGia { get; set; }
        public string MaLoai { get; set; }
    }
    public sealed class ThietBiRow
    {
        public string MaTB { get; set; }
        public string TenTB { get; set; }
        public DateTime NgayMua { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public string MaLoai { get; set; }
        public string TenLoai { get; set; }
    }
    public interface IThietBiRepository
    {
        List<ThietBiRow> LayTatCa();
        bool TonTai(string maTB);
        void Them(ThietBi entity);
        void Sua(string maGoc, ThietBi entity);
        void Xoa(string maTB);
    }
    public interface ILoaiThietBiRepository
    {
        List<LoaiThietBi> LayTatCa();
        bool TonTai(string maLoai);
    }
}
