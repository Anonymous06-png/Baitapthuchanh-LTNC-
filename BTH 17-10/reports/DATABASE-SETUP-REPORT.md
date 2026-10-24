# Database Setup Report — QuanLySinhVienDB

- **Ngày:** 2026-10-24
- **Server:** `localhost\SQLEXPRESS` (SQL Server 2022 Express, 16.0.1000.6) — Windows Authentication
- **Kết quả chung: PASS**

## 1. Input — đọc từ code hiện tại

| File | Nội dung rút ra |
|---|---|
| `Entity/LopHoc.cs` | MaLop (string, ≤20, required), TenLop (3–100), quan hệ 1 lớp — nhiều SV |
| `Entity/SinhVien.cs` | MaSV regex `^SV\d{4,6}$`, HoTen 2–100, NgaySinh (DateTime, ≥1900 & < hôm nay), GioiTinh, Email, SoDienThoai `^\d{9,11}$`, Diem `decimal?` 0–10, TrangThai, MaLop FK |
| `DataAccess/LopHocDAL.cs` | So khớp mã **không phân biệt hoa thường** (OrdinalIgnoreCase); chặn xóa lớp còn SV |
| `DataAccess/SinhVienDAL.cs` | Tra cứu theo MaLop (phục vụ index), kiểm tra lớp tồn tại khi add/update |
| `Presentation/MainForm.cs` | Giá trị GioiTinh: `Nam`/`Nữ`; TrangThai: `Đang học`/`Bảo lưu`/`Đã tốt nghiệp` (thêm 1 dòng grep, không scan repo) |

## 2. Thiết kế schema

Database: **`QuanLySinhVienDB`**, collation `Vietnamese_100_CI_AS` (CI khớp OrdinalIgnoreCase của DAL, hỗ trợ Unicode tiếng Việt).

**dbo.LopHoc**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| MaLop | nvarchar(20) | PK |
| TenLop | nvarchar(100) | CHECK LEN ≥ 3 |

**dbo.SinhVien**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| MaSV | nvarchar(8) | PK; CHECK `SV` + 4–6 chữ số (mirror regex) |
| HoTen | nvarchar(100) | CHECK LEN ≥ 2 |
| NgaySinh | **date** | CHECK ≥ `1900-01-01` và < ngày hiện tại |
| GioiTinh | nvarchar(10) | CHECK IN (`Nam`, `Nữ`) |
| Email | nvarchar(100) | CHECK LIKE `%_@_%._%` (xấp xỉ EmailAddress) |
| SoDienThoai | nvarchar(11) | CHECK 9–11 chữ số |
| Diem | **decimal(4,1)**, NULL | CHECK 0–10 (entity dùng `decimal?`) |
| TrangThai | nvarchar(20) | CHECK IN 3 giá trị UI cho phép |
| MaLop | nvarchar(20) | FK → LopHoc, **ON DELETE NO ACTION** = business rule "không xóa lớp còn SV" |

Index: `IX_SinhVien_MaLop` (phục vụ `GetSinhViensByMaLop`).

Không có thay đổi business rule hiện tại: các CHECK chỉ mirror validation attribute + tập giá trị UI.

## 3. Script

- File: `database/QuanLySinhVien-schema.sql` (UTF-8 **có BOM** — bắt buộc để sqlcmd đọc đúng Unicode tiếng Việt)
- Idempotent: `IF DB_ID(...) IS NULL` / `IF OBJECT_ID(...) IS NULL` / seed `IF NOT EXISTS` — chạy lại không nhân đôi dữ liệu.
- Lệnh chạy: `sqlcmd -S localhost\SQLEXPRESS -E -d master -i database/QuanLySinhVien-schema.sql`

## 4. Lỗi gặp phải trong quá trình (đã sửa)

1. **`CK_SinhVien_MaSV` sai logic (script v1):** `MaSV NOT LIKE N'%[^0-9]%'` áp cả prefix `SV` nên mọi mã hợp lệ đều bị chặn → sửa thành chỉ kiểm tra phần sau `SV` bằng `SUBSTRING(MaSV, 3, LEN(MaSV)-2) NOT LIKE N'%[^0-9]%'`.
2. **Seed Unicode bị corrupt (lần chạy đầu không BOM):** sqlcmd đọc file UTF-8-thiếu-BOM theo ANSI → `Nguyễn Văn An` thành mojibake. Xác minh bằng query so literal (`MISMATCH`), sửa bằng thêm BOM vào script, drop bảng `SinhVien` (bảng rỗng, DB mới tạo) và chạy lại.
3. **Constraint mojibake sót lại:** `CK_SinhVien_TrangThai` được tạo từ lần chạy không-BOM nên literal hỏng, chặn seed đúng → đã tự hết khi drop & recreate bảng từ file có BOM.
4. Sự cố phụ: `Start-Service` bị access denied (cần admin) nhưng service đã ở trạng thái Running; kết nối sqlcmd xác nhận server hoạt động.

## 5. Verify bằng query thật (output thực tế)

| Kiểm tra | Kết quả |
|---|---|
| `@@VERSION` qua `sqlcmd -E` | SQL Server 2022 (RTM) 16.0.1000.6 Express, `LAPTOP-214UR8HL\SQLEXPRESS` |
| Số bảng / dòng | `LopHoc` = 2, `SinhVien` = 4 |
| FK | `FK_SinhVien_LopHoc`: Child `SinhVien` → Parent `LopHoc`, OnDelete `NO_ACTION` |
| CHECK constraints | 8 constraint trên `SinhVien` (Diem, Email, GioiTinh, HoTen, MaSV, NgaySinh, SoDienThoai, TrangThai) |
| Kiểu dữ liệu | `NgaySinh` = date; `Diem` = decimal(4,1); `MaSV`/`HoTen` collation `Vietnamese_100_CI_AS` |
| Unicode integrity | `HoTen = N'Nguyễn Văn An'` → **MATCH**; `TenLop = N'Công nghệ thông tin K45'` → **MATCH** |
| FK chặn lớp không tồn tại | INSERT `MaLop = KHONG-TON-TAI` → **BLOCKED** (bắt lỗi 547) |
| Chặn xóa lớp còn SV | DELETE `CNTT-K45` → **BLOCKED** (bắt lỗi 547) |
| Regex MaSV chặn sai định dạng | INSERT `MaSV = XX01` → **BLOCKED** |
| Idempotency (chạy script lần 2) | Không lỗi, số dòng giữ nguyên 2 / 4 |

Mọi test insert/delete vi phạm đều bị chặn bởi constraint (không có dữ liệu test sót lại — tổng số dòng sau test vẫn 4 SV).

## 6. Scope đã tuân thủ

- Không sửa `master`, không DROP database có sẵn, không đụng DB khác.
- **Chưa migrate ADO.NET** (DAL vẫn dùng DataList); **không commit/push**; không sửa source code C#.
- File mới: `database/QuanLySinhVien-schema.sql`, `reports/DATABASE-SETUP-REPORT.md`.

## 7. Next step đề xuất

Bước tiếp theo duy nhất: migrate `DataAccess` sang ADO.NET (`Microsoft.Data.SqlClient`) — bắt đầu bằng viết `SinhVienDAL`/`LopHocDAL` phiên bản SQL (keep interface cũ, trả về entity hiện có), connection string dùng `Server=localhost\SQLEXPRESS;Database=QuanLySinhVienDB;Trusted_Connection=True;TrustServerCertificate=True`.
