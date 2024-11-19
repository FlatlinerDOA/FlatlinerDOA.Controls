namespace FlatlinerDOA.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Xml.Linq;

public class InkSvgExportOptions
{
    public bool IncludeBackgroundColor { get; set; } = false;
}


/// <summary>
/// Avalonia adaptation of SignaturePad (MIT Licence)
/// See https://github.com/szimek/signature_pad
/// </summary>
public class InkCanvas : Canvas
{
    /// <summary>
    /// DotSize StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<double> DotSizeProperty =
        AvaloniaProperty.Register<InkCanvas, double>(nameof(DotSize), 0d);

  
    /// <summary>
    /// MinimumInkWidth StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<double> MinimumInkWidthProperty =
        AvaloniaProperty.Register<InkCanvas, double>(nameof(MinimumInkWidth), 0.5d);
   

    /// <summary>
    /// MaximumInkWidth StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<double> MaximumInkWidthProperty =
        AvaloniaProperty.Register<InkCanvas, double>(nameof(MaximumInkWidth), 2.5d);
  


    /// <summary>
    /// MinimumInkDistance StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<double> MinimumInkDistanceProperty =
        AvaloniaProperty.Register<InkCanvas, double>(nameof(MinimumInkDistance), 5d);
    
    /// <summary>
    /// PenBrush StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<IBrush> PenBrushProperty =
        AvaloniaProperty.Register<InkCanvas, IBrush>(nameof(PenBrush), Brushes.Black);
   

    /// <summary>
    /// VelocityFilterWeight StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<double> VelocityFilterWeightProperty =
        AvaloniaProperty.Register<InkCanvas, double>(nameof(VelocityFilterWeight), 0.7d);
    
    /// <summary>
    /// Throttle StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<int> ThrottleProperty =
        AvaloniaProperty.Register<InkCanvas, int>(nameof(Throttle), 16);


    // Private stuff
    private bool _drawingStroke = false;
    private bool _isEmpty = true;


    // Stores up to 4 most recent points; used to generate a new curve
    private record class LastState
    {
        public LastState()
        {
        }

        public LastState(InkPointGroupOptions options)
        {
            this.Width = (options.MinWidth + options.MaxWidth) / 2d;
        }

        public List<InkPoint> Points { get; init; } = [];
        
        public double Velocity { get; private set; } = 0;

        public double Width { get; private set; } = 0;

        public (double start, double end) CalculateCurveWidths(InkPoint startPoint, InkPoint endPoint, InkPointGroupOptions options)
        {
            double velocity = options.VelocityFilterWeight * endPoint.VelocityFrom(startPoint) +
                (1 - options.VelocityFilterWeight) * this.Velocity;

            double newWidth = this.StrokeWidth(velocity, options);

            var widths = (
                start: this.Width,
                end: newWidth
            );

            this.Velocity = velocity;
            this.Width = newWidth;
            return widths;
        }

        public InkBezier? AddPoint(InkPoint point, InkPointGroupOptions options)
        {
            this.Points.Add(point);

            if (this.Points.Count > 2)
            {
                // To reduce the initial lag make it work with 3 points
                // by copying the first point to the beginning.
                if (this.Points.Count == 3)
                {
                    this.Points.Insert(0, this.Points[0]);
                }

                // _points array will always have 4 points here.
                var widths = this.CalculateCurveWidths(
                    this.Points[1],
                    this.Points[2],
                    options
                );
                var curve = InkBezier.FromPoints(this.Points, widths);

                // Remove the first element from the list, so that there are no more than 4 points at any time.
                this.Points.RemoveAt(0);
                return curve;
            }

            return null;
        }


        private double StrokeWidth(double velocity, InkPointGroupOptions options) =>
            Math.Max(options.MaxWidth / (velocity + 1), options.MinWidth);
    }

    private LastState _last = new();
    ////private List<InkPoint> _lastPoints = [];
    private List<InkPointGroup> _data = []; // Stores all points in groups (one group per line or dot)
    ////private double _lastVelocity = 0;
    ////private double _lastWidth = 0;

    private Action<InkPointerEvent> StrokeMoveUpdate;

    public InkCanvas()
    {
        this.StrokeMoveUpdate = Throttler.Create<InkPointerEvent>(this.StrokeUpdate, TimeSpan.FromMilliseconds(this.Throttle));
        this.ClipToBounds = true;
    }

    /// <summary>
    /// Gets or sets the DotSize property. This StyledProperty
    /// indicates ....
    /// </summary>
    public double DotSize
    {
        get => this.GetValue(DotSizeProperty);
        set => SetValue(DotSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the MinimumInkWidth property. This StyledProperty
    /// indicates ....
    /// </summary>
    public double MinimumInkWidth
    {
        get => this.GetValue(MinimumInkWidthProperty);
        set => SetValue(MinimumInkWidthProperty, value);
    }
    /// <summary>
    /// Gets or sets the MaximumInkWidth property. This StyledProperty
    /// indicates ....
    /// </summary>
    public double MaximumInkWidth
    {
        get => this.GetValue(MaximumInkWidthProperty);
        set => SetValue(MaximumInkWidthProperty, value);
    }
    /// <summary>
    /// Gets or sets the MinimumInkDistance property. This StyledProperty
    /// indicates ....
    /// </summary>
    public double MinimumInkDistance
    {
        get => this.GetValue(MinimumInkDistanceProperty);
        set => SetValue(MinimumInkDistanceProperty, value);
    }

    /// <summary>
    /// Gets or sets the PenBrush property. This StyledProperty
    /// indicates ....
    /// </summary>
    public IBrush PenBrush
    {
        get => this.GetValue(PenBrushProperty);
        set => SetValue(PenBrushProperty, value);
    }
    /// <summary>
    /// Gets or sets the VelocityFilterWeight property. This StyledProperty
    /// indicates ....
    /// </summary>
    public double VelocityFilterWeight
    {
        get => this.GetValue(VelocityFilterWeightProperty);
        set => SetValue(VelocityFilterWeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the Throttle property. This StyledProperty
    /// indicates the number of milliseconds to throttle ink operations.
    /// </summary>
    public int Throttle
    {
        get => this.GetValue(ThrottleProperty);
        set => SetValue(ThrottleProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (!this.IsLeftButtonPressed(e) || this._drawingStroke)
        {
            base.OnPointerPressed(e);
            return;
        }

        e.Handled = true;
        e.Pointer.Capture(this);
        this.StrokeBegin(this.PointerEventToSignatureEvent(e));
        ////Debug.WriteLine("OnPointerPressed");
        base.OnPointerPressed(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (e.Pointer.Captured == this)
        {
            if (!this.IsLeftButtonPressed(e, true) || !this._drawingStroke)
            {
                // Stop when primary button not pressed or multiple buttons pressed
                this.StrokeEnd(this.PointerEventToSignatureEvent(e), false);
                base.OnPointerMoved(e);
                return;
            }

            e.Handled = true;
            this.StrokeMoveUpdate(this.PointerEventToSignatureEvent(e));
            ////Debug.WriteLine("OnPointerMoved");
            base.OnPointerMoved(e);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (this.IsLeftButtonPressed(e))
        {
            // Wrong button was released
            base.OnPointerReleased(e);
            return;
        }

        e.Handled = true;
        this.StrokeEnd(this.PointerEventToSignatureEvent(e));
        e.Pointer.Capture(null);
        ////Debug.WriteLine("OnPointerReleased");
        base.OnPointerReleased(e);
    }

    private InkPointerEvent PointerEventToSignatureEvent(PointerEventArgs e) => new InkPointerEvent(e, "PointerPressed", e.GetCurrentPoint(this));

    public void StrokeBegin(InkPointerEvent signatureEvent)
    {
        bool cancelled = false; /*!this.DispatchEvent(
            new CustomEvent("beginStroke", new CustomEventInit { Detail = signatureEvent, Cancelable = true })
        );*/
        if (cancelled)
        {
            return;
        }

        this._drawingStroke = true;

        var pointGroupOptions = this.GetPointGroupOptions();

        var newPointGroup = new InkPointGroup([], pointGroupOptions);
        this._data.Add(newPointGroup);
        this.Reset(pointGroupOptions);
        this.StrokeUpdate(signatureEvent);
    }


    private void StrokeUpdate(InkPointerEvent e)
    {
        if (!this._drawingStroke)
        {
            return;
        }

        if (this._data.Count == 0)
        {
            // This can happen if clear() was called while a signature is still in progress,
            // or if there is a race condition between start/update events.
            this.StrokeBegin(e);
            return;
        }
        ////Debug.WriteLine($"StrokeUpdate");

        //this.DispatchEvent(
        //  new CustomEvent('beforeUpdateStroke', { detail: e }),
        //);

        var point = new InkPoint(e.X, e.Y, e.Pressure);
        var lastPointGroup = this._data[this._data.Count - 1];
        var lastPoints = lastPointGroup.Points;
        var lastPoint = lastPoints.LastOrDefault();
        var isLastPointTooClose = lastPoint is not null
          ? point.DistanceTo(lastPoint) <= this.MinimumInkDistance
          : false;
        var pointGroupOptions = this.GetPointGroupOptions(lastPointGroup);

        // Skip this point if it's too close to the previous one
        if (lastPoint is null || !(lastPoint is not null && isLastPointTooClose))
        {
            var curve = this._last.AddPoint(point, pointGroupOptions);

            if (lastPoint is null)
            {
                ////Debug.WriteLine("DrawPoint");
                this.DrawDot(point, pointGroupOptions);
            }
            else if (curve is not null)
            {
                ////Debug.WriteLine("DrawCurve");
                this.DrawCurve(curve, pointGroupOptions);
            }

            lastPoints.Add(new InkPoint(point.X, point.Y, point.Pressure, point.Time));
        }
    }

    private void StrokeEnd(InkPointerEvent signatureEvent, bool shouldUpdate = true)
    {
        if (!this._drawingStroke)
        {
            return;
        }

        ////Debug.WriteLine($"StrokeEnd - ShouldUpdate: {shouldUpdate}");

        if (shouldUpdate)
        {
            this.StrokeUpdate(signatureEvent);
        }

        this._drawingStroke = false;

        //var endStrokeEvent = new RoutedEventArgs(
        //    new RoutedEvent("EndStroke", RoutingStrategies.Bubble, typeof(RoutedEventArgs))
        //);

        //this.RaiseEvent(endStrokeEvent);
    }

    private InkPointGroupOptions GetPointGroupOptions(InkPointGroup? group = null) => new()
    {
        PenBrush = group?.PenBrush ?? this.PenBrush,
        DotSize = group?.DotSize ?? this.DotSize,
        MinWidth = group?.MinWidth ?? this.MinimumInkWidth,
        MaxWidth = group?.MaxWidth ?? this.MaximumInkWidth,
        VelocityFilterWeight = group?.VelocityFilterWeight ?? this.VelocityFilterWeight,
    };

    private void Reset(InkPointGroupOptions options)
    {
        this._last = new(options);
    }

    private void DrawDot(InkPoint point, InkPointGroupOptions options)
    {
        double width = options.DotSize > 0
            ? options.DotSize
            : (options.MaxWidth - options.MinWidth) / 2;

        var path = new Path
        {
            Fill = options.PenBrush,
            Stroke = options.PenBrush,
            StrokeThickness = options.MinWidth,
            StrokeLineCap = PenLineCap.Round,
            StrokeJoin = PenLineJoin.Round,
            Data = new EllipseGeometry(new Rect(point.X - width / 2, point.Y - width / 2, width, width)),
            Width = width,
            Height = width
        };

        this.Children.Add(path);
    }

    private void DrawCurveSegment(StreamGeometryContext context, double x, double y, double radius)
    {
        context.BeginFigure(new Point(x, y - radius), true);
        context.ArcTo(
            new Point(x, y + radius),
            new Size(radius, radius),
            0,
            false,
            SweepDirection.Clockwise
        );
        context.ArcTo(
            new Point(x, y - radius),
            new Size(radius, radius),
            0,
            false,
            SweepDirection.Clockwise
        );
        context.EndFigure(true);
    }

    private void DrawCurve(InkBezier curve, InkPointGroupOptions options)
    {
        double widthDelta = curve.EndWidth - curve.StartWidth;
        // '2' is just an arbitrary number here. If only length is used, then
        // there are gaps between curve segments :/
        int drawSteps = (int)Math.Ceiling(curve.Length()) * 2;

        //var geometry = new PathGeometry() { Figures = new PathFigures() };
        //var path = new Path
        //{
        //    Fill = options.PenBrush,
        //    Data = geometry
        //};

        //var pathFigure = new PathFigure { IsClosed = true };
        //geometry.Figures.Add(pathFigure);

        //for (int i = 0; i < drawSteps; i++)
        //{
        //    // Calculate the Bezier (x, y) coordinate for this step.
        //    double t = (double)i / drawSteps;
        //    double tt = t * t;
        //    double ttt = tt * t;
        //    double u = 1 - t;
        //    double uu = u * u;
        //    double uuu = uu * u;

        //    double x = uuu * curve.StartPoint.X;
        //    x += 3 * uu * t * curve.Control1.X;
        //    x += 3 * u * tt * curve.Control2.X;
        //    x += ttt * curve.EndPoint.X;

        //    double y = uuu * curve.StartPoint.Y;
        //    y += 3 * uu * t * curve.Control1.Y;
        //    y += 3 * u * tt * curve.Control2.Y;
        //    y += ttt * curve.EndPoint.Y;

        //    double width = Math.Min(
        //        curve.StartWidth + ttt * widthDelta,
        //        options.MaxWidth
        //    );

        //    var ellipse = new EllipseGeometry(new Rect(x, y, width / 2, width / 2));
        //    pathFigure.Segments.Add(new PathSegment());
        //    geometry.AddGeometry(ellipse);
        //}

        double minX = double.MaxValue, minY = double.MaxValue;
        double maxX = double.MinValue, maxY = double.MinValue;

        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            for (int i = 0; i < drawSteps; i++)
            {
                double t = (double)i / drawSteps;
                double tt = t * t;
                double ttt = tt * t;
                double u = 1d - t;
                double uu = u * u;
                double uuu = uu * u;

                double x = uuu * curve.StartPoint.X;
                x += 3d * uu * t * curve.Control1.X;
                x += 3d * u * tt * curve.Control2.X;
                x += ttt * curve.EndPoint.X;

                double y = uuu * curve.StartPoint.Y;
                y += 3d * uu * t * curve.Control1.Y;
                y += 3d * u * tt * curve.Control2.Y;
                y += ttt * curve.EndPoint.Y;

                double width = Math.Min(curve.StartWidth + ttt * widthDelta, options.MaxWidth);

                // Update bounding box
                minX = Math.Min(minX, x - width / 2);
                minY = Math.Min(minY, y - width / 2);
                maxX = Math.Max(maxX, x + width / 2);
                maxY = Math.Max(maxY, y + width / 2);

                this.DrawCurveSegment(context, x, y, width / 2);
            }
        }

        var path = new Path
        {
            Fill = null,
            Stroke = options.PenBrush,
            StrokeThickness = options.MinWidth,
            StrokeLineCap = PenLineCap.Round,
            StrokeJoin = PenLineJoin.Round,
            Data = geometry,
            Width = maxX - minX,
            Height = maxY - minY
        };

        //Canvas.SetLeft(path, minX);
        //Canvas.SetTop(path, minY);

        this.Children.Add(path);
    }

    /// <summary>
    /// Clears all ink that has been drawn.
    /// </summary>
    public void Clear()
    {
        this.Children.Clear();
        this._data.Clear();
    }
    
    private double GetDevicePixelRatio()
    {
        return this.VisualRoot?.RenderScaling ?? 1.0;
    }

    private string BackgroundColorHex => this.Background is ISolidColorBrush c ? c.Color.ToString() : "#FFFFFF";

    private string PenColorHex => this.PenBrush is ISolidColorBrush c ? c.Color.ToString() : "#000000";

    public XElement ToSvg(InkSvgExportOptions options = null)
    {
        options ??= new InkSvgExportOptions();

        var pointGroups = _data;

        double ratio = Math.Max(GetDevicePixelRatio(), 1);
        double minX = 0;
        double minY = 0;
        double maxX = this.Width / ratio;
        double maxY = this.Height / ratio;

        XNamespace svgNs = "http://www.w3.org/2000/svg";
        XNamespace xlinkNs = "http://www.w3.org/1999/xlink";

        var svg = new XElement(svgNs + "svg",
            new XAttribute("xmlns", svgNs.NamespaceName),
            new XAttribute(XNamespace.Xmlns + "xlink", xlinkNs.NamespaceName),
            new XAttribute("viewBox", $"{minX} {minY} {maxX} {maxY}"),
            new XAttribute("width", maxX),
            new XAttribute("height", maxY));

        if (options.IncludeBackgroundColor && !string.IsNullOrEmpty(this.BackgroundColorHex))
        {
            svg.Add(new XElement(svgNs + "rect",
                new XAttribute("width", "100%"),
                new XAttribute("height", "100%"),
                new XAttribute("fill", this.BackgroundColorHex)));
        }

        FromData(
            pointGroups,
            (curve, options) =>
            {
                if (!double.IsNaN(curve.Control1.X) &&
                    !double.IsNaN(curve.Control1.Y) &&
                    !double.IsNaN(curve.Control2.X) &&
                    !double.IsNaN(curve.Control2.Y))
                {
                    string attr =
                        $"M {curve.StartPoint.X:F3},{curve.StartPoint.Y:F3} " +
                        $"C {curve.Control1.X:F3},{curve.Control1.Y:F3} " +
                        $"{curve.Control2.X:F3},{curve.Control2.Y:F3} " +
                        $"{curve.EndPoint.X:F3},{curve.EndPoint.Y:F3}";

                    svg.Add(new XElement(svgNs + "path",
                        new XAttribute("d", attr),
                        new XAttribute("stroke-width", (curve.EndWidth * 2.25).ToString("F3")),
                        new XAttribute("stroke", this.PenColorHex),
                        new XAttribute("fill", "none"),
                        new XAttribute("stroke-linecap", "round")));
                }
            },
            (point, options) =>
            {
                double size = options.DotSize > 0 ? options.DotSize : (options.MinWidth + options.MaxWidth) / 2;
                svg.Add(new XElement(svgNs + "circle",
                    new XAttribute("r", size),
                    new XAttribute("cx", point.X),
                    new XAttribute("cy", point.Y),
                    new XAttribute("fill", this.PenColorHex)));
            });

        return svg;
    }

    private void FromData(List<InkPointGroup> pointGroups, Action<InkBezier, InkPointGroupOptions> drawCurve, Action<InkPoint, InkPointGroupOptions> drawDot)
    {
        LastState lastState = new();
        foreach (var group in pointGroups)
        {
            var points = group.Points;
            var pointGroupOptions = GetPointGroupOptions(group);

            if (points.Count > 1)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    var basicPoint = points[j];
                    var point = new InkPoint(
                        basicPoint.X,
                        basicPoint.Y,
                        basicPoint.Pressure,
                        basicPoint.Time
                    );

                    if (j == 0)
                    {
                        lastState = new(pointGroupOptions);
                    }

                    var curve = lastState.AddPoint(point, pointGroupOptions);

                    if (curve != null)
                    {
                        drawCurve(curve, pointGroupOptions);
                    }
                }
            }
            else
            {
                lastState = new(pointGroupOptions);
                drawDot(points[0], pointGroupOptions);
            }
        }
    }

    private bool IsLeftButtonPressed(PointerEventArgs e, bool only = false)
    {
        var pointProps = e.GetCurrentPoint(this).Properties;
        return only
            ? pointProps.IsLeftButtonPressed && !pointProps.IsRightButtonPressed && !pointProps.IsBarrelButtonPressed && !pointProps.IsXButton1Pressed && !pointProps.IsXButton2Pressed
            : pointProps.IsLeftButtonPressed;
    }
}
