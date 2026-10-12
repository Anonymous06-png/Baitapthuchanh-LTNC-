# Đối chiếu yêu cầu bài thực hành

## Bài 1 — Giao diện và xử lý sự kiện

| Yêu cầu | Thành phần triển khai |
|---|---|
| Thực thể Lớp học và Sinh viên có quan hệ 1–n | `Entity/LopHoc.cs`, `Entity/SinhVien.cs`; liên kết bằng `MaLop` |
| Data Annotations và phương thức validation | `IsInValid()` và `KiemTraHopLe()` của hai thực thể |
| Hiển thị lớp và sinh viên khi mở Form | `MainForm.NapComboLop()`, `TaiDanhSachLop()` |
| Thiết lập focus, thứ tự Tab và trạng thái nút | `KhoiTaoGiaoDien()`, `LamMoiForm()` |
| Tra cứu theo mã sinh viên | `TraCuuMa()`, `HienThiSinhVienLenForm()` |
| Thêm, sửa, xóa và làm mới | `LuuSinhVien()`, `XoaSinhVien()`, `LamMoiForm()` |
| Xác nhận thao tác nguy hiểm | Xác nhận sửa, xóa và `FormClosing` |

## Bài 2 — Tách tầng

| Yêu cầu | Thành phần triển khai |
|---|---|
| DAL riêng cho từng thực thể | `SinhVienDAL`, `LopHocDAL` |
| Thêm/sửa/xóa/đọc từ DataList | Các phương thức CRUD của hai DAL |
| Business riêng cho từng thực thể | `SinhVienBUL`, `LopHocBUL` |
| Presentation xử lý giao diện và gọi Business | `MainForm`, `LopHocForm` |
| Chọn lớp hiển thị sinh viên tương ứng | Sự kiện của `cboLopHoc` và `cboLocLop` |
| Tìm trên giao diện | Lọc danh sách DTO đã tải trong `MainForm.TimKiem()` |
| Tìm tại tầng dữ liệu | `SinhVienBUL.TimKiemTaiNguon()` → `SinhVienDAL.TimKiem()` |
| Validation khi thêm và sửa | Entity validation kết hợp kiểm tra nghiệp vụ tại BUL |
| Báo lỗi theo trường | `BusinessValidationException` → `MainForm.HienThiLoi()` → `ErrorProvider` |

## Xử lý dữ liệu đang nhập

Khi chưa có bản nháp, mã sinh viên đã tồn tại được dùng để nạp thông tin. Khi đang thêm sinh viên mới, đổi sang mã chưa có không xóa các trường đã nhập. Nếu đổi sang mã đã tồn tại khi có bản nháp, ứng dụng yêu cầu xác nhận trước khi thay thế nội dung form.

Đây là điều chỉnh hành vi tra cứu để tránh mất dữ liệu đang nhập. Nút **Làm mới** vẫn xóa form.

## Phạm vi nguồn dữ liệu

Tìm tại nguồn được thực hiện trên `DataList` qua DataAccess. Ứng dụng chưa triển khai truy vấn SQL hoặc lưu dữ liệu lâu dài.
