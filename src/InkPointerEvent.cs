namespace FlatlinerDOA.Controls;
using Avalonia.Input;
using System;

public record class InkPointerEvent(
    PointerEventArgs eventArgs,
    string Type,
    double X,
    double Y,
    float Pressure,
    long Timestamp)
{
    public InkPointerEvent(PointerEventArgs e, string type, PointerPoint pointerPoint) : this(e, type, pointerPoint.Position.X, pointerPoint.Position.Y, pointerPoint.Properties.Pressure, DateTime.UtcNow.Ticks)
    {
    }
}
