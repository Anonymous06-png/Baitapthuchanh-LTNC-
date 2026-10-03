using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien_;

public sealed class LopHoc
{
    [Required(ErrorMessage = "Mã lớp không được để trống.")]
    [StringLength(20, ErrorMessage = "Mã lớp không được vượt quá 20 ký tự.")]
    public string MaLop { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên lớp không được để trống.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Tên lớp phải có từ 3 đến 100 ký tự.")]
    public string TenLop { get; set; } = string.Empty;

    // Quan hệ 1-n: một lớp có nhiều sinh viên.
    public List<SinhVien> DanhSachSinhVien { get; } = new();

    public bool KiemTraHopLe(out List<string> danhSachLoi)
    {
        List<ValidationResult> ketQua = new();
        ValidationContext context = new(this);
        bool hopLe = Validator.TryValidateObject(this, context, ketQua, true);
        danhSachLoi = ketQua.Select(x => x.ErrorMessage ?? "Dữ liệu không hợp lệ.").ToList();
        return hopLe;
    }

    public override string ToString() => TenLop;
}
