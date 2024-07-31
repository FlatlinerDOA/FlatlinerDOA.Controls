namespace FlatlinerDOA.Controls;
using System;

public class InkBezier
{
    public static InkBezier FromPoints(IReadOnlyList<InkPoint> points, (double start, double end) widths)
    {
        var c2 = CalculateControlPoints(points[0], points[1], points[2]).c2;
        var c3 = CalculateControlPoints(points[1], points[2], points[3]).c1;

        return new InkBezier(points[1], c2, c3, points[2], widths.start, widths.end);
    }

    private static (InkPoint c1, InkPoint c2) CalculateControlPoints(InkPoint s1, InkPoint s2, InkPoint s3)
    {
        double dx1 = s1.X - s2.X;
        double dy1 = s1.Y - s2.Y;
        double dx2 = s2.X - s3.X;
        double dy2 = s2.Y - s3.Y;

        var m1 = new InkPoint((s1.X + s2.X) / 2.0f, (s1.Y + s2.Y) / 2.0f);
        var m2 = new InkPoint((s2.X + s3.X) / 2.0f, (s2.Y + s3.Y) / 2.0f);

        double l1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);
        double l2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);

        double dxm = m1.X - m2.X;
        double dym = m1.Y - m2.Y;

        double k = l2 / (l1 + l2);
        var cm = new InkPoint(m2.X + dxm * k, m2.Y + dym * k);

        double tx = s2.X - cm.X;
        double ty = s2.Y - cm.Y;

        return (
            new InkPoint(m1.X + tx, m1.Y + ty),
            new InkPoint(m2.X + tx, m2.Y + ty)
        );
    }

    public InkPoint StartPoint { get; }
    public InkPoint Control2 { get; }
    public InkPoint Control1 { get; }
    public InkPoint EndPoint { get; }
    public double StartWidth { get; }
    public double EndWidth { get; }

    public InkBezier(InkPoint startPoint, InkPoint control2, InkPoint control1, InkPoint endPoint, double startWidth, double endWidth)
    {
        StartPoint = startPoint;
        Control2 = control2;
        Control1 = control1;
        EndPoint = endPoint;
        StartWidth = startWidth;
        EndWidth = endWidth;
    }

    // Returns approximated length. Code taken from https://www.lemoda.net/maths/bezier-length/index.html.
    public double Length()
    {
        int steps = 10;
        double length = 0;
        double? px = null;
        double? py = null;

        for (int i = 0; i <= steps; i++)
        {
            double t = (double)i / steps;
            double cx = Point(t, StartPoint.X, Control1.X, Control2.X, EndPoint.X);
            double cy = Point(t, StartPoint.Y, Control1.Y, Control2.Y, EndPoint.Y);

            if (i > 0)
            {
                double xdiff = cx - px.Value;
                double ydiff = cy - py.Value;

                length += Math.Sqrt(xdiff * xdiff + ydiff * ydiff);
            }

            px = cx;
            py = cy;
        }

        return length;
    }

    // Calculate parametric value of x or y given t and the four point coordinates of a cubic bezier curve.
    private double Point(double t, double start, double c1, double c2, double end)
    {
        return (start * (1.0 - t) * (1.0 - t) * (1.0 - t))
             + (3.0 * c1 * (1.0 - t) * (1.0 - t) * t)
             + (3.0 * c2 * (1.0 - t) * t * t)
             + (end * t * t * t);
    }
}
