# Quản lý sinh viên — Bài thực hành 3, Bài 1 + Bài 2 (v3)

Project mới, độc lập với ConsoleApp6 và QuanLySinhVienCode trước đó. Giao diện Windows Forms được tạo hoàn toàn bằng C#, không có Form Designer. Framework: .NET 8.

## 1. Mở bằng Visual Studio tím

1. Giải nén ZIP ra một thư mục riêng, ví dụ `D:\BaiTap\QuanLySinhVien_Layer`.
2. Mở Visual Studio → File → Open → Project/Solution.
3. Đi vào thư mục vừa giải nén và chọn **QuanLySinhVien_Layer.sln**. Không chọn cả folder, không mở ZIP trực tiếp.
4. Trong Solution Explorer, chuột phải **QuanLySinhVien.Presentation** → **Set as Startup Project**.
5. Nhấn Ctrl+Shift+B để build, rồi F5 để chạy.
6. Nếu thiếu thành phần: mở Visual Studio Installer → Modify → chọn workload `.NET desktop development` và cài .NET 8 SDK; cập nhật Visual Studio nếu cần.

Không chép các file này vào ConsoleApp6. Không mở riêng Entity, Business hoặc DataAccess để chạy, vì đó là các thư viện. Project chạy giao diện là Presentation. Tests là chương trình console kiểm thử, không phải ứng dụng chính.

## 2. Cấu trúc

```text
QuanLySinhVien_Layer/
├── QuanLySinhVien_Layer.sln
├── Entity/
│   ├── SinhVien.cs
│   └── LopHoc.cs
├── DataAccess/
│   ├── DataList.cs
│   ├── SinhVienDAL.cs
│   └── LopHocDAL.cs
├── Business/
│   ├── SinhVienBUL.cs
│   ├── LopHocBUL.cs
│   └── BusinessValidationException.cs
├── Presentation/
│   ├── Program.cs
│   ├── MainForm.cs
│   └── LopHocForm.cs
├── Tests/
│   └── Program.cs
├── HUONG_DAN_KIEM_THU.md
└── KET_QUA_KIEM_TRA.txt
```

Mỗi thư mục code có một file `.csproj`. Các project được liên kết bằng ProjectReference, không cần chép DLL hay cài thư viện NuGet bên thứ ba.

## 3. Trách nhiệm từng tầng

### Entity

- SinhVien: mã, tên, ngày sinh, giới tính, email, điện thoại (SoDienThoai), điểm, trạng thái, MaLop và thuộc tính navigation LopHoc.
- LopHoc: MaLop, TenLop, DanhSachSinhVien.
- Data Annotations khai báo quy tắc dữ liệu; hai thực thể có `IsInValid()` trả List<ValidationResult> và `KiemTraHopLe(out List<ValidationResult>)` trả bool. Business gọi các phương thức này, sau đó kiểm tra quan hệ/mã trùng.
- Quan hệ 1–n được biểu diễn bằng MaLop và navigation; danh sách sinh viên của lớp được dựng từ cùng nguồn dữ liệu khi đọc.

### DataAccess

- DataList là nguồn dữ liệu trong bộ nhớ. Hai DAL sử dụng **cùng một instance** được tạo tại Program.cs.
- SinhVienDAL và LopHocDAL có đủ thêm, sửa, xóa, lấy tất cả và lấy theo mã; SinhVienDAL thêm truy vấn theo lớp và tìm kiếm.
- Không dùng MessageBox, ErrorProvider hay tham chiếu Windows Forms.
- Đọc trả về bản sao để Form không sửa trực tiếp danh sách gốc. Thêm/sửa lưu bản sao; quan hệ lớp được kiểm tra và dựng lại khi đọc.

### Business (BUL)

- BUL là tên theo code mẫu, tương đương BLL/Business Logic Layer.
- Chuẩn hóa mã, trim chuỗi và gộp khoảng trắng họ tên bằng Regex `@"\s+"`.
- Kiểm tra Data Annotations, mã trùng, lớp tồn tại, giới tính và trạng thái hợp lệ.
- Cấm sửa sinh viên không tồn tại và cấm xóa lớp còn sinh viên.
- Lỗi validation giữ tên thuộc tính trong ValidationResult.MemberNames, đưa về Presentation qua BusinessValidationException.
- Không chứa control hay MessageBox.

### Presentation

- MainForm chỉ tạo UI, đọc các ô nhập, gọi BUL, hiển thị kết quả và ánh xạ lỗi đến control.
- Đổi chuỗi điểm nhập thành decimal là xử lý biểu diễn UI; quy tắc khoảng 0–10 nằm trong Entity và được BUL kiểm tra.
- LopHocForm là giao diện CRUD lớp học bổ sung; gọi LopHocBUL.
- Program.cs là điểm lắp ráp dependency: tạo DataList → DAL → BUL → Form. Chỉ Program.cs của project này truy cập DataAccess để lắp ráp; các Form không gọi DAL.

Luồng thao tác:

```text
Người dùng → MainForm → SinhVienBUL → SinhVienDAL → DataList
                         ↓ lỗi theo thuộc tính
                  ErrorProvider ở Form
```

## 4. Đối chiếu yêu cầu bài tập

| Yêu cầu | Triển khai |
|---|---|
| DAL riêng cho mỗi thực thể | SinhVienDAL, LopHocDAL |
| CRUD trên DataList | DataList dùng chung; các DAL thêm/sửa/xóa/đọc |
| Business riêng | SinhVienBUL, LopHocBUL |
| Presentation chỉ xử lý giao diện | MainForm, LopHocForm gọi Business |
| Lấy danh sách lớp lên ComboBox | NapComboLop gọi LopHocBUL.GetAllLopHoc |
| Chọn lớp hiển thị sinh viên lớp đó | ComboBox “Lọc lớp” tự tải danh sách qua BUL; chọn lớp trong ô nhập hoặc nạp sinh viên theo mã cũng đồng bộ bộ lọc |
| Nhập mã tồn tại → hiển thị thông tin | txtMaSV.Leave → TraCuuMa → GetSinhVienByMaSV |
| Mã chưa tồn tại trên form mới → trống; giữ bản nháp nếu đang nhập | Giữ mã và chi tiết đang nhập; form mới đã trống sau Làm mới |
| Kiểm tra thêm/sửa và ErrorProvider | BusinessValidationException → HienThiLoi → ErrorProvider.SetError |
| Tìm trên giao diện | Lọc DTO đã tải, không gọi lại BUL/DAL trong nhánh tìm |
| Tìm phía nguồn dữ liệu | MainForm → BUL.TimKiemTaiNguon → DAL.TimKiem → DataList |

## 5. Hai cách tìm kiếm

- **Tìm tại nguồn DataList**: đọc và lọc dữ liệu tại DAL; kết quả trở thành danh sách mới đã tải. Lọc đồng thời lớp, từ khóa và điểm từ.
- **Tìm trên giao diện**: chỉ lọc danh sách DTO đã tải gần nhất. Không đọc nguồn lại. Nếu trước đó tìm tại nguồn ra 1 hàng, tìm trên giao diện chỉ tìm trong 1 hàng đó.
- Chọn lại một lớp hoặc “Hiển thị tất cả” để tải lại danh sách gốc trước khi đổi từ khóa.
- Từ khóa hỗ trợ mã, họ tên, email, điện thoại, không phân biệt hoa thường; không triển khai tìm không dấu.
- “Hiển thị tất cả” xóa từ khóa, bỏ giới hạn lớp và đặt điểm từ về 0.

**Quan trọng:** đề yêu cầu nguồn DataList, nên đây không phải SQL Server hay database bền vững. Cụm “tìm phía cơ sở dữ liệu” được minh họa bằng tìm tại tầng DataAccess trên DataList. Nếu giảng viên yêu cầu SQL thật, cần thay DataAccess bằng truy vấn database; bản này chưa có SQL.

## 6. Nhập liệu

- Mã hỗ trợ `SV` và **4–6 chữ số**: giữ các mã cũ `SV000123` và chấp nhận kiểu trong code mẫu `SV0001`.
- Nhập mã rồi nhấn Tab để tra cứu. Mã tồn tại sẽ bị khóa khi sửa; bấm “Làm mới” để nhập mã khác.
- Mã chưa có: giữ nguyên các ô đã nhập. Form mới sau Làm mới vẫn trống; DateTimePicker bỏ dấu chọn để biểu thị chưa nhập ngày, dù control vẫn hiển thị ngày mặc định.
- Chọn ngày sinh bằng cách tích checkbox trên DateTimePicker.
- Điểm dùng TextBox, chấp nhận `8.5` hoặc `8,5`; để trống, chữ hoặc điểm ngoài 0–10 đều bị báo lỗi.
- Nút “Đóng” và nút X của cửa sổ chính đều có xác nhận. Dữ liệu trong bộ nhớ không được lưu sau khi thoát.
- Khi lỗi xuất hiện, di chuột lên biểu tượng cạnh ô để xem nội dung. Sửa ô tương ứng sẽ xóa chỉ báo cũ của ô đó; bấm thêm/sửa để Business kiểm tra lại.
- Có xác nhận trước sửa/xóa. “Làm mới” chỉ xóa form nhập, không xóa danh sách nguồn và không xóa bộ lọc.
- Thêm/sửa thành công tải lại danh sách lớp đang chọn, bỏ kết quả tìm trước đó. Sinh viên chuyển sang lớp khác có thể không còn xuất hiện trong bộ lọc lớp cũ.

## 7. Khác biệt và lỗi đã tránh từ mẫu

- File DOCX mới có `AddSinhVien` và `AddSinhVien1` cùng mục đích; bản này dùng một phương thức thêm với quy trình validation thống nhất. Không coi đây là hai chữ ký trùng nhau.
- Dùng Regex `@"\s+"`, không dùng `"s+"` để chuẩn hóa khoảng trắng.
- Không tạo nguồn DataList riêng cho mỗi BUL/DAL, tránh mỗi tầng nhìn thấy dữ liệu khác nhau.
- Không chỉ ném một thông báo lỗi chung: giữ MemberNames để gắn đúng control.
- Không đưa List nguồn ra cho Form chỉnh sửa trực tiếp.

## 8. Dữ liệu và kiểm thử

- Có 3 lớp và 4 sinh viên mẫu. Đây là dữ liệu minh họa, không phải dữ liệu sinh viên thực.
- Thay đổi chỉ tồn tại trong lần chạy hiện tại; đóng và mở lại ứng dụng sẽ khởi tạo lại mẫu.
- Có project Tests kiểm thử nghiệp vụ bằng console, không cần gói test bên ngoài.
- Chạy trong terminal tại thư mục solution:

```powershell
dotnet run --project Tests/QuanLySinhVien.Tests.csproj -c Release
```

Hoặc đặt Tests làm startup để chạy kiểm thử, sau đó đặt lại Presentation làm startup để chạy giao diện.

Xem KET_QUA_KIEM_TRA.txt cho kết quả build và kiểm thử đã thực hiện. Build Windows Forms đã được kiểm tra bằng Windows targeting trên Linux; chưa chạy giao diện Windows thực tế. Hãy chạy checklist UI trong HUONG_DAN_KIEM_THU.md trên máy Windows của bạn.

## 9. Cập nhật theo DOCX đầy đủ

- Đã đọc cả phần Bài 1, Bài 2, ví dụ DAL/BUL/View và hình giao diện.
- Entity bổ sung phương thức validation đúng cách gọi `sv.IsInValid()` của mẫu.
- Tên ô mã là `txtMaSV`, tên thuộc tính điện thoại là `SoDienThoai` để thống nhất mẫu.
- Lookup chọn toàn bộ mã khi ô nhận focus; Enter chuyển đến ô tiếp theo theo thứ tự Tab. Không chuyển thẳng đến ngày sinh như ví dụ vì sẽ bỏ qua hai ô còn lại của hàng đầu trong bố cục ảnh.
- Kiểm tra Business trước khi xác nhận sửa. Dữ liệu sai nhiều trường được báo cùng lúc, kể cả khi chuỗi điểm không chuyển thành số được.
- Bổ sung nút Đóng và xác nhận khi nhấn X.
- TabIndex được cấu hình ở cả control và container: hàng 1 mã → tên → lớp; hàng 2 ngày sinh → nhóm giới tính → điểm; hàng 3 email → điện thoại → trạng thái; sau đó Thêm → Sửa → Xóa → Làm mới → Đóng.
- Vẫn là giao diện Windows Forms tạo bằng code, cùng bố cục/chức năng chính của ảnh; không phải bản sao pixel-perfect. Điểm nhập dùng TextBox thay NumericUpDown để có thể minh họa lỗi nhập chữ, để trống và ngoài khoảng bằng ErrorProvider.
- Không chép nguyên lỗi và phần chưa hoàn thiện của mẫu: các nút Sửa/Xóa đã có xử lý thật, danh sách được nạp và cập nhật sau CRUD, SelectedIndexChanged có kiểm tra trạng thái nạp tránh SelectedValue null.

Xem `DOI_CHIEU_DE_BAI.md` để đối chiếu từng yêu cầu với file/hàm.

## 10. Bản v3 — giữ bản nháp khi đổi MSSV lúc thêm

- Sửa MainForm.TraCuuMa: mã mới không còn gọi XoaThongTinChiTiet. Họ tên, email, điện thoại, ngày sinh, giới tính, điểm, lớp và trạng thái đang nhập được giữ.
- Xóa MSSV rồi nhập lại cũng không mất các trường khác; mã trống vẫn bị Business báo lỗi nếu bấm Thêm.
- Nếu đổi sang mã đã có trong nguồn và có bản nháp, hỏi xác nhận trước khi nạp sinh viên cũ. No (mặc định) giữ bản nháp và báo mã trùng; Yes nạp thông tin sinh viên đã có.
- Nếu form chưa có thông tin chi tiết, tra mã tồn tại vẫn tự nạp như trước.
- Làm mới và lưu thành công vẫn xóa form; mã của sinh viên đã có vẫn ReadOnly khi sửa. Bản này không bổ sung đổi khóa MSSV của bản ghi đã lưu.
- Đây là điều chỉnh theo yêu cầu UX mới: yêu cầu “mã không tồn tại thì xóa trống” chỉ đúng khi tra cứu trên form mới, không áp dụng để xóa bản nháp đang nhập.
- Các phần trước mô tả xóa trống khi mã chưa có cần hiểu theo ngoại lệ bản nháp trong mục này.
