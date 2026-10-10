using QuanLySinhVien.Entity;
namespace QuanLySinhVien.DataAccess;
public sealed class LopHocDAL
{
    private readonly DataList data;
    public LopHocDAL(DataList data) => this.data = data;
    private LopHoc Copy(LopHoc l) => new()
    {
        MaLop = l.MaLop, TenLop = l.TenLop,
        DanhSachSinhVien = data.SinhViens.Where(s => s.MaLop == l.MaLop).Select(s => Snapshot.SinhVien(s, l)).ToList()
    };
    public List<LopHoc> GetAllLopHoc() => data.LopHocs.Select(Copy).ToList();
    public LopHoc? GetLopHocByMa(string ma) => data.LopHocs.Find(l => l.MaLop.Equals(ma, StringComparison.OrdinalIgnoreCase)) is { } l ? Copy(l) : null;
    public void AddLopHoc(LopHoc l)
    {
        if (GetLopHocByMa(l.MaLop) != null) throw new InvalidOperationException("Mã lớp đã tồn tại.");
        data.LopHocs.Add(new LopHoc { MaLop = l.MaLop, TenLop = l.TenLop });
    }
    public void UpdateLopHoc(LopHoc l)
    {
        var old = data.LopHocs.Find(x => x.MaLop.Equals(l.MaLop, StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException("Lớp không tồn tại.");
        old.TenLop = l.TenLop;
    }
    public void DeleteLopHoc(string ma)
    {
        var old = data.LopHocs.Find(l => l.MaLop.Equals(ma, StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException("Lớp không tồn tại.");
        if (data.SinhViens.Any(s => s.MaLop == old.MaLop)) throw new InvalidOperationException("Không thể xóa lớp đang có sinh viên.");
        data.LopHocs.Remove(old);
    }
}
