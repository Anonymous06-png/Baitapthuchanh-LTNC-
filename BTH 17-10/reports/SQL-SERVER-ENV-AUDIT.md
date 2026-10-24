# Report: Trạng thái Repo + SQL Server trước khi migrate sang ADO.NET

- **Ngày audit:** 2026-10-24
- **Thư mục audit:** `BTH 10-10` (thư mục con của repo)
- **Scope:** chỉ audit repo + SQL Server + SSMS + sqlcmd + Zcode config. Không sửa source, không chỉnh SQL Server/TCP/firewall, không cài package, không tạo database, không commit/push.
- Tất cả giá trị dưới đây lấy từ command output thực tế đã chạy.

## A. Repo

| Mục | Giá trị (đã xác minh) |
|---|---|
| Repo root | `C:/Users/Ky/Documents/Baitapthuchanh-LTNC-` |
| Working dir hiện tại | `...\Baitapthuchanh-LTNC-\BTH 10-10` (thư mục con của repo) |
| Branch | `main` — tracking `origin/main`, không ahead/behind |
| Git status | Working tree **chưa sạch**: 5 file modified (`Business/QuanLySinhVien.Business.csproj`, `DataAccess/QuanLySinhVien.DataAccess.csproj`, `Entity/QuanLySinhVien.Entity.csproj`, `Presentation/QuanLySinhVien.Presentation.csproj`, `Tests/QuanLySinhVien.Tests.csproj`); 2 mục untracked: `QLTVDAO/`, `reports/` |
| Remote | `origin → https://github.com/Anonymous06-png/Baitapthuchanh-LTNC-.git` (fetch + push) |
| Latest commit | `8cec64e — Cap nhat tai lieu BTH 10-10` |

## B. SQL Server

| Mục | Giá trị (đã xác minh) |
|---|---|
| Engine đã cài | Có — qua registry `HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL`, chỉ **1 instance duy nhất**: `SQLEXPRESS` → `MSSQL16.SQLEXPRESS` (SQL Server **2022**) |
| Service | `MSSQL$SQLEXPRESS` — DisplayName "SQL Server (SQLEXPRESS)" |
| Status | **Stopped** |
| Instance thực tế | `SQLEXPRESS` (kết quả đọc registry trực tiếp, không mặc định trước khi kiểm tra) |
| Listener/port | **Không xác định được** — không có tiến trình `sqlservr` nào đang chạy (`NO_SQLSERVR_PROCESS`), nên không có TCP listener nào để liệt kê |

## C. SSMS

- **Installed: Yes** (xác minh qua uninstall registry keys, cả 64-bit và WOW6432Node)
- Version: **SQL Server Management Studio 22** — `22.11.0`
- Install location: `C:\Program Files\Microsoft SQL Server Management Studio 22\Release`

## D. SQL connection

- `sqlcmd` available: **Yes** — `C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE` (ODBC Driver 17)
- Windows Authentication query: **Failed** — đã test thực tế bằng lệnh:
  ```
  sqlcmd -S "localhost\SQLEXPRESS" -E -l 5 -W -Q "SELECT @@VERSION AS SqlVersion, SERVERPROPERTY('ServerName') AS ServerName, SERVERPROPERTY('InstanceName') AS InstanceName, DB_NAME() AS CurrentDatabase;"
  ```
- Lỗi kỹ thuật nguyên văn (không chứa secret — lệnh dùng `-E`, không có password nào trên dòng lệnh):
  ```
  Sqlcmd: Error: Microsoft ODBC Driver 17 for SQL Server : SQL Server Network Interfaces:
      Error Locating Server/Instance Specified [xFFFFFFFF].
  Sqlcmd: Error: Microsoft ODBC Driver 17 for SQL Server : Login timeout expired.
  Sqlcmd: Error: Microsoft ODBC Driver 17 for SQL Server : A network-related or
      instance-specific error has occurred while establishing a connection to SQL Server.
      Server is not found or not accessible.
  EXIT_CODE=1
  ```
- Nguyên nhân khớp chính xác với bằng chứng section B: service `MSSQL$SQLEXPRESS` đang **Stopped** nên không thể locate instance. Lỗi trạng thái service, không phải lỗi credentials.

## E. Zcode

- `C:\Users\Ky\.zcode\cli\config.json`: **tồn tại** (xác minh bằng file existence check)
- Chỉ kiểm tra sự tồn tại — **không đọc nội dung, không in secret/DSN/password**.
- Ghi nhận phụ từ memory cũ (không phải bằng chứng mới): MCP `mysql-bth` có vấn đề auth riêng (caching_sha2 cần TLS, config SSL DISABLED) — không liên quan SQL Server, chỉ ghi nhận.

## F. Kết luận

**Trạng thái: `SQL_SERVER_NOT_RUNNING`**

Căn cứ: mọi component môi trường đều đã cài (engine SQL 2022 Express, sqlcmd, SSMS 22) nhưng service `MSSQL$SQLEXPRESS` đang Stopped và truy vấn test qua Windows Authentication thất bại vì đúng lý do đó.

- Không phải `READY_FOR_DATABASE_SETUP` — chưa query được.
- Không phải `SQL_CONNECTION_NOT_VERIFIED` — đã verify, có lỗi xác định rõ nguyên nhân.
- Không phải `ENVIRONMENT_BLOCKED` — không thiếu component nào.

## G. Next step

**Một bước duy nhất** (ngoài scope audit này): khởi động service với quyền admin, rồi chạy lại đúng truy vấn `sqlcmd -S "localhost\SQLEXPRESS" -E` ở section D:

```powershell
Start-Service -Name "MSSQL`$SQLEXPRESS"
```

Nếu service start mà connection vẫn fail, khi đó mới xem xét TCP/IP/SQL Browser — hiện chưa có bằng chứng nào buộc phải làm vậy.

## Phụ lục: Tổng kết xác minh

### Các bước đã xác minh (có command output thực tế)

1. **Repo:** `git rev-parse --show-toplevel`, `git status --short --branch`, `git remote -v`, `git log -1 --oneline` — tất cả trả về kết quả hợp lệ.
2. **SQL Server service + registry instance:** `Get-Service MSSQL*` + `Get-ItemProperty` registry — thấy service Stopped, instance `SQLEXPRESS` (MSSQL16).
3. **Listener/port:** `Get-Process sqlservr` + `Get-NetTCPConnection` — xác nhận không có tiến trình → không có listener.
4. **SSMS:** uninstall registry query — thấy SSMS 22 (22.11.0).
5. **`sqlcmd`:** `Get-Command sqlcmd` — thấy tại ODBC 170 Tools.
6. **Windows Auth query:** chạy `sqlcmd -E` thật với login timeout 5s — **Failed** với lỗi nguyên văn ở trên.
7. **Zcode config:** file existence check — `CONFIG_EXISTS`.

### Các bước chưa xác minh

- TCP port / SQL Browser của instance — không thể xác minh khi service stopped (không cố, đúng scope).
- Truy vấn SQL thực sự qua Windows Authentication — thất bại như đã ghi, chưa từng Passed.
- Nội dung `C:\Users\Ky\.zcode\cli\config.json` — chủ động không đọc để tránh lộ secret.
- Hiện trạng database/schema trong SQL Server — chưa kết nối được, ngoài scope.

### Các thay đổi đã thực hiện

None
