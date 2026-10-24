# Kiểm thử

## Kiểm thử tự động

Chạy tại thư mục solution:

```powershell
dotnet run --project Tests/QuanLySinhVien.Tests.csproj -c Release
```

Các kiểm thử tập trung vào validation, CRUD, tìm kiếm, quan hệ lớp–sinh viên và tính độc lập của bản sao dữ liệu khi đọc.

## Kiểm thử giao diện

Thực hiện trên Windows. Các kịch bản dưới đây mô tả kết quả mong đợi, không thay thế báo cáo kết quả thực thi.

| STT | Thao tác | Kết quả mong đợi |
|---|---|---|
| 1 | Khởi động ứng dụng | Hiển thị 4 sinh viên mẫu và 3 lớp; Thêm bật, Sửa/Xóa tắt; focus ô mã |
| 2 | Chọn Kỹ thuật phần mềm 01 | Hiển thị SV000123 và SV000125 trên dữ liệu mẫu |
| 3 | Chọn AI01 | Hiển thị SV000124 trên dữ liệu mẫu |
| 4 | Nhập SV000123, nhấn Tab | Nạp thông tin, khóa mã; Sửa/Xóa bật, Thêm tắt |
| 5 | Làm mới, nhập mã chưa có | Giữ trạng thái thêm; chưa có thông tin chi tiết |
| 6 | Bấm Thêm khi thiếu thông tin | ErrorProvider báo các trường bắt buộc |
| 7 | Nhập email hoặc điện thoại sai | Báo lỗi đúng ô, không ghi dữ liệu |
| 8 | Nhập điểm bằng chữ, để trống hoặc ngoài 0–10 | Chặn thêm/sửa và báo lỗi điểm |
| 9 | Nhập điểm 8.5 hoặc 8,5 | Đọc đúng giá trị 8,5 |
| 10 | Chọn ngày sinh hôm nay hoặc tương lai | Báo lỗi ngày sinh |
| 11 | Thêm một sinh viên hợp lệ | Danh sách nguồn tăng một bản ghi; form được làm mới |
| 12 | Sửa thông tin hoặc chuyển lớp | No giữ dữ liệu cũ; Yes cập nhật và thay đổi sĩ số tương ứng |
| 13 | Xóa sinh viên | No giữ bản ghi; Yes xóa bản ghi |
| 14 | Tìm trên giao diện | Chỉ tìm trong danh sách đã tải, không nạp lại từ nguồn |
| 15 | Tìm tại nguồn theo lớp, từ khóa và điểm | Kết quả thỏa đồng thời các điều kiện |
| 16 | Bấm Hiển thị tất cả | Tải lại toàn bộ sinh viên, bỏ bộ lọc |
| 17 | Thêm/sửa/xóa lớp trống | Thực hiện được; xóa lớp có sinh viên bị chặn |
| 18 | Tab qua các ô nhập | Đi theo hàng từ trái sang phải, từ trên xuống dưới |
| 19 | Bấm Đóng hoặc X cửa sổ chính | Có xác nhận; No giữ cửa sổ, Yes thoát |
| 20 | Khởi động lại ứng dụng | Nạp lại dữ liệu mẫu vì nguồn nằm trong bộ nhớ |

## Hồi quy khi đổi mã sinh viên mới

1. Nhập mã mới và điền các trường chi tiết. Đổi sang một mã mới khác rồi nhấn Tab: các trường chi tiết phải được giữ.
2. Xóa mã rồi nhấn Tab: không mất chi tiết. Bấm Thêm phải báo thiếu mã.
3. Khi có bản nháp, đổi mã thành SV000123: phải hỏi xác nhận. No giữ bản nháp và báo mã trùng; Yes nạp sinh viên đã có.
4. Sau Làm mới, tra SV000123 trên form trống: nạp thông tin mà không hỏi bỏ bản nháp.
5. Làm mới hoặc thêm thành công vẫn xóa form. Mã bản ghi đã lưu vẫn khóa khi sửa.
