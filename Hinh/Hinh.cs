using System;
using System.Collections.Generic;
using System.Text;


namespace ExampleCAdvance.Hinh;

public abstract class Shape
{
    public string Name { get; set; } = string.Empty;

    protected Shape()
    {
        Name = "Shape";
    }

    protected Shape(string name)
    {
        Name = name;
    }

    // Abstract methods to be implemented by specific shapes
    public abstract double Tinhdientich();
    public abstract double Tinhchuvi();

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Shape: {Name} | Area: {Tinhdientich():F2} | Perimeter: {Tinhchuvi():F2}");
    }
}

// Alias for plural naming if referenced as Shapes
public abstract class Shapes : Shape
{
    protected Shapes() : base() { }
    protected Shapes(string name) : base(name) { }
}
