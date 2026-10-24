using System.ComponentModel.DataAnnotations;
namespace QuanLySinhVien.Entity;
public sealed class LopHoc
{
    [Required(ErrorMessage = "Mã lớp không được để trống.")]
    [StringLength(20, ErrorMessage = "Mã lớp tối đa 20 ký tự.")]
    public string MaLop { get; set; } = "";
    [Required(ErrorMessage = "Tên lớp không được để trống.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên lớp phải có 3–100 ký tự.")]
    public string TenLop { get; set; } = "";
    // Quan hệ một lớp — nhiều sinh viên.
    public List<SinhVien> DanhSachSinhVien { get; set; } = new();
    // Theo yêu cầu Bài 1 và cách gọi sv.IsInValid() trong code mẫu.
    // Danh sách rỗng nghĩa là hợp lệ; mỗi lỗi giữ MemberNames để gắn ErrorProvider.
    public List<ValidationResult> IsInValid()
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, new ValidationContext(this), results, validateAllProperties: true);
        return results;
    }
    public bool KiemTraHopLe(out List<ValidationResult> errors)
    {
        errors = IsInValid();
        return errors.Count == 0;
    }
    public override string ToString() => TenLop;
}
