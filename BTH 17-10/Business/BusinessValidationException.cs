using System.ComponentModel.DataAnnotations;
namespace QuanLySinhVien.Business;
// Mang tên thuộc tính để Presentation gắn ErrorProvider đúng control.
public sealed class BusinessValidationException : Exception
{
    public IReadOnlyList<ValidationResult> Errors { get; }
    public BusinessValidationException(IEnumerable<ValidationResult> errors) : base("Thông tin chưa hợp lệ.") => Errors = errors.ToList().AsReadOnly();
}
internal static class ValidationHelper
{
    internal static void ThrowIfAny(List<ValidationResult> errors)
    {
        if (errors.Count > 0) throw new BusinessValidationException(errors);
    }
}
