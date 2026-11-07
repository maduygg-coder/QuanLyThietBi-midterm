# Kết quả kiểm tra và nghiệm thu trên Windows

## Đã kiểm tra thực tế trong môi trường Linux

- Biên dịch Entities, BLL, DAL, GUI bằng Roslyn từ .NET SDK 8.0.427, reference assemblies chính thức .NET Framework 4.8 và Entity Framework 6.5.1. Bốn project biên dịch thành công, không cảnh báo trong lần biên dịch ứng dụng cuối.
- Biên dịch và chạy console test BLL trên .NET 8: **60/60 kiểm tra đạt**. Bao gồm Trim, trùng mã, chuỗi rỗng/độ dài, loại không tồn tại, ngày tương lai/ngoài SQL datetime, lỗi định dạng/số không dương, biên Int32/decimal, khóa chính bất biến, sửa/xóa không tồn tại, chuyển tiếp lỗi DAL và quy tắc test/save cấu hình.
- Log nguyên văn: docs/KetQuaTest.txt.
- Sinh model bằng EdmxWriter của EF6 từ class và DbContext thực tế; snapshot XML chỉ là metadata Code First, không dùng Database First. Snapshot nén cố định được lưu trong migration Designer.
- Đã gọi MigratorScriptingDecorator.ScriptUpdate từ 0 tới InitialCreate và sinh SQL offline: 2 bảng nghiệp vụ, PK/FK, index MaLoai và DonGia decimal(18,2). Script tham khảo tại docs/Migration_ThamKhao.sql; không cần chạy thủ công, không bao gồm seed C#.
- Kiểm tra DAL lưu/đọc cấu hình và DbContext mới nhận đúng server/database vừa đổi, không mở kết nối SQL thật trong kiểm tra này.
- Kiểm tra XML project/config, project references, cấu trúc model, ràng buộc độ dài/precision, snapshot nén và phân tầng.

## Giới hạn kiểm chứng

Chưa chạy Visual Studio/WinForms và SQL Server/LocalDB trên Windows ở môi trường này. Chưa xác nhận migration, seed, CRUD và persistence với database thật. Test repository giả và script SQL offline không thay cho kiểm thử tích hợp. Source được kiểm tra bằng compiler trực tiếp; chưa chạy Restore/Build Solution qua Visual Studio thực tế.

Công cụ kiểm tra model offline chạy EF bản netstandard trên .NET 8 và có cảnh báo tương thích tham chiếu System.Runtime 6/8 của ConfigurationManager; công cụ chạy thành công. Đây là công cụ kiểm tra trung gian, không phải project GUI net48 giao cho người dùng.

## Checklist Windows trước khi dùng/nộp bài

| Thao tác | Kết quả cần đạt |
|---|---|
| Cài .NET desktop development, targeting pack 4.8, LocalDB | Bốn project tải được, không còn incompatible |
| Restore, Build, đặt GUI startup, F5 | Form giữa màn hình, đúng tiêu đề và 6 cột |
| Database mới | 3 loại và 5 thiết bị mẫu được nạp; chọn loại hiện tên |
| Load/Mới | Ngày hôm nay, ô nhập sạch, loại/dòng chưa chọn, focus txtMaTB; Nhập bật, Sửa/Xóa tắt |
| Chọn TB001 | Đủ thông tin; mã khóa; Nhập tắt, Sửa/Xóa bật |
| Nhập TB006 hợp lệ, đóng/mở lại | Dữ liệu giữ trong SQL Server |
| Nhập mã ` TB006 ` lần nữa | Báo trùng, không lưu |
| Tên/mã trắng hoặc chưa chọn loại | Báo lỗi, giữ nội dung nhập |
| Ngày mai; số lượng 0/-1/chữ/1,5; giá 0/-1/chữ/0,001 | Báo lỗi, không lưu |
| Đơn giá 18500000,50 | Lưu đúng phần thập phân, lưới hiện vi-VN |
| Sửa TB006 | Không đổi mã; cập nhật đúng một dòng |
| Xóa chọn Không / chọn Có | Không giữ nguyên; Có xóa và không xuất hiện lại sau mở lại |
| Enter | Thực hiện Nhập khi nút đang bật; vẫn kiểm tra dữ liệu |
| Esc/X/nút Thoát/Alt+F4 | Một hộp xác nhận mỗi lần; Không giữ Form |
| SQL server sai hoặc chưa cài | Hiện lỗi và có thể cấu hình/thử lại; không báo lưu thành công |
| Cấu hình server đúng, Kết nối và lưu | Tự nạp lại, context mới nhận cấu hình mới |
| Đang nạp/ghi rồi bấm thao tác khác | Các nút cạnh tranh bị khóa, cửa sổ vẫn phản hồi |
| Đóng Form khi đang nạp | Không cập nhật control đã Dispose |
| Lưu xong nhưng mất kết nối khi nạp lại | Thông báo đã lưu và yêu cầu tải lại, không hướng dẫn nhập lặp |
| Xem schema trong SQL Server | Đúng MaTB20, TenTB150, MaLoai10, TenLoai100, decimal(18,2), PK/FK/index và MigrationHistory |

Migration đang cung cấp: `202611070753220_InitialCreate`. ID được tạo khi dựng migration; commit Git dùng thời điểm lưu thực tế. File/thư mục sau khi copy/giải nén có thể có timestamp khác theo môi trường.
