# BTH 10-10 — Quản lý sinh viên

Ứng dụng Windows Forms quản lý sinh viên theo lớp, tổ chức theo các tầng Entity, DataAccess, Business và Presentation. Giao diện được khởi tạo bằng C#.

## Công nghệ

- C# và .NET 8
- Windows Forms
- Data Annotations
- Nguồn dữ liệu: danh sách trong bộ nhớ (`DataList`)

## Chức năng

- Hiển thị, thêm, sửa và xóa sinh viên.
- Tra cứu sinh viên theo mã; chọn một dòng để xem thông tin.
- Quản lý lớp học và lọc sinh viên theo lớp.
- Tìm theo mã, họ tên, email hoặc điện thoại; lọc theo điểm tối thiểu.
- Hai chế độ tìm kiếm: trên danh sách đã tải và tại tầng DataAccess.
- Kiểm tra dữ liệu và hiển thị lỗi bằng `ErrorProvider` tại từng ô nhập.
- Giữ thông tin đang nhập khi thay đổi mã của sinh viên mới.
- Xác nhận trước khi sửa, xóa hoặc thoát ứng dụng.

## Cấu trúc

```text
BTH 10-10/
├── QuanLySinhVien_Layer.sln
├── Entity/          # Thực thể và quy tắc validation
├── DataAccess/      # DataList và các lớp truy cập dữ liệu
├── Business/        # Nghiệp vụ và kiểm tra dữ liệu
├── Presentation/    # Giao diện Windows Forms
└── Tests/           # Kiểm thử Entity, DataAccess và Business
```

Các Form gọi Business để thực hiện nghiệp vụ. `Program.cs` khởi tạo nguồn dữ liệu và các đối tượng DAL/BUL dùng chung.

## Chạy ứng dụng

### Visual Studio

Yêu cầu Windows, .NET 8 SDK và Visual Studio hỗ trợ .NET 8 với workload **.NET desktop development**.

1. Mở `QuanLySinhVien_Layer.sln`.
2. Đặt `QuanLySinhVien.Presentation` làm **Startup Project**.
3. Build solution và nhấn **F5** để chạy.

### Dòng lệnh

Chạy tại thư mục chứa file solution trên Windows:

```powershell
dotnet build QuanLySinhVien_Layer.sln
dotnet run --project Presentation/QuanLySinhVien.Presentation.csproj
```

## Kiểm thử

```powershell
dotnet run --project Tests/QuanLySinhVien.Tests.csproj -c Release
```

Các kịch bản kiểm thử giao diện được liệt kê trong `HUONG_DAN_KIEM_THU.md`.

## Quy tắc nhập liệu

- Mã sinh viên gồm `SV` và 4–6 chữ số, ví dụ `SV0001` hoặc `SV000123`.
- Họ tên có 2–100 ký tự; khoảng trắng thừa được chuẩn hóa.
- Ngày sinh từ năm 1900 và trước ngày hiện tại.
- Email hợp lệ; điện thoại gồm 9–11 chữ số.
- Điểm từ 0 đến 10, chấp nhận dấu chấm hoặc dấu phẩy thập phân khi nhập.
- Sinh viên phải thuộc một lớp đã có trong danh sách.
- Không được xóa lớp đang có sinh viên.

## Lưu ý

- `DataList` chỉ lưu dữ liệu trong bộ nhớ. Khi khởi động lại, ứng dụng nạp lại dữ liệu mẫu.
- Chưa tích hợp SQL Server hoặc cơ chế lưu dữ liệu lâu dài.
- Mã sinh viên của bản ghi đã lưu được khóa trong chế độ sửa.
- Tìm trên giao diện chỉ lọc danh sách đã tải gần nhất. Chọn lại lớp hoặc **Hiển thị tất cả** để tải lại dữ liệu.
