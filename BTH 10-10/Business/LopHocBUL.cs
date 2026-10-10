using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using QuanLySinhVien.DataAccess;
using QuanLySinhVien.Entity;
namespace QuanLySinhVien.Business;
public sealed class LopHocBUL
{
    private readonly LopHocDAL dal;
    public LopHocBUL(LopHocDAL dal) => this.dal = dal;
    public List<LopHoc> GetAllLopHoc() => dal.GetAllLopHoc();
    public LopHoc? GetLopHocByMa(string ma) => dal.GetLopHocByMa(Key(ma));
    public void AddLopHoc(LopHoc input)
    {
        var l = Normalize(input); var errors = l.IsInValid();
        if (dal.GetLopHocByMa(l.MaLop) != null) errors.Add(new ValidationResult("Mã lớp đã tồn tại.", new[] { nameof(LopHoc.MaLop) }));
        ValidationHelper.ThrowIfAny(errors); dal.AddLopHoc(l);
    }
    public void UpdateLopHoc(LopHoc input)
    {
        var l = Normalize(input); var errors = l.IsInValid();
        if (dal.GetLopHocByMa(l.MaLop) == null) errors.Add(new ValidationResult("Lớp không tồn tại.", new[] { nameof(LopHoc.MaLop) }));
        ValidationHelper.ThrowIfAny(errors); dal.UpdateLopHoc(l);
    }
    public void DeleteLopHoc(string ma)
    {
        var l = GetLopHocByMa(ma) ?? throw new InvalidOperationException("Lớp không tồn tại.");
        if (l.DanhSachSinhVien.Count > 0) throw new InvalidOperationException("Không thể xóa lớp đang có sinh viên. Hãy chuyển lớp hoặc xóa sinh viên trước.");
        dal.DeleteLopHoc(l.MaLop);
    }
    private static string Key(string? s) => (s ?? "").Trim().ToUpperInvariant();
    private static LopHoc Normalize(LopHoc l) => new() { MaLop = Key(l.MaLop), TenLop = Regex.Replace((l.TenLop ?? "").Trim(), @"\s+", " ") };
}
