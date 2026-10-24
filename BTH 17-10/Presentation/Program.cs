using QuanLySinhVien.DataAccess;
using QuanLySinhVien.Business;
namespace QuanLySinhVien.Presentation;
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        // Nguồn dữ liệu thật: SQL Server (QuanLySinhVienDB) qua ADO.NET.
        var lopDal = new LopHocDAL(Db.ConnectionString);
        var svDal = new SinhVienDAL(Db.ConnectionString);
        Application.Run(new MainForm(new SinhVienBUL(svDal, lopDal), new LopHocBUL(lopDal)));
    }
}
