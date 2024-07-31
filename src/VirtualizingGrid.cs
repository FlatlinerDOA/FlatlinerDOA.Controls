namespace FlatlinerDOA.Controls;

using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Media.Immutable;
using Avalonia.Media;
using Avalonia.Data.Converters;
using System.Globalization;

public class VirtualizingGrid : TemplatedControl
{
    public static readonly DirectProperty<VirtualizingGrid, IList<Column>?> ColumnsProperty =
        AvaloniaProperty.RegisterDirect<VirtualizingGrid, IList<Column>?>(
            "Columns",
            o => o.Columns,
            (o, v) => o.Columns = v);

    public static readonly DirectProperty<VirtualizingGrid, IList<Row>?> RowsProperty =
        AvaloniaProperty.RegisterDirect<VirtualizingGrid, IList<Row>?>(
            "Rows",
            o => o.Rows,
            (o, v) => o.Rows = v);

    //public static readonly StyledProperty<double> RowHeadersWidthProperty =
    //    AvaloniaProperty.Register<VirtualizingGrid, double>(nameof(RowHeadersWidth), 32);

    //public static readonly StyledProperty<double> ColumnHeadersHeightProperty =
    //    AvaloniaProperty.Register<VirtualizingGrid, double>(nameof(ColumnHeadersHeight), 32);

    public static readonly StyledProperty<List<List<object?>>?> ItemsProperty =
    AvaloniaProperty.Register<VirtualizingGrid, List<List<object?>>?>(nameof(Items));

    private IList<Column>? _columns = new AvaloniaList<Column>();
    private IList<Row>? _rows = new AvaloniaList<Row>();
    private RowsPresenter? _rowsItemsRepeater;
    //private RowHeadersPresenter? _rowHeadersItemsRepeater;
    //private ScrollViewer? _columnHeadersScrollViewer;
    private ScrollViewer? _rowsItemsRepeaterScrollViewer;

    public VirtualizingGrid()
    {
        
    }

    public IList<Column>? Columns
    {
        get => _columns;
        set => SetAndRaise(ColumnsProperty, ref _columns, value);
    }

    public IList<Row>? Rows
    {
        get => _rows;
        set => SetAndRaise(RowsProperty, ref _rows, value);
    }

    //public double RowHeadersWidth
    //{
    //    get => GetValue(RowHeadersWidthProperty);
    //    set => SetValue(RowHeadersWidthProperty, value);
    //}

    //public double ColumnHeadersHeight
    //{
    //    get => GetValue(ColumnHeadersHeightProperty);
    //    set => SetValue(ColumnHeadersHeightProperty, value);
    //}

    public List<List<object?>>? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _rowsItemsRepeater = e.NameScope.Find<RowsPresenter>("PART_RowsItemsRepeater");
        //_rowHeadersItemsRepeater = e.NameScope.Find<RowHeadersPresenter>("PART_RowHeadersItemsRepeater");
        //_columnHeadersScrollViewer = e.NameScope.Find<ScrollViewer>("PART_ColumnHeadersScrollViewer");

        if (_rowsItemsRepeater is { })
        {
            _rowsItemsRepeater.TemplateApplied += (_, _) =>
            {
                if (_rowsItemsRepeater.Scroll is ScrollViewer scrollViewer)
                {
                    _rowsItemsRepeaterScrollViewer = scrollViewer;
                    _rowsItemsRepeaterScrollViewer.ScrollChanged += RowsItemsRepeaterScrollViewerOnScrollChanged;
                }
            };

            _rowsItemsRepeater.DetachedFromVisualTree += (_, _) =>
            {
                if (_rowsItemsRepeaterScrollViewer is { })
                {
                    _rowsItemsRepeaterScrollViewer.ScrollChanged -= RowsItemsRepeaterScrollViewerOnScrollChanged;
                }
            };
        }
    }

    private void RowsItemsRepeaterScrollViewerOnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        InvalidateScroll();
    }

    private bool _isUpdatingOffset;

    private void InvalidateScroll()
    {
        if (_rowsItemsRepeater?.Scroll is null) //  _rowHeadersItemsRepeater?.Scroll is null || _columnHeadersScrollViewer is null
        {
            return;
        }

        if (Columns is null || Rows is null)
        {
            return;
        }

        if (_isUpdatingOffset)
        {
            return;
        }

        _isUpdatingOffset = true;

        var (x, y) = _rowsItemsRepeater.Scroll.Offset;

        var columnsCount = (double)Columns.Count;
        var rowsCount = (double)Rows.Count;

        var columnIndex = (int)Math.Round(x / (_rowsItemsRepeater.Scroll.Extent.Width / columnsCount), 0);
        var ox = columnIndex * (_rowsItemsRepeater.Scroll.Extent.Width / columnsCount);

        var rowIndex = (int)Math.Round(y / (_rowsItemsRepeater.Scroll.Extent.Height / rowsCount), 0);
        var oy = rowIndex * (_rowsItemsRepeater.Scroll.Extent.Height / rowsCount);

        if (double.IsNaN(ox) || double.IsNaN(oy))
        {
            return;
        }

        _rowsItemsRepeater.Scroll.Offset = new Vector(ox, oy);
        //_rowHeadersItemsRepeater.Scroll.Offset = new Vector(0, oy);
        //_columnHeadersScrollViewer.Offset = new Vector(ox, 0);

        _isUpdatingOffset = false;
    }
}

public class Column : AvaloniaObject
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<Column, object?>(nameof(Header));

    public static readonly StyledProperty<double> WidthProperty =
        AvaloniaProperty.Register<Column, double>(nameof(Width));

    public static readonly StyledProperty<int> IndexProperty =
        AvaloniaProperty.Register<Column, int>(nameof(Index));

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public double Width
    {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    public int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }
}

public class Row : AvaloniaObject
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<Row, object?>(nameof(Header));

    public static readonly StyledProperty<double> HeightProperty =
        AvaloniaProperty.Register<Row, double>(nameof(Height));

    public static readonly StyledProperty<int> IndexProperty =
        AvaloniaProperty.Register<Row, int>(nameof(Index));

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public double Height
    {
        get => GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }

    public int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }
}

public class RowsPresenterItem : ListBoxItem, IStyleable
{
    Type IStyleable.StyleKey => typeof(ListBoxItem);
}


public class CellsPresenter : ItemsRepeater
{
}

public class RowsPresenter : ListBox, IStyleable
{
    Type IStyleable.StyleKey => typeof(RowsPresenter);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new RowsPresenterItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<RowsPresenterItem>(item, out recycleKey);
    }
}

public class Cell : Decorator
{
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var thickness = 1.0;
        var offset = thickness * 0.5;

        context.DrawRectangle(
            Brushes.White,
            new ImmutablePen(Brushes.DarkGray, thickness),
            new Rect(new Point(offset, offset), new Size(Bounds.Size.Width + offset, Bounds.Size.Height + offset)));
    }
}

public class CellDataConverter : IMultiValueConverter
{
    public static CellDataConverter Instance = new();

    public object Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values?.Count == 3
            && values[0] is List<List<object>> items
            && values[1] is int columnIndex
            && values[2] is int rowIndex)
        {
            if (items.Count > rowIndex)
            {
                var fields = items[rowIndex];
                if (fields.Count > columnIndex)
                {
                    return fields[columnIndex];
                }
            }
        }

        return AvaloniaProperty.UnsetValue;
    }
}