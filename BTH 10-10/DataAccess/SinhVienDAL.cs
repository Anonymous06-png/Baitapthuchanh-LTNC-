using QuanLySinhVien.Entity;
namespace QuanLySinhVien.DataAccess;
public sealed class SinhVienDAL
{
    private readonly DataList data;
    public SinhVienDAL(DataList data) => this.data = data;
    private SinhVien Copy(SinhVien s) => Snapshot.SinhVien(s, data.LopHocs.Find(l => l.MaLop == s.MaLop));
    public List<SinhVien> GetAllSinhVien() => data.SinhViens.Select(Copy).ToList();
    public SinhVien? GetSinhVienByMaSV(string ma) => data.SinhViens.Find(s => s.MaSV.Equals(ma, StringComparison.OrdinalIgnoreCase)) is { } s ? Copy(s) : null;
    // Alias giữ tên hàm tra cứu trong code mẫu.
    public SinhVien? GetSinhVienById(string maSV) => GetSinhVienByMaSV(maSV);
    public List<SinhVien> GetSinhViensByMaLop(string ma) => data.SinhViens.Where(s => s.MaLop.Equals(ma, StringComparison.OrdinalIgnoreCase)).Select(Copy).ToList();
    // Tìm tại nguồn DataList, không phải tìm trên các hàng đang hiển thị.
    public List<SinhVien> TimKiem(string maLop, string tuKhoa, decimal diemTu) => data.SinhViens.Where(s =>
        (maLop.Length == 0 || s.MaLop.Equals(maLop, StringComparison.OrdinalIgnoreCase)) && s.Diem >= diemTu &&
        (s.MaSV.Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase) ||
         s.HoTen.Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase) ||
         s.Email.Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase) ||
         s.SoDienThoai.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase))).Select(Copy).ToList();
    public void AddSinhVien(SinhVien s)
    {
        if (GetSinhVienByMaSV(s.MaSV) != null) throw new InvalidOperationException("Mã sinh viên đã tồn tại.");
        KiemTraLop(s.MaLop);
        data.SinhViens.Add(Snapshot.SinhVien(s));
    }
    public void UpdateSinhVien(SinhVien s)
    {
        int i = data.SinhViens.FindIndex(x => x.MaSV.Equals(s.MaSV, StringComparison.OrdinalIgnoreCase));
        if (i < 0) throw new InvalidOperationException("Sinh viên không tồn tại.");
        KiemTraLop(s.MaLop);
        data.SinhViens[i] = Snapshot.SinhVien(s);
    }
    public void DeleteSinhVien(string ma)
    {
        int i = data.SinhViens.FindIndex(s => s.MaSV.Equals(ma, StringComparison.OrdinalIgnoreCase));
        if (i < 0) throw new InvalidOperationException("Sinh viên không tồn tại.");
        data.SinhViens.RemoveAt(i);
    }
    private void KiemTraLop(string ma)
    {
        if (!data.LopHocs.Any(l => l.MaLop == ma)) throw new InvalidOperationException("Lớp học không tồn tại.");
    }
}
