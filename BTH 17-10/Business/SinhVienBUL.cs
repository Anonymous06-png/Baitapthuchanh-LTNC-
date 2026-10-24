using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using QuanLySinhVien.DataAccess;
using QuanLySinhVien.Entity;
namespace QuanLySinhVien.Business;
public sealed class SinhVienBUL
{
    private readonly SinhVienDAL svd;
    private readonly LopHocDAL lopd;
    public SinhVienBUL(SinhVienDAL svd, LopHocDAL lopd) { this.svd = svd; this.lopd = lopd; }
    public List<SinhVien> GetAllSinhVien() => svd.GetAllSinhVien();
    public List<SinhVien> GetSinhVienByMaLop(string ma) => svd.GetSinhViensByMaLop(NormalizeKey(ma));
    public SinhVien? GetSinhVienByMaSV(string ma) => svd.GetSinhVienByMaSV(NormalizeKey(ma));
    public List<SinhVien> TimKiemTaiNguon(string maLop, string tuKhoa, decimal diemTu)
    {
        if (diemTu < 0 || diemTu > 10) throw new ArgumentOutOfRangeException(nameof(diemTu));
        return svd.TimKiem(NormalizeKey(maLop), tuKhoa.Trim(), diemTu);
    }
    // Cho UI yêu cầu validation trước hộp thoại xác nhận; không ghi dữ liệu.
    public List<ValidationResult> KiemTraDuLieu(SinhVien input, bool sua = false)
    {
        var s = Normalize(input);
        var errors = Validate(s);
        bool exists = svd.GetSinhVienByMaSV(s.MaSV) != null;
        if (!sua && exists)
            errors.Add(new ValidationResult("Mã sinh viên đã tồn tại.", new[] { nameof(SinhVien.MaSV) }));
        if (sua && !exists)
            errors.Add(new ValidationResult("Không tìm thấy sinh viên cần sửa.", new[] { nameof(SinhVien.MaSV) }));
        return errors;
    }
    public void AddSinhVien(SinhVien input)
    {
        var s = Normalize(input);
        ValidationHelper.ThrowIfAny(KiemTraDuLieu(s));
        svd.AddSinhVien(s);
    }
    public void UpdateSinhVien(SinhVien input)
    {
        var s = Normalize(input);
        ValidationHelper.ThrowIfAny(KiemTraDuLieu(s, sua: true));
        svd.UpdateSinhVien(s);
    }
    public void DeleteSinhVien(string ma) => svd.DeleteSinhVien(NormalizeKey(ma));
    private List<ValidationResult> Validate(SinhVien s)
    {
        var errors = s.IsInValid();
        if (s.MaLop.Length > 0 && lopd.GetLopHocByMa(s.MaLop) == null)
            errors.Add(new ValidationResult("Lớp học không tồn tại.", new[] { nameof(SinhVien.MaLop) }));
        if (s.GioiTinh.Length > 0 && s.GioiTinh != "Nam" && s.GioiTinh != "Nữ")
            errors.Add(new ValidationResult("Giới tính phải là Nam hoặc Nữ.", new[] { nameof(SinhVien.GioiTinh) }));
        if (s.TrangThai.Length > 0 && s.TrangThai != "Đang học" && s.TrangThai != "Bảo lưu" && s.TrangThai != "Đã tốt nghiệp")
            errors.Add(new ValidationResult("Trạng thái không hợp lệ.", new[] { nameof(SinhVien.TrangThai) }));
        return errors;
    }
    private static string NormalizeKey(string? s) => (s ?? "").Trim().ToUpperInvariant();
    private static SinhVien Normalize(SinhVien s) => new()
    {
        MaSV = NormalizeKey(s.MaSV), HoTen = Regex.Replace((s.HoTen ?? "").Trim(), @"\s+", " "),
        NgaySinh = s.NgaySinh.Date, GioiTinh = (s.GioiTinh ?? "").Trim(),
        Email = (s.Email ?? "").Trim(), SoDienThoai = (s.SoDienThoai ?? "").Trim(),
        Diem = s.Diem, TrangThai = (s.TrangThai ?? "").Trim(), MaLop = NormalizeKey(s.MaLop)
    };
}
