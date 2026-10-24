using System.ComponentModel.DataAnnotations;
namespace QuanLySinhVien.Entity;
public sealed class SinhVien
{
    [Required(ErrorMessage = "Mã sinh viên không được để trống.")]
    [RegularExpression(@"^SV\d{4,6}$", ErrorMessage = "Mã phải có dạng SV và 4–6 chữ số, ví dụ SV0001 hoặc SV000123.")]
    public string MaSV { get; set; } = "";
    [Required(ErrorMessage = "Họ tên không được để trống.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải có 2–100 ký tự.")]
    public string HoTen { get; set; } = "";
    [NgaySinhHopLe]
    public DateTime NgaySinh { get; set; }
    [Required(ErrorMessage = "Hãy chọn giới tính.")]
    public string GioiTinh { get; set; } = "";
    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Điện thoại không được để trống.")]
    [RegularExpression(@"^\d{9,11}$", ErrorMessage = "Điện thoại phải có 9–11 chữ số.")]
    public string SoDienThoai { get; set; } = "";
    [Required(ErrorMessage = "Hãy nhập điểm.")]
    [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10.")]
    public decimal? Diem { get; set; }
    [Required(ErrorMessage = "Hãy chọn trạng thái.")]
    public string TrangThai { get; set; } = "";
    [Required(ErrorMessage = "Hãy chọn lớp học.")]
    public string MaLop { get; set; } = "";
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
    // Navigation chỉ được dựng khi đọc; khóa liên kết là MaLop.
    public LopHoc? LopHoc { get; set; }
}
public sealed class NgaySinhHopLeAttribute : ValidationAttribute
{
    public NgaySinhHopLeAttribute() => ErrorMessage = "Ngày sinh phải từ năm 1900 và nhỏ hơn ngày hiện tại.";
    public override bool IsValid(object? value) => value is DateTime d && d.Year >= 1900 && d.Date < DateTime.Today;
}
