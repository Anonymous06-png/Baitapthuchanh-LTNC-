# Checklist chạy trên Visual Studio/Windows

Các bước dưới đây là hướng dẫn kiểm thử thủ công, không phải tuyên bố đã chạy UI.

1. Mở solution, đặt Presentation làm startup, F5. Mong đợi 4 sinh viên, 3 lớp ở combobox; Thêm bật, Sửa/Xóa tắt.
2. Chọn “Kỹ thuật phần mềm 01” trong “Lọc lớp”. Mong đợi 2 sinh viên SV000123 và SV000125.
3. Chọn lớp AI01. Mong đợi SV000124. Chọn “Tất cả lớp” để quay lại 4 hàng.
4. Nhập `SV000123`, nhấn Tab. Mong đợi thông tin Nguyễn Văn An, mã khóa, Sửa/Xóa bật.
5. Bấm “Làm mới”, nhập `SV999999`, Tab. Các chi tiết phải trống; ngày sinh bỏ chọn; giữ mã mới để thêm.
6. Chỉ nhập mã rồi Thêm. Kiểm tra biểu tượng lỗi ở tên, ngày, giới tính, email, điện thoại, điểm, lớp và trạng thái.
7. Điền dữ liệu hợp lệ nhưng email `abc`. Mong đợi lỗi cạnh email, danh sách không đổi.
8. Nhập điểm `abc`, rồi `11`, rồi để trống. Mỗi trường hợp đều phải bị chặn; nhập `8,5` hoặc `8.5` phải đọc được.
9. Chọn ngày hôm nay hoặc tương lai. Mong đợi lỗi ngày sinh.
10. Thêm một sinh viên hợp lệ. Chọn đúng lớp ở bộ lọc để nhìn thấy. Họ tên có nhiều khoảng trắng sẽ được Business chuẩn hóa.
11. Chọn sinh viên mới, sửa lớp sang lớp khác và bấm Sửa. Bấm No không thay đổi; Yes cập nhật. Kiểm tra sĩ số ở cửa sổ “Quản lý lớp”.
12. Xóa sinh viên: No giữ nguyên, Yes xóa. Bấm Làm mới: form nhập rỗng nhưng không mất danh sách.
13. “Hiển thị tất cả”, chọn “Tìm trên giao diện”, nhập từ khóa `An` và Tìm kiếm. Xóa từ khóa rồi tìm lại để lấy toàn bộ danh sách đã tải.
14. “Hiển thị tất cả”, chọn “Tìm tại nguồn DataList”, lọc lớp KTPM01 và điểm từ 8. Mong đợi Nguyễn Văn An (trước khi dữ liệu mẫu bị thay đổi).
15. Chạy tìm tại nguồn ra một kết quả; đổi sang tìm trên giao diện. Nhánh giao diện chỉ tìm trong snapshot đã tải này. Chọn lại lớp/Hiển thị tất cả để tải lại.
16. “Quản lý lớp”: thêm lớp trống, sửa tên, xóa lớp trống; thử xóa KTPM01 đang có sinh viên → phải bị chặn.
17. Đóng ứng dụng rồi chạy lại. Dữ liệu mẫu được khởi tạo lại vì DataList chỉ lưu trong bộ nhớ.

18. Tab từ ô mã: tên → lớp → ngày sinh → nhóm giới tính → điểm → email → điện thoại → trạng thái → các nút CRUD. Trong nhóm giới tính dùng phím mũi tên để chọn Nam/Nữ.
19. Bấm Đóng hoặc X của cửa sổ chính: No giữ cửa sổ, Yes thoát. Mặc định là No.
20. Để trống nhiều trường và nhập điểm `abc`, bấm Thêm: phải xuất hiện đồng thời lỗi chuỗi điểm và lỗi các trường còn thiếu.
21. Tra mã của sinh viên thuộc lớp khác: combobox lớp và danh sách hiển thị phải đồng bộ với lớp sinh viên đó.


## Kiểm thử hồi quy v3: đổi mã khi đang thêm

22. Làm mới, nhập mã mới SV999999, Tab, điền tất cả thông tin. Quay lại ô mã đổi thành SV999998, Tab. Tất cả chi tiết phải giữ nguyên. Bấm Thêm để lưu với mã mới.
23. Điền bản nháp, xóa mã rồi Tab. Chi tiết vẫn còn. Bấm Thêm phải báo lỗi mã; không được xóa bản nháp.
24. Khi có bản nháp, đổi mã thành SV000123, Tab. Hộp thoại phải xuất hiện. No giữ toàn bộ bản nháp và báo mã trùng; đổi sang mã mới rồi Thêm vẫn phải lưu được.
25. Lặp lại bước 24 và chọn Yes: phải nạp Nguyễn Văn An, khóa ô mã, bật Sửa/Xóa và tắt Thêm.
26. Form trống sau Làm mới: nhập SV000123, Tab → nạp ngay sinh viên cũ, không cần xác nhận vì không có bản nháp.
27. Nhấn Làm mới và thêm thành công: vẫn xóa các ô nhập như trước. Sinh viên đã lưu vẫn không cho đổi MSSV trực tiếp ở chế độ Sửa.

Các bước này cần chạy thực tế trên Windows; kiểm thử console không kiểm tra control Windows Forms.
