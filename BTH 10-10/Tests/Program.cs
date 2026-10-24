using QuanLySinhVien.Business;
using QuanLySinhVien.DataAccess;
using QuanLySinhVien.Entity;

var data = new DataList();
var ld = new LopHocDAL(data); var sd = new SinhVienDAL(data);
var lop = new LopHocBUL(ld); var sv = new SinhVienBUL(sd, ld);
int passed = 0;
void Check(bool ok, string title) { if (!ok) throw new Exception("FAIL: " + title); Console.WriteLine("PASS: " + title); passed++; }
void Invalid(Action action, string member, string title)
{
    try { action(); throw new Exception("Expected validation: " + title); }
    catch (BusinessValidationException ex) { Check(ex.Errors.Any(e => e.MemberNames.Contains(member)), title); }
}
SinhVien Valid(string ma = "SV009999") => new()
{
    MaSV = ma, HoTen = "  Nguyễn   Văn   Test  ", NgaySinh = new DateTime(2005, 1, 1), GioiTinh = "Nam",
    Email = "test@example.com", SoDienThoai = "0912345678", Diem = 8.5m, TrangThai = "Đang học", MaLop = "KTPM01"
};
Check(sv.GetAllSinhVien().Count == 4, "Seed has four students");
Check(sv.GetSinhVienByMaLop("ktpm01").Count == 2, "Class filtering");
Check(sv.GetSinhVienByMaSV(" sv000123 ")?.HoTen == "Nguyễn Văn An", "Lookup normalized code");
Check(sv.GetSinhVienByMaSV("SV888888") == null, "Unknown code returns null");
sv.AddSinhVien(Valid());
Check(sv.GetAllSinhVien().Count == 5, "Student add");
Check(sv.GetSinhVienByMaSV("SV009999")?.HoTen == "Nguyễn Văn Test", "Whitespace normalization");
Invalid(() => sv.AddSinhVien(Valid()), "MaSV", "Duplicate code rejected");
var bad = Valid("SV008888"); bad.Email = "abc";
Invalid(() => sv.AddSinhVien(bad), "Email", "Invalid email reported by member");
bad = Valid("SV008888"); bad.Diem = 11;
Invalid(() => sv.AddSinhVien(bad), "Diem", "Out-of-range score rejected");
bad = Valid("SV008888"); bad.Diem = null;
Invalid(() => sv.AddSinhVien(bad), "Diem", "Missing score rejected");
bad = Valid("SV008888"); bad.NgaySinh = DateTime.Today;
Invalid(() => sv.AddSinhVien(bad), "NgaySinh", "Future or today birthday rejected");
bad = Valid("SV008888"); bad.MaLop = "MISSING";
Invalid(() => sv.AddSinhVien(bad), "MaLop", "Unknown class rejected");
bad = Valid("SV008888"); bad.GioiTinh = "";
Invalid(() => sv.AddSinhVien(bad), "GioiTinh", "Missing gender rejected");
bad = Valid("SV008888"); bad.SoDienThoai = "012abc";
Invalid(() => sv.AddSinhVien(bad), "SoDienThoai", "Invalid phone rejected");
bad = Valid("SV008888"); bad.TrangThai = "Unknown";
Invalid(() => sv.AddSinhVien(bad), "TrangThai", "Invalid status rejected");
Check(sv.GetAllSinhVien().Count == 5, "Rejected adds do not change source");
var update = Valid(); update.MaLop = "AI01"; update.HoTen = "Tên mới";
sv.UpdateSinhVien(update);
Check(sv.GetSinhVienByMaSV(update.MaSV)?.HoTen == "Tên mới", "Student update");
Check(lop.GetLopHocByMa("KTPM01")?.DanhSachSinhVien.Count == 2 && lop.GetLopHocByMa("AI01")?.DanhSachSinhVien.Count == 2, "Class relations after transfer");
update.Email = "bad";
Invalid(() => sv.UpdateSinhVien(update), "Email", "Invalid update rejected");
Check(sv.GetSinhVienByMaSV(update.MaSV)?.Email == "test@example.com", "Rejected update keeps stored data");
Invalid(() => sv.UpdateSinhVien(Valid("SV008888")), "MaSV", "Missing student cannot update");
var copy = sv.GetSinhVienByMaSV("SV000123")!; copy.HoTen = "Unauthorized change"; copy.LopHoc!.TenLop = "Changed";
Check(sv.GetSinhVienByMaSV("SV000123")?.HoTen == "Nguyễn Văn An" && lop.GetLopHocByMa("KTPM01")?.TenLop == "Kỹ thuật phần mềm 01", "Read snapshots do not leak mutable source");
Check(sv.TimKiemTaiNguon("AI01", "Tên mới", 8).Count == 1, "DAL search combines class, keyword and score");
sv.DeleteSinhVien("SV009999");
Check(sv.GetSinhVienByMaSV("SV009999") == null && sv.GetAllSinhVien().Count == 4, "Student deletion");
try { lop.DeleteLopHoc("KTPM01"); throw new Exception("Expected blocked deletion"); }
catch (InvalidOperationException) { Check(true, "Non-empty class deletion blocked"); }
lop.AddLopHoc(new LopHoc { MaLop = " test01 ", TenLop = "  Lớp   Test  " });
Check(lop.GetLopHocByMa("TEST01")?.TenLop == "Lớp Test", "Class add and normalization");
lop.UpdateLopHoc(new LopHoc { MaLop = "TEST01", TenLop = "Lớp cập nhật" });
Check(lop.GetLopHocByMa("TEST01")?.TenLop == "Lớp cập nhật", "Class update");
Invalid(() => lop.AddLopHoc(new LopHoc { MaLop = "TEST01", TenLop = "Lớp trùng" }), "MaLop", "Duplicate class rejected");
lop.DeleteLopHoc("TEST01"); Check(lop.GetLopHocByMa("TEST01") == null, "Empty class deletion");
var shortCode = Valid("SV9999"); sv.AddSinhVien(shortCode);
Check(sv.GetSinhVienByMaSV("SV9999") != null, "Sample four-digit student code supported");
sv.DeleteSinhVien("SV9999");
Check(Valid().IsInValid().Count == 0, "Entity IsInValid accepts valid student");
Check(Valid().KiemTraHopLe(out var validErrors) && validErrors.Count == 0, "Entity boolean validation method");
var empty = new SinhVien();
var entityErrors = empty.IsInValid();
Check(entityErrors.Any(e => e.MemberNames.Contains("MaSV")) && entityErrors.Any(e => e.MemberNames.Contains("SoDienThoai")) && entityErrors.Any(e => e.MemberNames.Contains("NgaySinh")), "Entity invalid fields preserve MemberNames");
Check(!empty.KiemTraHopLe(out var emptyErrors) && emptyErrors.Count > 0, "Entity boolean rejects empty student");
Check(new LopHoc().IsInValid().Count == 2, "Class entity validates required fields");
Check(new LopHoc { MaLop = "L01", TenLop = "Lớp một" }.KiemTraHopLe(out var classErrors) && classErrors.Count == 0, "Class entity boolean validation");
try { sv.AddSinhVien(empty); throw new Exception("Expected multiple validation errors"); }
catch (BusinessValidationException ex)
{
    Check(new[] { "MaSV", "HoTen", "NgaySinh", "GioiTinh", "Email", "SoDienThoai", "Diem", "MaLop", "TrangThai" }
        .All(member => ex.Errors.Any(e => e.MemberNames.Contains(member))), "BUL reports all nine missing fields");
}
Check(sd.GetSinhVienById("SV000123")?.MaSV == "SV000123", "DAL sample lookup alias");
var failed = Valid("SV9998"); failed.Email = "bad"; failed.Diem = -1; failed.SoDienThoai = "abc";
try { sv.AddSinhVien(failed); throw new Exception("Expected multiple invalid values"); }
catch (BusinessValidationException ex)
{
    Check(new[] { "Email", "Diem", "SoDienThoai" }.All(member => ex.Errors.Any(e => e.MemberNames.Contains(member))), "Multiple field errors returned together");
}
int beforeValidate = sv.GetAllSinhVien().Count;
Check(sv.KiemTraDuLieu(Valid("SV9988")).Count == 0 && sv.GetAllSinhVien().Count == beforeValidate, "Business pre-validation does not write data");
Check(sv.KiemTraDuLieu(Valid("SV000123")).Any(e => e.MemberNames.Contains("MaSV")), "Business pre-validation detects duplicate code");
Console.WriteLine($"All {passed} tests passed.");
