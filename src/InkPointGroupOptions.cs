namespace FlatlinerDOA.Controls;
using Avalonia.Media;

public record InkPointGroupOptions
{
    public double DotSize { get; init;  }
    public double MinWidth { get; init; }
    public double MaxWidth { get; init; }
    public IBrush? PenBrush { get; init; }
    public double VelocityFilterWeight { get; init; }
}
