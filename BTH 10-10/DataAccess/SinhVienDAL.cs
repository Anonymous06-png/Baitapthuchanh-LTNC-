using System.Data;
using Microsoft.Data.SqlClient;
using QuanLySinhVien.Entity;
namespace QuanLySinhVien.DataAccess;
// Đọc/ghi trực tiếp SQL Server qua ADO.NET.
// Lỗi từ ADO.NET/SQL Server được để lan tự nhiên DataAccess → Business → View.
public sealed class SinhVienDAL
{
    private const string SelectSinhVien =
        "SELECT s.MaSV, s.HoTen, s.NgaySinh, s.GioiTinh, s.Email, s.SoDienThoai, s.Diem, s.TrangThai, s.MaLop, l.TenLop " +
        "FROM dbo.SinhVien s LEFT JOIN dbo.LopHoc l ON l.MaLop = s.MaLop";
    private readonly string connectionString;
    public SinhVienDAL(string connectionString) => this.connectionString = connectionString;
    private SqlConnection Open()
    {
        var con = new SqlConnection(connectionString);
        con.Open();
        return con;
    }
    public List<SinhVien> GetAllSinhVien()
    {
        using var con = Open();
        using var cmd = new SqlCommand(SelectSinhVien, con);
        using var r = cmd.ExecuteReader();
        var list = new List<SinhVien>();
        while (r.Read()) list.Add(Snapshot.SinhVien(r));
        return list;
    }
    public SinhVien? GetSinhVienByMaSV(string ma)
    {
        using var con = Open();
        using var cmd = new SqlCommand(SelectSinhVien + " WHERE s.MaSV = @MaSV", con);
        cmd.Parameters.Add("@MaSV", SqlDbType.NVarChar, 8).Value = ma;
        using var r = cmd.ExecuteReader();
        return r.Read() ? Snapshot.SinhVien(r) : null;
    }
    // Alias giữ tên hàm tra cứu trong code mẫu.
    public SinhVien? GetSinhVienById(string maSV) => GetSinhVienByMaSV(maSV);
    public List<SinhVien> GetSinhViensByMaLop(string ma)
    {
        using var con = Open();
        using var cmd = new SqlCommand(SelectSinhVien + " WHERE s.MaLop = @MaLop", con);
        cmd.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = ma;
        using var r = cmd.ExecuteReader();
        var list = new List<SinhVien>();
        while (r.Read()) list.Add(Snapshot.SinhVien(r));
        return list;
    }
    // Tìm tại nguồn dữ liệu (database), không phải tìm trên các hàng đang hiển thị.
    // Collation Vietnamese_100_CI_AS của cột cho LIKE không phân biệt hoa thường,
    // khớp hành vi Contains(..., CurrentCultureIgnoreCase) của bản in-memory trước đây.
    public List<SinhVien> TimKiem(string maLop, string tuKhoa, decimal diemTu)
    {
        using var con = Open();
        using var cmd = new SqlCommand(SelectSinhVien +
            " WHERE (@MaLop = N'' OR s.MaLop = @MaLop) AND s.Diem >= @DiemTu " +
            "AND (s.MaSV LIKE @TuKhoa OR s.HoTen LIKE @TuKhoa OR s.Email LIKE @TuKhoa OR s.SoDienThoai LIKE @TuKhoa)", con);
        cmd.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = maLop;
        var diemTuP = cmd.Parameters.Add("@DiemTu", SqlDbType.Decimal);
        diemTuP.Precision = 4; diemTuP.Scale = 1;
        diemTuP.Value = diemTu;
        cmd.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 106).Value = "%" + tuKhoa + "%";
        using var r = cmd.ExecuteReader();
        var list = new List<SinhVien>();
        while (r.Read()) list.Add(Snapshot.SinhVien(r));
        return list;
    }
    public void AddSinhVien(SinhVien s)
    {
        if (GetSinhVienByMaSV(s.MaSV) != null) throw new InvalidOperationException("Mã sinh viên đã tồn tại.");
        KiemTraLop(s.MaLop);
        using var con = Open();
        using var cmd = new SqlCommand(
            "INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Email, SoDienThoai, Diem, TrangThai, MaLop) " +
            "VALUES (@MaSV, @HoTen, @NgaySinh, @GioiTinh, @Email, @SoDienThoai, @Diem, @TrangThai, @MaLop)", con);
        GanThamSo(cmd, s);
        if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Không thêm được sinh viên.");
    }
    public void UpdateSinhVien(SinhVien s)
    {
        if (GetSinhVienByMaSV(s.MaSV) == null) throw new InvalidOperationException("Sinh viên không tồn tại.");
        KiemTraLop(s.MaLop);
        using var con = Open();
        using var cmd = new SqlCommand(
            "UPDATE dbo.SinhVien SET HoTen = @HoTen, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh, Email = @Email, " +
            "SoDienThoai = @SoDienThoai, Diem = @Diem, TrangThai = @TrangThai, MaLop = @MaLop WHERE MaSV = @MaSV", con);
        GanThamSo(cmd, s);
        if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Sinh viên không tồn tại.");
    }
    public void DeleteSinhVien(string ma)
    {
        using var con = Open();
        using var cmd = new SqlCommand("DELETE FROM dbo.SinhVien WHERE MaSV = @MaSV", con);
        cmd.Parameters.Add("@MaSV", SqlDbType.NVarChar, 8).Value = ma;
        if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Sinh viên không tồn tại.");
    }
    private void KiemTraLop(string ma)
    {
        using var con = Open();
        using var cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.LopHoc WHERE MaLop = @MaLop", con);
        cmd.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = ma;
        if ((int)cmd.ExecuteScalar()! == 0) throw new InvalidOperationException("Lớp học không tồn tại.");
    }
    private static void GanThamSo(SqlCommand cmd, SinhVien s)
    {
        cmd.Parameters.Add("@MaSV", SqlDbType.NVarChar, 8).Value = s.MaSV;
        cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100).Value = s.HoTen;
        cmd.Parameters.Add("@NgaySinh", SqlDbType.Date).Value = s.NgaySinh.Date;
        cmd.Parameters.Add("@GioiTinh", SqlDbType.NVarChar, 10).Value = s.GioiTinh;
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = s.Email;
        cmd.Parameters.Add("@SoDienThoai", SqlDbType.NVarChar, 11).Value = s.SoDienThoai;
        var diem = cmd.Parameters.Add("@Diem", SqlDbType.Decimal);
        diem.Precision = 4; diem.Scale = 1;
        diem.Value = (object?)s.Diem ?? DBNull.Value;
        cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 20).Value = s.TrangThai;
        cmd.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = s.MaLop;
    }
}
