
namespace FlatlinerDOA.Controls
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Layout;
    using Avalonia.Utilities;
    using System.Data;
/*
    public class VirtualizingWrapPanel : VirtualizingPanel
    {
        /// <summary>
        /// Orientation StyledProperty definition
        /// </summary>
        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<VirtualizingWrapPanel, Orientation>(nameof(Orientation));

        /// <summary>
        /// ItemWidth StyledProperty definition
        /// </summary>
        public static readonly StyledProperty<double> ItemWidthProperty =
            AvaloniaProperty.Register<VirtualizingWrapPanel, double>(nameof(ItemWidth));

        /// <summary>
        /// ItemHeight StyledProperty definition
        /// </summary>
        public static readonly StyledProperty<double> ItemHeightProperty =
            AvaloniaProperty.Register<VirtualizingWrapPanel, double>(nameof(ItemHeight));

        /// <summary>
        /// Gets or sets the ItemWidth property. This StyledProperty
        /// indicates ....
        /// </summary>
        public double ItemWidth
        {
            get => this.GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the ItemHeight property. This StyledProperty
        /// indicates ....
        /// </summary>
        public double ItemHeight
        {
            get => this.GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the Orientation property. This StyledProperty
        /// indicates ....
        /// </summary>
        public Orientation Orientation
        {
            get => this.GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Gets the number of items that will fit along the U axis.
        /// e.g. Orientation = Horizontal, ItemWidth = 50, DesiredSize.Width = 600, then the result of this is 600 / 50 = 12
        /// Orientation = Vertical, ItemHeight = 100, DesiredSize.Height = 600, then the result of this is 600 / 100 = 6
        /// </summary>
        public int ItemsAlongAxis =>
            this.Orientation == Orientation.Horizontal ?
            (int)Math.Ceiling(this.DesiredSize.Height / this.ItemWidth) :
            (int)Math.Ceiling(this.DesiredSize.Height / this.ItemHeight);

        protected override Control? ContainerFromIndex(int index)
        {
            if (index < 0 || index >= Items.Count)
                return null;
            if (_scrollToIndex == index)
                return _scrollToElement;
            if (_focusedIndex == index)
                return _focusedElement;
            if (index == _realizingIndex)
                return _realizingElement;
            if (GetRealizedElement(index) is { } realized)
                return realized;
            if (Items[index] is Control c && c.GetValue(RecycleKeyProperty) == s_itemIsItsOwnContainer)
                return c;
            return null;
        }

        protected override IInputElement? GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
        {
            var v = new VirtualizingStackPanel();
            throw new NotImplementedException();
        }

        protected override IEnumerable<Control>? GetRealizedContainers()
        {
            throw new NotImplementedException();
        }

        protected override int IndexFromContainer(Control container)
        {
            throw new NotImplementedException();
        }

        protected override Control? ScrollIntoView(int index)
        {
            throw new NotImplementedException();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            var orientation = Orientation;
            var children = Children;
            var curLineSize = new UVSize(orientation);
            var panelSize = new UVSize(orientation);
            var uvConstraint = new UVSize(orientation, availableSize.Width, availableSize.Height);
            bool itemWidthSet = !double.IsNaN(itemWidth);
            bool itemHeightSet = !double.IsNaN(itemHeight);

            var childConstraint = new Size(
                itemWidthSet ? itemWidth : availableSize.Width,
                itemHeightSet ? itemHeight : availableSize.Height);

            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];
                // Flow passes its own constraint to children
                child.Measure(childConstraint);

                // This is the size of the child in UV space
                var sz = new UVSize(orientation,
                    itemWidthSet ? itemWidth : child.DesiredSize.Width,
                    itemHeightSet ? itemHeight : child.DesiredSize.Height);

                if (MathUtilities.GreaterThan(curLineSize.U + sz.U, uvConstraint.U)) // Need to switch to another line
                {
                    panelSize.U = Math.Max(curLineSize.U, panelSize.U);
                    panelSize.V += curLineSize.V;
                    curLineSize = sz;

                    if (MathUtilities.GreaterThan(sz.U, uvConstraint.U)) // The element is wider then the constraint - give it a separate line
                    {
                        panelSize.U = Math.Max(sz.U, panelSize.U);
                        panelSize.V += sz.V;
                        curLineSize = new UVSize(orientation);
                    }
                }
                else // Continue to accumulate a line
                {
                    curLineSize.U += sz.U;
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                }
            }

            // The last line size, if any should be added
            panelSize.U = Math.Max(curLineSize.U, panelSize.U);
            panelSize.V += curLineSize.V;

            // Go from UV space to W/H space
            return new Size(panelSize.Width, panelSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            var orientation = Orientation;
            var children = Children;
            int firstInLine = 0;
            double accumulatedV = 0;
            double itemU = orientation == Orientation.Horizontal ? itemWidth : itemHeight;
            var curLineSize = new UVSize(orientation);
            var uvFinalSize = new UVSize(orientation, finalSize.Width, finalSize.Height);
            bool itemWidthSet = !double.IsNaN(itemWidth);
            bool itemHeightSet = !double.IsNaN(itemHeight);
            bool useItemU = orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet;

            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                var sz = new UVSize(orientation,
                    itemWidthSet ? itemWidth : child.DesiredSize.Width,
                    itemHeightSet ? itemHeight : child.DesiredSize.Height);

                if (MathUtilities.GreaterThan(curLineSize.U + sz.U, uvFinalSize.U)) // Need to switch to another line
                {
                    ArrangeLine(accumulatedV, curLineSize.V, firstInLine, i, useItemU, itemU);

                    accumulatedV += curLineSize.V;
                    curLineSize = sz;

                    if (MathUtilities.GreaterThan(sz.U, uvFinalSize.U)) // The element is wider then the constraint - give it a separate line
                    {
                        // Switch to next line which only contain one element
                        ArrangeLine(accumulatedV, sz.V, i, ++i, useItemU, itemU);

                        accumulatedV += sz.V;
                        curLineSize = new UVSize(orientation);
                    }
                    firstInLine = i;
                }
                else // Continue to accumulate a line
                {
                    curLineSize.U += sz.U;
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                }
            }

            // Arrange the last line, if any
            if (firstInLine < children.Count)
            {
                ArrangeLine(accumulatedV, curLineSize.V, firstInLine, children.Count, useItemU, itemU);
            }

            return finalSize;
        }

        private void ArrangeLine(double v, double lineV, int start, int end, bool useItemU, double itemU)
        {
            var orientation = Orientation;
            var children = Children;
            double u = 0;
            bool isHorizontal = orientation == Orientation.Horizontal;

            for (int i = start; i < end; i++)
            {
                var child = children[i];
                var childSize = new UVSize(orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                double layoutSlotU = useItemU ? itemU : childSize.U;
                child.Arrange(new Rect(
                    isHorizontal ? u : v,
                    isHorizontal ? v : u,
                    isHorizontal ? layoutSlotU : lineV,
                    isHorizontal ? lineV : layoutSlotU));
                u += layoutSlotU;
            }
        }

        //private int IndexFromXY(int x, int y)
        //{

        //}

        private struct UVSize
        {
            internal UVSize(Orientation orientation, double width, double height)
            {
                U = V = 0d;
                _orientation = orientation;
                Width = width;
                Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                U = V = 0d;
                _orientation = orientation;
            }

            internal double U;
            internal double V;
            private Orientation _orientation;

            internal double Width
            {
                get => _orientation == Orientation.Horizontal ? U : V;
                set { if (_orientation == Orientation.Horizontal) U = value; else V = value; }
            }
            internal double Height
            {
                get => _orientation == Orientation.Horizontal ? V : U;
                set { if (_orientation == Orientation.Horizontal) V = value; else U = value; }
            }
        }
    }
*/
}
