using System;

namespace ExampleCAdvance.Hinh;

public class Triangle : Shape
{
    private double canhA;
    private double canhB;
    private double canhC;

    // Constructor mặc định
    public Triangle() : base("Triangle")
    {
        canhA = 0;
        canhB = 0;
        canhC = 0;
    }

    // Constructor có tham số
    public Triangle(double a, double b, double c)
        : base("Triangle")
    {
        canhA = a;
        canhB = b;
        canhC = c;
    }

    // Kiểm tra 3 cạnh có tạo thành tam giác không
    private bool KiemTraTamGiac()
    {
        return canhA + canhB > canhC &&
               canhA + canhC > canhB &&
               canhB + canhC > canhA;
    }

    // Tính chu vi
    public override double Tinhchuvi()
    {
        return canhA + canhB + canhC;
    }

    // Tính diện tích bằng công thức Heron
    public override double Tinhdientich()
    {
        if (!KiemTraTamGiac())
        {
            return 0;
        }

        double p = Tinhchuvi() / 2;

        return Math.Sqrt(
            p *
            (p - canhA) *
            (p - canhB) *
            (p - canhC)
        );
    }

    // Hiển thị thông tin
    public override void DisplayInfo()
    {
        if (!KiemTraTamGiac())
        {
            Console.WriteLine("Ba cạnh không tạo thành tam giác.");
            return;
        }

        Console.WriteLine(
            $"Shape: {Name} | " +
            $"Canh A: {canhA} | " +
            $"Canh B: {canhB} | " +
            $"Canh C: {canhC} | " +
            $"Area: {Tinhdientich():F2} | " +
            $"Perimeter: {Tinhchuvi():F2}"
        );
    }
}
