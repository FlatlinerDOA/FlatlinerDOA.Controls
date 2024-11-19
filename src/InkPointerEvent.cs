using Avalonia.Interactivity;

namespace FlatlinerDOA.Controls;
using Avalonia.Input;
using System;

public sealed class InkPointerEvent : RoutedEventArgs
{
    public string Type { get; }
    public double X { get; }
    public double Y { get; }
    public float Pressure { get; }
    public long Timestamp { get; }

    public InkPointerEvent(
        PointerEventArgs eventArgs,
        string type,
        double x,
        double y,
        float pressure,
        long timestamp) : base(eventArgs.RoutedEvent)
    {
        this.Type = type;
        this.X = x;
        this.Y = y;
        this.Pressure = pressure;
        this.Timestamp = timestamp;
    }
    public InkPointerEvent(PointerEventArgs e, string type, PointerPoint pointerPoint) : this(e, type, pointerPoint.Position.X, pointerPoint.Position.Y, pointerPoint.Properties.Pressure, DateTime.UtcNow.Ticks)
    {
    }
}
