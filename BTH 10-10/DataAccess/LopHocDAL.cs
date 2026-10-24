using System.Data;
using Microsoft.Data.SqlClient;
using QuanLySinhVien.Entity;
namespace QuanLySinhVien.DataAccess;
// Đọc/ghi trực tiếp SQL Server qua ADO.NET.
// Lỗi từ ADO.NET/SQL Server được để lan tự nhiên DataAccess → Business → View.
public sealed class LopHocDAL
{
    private readonly string connectionString;
    public LopHocDAL(string connectionString) => this.connectionString = connectionString;
    private SqlConnection Open()
    {
        var con = new SqlConnection(connectionString);
        con.Open();
        return con;
    }
    public List<LopHoc> GetAllLopHoc()
    {
        using var con = Open();
        var lops = new List<LopHoc>();
        using (var cmd = new SqlCommand("SELECT MaLop, TenLop FROM dbo.LopHoc", con))
        using (var r = cmd.ExecuteReader())
            while (r.Read()) lops.Add(new LopHoc { MaLop = r.GetString(0), TenLop = r.GetString(1) });
        foreach (var l in lops) l.DanhSachSinhVien = DocSinhVienCuaLop(con, l);
        return lops;
    }
    public LopHoc? GetLopHocByMa(string ma)
    {
        using var con = Open();
        LopHoc? lop;
        using (var cmd = new SqlCommand("SELECT MaLop, TenLop FROM dbo.LopHoc WHERE MaLop = @MaLop", con))
        {
            cmd.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = ma;
            using var r = cmd.ExecuteReader();
            lop = r.Read() ? new LopHoc { MaLop = r.GetString(0), TenLop = r.GetString(1) } : null;
        }
        if (lop != null) lop.DanhSachSinhVien = DocSinhVienCuaLop(con, lop);
        return lop;
    }
    public void AddLopHoc(LopHoc l)
    {
        if (GetLopHocByMa(l.MaLop) != null) throw new InvalidOperationException("Mã lớp đã tồn tại.");
        using var con = Open();
        using var cmd = new SqlCommand("INSERT INTO dbo.LopHoc (MaLop, TenLop) VALUES (@MaLop, @TenLop)", con);
        cmd.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = l.MaLop;
        cmd.Parameters.Add("@TenLop", SqlDbType.NVarChar, 100).Value = l.TenLop;
        if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Không thêm được lớp học.");
    }
    public void UpdateLopHoc(LopHoc l)
    {
        if (GetLopHocByMa(l.MaLop) == null) throw new InvalidOperationException("Lớp không tồn tại.");
        using var con = Open();
        using var cmd = new SqlCommand("UPDATE dbo.LopHoc SET TenLop = @TenLop WHERE MaLop = @MaLop", con);
        cmd.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = l.MaLop;
        cmd.Parameters.Add("@TenLop", SqlDbType.NVarChar, 100).Value = l.TenLop;
        if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Lớp không tồn tại.");
    }
    public void DeleteLopHoc(string ma)
    {
        if (GetLopHocByMa(ma) == null) throw new InvalidOperationException("Lớp không tồn tại.");
        using var con = Open();
        using (var cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.SinhVien WHERE MaLop = @MaLop", con))
        {
            cmd.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = ma;
            if ((int)cmd.ExecuteScalar()! > 0) throw new InvalidOperationException("Không thể xóa lớp đang có sinh viên.");
        }
        using var del = new SqlCommand("DELETE FROM dbo.LopHoc WHERE MaLop = @MaLop", con);
        del.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = ma;
        if (del.ExecuteNonQuery() != 1) throw new InvalidOperationException("Lớp không tồn tại.");
    }
    // Dựng navigation DanhSachSinhVien: mỗi SinhVien là bản copy, LopHoc gắn theo lớp.
    private List<SinhVien> DocSinhVienCuaLop(SqlConnection con, LopHoc lop)
    {
        using var cmd = new SqlCommand(
            "SELECT s.MaSV, s.HoTen, s.NgaySinh, s.GioiTinh, s.Email, s.SoDienThoai, s.Diem, s.TrangThai, s.MaLop, l.TenLop " +
            "FROM dbo.SinhVien s LEFT JOIN dbo.LopHoc l ON l.MaLop = s.MaLop WHERE s.MaLop = @MaLop", con);
        cmd.Parameters.Add("@MaLop", SqlDbType.NVarChar, 20).Value = lop.MaLop;
        using var r = cmd.ExecuteReader();
        var list = new List<SinhVien>();
        while (r.Read()) list.Add(Snapshot.SinhVien(r));
        return list;
    }
}
