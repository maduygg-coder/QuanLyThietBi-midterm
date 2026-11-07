using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLyThietBi.Entities
{
    [Table("LoaiThietBi")]
    public class LoaiThietBi
    {
        [Key, Required, StringLength(10)]
        public string MaLoai { get; set; }
        [Required, StringLength(100)]
        public string TenLoai { get; set; }
        public virtual ICollection<ThietBi> ThietBis { get; set; } = new HashSet<ThietBi>();
    }
}
