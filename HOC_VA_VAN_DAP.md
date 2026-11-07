# Lộ trình hiểu bài và ôn vấn đáp — Quản lý thiết bị phòng học

Đọc tài liệu cùng source, chạy các thao tác và tự giải thích lại bằng lời của mình. Những file sau đều nằm trong solution QuanLyThietBi.

## Bước 1 — Nói được bài toán trước khi xem code

Ứng dụng quản lý thiết bị theo loại. Một loại có nhiều thiết bị; mỗi thiết bị thuộc đúng một loại. Người dùng nhập mới, chọn dòng để sửa/xóa và dữ liệu được giữ trong SQL Server sau khi đóng ứng dụng.

Thực hành: mở Form, chọn TB001, chỉ ra mã, tên, loại, ngày mua, số lượng và đơn giá. Bấm Mới, giải thích tại sao loại bị bỏ chọn, mã được nhập lại và Sửa/Xóa bị tắt.

Tự kiểm tra: phân biệt **một dòng thiết bị** với **số lượng của dòng đó**. SoLuong=10 không có nghĩa là tạo 10 dòng trong lưới.

## Bước 2 — Hiểu Entities và quan hệ

Mở `QuanLyThietBi.Entities/LoaiThietBi.cs`, `ThietBi.cs`, `Contracts.cs`.

- Key xác định MaLoai/MaTB là khóa chính, không trùng giữa các bản ghi cùng bảng.
- StringLength chốt độ dài chuỗi: 10, 100, 20 và 150 theo đề.
- Required làm các trường tương ứng không được null; BLL còn Trim và chặn chuỗi rỗng/khoảng trắng.
- MaLoai trong ThietBi là khóa ngoại. ForeignKey nối thuộc tính navigation LoaiThietBi với MaLoai.
- ICollection<ThietBi> biểu diễn tập nhiều thiết bị thuộc một loại; ThietBi.LoaiThietBi biểu diễn phía một.
- ThietBiInput giữ số lượng/giá dạng chuỗi để BLL kiểm tra cả lỗi định dạng; ThietBiRow mang TenLoai để lưới hiện tên thay vì chỉ mã.

Thực hành: vẽ LoaiThietBi 1 → n ThietBi; lấy ví dụ hai thiết bị chung MaLoai. Chỉ đúng property nào là FK và property nào là navigation.

## Bước 3 — Hiểu database và Code First

Mở `DAL/ThietBiContext.cs`, `Migrations/Configuration.cs`, file migration `*_InitialCreate.cs` và `GUI/App.config`.

- DbContext quản lý kết nối và theo dõi thay đổi entity; DbSet tương ứng tập entity để truy vấn và ghi.
- Code First bắt đầu từ class C#; Migration mô tả chuyển đổi schema. Up tạo bảng, PK/FK/index; Down đảo thao tác theo thứ tự ngược.
- DatabaseBootstrapper gọi DbMigrator.Update để áp dụng migration chưa có; __MigrationHistory giúp EF biết migration đã chạy.
- Seed thêm dữ liệu mẫu bằng EF; dùng transaction để lỗi thì rollback, không để dữ liệu mẫu dở dang.
- decimal(18,2) có tối đa 18 chữ số, trong đó 2 chữ số sau dấu thập phân, tức 16 chữ số phần nguyên.
- App.config chứa chuỗi kết nối. ThietBiContext nhận SqlConnection được DAL tạo từ cấu hình, không có chuỗi kết nối đầy đủ viết trực tiếp trong C#.

Thực hành: mở SQL Server Object Explorer, tìm database QuanLyThietBiPhongHoc, kiểm tra hai bảng và __MigrationHistory. Giải thích vì sao đổi tên database trong cấu hình có thể cho thấy một bộ dữ liệu khác.

## Bước 4 — Theo một lần nhấn Nhập từ đầu đến cuối

1. `GUI/FrmThietBi.cs:LuuAsync(false)` gọi DocDuLieu để chụp dữ liệu ô nhập thành ThietBiInput.
2. ThucHienGhiAsync chạy công việc nền; gọi `BLL/ThietBiService.cs:Nhap`.
3. Nhap gọi KiemTra: Trim chuỗi, kiểm tra độ dài, loại, ngày, TryParse số lượng và giá, kiểm tra giới hạn giá.
4. Gọi repository.TonTai để chặn mã đã có, sau đó repository.Them.
5. `DAL/Repositories.cs:Them` tạo context trong using, Add entity, SaveChanges.
6. GUI gọi lại LayDanhSach, gán lưới, làm mới ô nhập rồi hiển thị thông báo.

Thực hành: nhập TB006 hợp lệ; thử lại ` TB006 `; nhập SoLuong=0; chọn ngày mai. Với mỗi trường hợp, nói được dữ liệu có đi tới SaveChanges hay không và vì sao.

## Bước 5 — So sánh Sửa và Xóa

Sửa: ChonDong giữ maDangChon và khóa txtMaTB. Service.Sua đối chiếu mã gốc; DAL Find entity rồi chỉ gán thuộc tính không khóa. SaveChanges cập nhật dòng đang được theo dõi.

Xóa: XoaAsync hỏi Có/Không trước. Nếu Không, không gọi service. Nếu Có, service kiểm tra tồn tại, DAL Find → Remove → SaveChanges.

Thực hành: sửa SoLuong của TB006, đóng/mở lại để kiểm tra; sau đó xóa TB006 và kiểm tra nó không quay lại. Không dùng thiết bị mẫu làm bản ghi mới trùng mã.

## Bước 6 — Hiểu trạng thái giao diện và lỗi

- maDangChon=null: chế độ nhập mới. Có mã đang chọn: chế độ sửa/xóa.
- sanSang=false: chưa nạp được database, không cho ghi.
- dangXuLy=true: đang có một công việc nạp/ghi; tắt thao tác cạnh tranh.
- dangGanDuLieu=true: đang bind/reset bằng code, bỏ qua SelectionChanged tự phát.
- Task.Run xử lý SQL nền, await quay lại UI thread để cập nhật control. Mỗi repository tự tạo context, không dùng chung một context giữa nhiều thread.
- FormClosing là nơi xác nhận thoát duy nhất cho nút, X, Esc và Alt+F4.
- Nếu SaveChanges lỗi, không báo thành công. Nếu đã lưu nhưng tải lại lưới lỗi, phải nói rõ đã lưu và yêu cầu tải lại, tránh thêm trùng.

Thực hành: dùng cấu hình máy chủ sai để quan sát thông báo rồi đổi lại đúng. Phân biệt lỗi biên dịch/project incompatible với lỗi chạy không kết nối SQL Server.

## Bước 7 — Tập sửa yêu cầu nhỏ

1. Đổi tên sinh viên: sửa HoTenSinhVien trong App.config, build/chạy lại.
2. Đổi tiêu đề cột: sửa ThemCot trong TaoGiaoDien, không đổi DataPropertyName nếu property vẫn giữ nguyên.
3. Thêm quy tắc số lượng tối đa: đặt tại KiemTra trong BLL, giải thích và thêm test biên.
4. Nếu thêm cột mới lưu trong database: sửa entity, DTO/input, BLL, DAL và GUI rồi Add-Migration/Update-Database. Không chỉ thêm TextBox.

## Câu hỏi vấn đáp và ý trả lời

**1. Tại sao chia ba tầng?** GUI nhận/hiện dữ liệu, BLL giữ quy tắc, DAL truy cập database. Phân chia giúp đổi giao diện và kiểm thử nghiệp vụ độc lập.

**2. BLL có gọi trực tiếp DbContext không?** Không. BLL gọi repository interface; DAL triển khai bằng EF. Program nối các dependency khi ứng dụng khởi động.

**3. Tại sao cần BLL khi GUI đã có MaxLength?** GUI hỗ trợ nhập liệu; BLL vẫn phải bảo vệ quy tắc khi được gọi từ nguồn khác hoặc dữ liệu truyền thẳng vào dịch vụ.

**4. Interface repository có tác dụng gì?** BLL phụ thuộc hợp đồng thay vì EF cụ thể. Test có thể thay repository SQL bằng bản giả để kiểm tra nhánh hợp lệ/lỗi mà không cần SQL.

**5. Code First khác Database First thế nào?** Bài này định nghĩa model C# rồi Migration tạo/cập nhật schema; Database First lấy database có sẵn để sinh mô hình. Snapshot EDMX của migration không biến bài thành Database First.

**6. DbContext và DbSet khác nhau ra sao?** Context quản lý phiên làm việc và theo dõi thay đổi; DbSet là tập entity để truy vấn/thêm/xóa trong context đó.

**7. SaveChanges làm gì?** Gửi thay đổi entity được theo dõi xuống database. Add/Remove chưa đồng nghĩa thay đổi đã được lưu.

**8. Tại sao dùng decimal thay vì double cho giá?** decimal phù hợp tiền vì biểu diễn thập phân chính xác trong phạm vi kiểu; double dùng nhị phân có thể gây sai số làm tròn ở các số thập phân.

**9. Vì sao Column("decimal") chưa đủ diễn đạt 18,2?** EF6 không có Precision attribute như EF Core mới. Bài chốt HasPrecision(18,2) và migration để schema đúng chính xác.

**10. Kiểm tra mã trùng ở đâu?** BLL Trim rồi gọi TonTai trước khi thêm. PK trong SQL vẫn là lớp bảo vệ cuối khi hai phiên cùng thêm một mã.

**11. Vì sao khóa ô mã khi sửa và còn kiểm tra lại trong BLL?** Khóa ô giúp người dùng; kiểm tra trong BLL ngăn thay mã từ nguồn gọi khác. DAL cũng không gán lại khóa chính.

**12. Làm sao lưới hiện tên loại?** Truy vấn DAL chiếu sang ThietBiRow, lấy TenLoai qua navigation; cột lưới bind property TenLoai. Khi chọn dòng, ComboBox chọn theo MaLoai.

**13. DisplayMember và ValueMember khác nhau?** DisplayMember=TenLoai là chữ người dùng thấy; ValueMember=MaLoai là giá trị FK dùng để lưu.

**14. TryParse tốt hơn Parse ở đâu?** TryParse trả false khi sai định dạng để BLL báo lỗi phù hợp, thay vì phát sinh FormatException chỉ vì người dùng nhập chữ.

**15. Vì sao so ngày bằng .Date?** Đề ràng buộc theo ngày. Bỏ phần giờ để một thời điểm trong hôm nay vẫn được coi là ngày hiện tại; thêm giới hạn tối thiểu cho SQL datetime.

**16. AsNoTracking dùng khi nào?** Khi đọc danh sách để hiển thị, không cần context theo dõi entity trả về để sửa. Việc sửa thực tế dùng Find trong context mới.

**17. using context có ý nghĩa gì?** Bảo đảm Dispose khi xong hoặc khi lỗi, giải phóng tài nguyên kết nối. SqlClient có connection pooling, không cần giữ context suốt đời Form.

**18. Vì sao không truy cập control trong Task.Run?** WinForms control gắn với UI thread. Tác vụ nền chỉ gọi dịch vụ và trả DTO; sau await, UI thread mới gán ComboBox/DataGridView.

**19. Làm sao tránh xác nhận thoát hai lần?** Nút Thoát chỉ gọi Close; chỉ FormClosing hỏi. Chọn Không đặt e.Cancel=true.

**20. Vì sao seed không thêm trùng khi chạy lại?** Kiểm tra mã loại trước khi bổ sung; thiết bị mẫu chỉ tạo khi database hoàn toàn trống. Không dùng AddRange mẫu ở mọi lần chạy.

**21. Nếu SQL Server chưa cài, chương trình có tự tạo dữ liệu trong RAM để chạy không?** Không. Đề yêu cầu EF và SQL; ứng dụng báo lỗi, cho cấu hình lại và không báo lưu thành công giả.

**22. Nếu lưu thành công nhưng lưới tải lại thất bại?** Cờ daLuu phân biệt hai giai đoạn; thông báo đã lưu và yêu cầu tải lại. Không coi là chưa thêm rồi yêu cầu người dùng thêm lần nữa.

**23. Thiết bị có thể không thuộc loại nào không?** Không. MaLoai Required, quan hệ HasRequired và BLL yêu cầu chọn loại tồn tại.

**24. Tắt cascade delete nghĩa là gì?** Không tự xóa toàn bộ thiết bị khi xóa loại. Nếu muốn quản lý loại về sau, phải xử lý ràng buộc thiết bị phụ thuộc phù hợp.

## Cách tập trả lời

Với mỗi câu, trả lời ngắn trước, rồi mở đúng file và chỉ hàm chứng minh. Sau đó làm một thao tác trên Form để minh họa. Nếu chưa chạy SQL Server hoặc còn lỗi, nói đúng trạng thái đó thay vì dùng kết quả test giả làm bằng chứng đã chạy database.

Buổi học tiếp theo có thể lần lượt: Entities → KiemTra → Nhap → Sua/Xoa → Migration → trạng thái Form; sau mỗi phần tự giải thích lại và trả lời 3–5 câu hỏi.
