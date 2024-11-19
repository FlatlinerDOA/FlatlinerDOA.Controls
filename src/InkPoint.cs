namespace FlatlinerDOA.Controls;
using System;

public record class InkPoint
{
    public double X { get; }
    public double Y { get; }
    public double Pressure { get; }
    public DateTime Time { get; }

    public InkPoint(double x, double y, double? pressure = null, DateTime? time = null)
    {
        if (double.IsNaN(x) || double.IsNaN(y))
        {
            throw new ArgumentException($"Point is invalid: (${x}, ${y})");
        }

        this.X = +x;
        this.Y = +y;
        this.Pressure = pressure ?? 0f;
        this.Time = time ?? DateTime.UtcNow;
    }

    public double DistanceTo(InkPoint start) =>
        (float)Math.Sqrt(Math.Pow(this.X - start.X, 2) + Math.Pow(this.Y - start.Y, 2));

    public double VelocityFrom(InkPoint start) =>
        this.Time != start.Time ? this.DistanceTo(start) / (this.Time - start.Time).TotalMilliseconds : 0;
}
