using System;

namespace ConsoleApp1.Hinh
{
    public class HinhTron : Hinh
    {
        private double banKinh;

        public double BanKinh
        {
            get
            {
                return banKinh;
            }

            set
            {
                if (value > 0)
                {
                    banKinh = value;
                }
                else
                {
                    throw new ArgumentException("Ban kinh phai lon hon 0.");
                }
            }
        }

        // Constructor mặc định
        public HinhTron()
        {
            banKinh = 1;
        }

        // Constructor có tham số
        public HinhTron(double banKinh)
        {
            BanKinh = banKinh;
        }

        public double GetDienTich()
        {
            return Math.PI * BanKinh * BanKinh;
        }

        public double GetChuVi()
        {
            return 2 * Math.PI * BanKinh;
        }

        public void Nhap()
        {
            double r;

            do
            {
                Console.Write("Nhap ban kinh: ");

                if (double.TryParse(Console.ReadLine(), out r) && r > 0)
                {
                    BanKinh = r;
                    break;
                }

                Console.WriteLine("Ban kinh phai lon hon 0.");

            } while (true);
        }

        public void HienThi()
        {
            Console.WriteLine("===== HINH TRON =====");
            Console.WriteLine($"Ban kinh: {BanKinh}");
            Console.WriteLine($"Dien tich: {GetDienTich():F2}");
            Console.WriteLine($"Chu vi: {GetChuVi():F2}");
        }
    }
}
