using System;

namespace ConsoleApp1.Hinh
{
    public class HinhTamGiac : Hinh
    {
        private double canhA;
        private double canhB;
        private double canhC;

        public double CanhA
        {
            get
            {
                return canhA;
            }

            set
            {
                if (value > 0)
                {
                    canhA = value;
                }
                else
                {
                    throw new ArgumentException("Canh A phai lon hon 0.");
                }
            }
        }

        public double CanhB
        {
            get
            {
                return canhB;
            }

            set
            {
                if (value > 0)
                {
                    canhB = value;
                }
                else
                {
                    throw new ArgumentException("Canh B phai lon hon 0.");
                }
            }
        }

        public double CanhC
        {
            get
            {
                return canhC;
            }

            set
            {
                if (value > 0)
                {
                    canhC = value;
                }
                else
                {
                    throw new ArgumentException("Canh C phai lon hon 0.");
                }
            }
        }

        // Constructor mặc định
        public HinhTamGiac()
        {
            canhA = 1;
            canhB = 1;
            canhC = 1;
        }

        // Constructor có tham số
        public HinhTamGiac(double a, double b, double c)
        {
            if (!IsTamGiac(a, b, c))
            {
                throw new ArgumentException(
                    "Ba canh khong tao thanh tam giac."
                );
            }

            CanhA = a;
            CanhB = b;
            CanhC = c;
        }

        // Kiểm tra 3 giá trị truyền vào
        public bool IsTamGiac(double a, double b, double c)
        {
            return a > 0 &&
                   b > 0 &&
                   c > 0 &&
                   a + b > c &&
                   a + c > b &&
                   b + c > a;
        }

        // Kiểm tra chính đối tượng hiện tại
        public bool IsTamGiac()
        {
            return IsTamGiac(CanhA, CanhB, CanhC);
        }

        public double GetChuVi()
        {
            return CanhA + CanhB + CanhC;
        }

        public double GetDienTich()
        {
            double p = GetChuVi() / 2;

            return Math.Sqrt(
                p *
                (p - CanhA) *
                (p - CanhB) *
                (p - CanhC)
            );
        }

        public void Nhap()
        {
            double a;
            double b;
            double c;

            do
            {
                Console.Write("Nhap canh A: ");
                bool hopLeA = double.TryParse(Console.ReadLine(), out a);

                Console.Write("Nhap canh B: ");
                bool hopLeB = double.TryParse(Console.ReadLine(), out b);

                Console.Write("Nhap canh C: ");
                bool hopLeC = double.TryParse(Console.ReadLine(), out c);

                if (hopLeA &&
                    hopLeB &&
                    hopLeC &&
                    IsTamGiac(a, b, c))
                {
                    CanhA = a;
                    CanhB = b;
                    CanhC = c;

                    break;
                }

                Console.WriteLine(
                    "Ba canh khong hop le. Vui long nhap lai."
                );

            } while (true);
        }

        public void HienThi()
        {
            Console.WriteLine("===== HINH TAM GIAC =====");
            Console.WriteLine($"Canh A: {CanhA}");
            Console.WriteLine($"Canh B: {CanhB}");
            Console.WriteLine($"Canh C: {CanhC}");
            Console.WriteLine($"Dien tich: {GetDienTich():F2}");
            Console.WriteLine($"Chu vi: {GetChuVi():F2}");
        }
    }
}
