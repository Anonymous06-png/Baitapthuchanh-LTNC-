# Đối chiếu với DOCX đầy đủ

Nguồn đối chiếu: “Bài thực hành 3 Lập trình sự kiện (1).docx” do người dùng gửi. Bản v2 tổng hợp Bài 1 và Bài 2, không tạo hai ứng dụng trùng nhau.

| Yêu cầu trong đề | File/hàm thực hiện |
|---|---|
| Bố cục thông tin SV, tìm kiếm, bảng danh sách trong ảnh | Presentation/MainForm.cs → KhoiTaoGiaoDien |
| LopHoc/SinhVien quan hệ 1–n | Entity/LopHoc.cs, SinhVien.cs; DataAccess/LopHocDAL.Copy và SinhVienDAL.Copy dựng navigation từ MaLop |
| Data Annotations và phương thức kiểm tra đối tượng ở hai lớp | Hai Entity → IsInValid, KiemTraHopLe |
| FormLoad nạp lớp, sinh viên và bật/tắt nút phù hợp | MainForm.DangKySuKien (Load), NapComboLop, TaiDanhSachLop, LamMoiForm |
| Focus mặc định ở txtMaSV | LamMoiForm → BeginInvoke(txtMaSV.Focus) |
| Tab trên xuống dưới, trái sang phải | TabIndex control + container trong KhoiTaoGiaoDien |
| Mã tồn tại → hiển thị, Thêm tắt, Sửa/Xóa bật | TraCuuMa, HienThiSinhVienLenForm |
| Mã chưa có → xóa chi tiết, Thêm bật, Sửa/Xóa tắt | TraCuuMa, XoaThongTinChiTiet |
| Làm mới và trả focus về mã | LamMoiForm |
| Xác nhận thao tác nguy hiểm | LuuSinhVien (sửa), XoaSinhVien, FormClosing; LopHocForm sửa/xóa lớp |
| DAL cho mỗi thực thể và CRUD từ DataList | DataAccess/SinhVienDAL.cs, LopHocDAL.cs, DataList.cs |
| Business cho mỗi thực thể/use case | Business/SinhVienBUL.cs, LopHocBUL.cs |
| Form chỉ giữ xử lý giao diện | Hai Form chỉ gọi BUL; Program lắp ráp dependency |
| Lấy lớp lên combobox, chọn lớp lọc sinh viên | NapComboLop; SelectedIndexChanged của cboLopHoc và cboLocLop |
| Tìm trên giao diện | TimKiem nhánh rdoTimGiaoDien: lọc DTO đã tải |
| Tìm tại tầng dữ liệu | TimKiem nhánh rdoTimNguon → BUL.TimKiemTaiNguon → DAL.TimKiem |
| Thêm/sửa validation, lỗi theo trường | Entity.IsInValid → BUL.KiemTraDuLieu/Add/Update → BusinessValidationException → MainForm.HienThiLoi |
| ErrorProvider | HienThiLoi ánh xạ đầy đủ MaSV, HoTen, NgaySinh, GioiTinh, Email, SoDienThoai, Diem, MaLop, TrangThai |
| Chọn mã khi focus, sự kiện Enter trong mẫu | txtMaSV.Enter, txtMaSV.KeyDown |
| Đóng có xác nhận trong mẫu | btnDong.Click → Close → FormClosing |
| Sửa/Xóa đang bỏ trống trong mẫu | LuuSinhVien, XoaSinhVien đã hoàn thiện |

## Chỗ điều chỉnh so với mẫu

- Tên project/namespace tổ chức theo Entity, DataAccess, Business, Presentation, không chép nguyên QuanLySinhVien.Data.Entity / QuanLySinhVien.BUL / QuanLySinhVien.Views. Vai trò tầng tương đương.
- LopBus trong ví dụ được triển khai đầy đủ bằng LopHocBUL và LopHocDAL.
- Mã mẫu SV0001 và mã ảnh SV000123 đều được hỗ trợ. Dữ liệu ban đầu dùng bộ bốn sinh viên ở project trước; không sử dụng SV0001 làm dữ liệu seed.
- Một ErrorProvider dùng chung vẫn gắn lỗi riêng từng control, thay cho nhiều ErrorProvider trong mẫu.
- Validation được yêu cầu qua Business thay vì Form gọi trực tiếp Entity.IsInValid; Business vẫn gọi phương thức Entity và kiểm tra lại trước khi ghi.
- AddSinhVien1 là biến thể thao tác thêm trong DOCX mới, được gom vào AddSinhVien. Regex sai “s+” được sửa thành @"\s+".
- Không gọi SelectedValue.ToString khi có thể null hoặc khi đang nạp DataSource.
- Khi làm mới/không tìm thấy, giữ ngày mặc định bên trong DateTimePicker nhưng bỏ checkbox để biểu thị chưa nhập. Không có ngày rỗng thật trong DateTimePicker.
- DataList chỉ trong bộ nhớ. “Tìm phía cơ sở dữ liệu” minh họa tại tầng DataAccess, không phải SQL thật. Đề nêu DataList nên bản này không bổ sung database server.
- Thứ tự Tab, các chỉ báo lỗi, kích thước UI và hành vi bàn phím cần kiểm thử thủ công trên Windows; kiểm thử console không chứng minh những hành vi UI này.


## Điều chỉnh v3 theo phản hồi người dùng

Khi sửa MSSV trong lúc thêm, không xóa các trường đã nhập. TraCuuMa giữ bản nháp khi mã chưa tồn tại; mã trùng có hộp thoại xác nhận trước khi nạp bản ghi cũ. Điều này ưu tiên tránh mất dữ liệu đang nhập, nên thay thế hành vi xóa vô điều kiện của yêu cầu ban đầu. Form mới sau Làm mới vẫn trống. Mã bản ghi đã lưu vẫn khóa khi sửa.
