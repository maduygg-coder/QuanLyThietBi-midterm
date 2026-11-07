# Quản lý thiết bị phòng học — đề giữa kỳ mã 03

Windows Forms, .NET Framework 4.8, Entity Framework 6.5.1 Code First, SQL Server, mô hình Three-Layer. Bộ mã nguồn dành riêng cho hai ảnh đề thiết bị đã cung cấp.

## Chạy trên Windows

1. Visual Studio 2022/2026 → Visual Studio Installer → Modify → cài workload **.NET desktop development**, .NET Framework 4.8 targeting pack/SDK và **SQL Server Express LocalDB**. Có thể dùng SQL Server Express/SQL Server khác đang chạy.
2. Giải nén, mở **QuanLyThietBi.sln**, Restore NuGet Packages nếu Visual Studio chưa tự restore.
3. Đặt **QuanLyThietBi.GUI** làm Startup Project, Build Solution rồi nhấn F5. GUI đã được xếp đầu solution để ưu tiên chọn khi mở mới.
4. Chuỗi kết nối mặc định trong `QuanLyThietBi.GUI/App.config`: máy chủ `(localdb)\MSSQLLocalDB`, database **QuanLyThietBiPhongHoc**. Database này độc lập với bài sách trước đó.
5. Sau khi cửa sổ hiện, ứng dụng kiểm tra máy chủ, áp dụng Migration, seed dữ liệu rồi nạp ComboBox và DataGridView. Database mới có 3 loại thiết bị và 5 thiết bị mẫu.
6. Chọn một dòng, ví dụ **TB001**, để hiện thông tin như hình đề. Mới đưa về trạng thái nhập mới, chưa chọn loại/dòng, ngày hiện tại, focus mã.

Tiêu đề hiện **Quản lý thiết bị – SV001 – Ma Đức Duy**. SV001 là mã ví dụ, cần thay bằng mã sinh viên thật trong App.config. Họ tên cũng có thể sửa tại `HoTenSinhVien`.

## Nếu không có thể loại hoặc thiết bị

Đợi thông báo ở thanh trạng thái. Nếu báo lỗi, bấm **Cấu hình CSDL** ở cuối cửa sổ:

- Máy chủ LocalDB: `(localdb)\MSSQLLocalDB`.
- Máy chủ SQL Express: `.\SQLEXPRESS` (chỉ dùng nếu máy thật sự có instance này).
- Database: `QuanLyThietBiPhongHoc` hoặc tên database mới phù hợp.
- Bấm **Kết nối và lưu**; kiểm tra được máy chủ mới lưu cấu hình và tự nạp lại.

Cấu hình giữ kiểu đăng nhập đã có trong App.config, mặc định tài khoản Windows. Tài khoản cần quyền tạo database lần đầu. Chọn tên database khác nếu đã có database cùng tên với schema không thuộc bài này; không xóa database có dữ liệu để khắc phục lỗi.

Để kiểm tra LocalDB trong Command Prompt:

```bat
sqllocaldb info
sqllocaldb start MSSQLLocalDB
```

Nếu chưa có instance MSSQLLocalDB nhưng LocalDB đã cài:

```bat
sqllocaldb create MSSQLLocalDB -s
```

SQL Server/LocalDB là thành phần phải cài trên máy Windows. Ứng dụng không tạo dữ liệu giả thay cho SQL và không tự cài SQL Server.

## Dùng các nút theo đề

- **Mới:** làm sạch nhập liệu, bỏ chọn dòng/loại, ngày mua hôm nay; bật Nhập, tắt Sửa/Xóa, mở khóa mã.
- **Nhập:** thêm thiết bị qua BLL → DAL. Enter kích hoạt khi nút Nhập bật.
- **Sửa:** chọn dòng trước; mã giữ nguyên; chỉ cập nhật thông tin không khóa.
- **Xóa:** chọn dòng, xác nhận Có/Không. Không hủy thao tác trước khi gọi BLL.
- **Thoát:** nút Thoát, X, Esc hoặc Alt+F4 đều đi qua một FormClosing, xác nhận đúng một lần.
- **Tải lại:** kiểm tra/kết nối và nạp lại dữ liệu bất đồng bộ. Cấu hình CSDL và Tải lại là hai nút hỗ trợ thêm ở chân Form, ngoài 5 nút chính theo đề.

Trong khi đang xử lý SQL, các nút ghi và vùng nhập bị khóa để tránh hai thao tác cùng lúc. Sau thao tác, giao diện được mở lại. Nếu database đã lưu nhưng tải lại lưới lỗi, thông báo phân biệt rõ để tránh thêm trùng.

Đơn giá nhập dạng `18500000` hoặc `18500000,50`, không dùng dấu phân cách hàng nghìn ở ô nhập; lưới hiển thị định dạng vi-VN. Số lượng phải là số nguyên dương. Ngày mua không được sau hôm nay.

## Cấu trúc source

```text
QuanLyThietBi.GUI      FrmThietBi, FrmDatabaseSettings, Program, App.config
          ↓
QuanLyThietBi.BLL      ThietBiService, DatabaseSetupService
          ↓ qua repository interfaces
QuanLyThietBi.DAL      Repositories, ThietBiContext, Migration, ConnectionSettingsStore
          ↓
SQL Server

QuanLyThietBi.Entities LoaiThietBi, ThietBi, DTO và interfaces dùng chung
QuanLyThietBi.Tests    Kiểm tra BLL với repository giả, không dùng SQL
```

GUI không tạo DbContext/DbSet và không gọi SaveChanges. Program.cs chỉ nối dependency. DAL không tham chiếu WinForms. Các thao tác Nhập/Sửa/Xóa từ Form luôn đi qua ThietBiService.

## Mô hình và Migration

- LoaiThietBi: MaLoai nvarchar(10) PK; TenLoai nvarchar(100) bắt buộc.
- ThietBi: MaTB nvarchar(20) PK; TenTB nvarchar(150) bắt buộc; NgayMua datetime; SoLuong int; DonGia decimal(18,2); MaLoai nvarchar(10) FK bắt buộc.
- Quan hệ một loại có nhiều thiết bị. Navigation: LoaiThietBi.ThietBis và ThietBi.LoaiThietBi.
- Entity dùng Key, Required, StringLength, Table, Column, ForeignKey. EF6 chưa có Precision attribute nên precision/scale được chốt thêm bằng HasPrecision(18,2) và migration.
- AutomaticMigrationsEnabled=false; DbMigrator.Update áp dụng migration khai báo sẵn. Snapshot EDMX là metadata Code First của migration, không phải mô hình thiết kế Database First.
- Seed chạy trong một transaction: bổ sung 3 loại theo mã nếu thiếu; chỉ tạo 5 thiết bị mẫu khi cả hai bảng trống từ đầu. Không ghi đè thiết bị đã sửa hoặc khôi phục thiết bị đã xóa.

Package Manager Console, Startup Project là GUI:

```powershell
Update-Database -ProjectName QuanLyThietBi.DAL -StartUpProjectName QuanLyThietBi.GUI -Verbose
```

Khi thay đổi entity về sau:

```powershell
Add-Migration TenThayDoi -ProjectName QuanLyThietBi.DAL -StartUpProjectName QuanLyThietBi.GUI
Update-Database -ProjectName QuanLyThietBi.DAL -StartUpProjectName QuanLyThietBi.GUI -Verbose
```

## Cấu hình runtime và kiểm thử

Cấu hình đổi từ Form được lưu vào file `QuanLyThietBi.GUI.exe.config` cạnh EXE. Build lại có thể chép App.config nguồn đè file này; sửa cả nguồn nếu muốn giữ cấu hình cho các lần build. DAL đọc lại ConfigurationManager khi tạo context; không ghi trực tiếp chuỗi kết nối trong code.

```powershell
dotnet build QuanLyThietBi.sln -c Release
dotnet run --project QuanLyThietBi.Tests/QuanLyThietBi.Tests.csproj
```

Chạy test console cần .NET 8 SDK. Xem **KIEM_THU.md** cho kết quả thực tế và giới hạn; **HOC_VA_VAN_DAP.md** để học từng phần, truy vết chức năng và luyện trả lời câu hỏi.

Form được dựng bằng C# trong `FrmThietBi.TaoGiaoDien()`, không có WinForms Designer kéo thả. Nhấp đúp FrmThietBi.cs để đọc code; ứng dụng hiện giao diện khi F5.
