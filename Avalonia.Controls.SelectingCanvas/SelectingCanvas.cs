namespace Avalonia.Controls.SelectingCanvas;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using System;
using System.Linq;

public class SelectingCanvas : Canvas
{
    public static readonly StyledProperty<IBrush?> SelectionFillProperty =
         AvaloniaProperty.Register<SelectingCanvas, IBrush?>(nameof(SelectionFill), new SolidColorBrush(Colors.CornflowerBlue, 0.1).ToImmutable());

    public static readonly StyledProperty<IBrush?> SelectionStrokeProperty =
     AvaloniaProperty.Register<SelectingCanvas, IBrush?>(nameof(SelectionStroke), Brushes.CornflowerBlue);

    public static readonly StyledProperty<double> SelectionStrokeThicknessProperty =
        AvaloniaProperty.Register<SelectingCanvas, double>(nameof(SelectionStrokeThickness), 1.0d);

    public static readonly AttachedProperty<bool> IsSelectedProperty =
        AvaloniaProperty.RegisterAttached<SelectingCanvas, Control, bool>("IsSelected", false, defaultBindingMode: Data.BindingMode.TwoWay);

    public static readonly AttachedProperty<bool> IsSelectableProperty =
        AvaloniaProperty.RegisterAttached<SelectingCanvas, Control, bool>("IsSelectable", true);

    private Point? startPoint;
    private Point? endPoint;

    public SelectingCanvas()
    {
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;

    }

    private Rectangle? SelectionArea { get; set; }

    /// <summary>
    /// Gets or sets the background fill brush of the selection area (Default is CornflowBlue @ 10% opacity).
    /// </summary>
    public IBrush? SelectionFill
    {
        get => this.GetValue(SelectionFillProperty);
        set => this.SetValue(SelectionFillProperty, value);
    }

    /// <summary>
    /// Gets or sets the outline stroke brush of the selection area (default is CornflowerBlue).
    /// </summary>
    public IBrush? SelectionStroke
    {
        get => this.GetValue(SelectionStrokeProperty);
        set => this.SetValue(SelectionStrokeProperty, value);
    }

    /// <summary>
    /// Gets or sets the outline stroke thickness of the selection area (default is 1).
    /// </summary>
    public double SelectionStrokeThickness
    {
        get => this.GetValue(SelectionStrokeThicknessProperty);
        set => this.SetValue(SelectionStrokeThicknessProperty, value);
    }

    /// <summary>
    /// Gets the attached value whether the control is selected.
    /// </summary>
    /// <param name="element">The control.</param>
    /// <returns>The control's left coordinate.</returns>
    public static bool GetIsSelected(AvaloniaObject element)
    {
        return element.GetValue(IsSelectedProperty);
    }

    /// <summary>
    /// Sets the attached value of whether the control is is selected.
    /// </summary>
    /// <param name="element">The control.</param>
    /// <param name="value">The left value.</param>
    public static void SetIsSelected(AvaloniaObject element, bool value)
    {
        element.SetValue(IsSelectedProperty, value);
    }


    /// <summary>
    /// Gets the attached value whether the control can be selected.
    /// </summary>
    /// <param name="element">The control.</param>
    /// <returns>The control's left coordinate.</returns>
    public static bool GetIsSelectable(AvaloniaObject element)
    {
        return element.GetValue(IsSelectableProperty);
    }

    /// <summary>
    /// Sets the attached value whether the control can be selected.
    /// </summary>
    /// <param name="element">The control.</param>
    /// <param name="value">The left value.</param>
    public static void SetIsSelectable(AvaloniaObject element, bool value)
    {
        element.SetValue(IsSelectableProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (this.startPoint == null)
        {
            this.startPoint = e.GetCurrentPoint(this).Position;
            this.endPoint = e.GetCurrentPoint(this).Position;
            this.SelectionArea = new Rectangle();
            this.SelectionArea.Fill = this.SelectionFill;
            this.SelectionArea.Stroke = this.SelectionStroke;
            this.SelectionArea.StrokeThickness = this.SelectionStrokeThickness;
            this.SelectionArea.ZIndex = 100000;
            this.Children.Add(this.SelectionArea);
            this.UpdateSelectionArea();
            base.OnPointerPressed(e);
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (this.startPoint != null)
        {
            this.endPoint = e.GetCurrentPoint(this).Position;
            this.UpdateSelectionArea();
        }

        base.OnPointerMoved(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        this.startPoint = null;
        this.endPoint = null;
        this.UpdateSelectionArea();
        base.OnPointerReleased(e);
    }

    /// <summary>
    /// Updates the bounding box of the rectangle or removes it, and updates whether any controls are selected or not.
    /// </summary>
    private void UpdateSelectionArea()
    {
        if (this.startPoint is not null && this.endPoint is not null && this.SelectionArea is not null)
        {
            var bounds = new Rect(0, 0, this.Bounds.Width, this.Bounds.Height);
            var minX = Math.Min(this.startPoint.Value.X, this.endPoint.Value.X);
            var maxX = Math.Max(this.startPoint.Value.X, this.endPoint.Value.X);
            var minY = Math.Min(this.startPoint.Value.Y, this.endPoint.Value.Y);
            var maxY = Math.Max(this.startPoint.Value.Y, this.endPoint.Value.Y);

            // Constrain the selection box to the canvas bounds
            minX = Math.Max(minX, bounds.X);
            maxX = Math.Min(maxX, bounds.Right);
            minY = Math.Max(minY, bounds.Y);
            maxY = Math.Min(maxY, bounds.Bottom);

            var selectionBounds = new Rect(
                minX,
                minY,
                maxX - minX,
                maxY - minY);


            this.SelectionArea.SetCurrentValue(Canvas.TopProperty, selectionBounds.Y);
            this.SelectionArea.SetCurrentValue(Canvas.LeftProperty, selectionBounds.X);
            this.SelectionArea.Width = selectionBounds.Width;
            this.SelectionArea.Height = selectionBounds.Height;

            foreach (var child in this.GetLogicalDescendants().OfType<Control>())
            {
                if (child != this.SelectionArea && GetIsSelectable(child))
                {
                    var childBounds = child.TransformToVisual(this) is Matrix m
                        ? m.TransformBounds(new Rect(0, 0, child.Width, child.Height)) :
                        child.Bounds;
                    SetIsSelected(child, childBounds.Intersects(selectionBounds));

                    //    var childBounds = new Rect(child.GetValue(Canvas.LeftProperty), child.GetValue(Canvas.TopProperty), child.Width, child.Height);
                    //    SetIsSelected(child, childBounds.Intersects(selectionBounds));
                }
            }

            this.InvalidateArrange();
        }
        else if (this.SelectionArea is not null)
        {
            this.Children.Remove(this.SelectionArea);
            this.SelectionArea = null;
            this.InvalidateArrange();
        }
    }
}
