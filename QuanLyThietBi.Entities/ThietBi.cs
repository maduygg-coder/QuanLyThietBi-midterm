using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLyThietBi.Entities
{
    [Table("ThietBi")]
    public class ThietBi
    {
        [Key, Required, StringLength(20)]
        public string MaTB { get; set; }
        [Required, StringLength(150)]
        public string TenTB { get; set; }
        public DateTime NgayMua { get; set; }
        public int SoLuong { get; set; }
        [Column(TypeName = "decimal")]
        public decimal DonGia { get; set; }
        [Required, StringLength(10)]
        public string MaLoai { get; set; }
        [ForeignKey(nameof(MaLoai))]
        public virtual LoaiThietBi LoaiThietBi { get; set; }
    }
}
