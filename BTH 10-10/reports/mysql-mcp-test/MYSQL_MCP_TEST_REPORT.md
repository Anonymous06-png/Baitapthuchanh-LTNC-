# Báo cáo kiểm tra kết nối MySQL MCP (`mysql-bth`)

- **Ngày thực hiện:** 2026-10-17
- **Phạm vi:** Chỉ kiểm tra kết nối, không thay đổi dữ liệu hay tệp dự án.

## 1. Trạng thái MCP server

- MCP server `mysql-bth` **khả dụng và đã kết nối** ở cấp giao thức.
- Các công cụ MCP có sẵn:
  - `mcp__mysql-bth__execute_sql`
  - `mcp__mysql-bth__get_schema_info`
  - `mcp__mysql-bth__get_table_sample`
- Cấu hình: kết nối tới `127.0.0.1:3306`, người dùng `mcp_reader`, cơ sở dữ liệu `quanly_sinhvien`, chế độ SSL `DISABLED`.

## 2. Trạng thái kết nối MySQL

- **Kết nối thất bại** khi thực thi truy vấn.
- Lỗi thực tế nhận được (giống nhau ở cả hai lệnh gọi):

  > `Error executing query: Authentication plugin 'caching_sha2_password' reported error: Authentication requires secure connection.`

- **Nguyên nhân:** tài khoản MySQL sử dụng plugin `caching_sha2_password`, plugin này yêu cầu kết nối bảo mật (TLS). Trong khi đó MCP server cấu hình `MYSQL_SSL_MODE=DISABLED`.

## 3. Thông tin cơ sở dữ liệu và bảng

- Cơ sở dữ liệu đích được cấu hình: `quanly_sinhvien`.
- Danh sách bảng: **Chưa xác định** (truy vấn thất bại do lỗi xác thực).
- Lược đồ bảng `LopHoc`: **Chưa xác định**.
- Lược đồ bảng `SinhVien`: **Chưa xác định**.

## 4. Kết quả truy vấn mẫu

- Dữ liệu mẫu từ `LopHoc`: **Chưa xác định**.
- Dữ liệu mẫu từ `SinhVien`: **Chưa xác định**.
- Không có truy vấn nào trả về kết quả do lỗi xác thực ở bước kết nối.

## 5. Lỗi và cách chẩn đoán

- **Lỗi chính:** `Authentication requires secure connection` (plugin `caching_sha2_password`).
- **Cách khắc phục đề xuất (chưa thực hiện):**
  1. Đặt `MYSQL_SSL_MODE` thành `REQUIRED` (hoặc `PREFERRED`) trong cấu hình MCP rồi khởi động lại server — cách ưu tiên.
  2. Hoặc chuyển tài khoản sang plugin `mysql_native_password` (lưu ý plugin này đã bị loại bỏ từ MySQL 9).
- **Kiểm tra bổ sung:** chạy `SELECT user, host, plugin FROM mysql.user WHERE user='mcp_reader';` với quyền quản trị, và xác nhận SSL của server MySQL.

## 6. Kết luận

MCP server `mysql-bth` hoạt động bình thường, nhưng phiên MySQL chưa thể xác thực do cấu hình SSL bị tắt so với plugin xác thực hiện tại. Do đó toàn bộ thông tin về bảng và dữ liệu trong `quanly_sinhvien` chưa thể xác định.
