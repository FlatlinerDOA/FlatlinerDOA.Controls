namespace FlatlinerDOA.Controls;
using System;

public record class InkPoint
{
    public double X { get; }
    public double Y { get; }
    public float Pressure { get; }
    public long Time { get; }

    public InkPoint(double x, double y, float? pressure = null, long? time = null)
    {
        if (double.IsNaN(x) || double.IsNaN(y))
        {
            throw new ArgumentException($"Point is invalid: (${x}, ${y})");
        }

        this.X = +x;
        this.Y = +y;
        this.Pressure = pressure ?? 0f;
        this.Time = time ?? DateTimeOffset.UtcNow.Ticks;
    }

    public float DistanceTo(InkPoint start) =>
        (float)Math.Sqrt(Math.Pow(this.X - start.X, 2) + Math.Pow(this.Y - start.Y, 2));

    public float VelocityFrom(InkPoint start) =>
        this.Time != start.Time ? this.DistanceTo(start) / (this.Time - start.Time) : 0;
}
