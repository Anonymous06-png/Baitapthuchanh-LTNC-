# Thay đổi v3

## Tra cứu mã sinh viên

- Giữ các trường chi tiết khi đổi mã trong lúc thêm sinh viên mới.
- Xóa mã để nhập lại không làm mất bản nháp.
- Khi có bản nháp và mã mới trùng một sinh viên đã lưu, yêu cầu xác nhận trước khi nạp bản ghi đó. Lựa chọn mặc định là No.
- Tra cứu trên form trống vẫn nạp sinh viên đã có như trước.

## Các hành vi giữ nguyên

- Làm mới và lưu thành công xóa form nhập.
- Business kiểm tra mã trùng trước khi thêm.
- Mã sinh viên đã lưu không được thay đổi trong chế độ sửa.

Thành phần thay đổi: `Presentation/MainForm.cs`, các phương thức `TraCuuMa()` và `CoThongTinDangNhap()`.
