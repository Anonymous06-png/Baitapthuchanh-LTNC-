using QuanLySinhVien.Entity;
namespace QuanLySinhVien.DataAccess;
// Một nguồn dùng chung cho hai DAL; khởi tạo một lần ở Program.
// Danh sách thật không đưa ra ngoài assembly DataAccess.
public sealed class DataList
{
    internal List<LopHoc> LopHocs { get; } = new();
    internal List<SinhVien> SinhViens { get; } = new();
    public DataList(bool taoDuLieuMau = true)
    {
        if (!taoDuLieuMau) return;
        LopHocs.AddRange(new[] {
            new LopHoc { MaLop = "KTPM01", TenLop = "Kỹ thuật phần mềm 01" },
            new LopHoc { MaLop = "AI01", TenLop = "Trí tuệ nhân tạo 01" },
            new LopHoc { MaLop = "KHDL01", TenLop = "Khoa học dữ liệu 01" }
        });
        SinhViens.AddRange(new[] {
            Tao("SV000123", "Nguyễn Văn An", "Nam", "KTPM01", 8.5m, "0912345678"),
            Tao("SV000124", "Trần Minh Anh", "Nữ", "AI01", 9m, "0987654321"),
            Tao("SV000125", "Lê Hoàng Bình", "Nam", "KTPM01", 7.4m, "0355556677"),
            Tao("SV000126", "Đỗ Thị Hồng", "Nữ", "KHDL01", 8.1m, "0777888999")
        });
    }
    private static SinhVien Tao(string ma, string ten, string gt, string lop, decimal diem, string sdt) => new()
    {
        MaSV = ma, HoTen = ten, GioiTinh = gt, MaLop = lop, Diem = diem,
        NgaySinh = new DateTime(2006, 8, 15), Email = ma.ToLowerInvariant() + "@example.com",
        SoDienThoai = sdt, TrangThai = "Đang học"
    };
}
internal static class Snapshot
{
    internal static SinhVien SinhVien(SinhVien s, LopHoc? lop = null) => new()
    {
        MaSV = s.MaSV, HoTen = s.HoTen, NgaySinh = s.NgaySinh, GioiTinh = s.GioiTinh,
        Email = s.Email, SoDienThoai = s.SoDienThoai, Diem = s.Diem, TrangThai = s.TrangThai,
        MaLop = s.MaLop, LopHoc = lop == null ? null : new LopHoc { MaLop = lop.MaLop, TenLop = lop.TenLop }
    };
}
