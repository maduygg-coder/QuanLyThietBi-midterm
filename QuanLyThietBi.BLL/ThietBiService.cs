using System;
using System.Collections.Generic;
using System.Globalization;
using QuanLyThietBi.Entities;
namespace QuanLyThietBi.BLL
{
    public sealed class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
    public sealed class ThietBiService
    {
        private readonly IThietBiRepository thietBi;
        private readonly ILoaiThietBiRepository loai;
        private readonly Func<DateTime> ngayHienTai;
        private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");
        public ThietBiService(IThietBiRepository thietBi, ILoaiThietBiRepository loai, Func<DateTime> ngayHienTai = null)
        {
            this.thietBi = thietBi ?? throw new ArgumentNullException(nameof(thietBi));
            this.loai = loai ?? throw new ArgumentNullException(nameof(loai));
            this.ngayHienTai = ngayHienTai ?? (() => DateTime.Today);
        }
        public List<ThietBiRow> LayDanhSach() => thietBi.LayTatCa();
        public List<LoaiThietBi> LayLoaiThietBi() => loai.LayTatCa();
        private ThietBi KiemTra(ThietBiInput input)
        {
            if (input == null) throw new BusinessException("Vui lòng nhập thông tin thiết bị.");
            var ma = (input.MaTB ?? "").Trim();
            var ten = (input.TenTB ?? "").Trim();
            var maLoai = (input.MaLoai ?? "").Trim();
            if (ma.Length == 0) throw new BusinessException("Mã thiết bị không được để trống.");
            if (ma.Length > 20) throw new BusinessException("Mã thiết bị tối đa 20 ký tự.");
            if (ten.Length == 0) throw new BusinessException("Tên thiết bị không được để trống.");
            if (ten.Length > 150) throw new BusinessException("Tên thiết bị tối đa 150 ký tự.");
            if (maLoai.Length == 0) throw new BusinessException("Vui lòng chọn loại thiết bị.");
            if (maLoai.Length > 10 || !loai.TonTai(maLoai)) throw new BusinessException("Loại thiết bị không tồn tại. Hãy tải lại dữ liệu.");
            if (input.NgayMua.Date > ngayHienTai().Date) throw new BusinessException("Ngày mua không được lớn hơn ngày hiện tại.");
            if (input.NgayMua.Date < new DateTime(1753, 1, 1)) throw new BusinessException("Ngày mua phải từ 01/01/1753 trở đi.");
            if (!int.TryParse((input.SoLuong ?? "").Trim(), NumberStyles.Integer, Vi, out var soLuong) || soLuong <= 0)
                throw new BusinessException("Số lượng phải là số nguyên lớn hơn 0 và không vượt giới hạn Int32.");
            if (!decimal.TryParse((input.DonGia ?? "").Trim(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, Vi, out var donGia) || donGia <= 0)
                throw new BusinessException("Đơn giá phải lớn hơn 0. Nhập 15000000 hoặc 15000000,50; không nhập dấu phân cách hàng nghìn.");
            if (donGia > 9999999999999999.99m) throw new BusinessException("Đơn giá vượt giới hạn decimal(18,2).");
            if (decimal.Round(donGia, 2) != donGia) throw new BusinessException("Đơn giá chỉ được có tối đa 2 chữ số thập phân.");
            return new ThietBi { MaTB=ma, TenTB=ten, MaLoai=maLoai, NgayMua=input.NgayMua.Date, SoLuong=soLuong, DonGia=donGia };
        }
        public void Nhap(ThietBiInput input)
        {
            var entity = KiemTra(input);
            if (thietBi.TonTai(entity.MaTB)) throw new BusinessException("Mã thiết bị đã tồn tại.");
            thietBi.Them(entity);
        }
        public void Sua(string maGoc, ThietBiInput input)
        {
            var entity = KiemTra(input);
            if (string.IsNullOrWhiteSpace(maGoc) || !string.Equals(maGoc, entity.MaTB, StringComparison.Ordinal))
                throw new BusinessException("Không được thay đổi mã thiết bị khi sửa.");
            if (!thietBi.TonTai(maGoc)) throw new BusinessException("Thiết bị không còn tồn tại.");
            thietBi.Sua(maGoc, entity);
        }
        public void Xoa(string maTB)
        {
            var ma = (maTB ?? "").Trim();
            if (ma.Length == 0) throw new BusinessException("Vui lòng chọn thiết bị cần xóa.");
            if (!thietBi.TonTai(ma)) throw new BusinessException("Thiết bị không còn tồn tại.");
            thietBi.Xoa(ma);
        }
    }
}
