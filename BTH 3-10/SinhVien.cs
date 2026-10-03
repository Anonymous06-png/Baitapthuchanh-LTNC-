using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien_;

public sealed class SinhVien
{
    [Required(ErrorMessage = "Mã sinh viên không được để trống.")]
    [RegularExpression(@"^SV\d{6}$",
        ErrorMessage = "Mã sinh viên phải có dạng SV và 6 chữ số, ví dụ SV000123.")]
    public string MaSV { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên không được để trống.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Họ tên phải có từ 2 đến 100 ký tự.")]
    public string HoTen { get; set; } = string.Empty;

    [NgaySinhHopLe]
    public DateTime NgaySinh { get; set; }

    [Required(ErrorMessage = "Bạn phải chọn giới tính.")]
    public string GioiTinh { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Điện thoại không được để trống.")]
    [RegularExpression(@"^\d{9,11}$",
        ErrorMessage = "Điện thoại phải gồm từ 9 đến 11 chữ số.")]
    public string DienThoai { get; set; } = string.Empty;

    [Range(0, 10, ErrorMessage = "Điểm phải nằm trong khoảng từ 0 đến 10.")]
    public decimal Diem { get; set; }

    [Required(ErrorMessage = "Trạng thái không được để trống.")]
    public string TrangThai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lớp học không được để trống.")]
    public string MaLop { get; set; } = string.Empty;

    // Phía n-1: mỗi sinh viên thuộc một lớp.
    [Required(ErrorMessage = "Sinh viên phải thuộc một lớp học.")]
    public LopHoc? LopHoc { get; set; }

    public bool KiemTraHopLe(out List<string> danhSachLoi)
    {
        List<ValidationResult> ketQua = new();
        ValidationContext context = new(this);
        bool hopLe = Validator.TryValidateObject(this, context, ketQua, true);
        danhSachLoi = ketQua.Select(x => x.ErrorMessage ?? "Dữ liệu không hợp lệ.").ToList();
        return hopLe;
    }
}

public sealed class NgaySinhHopLeAttribute : ValidationAttribute
{
    public NgaySinhHopLeAttribute()
    {
        ErrorMessage = "Ngày sinh phải nhỏ hơn ngày hiện tại.";
    }

    public override bool IsValid(object? value)
    {
        return value is DateTime ngaySinh
               && ngaySinh.Date < DateTime.Today
               && ngaySinh.Year >= 1900;
    }
}
