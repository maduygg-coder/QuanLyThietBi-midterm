# Đối chiếu hai trang đề mã 03 — Quản lý thiết bị

| Yêu cầu | Triển khai |
|---|---|
| GUI trên, nút bên phải, danh sách dưới | FrmThietBi.TaoGiaoDien |
| Tiêu đề tên sinh viên, giữa màn hình | App.config, StartPosition.CenterScreen |
| Loại/Mã/Tên/Ngày mua/Số lượng/Đơn giá | cboLoaiThietBi, txtMaTB, txtTenTB, dtpNgayMua, txtSoLuong, txtDonGia |
| Lưới gồm 6 cột | dgvThietBi: MaTB, TenTB, NgayMua, SoLuong, DonGia, TenLoai |
| Mới, Nhập, Sửa, Xóa, Thoát | Đúng 5 nút chính, thêm 2 nút hỗ trợ kết nối dưới Form |
| Tab, Enter, Esc | TabIndex, AcceptButton=btnNhap, CancelButton=btnThoat |
| Load/Mới xóa nhập liệu, bỏ chọn, focus mã | NapDuLieuAsync, LamMoi, CapNhatNut |
| Chọn dòng: khóa mã, Nhập tắt, Sửa/Xóa bật | ChonDong, CapNhatNut |
| Entity và annotation đúng độ dài | LoaiThietBi.cs, ThietBi.cs; TenTB 150 ký tự theo xác nhận |
| Một loại nhiều thiết bị | Navigation hai chiều và FK bắt buộc |
| decimal(18,2) | Column annotation, HasPrecision, InitialCreate |
| App.config và SQL Server | ConnectionSettingsStore, ThietBiContext |
| Migration, không Database First | InitialCreate Up/Down + snapshot Code First, DbMigrator.Update |
| Tối thiểu ba project chức năng | GUI, BLL, DAL; Entities dùng chung |
| GUI → BLL → DAL | LuuAsync/XoaAsync → ThietBiService → repository |
| Không trống, không trùng mã sau Trim | ThietBiService.KiemTra/Nhap |
| Ngày mua không tương lai | KiemTra so sánh .Date với ngày hệ thống |
| Số lượng nguyên > 0, giá > 0 | TryParse và giới hạn kiểu dữ liệu tại BLL |
| Sửa không đổi PK | BLL so mã gốc; DAL không gán MaTB |
| Lưu xong tải lại; mở lại còn dữ liệu | SaveChanges và truy vấn SQL Server; không dùng lưu tạm trong RAM |
| Xác nhận trước xóa/thoát, không hỏi hai lần | XoaAsync và duy nhất XacNhanThoat ở FormClosing |
| Không báo thành công khi ghi lỗi | Chỉ báo sau write thành công; phân biệt write lỗi và reload lỗi |

Khởi tạo database/nạp dữ liệu bắt đầu ở Shown và chạy nền để cửa sổ hiện ngay. Trạng thái cuối khi nạp xong tương ứng yêu cầu Form Load; nếu SQL chưa hoạt động, Form báo lỗi và tắt thao tác ghi thay vì hiển thị dữ liệu giả.

Bảng này ghi vị trí thực hiện, không phải kết quả chấm điểm hay bằng chứng đã kiểm thử WinForms/SQL Server trên Windows. Dữ liệu mẫu có tên và giá để minh họa; ngày mua tính từ ngày hiện tại cho hợp lệ, không cố định theo ảnh.
