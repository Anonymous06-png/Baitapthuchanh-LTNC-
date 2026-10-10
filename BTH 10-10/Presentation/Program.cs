using QuanLySinhVien.DataAccess;
using QuanLySinhVien.Business;
namespace QuanLySinhVien.Presentation;
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        var data = new DataList();
        var lopDal = new LopHocDAL(data);
        var svDal = new SinhVienDAL(data);
        Application.Run(new MainForm(new SinhVienBUL(svDal, lopDal), new LopHocBUL(lopDal)));
    }
}
