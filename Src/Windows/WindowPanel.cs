using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace DevZest.Windows
{
    /// <summary>Defines an area where you can position child <see cref="WindowControl"/> elements.</summary>
    /// <example>
    ///     This example demostrates use of <see cref="WindowPanel"/> and <see cref="WindowControl"/> 
    ///     (reference to assembly PresentationFramework.Classic.dll required):
    ///     <code lang="xaml" source="..\..\Samples\Common\CSharp\WindowPanelAndWindowControlSample\Window1.xaml" />
    /// </example>
    public partial class WindowPanel : Panel
    {
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static WindowPanel()
        {
            FocusableProperty.OverrideMetadata(typeof(WindowPanel), new FrameworkPropertyMetadata(BooleanBoxes.False));
            FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(WindowPanel), new FrameworkPropertyMetadata(BooleanBoxes.False));
            ClipToBoundsProperty.OverrideMetadata(typeof(WindowPanel), new FrameworkPropertyMetadata(BooleanBoxes.True));
        }

        /// <exclude/>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size infinitySize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement uiElement in InternalChildren)
            {
                WindowControl window = uiElement as WindowControl;
                if (window == null)
                    continue;
                window.Measure(infinitySize);
            }

            return new Size(double.IsPositiveInfinity(availableSize.Width) ? 0 : availableSize.Width,
                double.IsPositiveInfinity(availableSize.Height) ? 0 : availableSize.Height);
        }

        /// <exclude/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement uiElement in InternalChildren)
            {
                WindowControl window = uiElement as WindowControl;
                if (window == null)
                    continue;
                Point position = GetWindowPosition(window, finalSize);
                Size size = window.DesiredSize;
                position = window.EnsureVisibleLocation(finalSize, new Rect(position, size));
                window.ActualLeft = position.X;
                if (double.IsNaN(window.Left))
                    window.Left = position.X;
                window.ActualTop = position.Y;
                if (double.IsNaN(window.Top))
                    window.Top = position.Y;
                window.Arrange(new Rect(position, size));
            }

            return finalSize;
        }

        private Point GetWindowPosition(WindowControl window, Size availableSize)
        {
            double left = double.IsNaN(window.Left) ? window.ActualLeft : window.Left;
            double top = double.IsNaN(window.Top) ? window.ActualTop : window.Top;

            Point defaultPosition = new Point();
            if (double.IsNaN(left) || double.IsNaN(top))
                defaultPosition = GetDefaultLocation(availableSize, window);

            if (double.IsNaN(left))
                left = defaultPosition.X;

            if (double.IsNaN(top))
                top = window.Top = defaultPosition.Y;

            return new Point(left, top);
        }

        private static Random s_random = new Random();
        private static Random Random
        {
            get
            {
                if (s_random == null)
                    s_random = new Random();
                return s_random;
            }
        }

        /// <summary>Gets the default location for specified <see cref="WindowControl" />.</summary>
        /// <param name="availableSize">The size of the <see cref="WindowControl"/> container.</param>
        /// <param name="window">The specified <see cref="WindowControl" />.</param>
        /// <returns>The default location.</returns>
        /// <remarks>The default implementation returns a random location.</remarks>
        protected virtual Point GetDefaultLocation(Size availableSize, WindowControl window)
        {
            double x, y;

            x = (availableSize.Width - window.DesiredSize.Width) * Random.NextDouble();
            y = (availableSize.Height - window.DesiredSize.Height) * Random.NextDouble();

            return new Point(Math.Max(x, 0), Math.Max(y, 0));
        }
    }
}
