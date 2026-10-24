# ADO.NET Migration Report — DataAccess từ DataList sang SQL Server

- **Ngày:** 2026-10-24
- **Kết quả chung: PASS** — build 0 lỗi/0 warning; **41/41 test PASS** chạy trên `QuanLySinhVienDB` thật tại `localhost\SQLEXPRESS`.
- **Không commit/push.**

## 1. Kiến trúc sau migration

```
Presentation (WinForms)  — không đổi UI
        │  SinhVienBUL / LopHocBUL — KHÔNG đổi (validation, exception flow giữ nguyên)
        ▼
DataAccess — LopHocDAL / SinhVienDAL: Microsoft.Data.SqlClient, 100% parameterized SQL
        ▼
SQL Server 2022 Express — localhost\SQLEXPRESS, DB QuanLySinhVienDB, Windows Auth
```

- **Xóa hoàn toàn `DataList`** khỏi DataAccess (không còn fallback im lặng — DAL đọc/ghi thẳng DB).
- `Snapshot` giữ lại (nội bộ DataAccess): dựng bản copy khi đọc để tầng ngoài không giữ tham chiếu tới nguồn — giữ hành vi "Read snapshots do not leak mutable source" của test.
- `Db.ConnectionString` (const, Windows Auth, **không có secret**): `Data Source=localhost\SQLEXPRESS;Initial Catalog=QuanLySinhVienDB;Integrated Security=True;TrustServerCertificate=True`.
- DAL constructor giờ nhận connection string (string, không lộ kiểu SqlClient ra API công khai).

## 2. File đã thay đổi (của task này)

| File | Thay đổi |
|---|---|
| `DataAccess/DataList.cs` | **Deleted** — nguồn dữ liệu in-memory loại bỏ |
| `DataAccess/Db.cs` | **New** — connection string dev local |
| `DataAccess/Snapshot.cs` | **New** — copy-read + mapping `SqlDataReader` → `SinhVien` (10 cột chuẩn, LEFT JOIN LopHoc) |
| `DataAccess/SinhVienDAL.cs` | Rewrite SQL-backed, public API giữ nguyên (GetAllSinhVien, GetSinhVienByMaSV, GetSinhVienById, GetSinhViensByMaLop, TimKiem, Add/Update/Delete) |
| `DataAccess/LopHocDAL.cs` | Rewrite SQL-backed, public API giữ nguyên (GetAllLopHoc + DanhSachSinhVien, GetLopHocByMa, Add/Update/Delete) |
| `Presentation/Program.cs` | Composition root: `new LopHocDAL(Db.ConnectionString)` thay `new DataList()` |
| `Tests/Program.cs` | Chỉ 2 dòng khởi tạo DAL đổi sang `Db.ConnectionString`; toàn bộ logic test giữ nguyên |
| `database/QuanLySinhVien-schema.sql` | Seed đổi sang bộ mẫu v3 của DataList (KTPM01/AI01/KHDL01 + SV000123–126) — khớp điều kiện tiên quyết của Tests |

Các csproj (đã có sẵn `Microsoft.Data.SqlClient 7.1.1` trước task này từ git status ban đầu) **không đổi thêm**. Business layer **không đổi**. MainForm/LopHocForm **không đổi** (UI không đụng).

## 3. Điểm chính của DAL mới

- **Parameterized SQL 100%** — mọi câu lệnh dùng `SqlParameter` với **typed parameters** (`SqlDbType.NVarChar` + size, `SqlDbType.Date`, `SqlDbType.Decimal` + Precision 4/Scale 1; `Diem` NULL → `DBNull.Value`).
- **`using`/dispose đúng** cho `SqlConnection`/`SqlCommand`/`SqlDataReader` ở mọi path.
- **Affected-row check**: Insert/Update/Delete đều kiểm tra `ExecuteNonQuery() == 1`, không đạt → `InvalidOperationException` với đúng message của bản DataList.
- **Exception flow giữ nguyên**: `InvalidOperationException` ("Mã sinh viên đã tồn tại.", "Lớp học không tồn tại.", "Không thể xóa lớp đang có sinh viên.", …) ném từ DAL như cũ; `SqlException` không bị nuốt — lan DataAccess → Business → View.
- `TimKiem`: WHERE `(@MaLop = N'' OR MaLop = @MaLop) AND Diem >= @DiemTu AND (MaSV/HoTen/Email/SoDienThoai LIKE '%@TuKhoa%')` — hành vi khớp bản DataList (LIKE không phân biệt hoa thường nhờ collation `Vietnamese_100_CI_AS` của cột, tương đương `CurrentCultureIgnoreCase`).
- Danh sách SV trong `LopHoc.DanhSachSinhVien` dựng bằng query riêng theo `MaLop` (giữ nguyên ngữ nghĩa của `Copy` cũ).

## 4. Verify (output thực tế)

**Build:** `dotnet build` — `Build succeeded. 0 Warning(s) 0 Error(s)` (cả 5 project: Entity, DataAccess, Business, Presentation, Tests).

**Tests:** `dotnet run --project Tests` — **`All 41 tests passed.`** bao gồm toàn bộ nhóm CRUD/validation/search chạy trên DB thật:
- Seed/lookup: "Seed has four students", "Class filtering", "Lookup normalized code" (`" sv000123 "` → uppercase/trim trước khi query)
- CRUD: add/update/delete sinh viên, add/update/delete lớp — đều vào DB thật
- Reject paths: duplicate mã, email/điểm/phone/trạng thái sai, birthday hôm nay, lớp không tồn tại — BUL chặn trước khi chạm DB, DB state không đổi ("Rejected adds do not change source")
- Snapshot: "Read snapshots do not leak mutable source"
- Search: "DAL search combines class, keyword and score" (TimKiem qua SQL LIKE + Diem >=)
- Ràng buộc DB: "Non-empty class deletion blocked"

**DB state sau test (sqlcmd):** đúng seed — 3 lớp (AI01, KTPM01, KHDL01), 4 SV (SV000123–126), không có dữ liệu test sót lại (test tự dọn: xóa SV009999/SV9999/TEST01).

**Unicode:** seed v3 MATCH qua query so literal (file .sql có BOM — bài học từ task setup DB trước).

## 5. Khác biệt nhỏ có chủ đích (ghi nhận, không phải regression)

1. `KiemTraLop` trong bản DataList so khớp `MaLop` **case-sensitive** (dùng `==`), trong khi DB collation CI → mọi so khớp mã qua SQL là case-insensitive. Không ảnh hưởng thực tế vì BUL normalize toàn bộ mã về uppercase trước khi gọi DAL.
2. `TimKiem` dùng `LIKE '%...%'` — ký tự wildcard (`%`, `[`, `_`) trong từ khóa sẽ được hiểu là wildcard (bản DataList là literal `Contains`). Tests và UI không dùng ký tự này.
3. Thứ tự dòng đọc từ DB là thứ tự PK (không phải thứ tự insert như DataList). Tests/UI không phụ thuộc thứ tự.
4. Nhãn UI `"Tìm tại nguồn DataList (qua Business → DAL)"` và vài comment trong MainForm vẫn nhắc "DataList" — là text/comment, không phải behavior; giữ nguyên theo yêu cầu "Do not redesign the UI".

## 6. Giới hạn

- WinForms app chưa chạy được tự động headless — xác nhận CRUD qua test console (cùng BUL/DAL) + sqlcmd; UI dùng chính BUL này.
- Tests chạy trực tiếp trên DB thật (không transaction rollback): nếu test fail giữa chừng có thể để lại dữ liệu test; hiện trạng sạch đã xác minh.
- `MySql.Data 26.7.0` trong csproj là thay đổi chưa commit có trước từ work khác (QLTVDAO) — không đụng theo scope.
