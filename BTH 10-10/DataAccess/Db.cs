namespace QuanLySinhVien.DataAccess;
// Chuỗi kết nối dev local bằng Windows Authentication — không chứa secret.
// Có thể truyền chuỗi khác qua constructor DAL khi cần.
public static class Db
{
    public const string ConnectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=QuanLySinhVienDB;Integrated Security=True;TrustServerCertificate=True";
}
