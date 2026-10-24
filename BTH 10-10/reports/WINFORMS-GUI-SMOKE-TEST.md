# WinForms GUI Smoke Test Report — Quản lý sinh viên (ADO.NET + QuanLySinhVienDB)

- **Ngày:** 2026-10-24
- **Cách test:** điều khiển GUI thật bằng Computer Use (accessibility + click thật), chạy trên app build `Presentation/bin/Debug/net8.0-windows/QuanLySinhVien.Presentation.exe`, DB thật `QuanLySinhVienDB` tại `localhost\SQLEXPRESS` (Windows Auth). Mỗi thao tác CRUD được đối chiếu cả trên grid lẫn bằng `sqlcmd`.
- **Kết quả chung: PASS** — 11/12 mục PASS, 1 mục (connection-failure trực tiếp) không kiểm tra được do thiếu quyền admin, đã ghi nhận kèm quan sát code.

## 1. Kết quả từng mục

| # | Mục | Kết quả | Bằng chứng |
|---|---|---|---|
| 1 | App khởi động, load lớp/SV từ SQL Server | **PASS** | Grid hiển thị đúng 4 SV (SV000123–126) + 3 lớp, tên tiếng Việt chuẩn (đọc cả qua a11y lẫn screenshot); app mới mở lần 2 cũng load đủ 5/5 dòng khi có SV test |
| 2 | Lọc theo lớp | **PASS** | Chọn `Lọc lớp = Kỹ thuật phần mềm 01` → grid còn đúng 2 SV (SV000123, SV000125), label "Hiển thị: 2 sinh viên"; chọn ngược trong `Lớp học` cũng đồng bộ `Lọc lớp` (behavior sync giữ nguyên) |
| 3 | Tìm kiếm | **PASS** | Từ khóa "Minh" + radio "Tìm tại nguồn" (SQL `TimKiem`) → đúng 1 dòng SV000124 Trần Minh Anh |
| 4 | Thêm sinh viên | **PASS** | SV007777 / "Nguyễn GUI Test" / AI01 / 8.8 → dialog "Thêm thành công." → xuất hiện trên grid **và trong DB** (sqlcmd: 5 dòng) |
| 5 | Sửa sinh viên | **PASS** | Chọn dòng SV007777 → sửa Điểm 8.8 → 9.9 → confirm "Bạn có chắc muốn sửa…?" Yes → "Sửa thành công." → **DB: `Diem = 9.9`** |
| 6 | Xóa sinh viên | **PASS** | Dialog đúng text "Bạn có chắc muốn xóa sinh viên này?" → Yes → mất khỏi grid, **DB: SV007777 DELETED, còn 4 dòng** |
| 7 | Validation + ErrorProvider | **PASS** | Bấm Thêm khi form thiếu dữ liệu → ErrorProvider hiện đúng từng lỗi (đọc trực tiếp từ a11y tree): "Mã phải có dạng SV và 4–6 chữ số…", "Họ tên không được để trống.", "Ngày sinh phải từ năm 1900…", "Hãy chọn giới tính." + label "Thông tin chưa hợp lệ — di chuột vào dấu lỗi…". Không crash, không ghi DB |
| 8 | Đổi mã draft KHÔNG xóa dữ liệu đã nhập (v3) | **PASS** | Nhập draft đầy đủ (tên/email/phone/điểm/lớp/trạng thái/ngày sinh), đổi Mã SV SV007777 → SV007778 rồi rời ô → **toàn bộ field còn nguyên** (screenshot), Thêm vẫn bật |
| 9 | Đổi draft sang mã ĐÃ TỒN TẠI giữ confirm v3 | **PASS** | Đổi mã sang SV000123 khi draft còn → dialog "Mã sinh viên này đã tồn tại. Bạn có muốn bỏ thông tin đang nhập…?" (Yes/No, default No) → Yes → form load SV000123 (Nguyễn Văn An) từ DB, Mã SV thành read-only, Thêm disabled, Sửa/Xóa enabled |
| 10 | Thêm/Sửa/Xóa lớp | **PASS** | Quản lý lớp: thêm GUI01 (Sĩ số 0) → sửa tên thành "Lớp GUI Đã Sửa" (confirm Yes) → xóa GUI01 → mất khỏi grid; DB sau test về đúng 3 lớp seed |
| 11 | Chặn xóa lớp còn SV qua View | **PASS** | Chọn KTPM01 (2 SV) → Xóa → Yes → warning "Không thể xóa lớp đang có sinh viên. Hãy chuyển lớp hoặc xóa sinh viên trước.", lớp còn nguyên, **app không crash** (path DAL `InvalidOperationException` → View) |
| 12 | Lỗi DB (mất kết nối) xử lý tại View | **KHÔNG TEST ĐƯỢC TRỰC TIẾP** | `Stop-Service MSSQL$SQLEXPRESS` bị từ chối (shell không có quyền admin) — không thể ngắt kết nối thật khi app đang chạy. Xem mục 3 |

## 2. Vấn đề phát hiện

1. **(Nhẹ, không phải lỗi chức năng) Text UI còn nhắc "DataList" sau khi đã migrate sang SQL Server:**
   - Radio: "Tìm tại nguồn **DataList** (qua Business →DAL)" — nguồn thật giờ là SQL Server.
   - Dialog thoát app: "…Dữ liệu **DataList** của lần chạy này sẽ không được lưu." — thực tế dữ liệu đã lưu DB ngay khi thao tác.
   - Không sửa trong task này theo yêu cầu "Do not redesign the UI".
2. **(Nhẹ) View chưa catch `SqlException` riêng:** `LuuSinhVien` chỉ catch `BusinessValidationException` + `InvalidOperationException`; các hàm load/tìm không catch. Nếu SQL Server mất kết nối, `SqlException` sẽ đi tới WinForms `ThreadExceptionDialog` (app vẫn được Continue, không hard-crash, nhưng message kỹ thuật thay vì thông báo thân thiện). Không sửa trong task này.
3. **(Quan sát automation, không phải bug app):** click kiểu accessibility vào nút "Xóa" một lần bị lệch target khiến hộp thoại xác nhận thoát app xuất hiện và app đóng đúng thủ tục (có hỏi "Bạn có muốn thoát không?"). Chạy lại với đọc text dialog trước khi bấm thì flow xóa chuẩn. App tự hỏi trước khi thoát = behavior đúng.

## 3. Connection-failure — quan sát tĩnh (không chạy được live)

Không có quyền admin để dừng service, nên không mô phỏng mất kết nối thật. Theo code hiện tại: các thao tác lưu đã có catch `InvalidOperationException` (DB rule) nhưng chưa có catch `SqlException`; rủi ro là dialog lỗi kỹ thuật thay vì thông báo đẹp — **không crash cứng app** (WinForms default ThreadExceptionDialog cho phép Continue). Đề xuất step sau: thêm catch `SqlException` trong View (hoặc global handler) khi được phép sửa code.

## 4. Dữ liệu sau test (sqlcmd)

- `LopHoc`: đúng 3 lớp seed — AI01, KTPM01, KHDL01 (GUI01 đã xóa sạch).
- `SinhVien`: đúng 4 SV seed — SV000123 (8.5), SV000124 (9.0), SV000125 (7.4), SV000126 (8.1) (SV007777 đã xóa).
- **Không còn dữ liệu test sót lại.**

## 5. Scope tuân thủ

- Không đổi code/UI, không refactor, không commit/push. Chỉ chạy app build sẵn + thao tác GUI + query đọc bằng sqlcmd.
- App đã đóng sau test (Stop-Process; dữ liệu đã lưu DB nên không mất gì).
