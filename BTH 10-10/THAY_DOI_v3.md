# Fix đổi MSSV không làm mất bản nháp

File code duy nhất thay đổi so với v2: Presentation/MainForm.cs.

Nếu đã đưa bài lên GitHub trong BTH 10-10, chỉ cần thay file MainForm.cs ở Presentation bằng file bản v3 này, giữ nguyên cấu trúc solution và .git của repo. Có thể cập nhật thêm tài liệu đi kèm. Không tạo repository mới.

Nguyên nhân: txtMaSV.Leave gọi TraCuuMa; nhánh mã không tồn tại gọi XoaThongTinChiTiet, khiến bản nháp bị xóa mỗi lần đổi mã.

Fix: bỏ thao tác xóa ở nhánh mã không tồn tại, giữ trạng thái thêm; bổ sung hỏi xác nhận khi có bản nháp nhưng mã mới trùng sinh viên đã lưu.

Phạm vi: đổi MSSV trong khi nhập sinh viên mới. Không thay đổi MSSV của bản ghi đã lưu.
