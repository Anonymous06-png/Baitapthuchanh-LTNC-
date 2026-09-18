using System;
using ConsoleApp1.Hinh;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Hinh[] danhSachHinh =
            {
                new HinhTron(),
                new HinhChuNhat(),
                new HinhTamGiac()
            };

            for (int i = 0; i < danhSachHinh.Length; i++)
            {
                danhSachHinh[i].Nhap();

                Console.WriteLine();

                danhSachHinh[i].HienThi();

                Console.WriteLine("-------------------------");
            }

            Console.ReadKey();
        }
    }
}
