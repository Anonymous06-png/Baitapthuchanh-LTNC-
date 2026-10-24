# Final Cleanup Report — ADO.NET Migration (BTH 10-10)

- **Ngày:** 2026-10-24
- **Kết quả chung: PASS** — `dotnet build` 0 lỗi/0 warning; **41/41 test PASS** trên DB thật.
- **Không commit/push.**

## 1. Thay đổi của task này

### 1a. Bỏ text/comment còn nhắc "DataList" → wording SQL Server/database

| File | Trước | Sau |
|---|---|---|
| `Presentation/MainForm.cs` (comment đầu class) | "Không chứa DataList, không truy cập DAL…" | "Không truy cập DAL/database trực tiếp…" |
| `Presentation/MainForm.cs` (dialog thoát app) | "…Dữ liệu DataList của lần chạy này sẽ không được lưu." | "Bạn có muốn thoát không?" (dữ liệu đã lưu DB ngay khi thao tác) |
| `Presentation/MainForm.cs` (radio tìm kiếm) | "Tìm tại nguồn DataList (qua Business → DAL)" | "Tìm tại nguồn dữ liệu (qua Business → DAL)" |
| `DataAccess/LopHocDAL.cs` (2 comment) | "(thay DataList)" / "như bản DataList" | bỏ nhắc DataList |
| `DataAccess/SinhVienDAL.cs` (2 comment) | "(thay DataList)" / "của bản DataList" | bỏ nhắc DataList ("bản in-memory trước đây") |

Verify: `grep DataList --include=*.cs` trên 5 project → **0 kết quả** (riêng `Tests/Program.cs` không bao giờ nhắc).

### 1b. SqlException handling trong View (new `Presentation/DatabaseError.cs`)

Helper nội bộ `DatabaseError.ThongBao()` — MessageBox lỗi tiếng Việt ngắn gọn:
> "Không làm việc được với cơ sở dữ liệu. Hãy kiểm tra SQL Server (SQLEXPRESS) đang chạy rồi thử lại."

**Không lộ** connection string / credentials / raw SQL / stack trace / exception message.

Các path được phủ catch `SqlException`:

| Form | Path |
|---|---|
| `MainForm` | Load (NapComboLop + TaiDanhSachLop) |
| `MainForm` | `TaiDanhSachLop` (bên trong — phủ mọi reload: lọc lớp, Hiển thị tất cả, sau CRUD, sau đóng Quản lý lớp) |
| `MainForm` | `NapComboLop` (catch thêm trước `finally dangNapCombo = false` — guard combo không bị kẹt) |
| `MainForm` | `TraCuuMa` (lookup mã SV; reset `maDaTraCuu = null` để thử lại được) |
| `MainForm` | grid `CellClick` (load SV lên form) |
| `MainForm` | `LuuSinhVien` — cả phần pre-validation `KiemTraDuLieu` (lookup DB trước try cũ) lẫn phần save (catch mới trong try hiện hữu) |
| `MainForm` | `XoaSinhVien` |
| `MainForm` | `TimKiem` (nhánh "Tìm tại nguồn"; nhánh tìm trên giao diện không đụng DB — giữ nguyên) |
| `LopHocForm` | `RefreshList` (load danh sách lớp + Sĩ số) |
| `LopHocForm` | `Save` (thêm/sửa lớp) |
| `LopHocForm` | Xóa lớp |

**Giữ nguyên behavior hiện hữu:**
- `BusinessValidationException` → ErrorProvider gắn theo MemberNames (không đổi).
- `InvalidOperationException` → warning/message box với đúng message business (không đổi).
- Thứ tự catch: Business → InvalidOperation → SqlException.
- v3 draft/MaSV: `TraCuuMa` chỉ thêm catch quanh lookup; logic draft/confirm không đổi. `LuuSinhVien` tách biến `loi` nhưng flow giữ nguyên.

### 1c. Xóa `MySql.Data 26.7.0`

- Verify trước khi xóa: `grep MySql --include=*.cs` trên Entity/DataAccess/Business/Presentation/Tests → **0 file dùng**. `QLTVDAO/` (untracked) là project độc lập có csproj riêng, không tham chiếu các project này → không bị ảnh hưởng.
- Đã xóa `<PackageReference Include="MySql.Data" … />` khỏi **5 csproj**: Entity, DataAccess, Business, Presentation, Tests.
- `Microsoft.Data.SqlClient 7.1.1` giữ nguyên ở DataAccess + Presentation (Presentation cần kiểu `SqlException`) + Tests.

## 2. File thay đổi (của task này)

- `Presentation/MainForm.cs` — text/comment + SqlException handling (8 path)
- `Presentation/LopHocForm.cs` — using + SqlException handling (3 path)
- `Presentation/DatabaseError.cs` — **new** (helper thông báo lỗi DB)
- `DataAccess/LopHocDAL.cs`, `DataAccess/SinhVienDAL.cs` — chỉ comment
- `Entity/DataAccess/Business/Presentation/Tests …csproj` — xóa MySql.Data (5 file)

## 3. Build + Test (output thật)

- `dotnet build` → `Build succeeded. 0 Warning(s) 0 Error(s)` (5 project).
- `dotnet run --project Tests` → **`All 41 tests passed.`** (cùng bộ test CRUD/validation chạy trên `QuanLySinhVienDB` thật — bao phủ các path BUL/DAL mà View gọi; behavior không đổi).

## 4. Giới hạn

- Connection-failure qua GUI vẫn không mô phỏng được live (cần quyền admin để dừng SQL service — đã ghi trong smoke test report). Đường lỗi giờ được catch + hiện message thân thiện theo code path; đã build/test bằng bộ test hiện có.
- Không thêm global handler (`Application.ThreadException`) — ngoài scope tối thiểu; nếu muốn phủ mọi path có thể làm bước riêng.
