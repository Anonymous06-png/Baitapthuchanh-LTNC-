using System;
using System.Collections.Generic;
using System.Text;


namespace ExampleCAdvance.Hinh;

public class Circle : Shape
{
    private double radius;

    public double Radius
    {
        get => radius;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Radius cannot be negative.");
            }
            radius = value;
        }
    }

    // Vietnamese alias if needed in coursework
    public double BanKinh
    {
        get => Radius;
        set => Radius = value;
    }

    public Circle() : base("Circle")
    {
        Radius = 0;
    }

    public Circle(double radius) : base("Circle")
    {
        Radius = radius;
    }

    public Circle(string name, double radius) : base(name)
    {
        Radius = radius;
    }

    public override double Tinhdientich()
    {
        return Math.PI * radius * radius;
    }

    public override double Tinhchuvi()
    {
        return 2 * Math.PI * radius;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Circle [{Name}] - Radius: {Radius}, Area: {Tinhdientich():F2}, Perimeter: {Tinhchuvi():F2}");
    }
}
