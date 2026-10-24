using Microsoft.Data.SqlClient;
using QuanLySinhVien.Entity;
namespace QuanLySinhVien.DataAccess;
// Bản copy đọc-ra để tầng ngoài không giữ tham chiếu tới dữ liệu nguồn.
internal static class Snapshot
{
    internal static SinhVien SinhVien(SinhVien s, LopHoc? lop = null) => new()
    {
        MaSV = s.MaSV, HoTen = s.HoTen, NgaySinh = s.NgaySinh, GioiTinh = s.GioiTinh,
        Email = s.Email, SoDienThoai = s.SoDienThoai, Diem = s.Diem, TrangThai = s.TrangThai,
        MaLop = s.MaLop, LopHoc = lop == null ? null : new LopHoc { MaLop = lop.MaLop, TenLop = lop.TenLop }
    };
    // Cột theo thứ tự chuẩn của các SELECT trong DAL:
    // 0 MaSV, 1 HoTen, 2 NgaySinh, 3 GioiTinh, 4 Email, 5 SoDienThoai, 6 Diem, 7 TrangThai, 8 MaLop, 9 TenLop (JOIN).
    internal static SinhVien SinhVien(SqlDataReader r) => new()
    {
        MaSV = r.GetString(0), HoTen = r.GetString(1), NgaySinh = r.GetDateTime(2),
        GioiTinh = r.GetString(3), Email = r.GetString(4), SoDienThoai = r.GetString(5),
        Diem = r.IsDBNull(6) ? null : r.GetDecimal(6), TrangThai = r.GetString(7), MaLop = r.GetString(8),
        LopHoc = r.IsDBNull(9) ? null : new LopHoc { MaLop = r.GetString(8), TenLop = r.GetString(9) }
    };
}
