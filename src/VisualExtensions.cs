namespace FlatlinerDOA.Controls;

using Avalonia;
using System;

internal static class VisualExtensions
{
    public static Rect TransformBounds(this Matrix transform, Rect rect) => rect.TransformBounds(transform);

    public static Rect TransformBounds(this Rect rect, Matrix transform)
    {
        // Transform the corners using the specified matrix
        var transformedTopLeft = transform.Transform(rect.TopLeft);
        var transformedTopRight = transform.Transform(rect.TopRight);
        var transformedBottomLeft = transform.Transform(rect.BottomLeft);
        var transformedBottomRight = transform.Transform(rect.BottomRight);

        // Find the minimum and maximum coordinates of the transformed corners
        var minX = Math.Min(Math.Min(transformedTopLeft.X, transformedTopRight.X),
                            Math.Min(transformedBottomLeft.X, transformedBottomRight.X));
        var minY = Math.Min(Math.Min(transformedTopLeft.Y, transformedTopRight.Y),
                            Math.Min(transformedBottomLeft.Y, transformedBottomRight.Y));
        var maxX = Math.Max(Math.Max(transformedTopLeft.X, transformedTopRight.X),
                            Math.Max(transformedBottomLeft.X, transformedBottomRight.X));
        var maxY = Math.Max(Math.Max(transformedTopLeft.Y, transformedTopRight.Y),
                            Math.Max(transformedBottomLeft.Y, transformedBottomRight.Y));

        // Create and return the axis-aligned bounding box
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}
