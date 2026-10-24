namespace QuanLySinhVien.Presentation;
// Thông báo lỗi database ngắn gọn cho người dùng cuối.
// Không hiển thị connection string, thông tin đăng nhập, câu SQL hay stack trace.
internal static class DatabaseError
{
    public const string ThongDiep = "Không làm việc được với cơ sở dữ liệu. Hãy kiểm tra SQL Server (SQLEXPRESS) đang chạy rồi thử lại.";
    public static void ThongBao() => MessageBox.Show(ThongDiep, "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
