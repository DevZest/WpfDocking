using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DevZest.Windows
{
    /// <summary>Represents a movable and resizable window hosted in <see cref="WindowPanel"/>.</summary>
    /// <remarks>
    /// <para>
    /// Use <see cref="Left"/> and <see cref="Top"/>  to specify the location of the window.
    /// <see cref="Double.NaN"/> value results in auto location - the location will be
    /// decided by its containing <see cref="WindowPanel"/>. The containing <see cref="WindowPanel"/>
    /// sets the window's <see cref="ActualLeft"/> and <see cref="ActualTop"/> properties.
    /// </para>
    /// <para>
    /// Use <see cref="P:DevZest.Windows.WindowControl.Hotspot"/> attached property to specify the <see cref="WindowHotspot"/>
    /// mouse dragging value for elements in the control template. By default,
    /// <see cref="WindowControl"/> inherits <see cref="ContentControl"/>'s control template which
    /// provides no movable or resizable user interface.
    /// </para>
    /// </remarks>
    /// <example>
    ///     This example demostrates use of <see cref="WindowPanel"/> and <see cref="WindowControl"/> 
    ///     (reference to assembly PresentationFramework.Classic.dll required):
    ///     <code lang="xaml" source="..\..\Samples\Common\CSharp\WindowPanelAndWindowControlSample\Window1.xaml" />
    /// </example>
    public partial class WindowControl : ContentControl, IWindow
    {
        /// <summary>Identifies the <see cref="P:DevZest.Windows.WindowControl.Hotspot"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the the <see cref="WindowHotspot"/> value.</summary>
        /// <value>One of the <see cref="WindowHotspot"/> values. The default value is <see cref="WindowHotspot.None"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty HotspotProperty;
        /// <summary>Identifies the <see cref="Left"/> dependency property.</summary>
        public static readonly DependencyProperty LeftProperty;
        /// <summary>Identifies the <see cref="Top"/> dependency property.</summary>
        public static readonly DependencyProperty TopProperty;
        private static readonly DependencyPropertyKey ActualLeftPropertyKey;
        private static readonly DependencyPropertyKey ActualTopPropertyKey;
        /// <summary>Identifies the <see cref="ActualLeft"/> dependency property.</summary>
        public static readonly DependencyProperty ActualLeftProperty;
        /// <summary>Identifies the <see cref="ActualTop"/> dependency property.</summary>
        public static readonly DependencyProperty ActualTopProperty;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static WindowControl()
        {
            HotspotProperty = DependencyProperty.RegisterAttached("Hotspot", typeof(WindowHotspot), typeof(WindowControl),
                new FrameworkPropertyMetadata(WindowHotspot.None));
            TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(WindowControl),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsParentArrange));
            LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(WindowControl),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsParentArrange));
            ActualLeftPropertyKey = DependencyProperty.RegisterReadOnly("ActualLeft", typeof(double), typeof(WindowControl),
                new FrameworkPropertyMetadata(double.NaN));
            ActualTopPropertyKey = DependencyProperty.RegisterReadOnly("ActualTop", typeof(double), typeof(WindowControl),
                new FrameworkPropertyMetadata(double.NaN));
            ActualLeftProperty = ActualLeftPropertyKey.DependencyProperty;
            ActualTopProperty = ActualTopPropertyKey.DependencyProperty;
            FocusableProperty.OverrideMetadata(typeof(WindowControl), new FrameworkPropertyMetadata(BooleanBoxes.False));
            FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(WindowControl), new FrameworkPropertyMetadata(BooleanBoxes.False));
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.WindowControl.Hotspot"/> attached property for a given <see cref="DependencyObject"/>.</summary>
        /// <param name="element">The element from which the property value is read.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.WindowControl.Hotspot"/> attached property.</returns>
        public static WindowHotspot GetHotspot(DependencyObject element)
        {
            return (WindowHotspot)element.GetValue(HotspotProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.WindowControl.Hotspot"/> attached property for a given dependency object.</summary>
        /// <param name="element">The element to which the property value is written.</param>
        /// <param name="value">The value of <see cref="P:DevZest.Windows.WindowControl.Hotspot"/> attached property.</param>
        public static void SetHotspot(DependencyObject element, WindowHotspot value)
        {
            element.SetValue(HotspotProperty, value);
        }

        /// <summary>Gets or sets a value that represents the distance between the left side of <see cref="WindowControl"/> and the left side of its parent <see cref="WindowPanel"/>. This is a dependency property.</summary>
        /// <value>A <see cref="Double"/> that represents the offset position from the left side of a parent <see cref="WindowPanel"/>. The default value is <see cref="Double.NaN"/>.</value>
        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        /// <summary>Gets or sets a value that represents the distance between the top side of <see cref="WindowControl"/> and the top side of its parent <see cref="WindowPanel"/>. This is a dependency property.</summary>
        /// <value>A <see cref="Double"/> that represents the offset position from the top side of a parent <see cref="WindowPanel"/>. The default value is <see cref="Double.NaN"/>.</value>
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        /// <summary>Gets the actual left position of this <see cref="WindowControl"/>.</summary>
        /// <value>The <see cref="WindowControl"/>'s left position, as a value in device-independent units (1/96th inch per unit).</value>
        public double ActualLeft
        {
            get { return (double)GetValue(ActualLeftProperty); }
            internal set { SetValue(ActualLeftPropertyKey, value); }
        }

        /// <summary>Gets the actual top position of this <see cref="WindowControl"/>.</summary>
        /// <value>The <see cref="WindowControl"/>'s top position, as a value in device-independent units (1/96th inch per unit).</value>
        public double ActualTop
        {
            get { return (double)GetValue(ActualTopProperty); }
            internal set { SetValue(ActualTopPropertyKey, value); }
        }

        /// <exclude/>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
                return;

            BeginDrag(this, TopLevelWindow, e);
        }

        internal static bool BeginDrag(UIElement container, IWindow topLevelWindow, MouseButtonEventArgs e)
        {
            UIElement element = e.OriginalSource as UIElement;
            WindowHotspot hotspot = WindowHotspot.None;
            while (element != null && element != container)
            {
                hotspot = GetHotspot(element);
                if (hotspot != WindowHotspot.None)
                    break;

                element = VisualTreeHelper.GetParent(element) as UIElement;
            }

            if (hotspot == WindowHotspot.None)
                return false;

            if (e.ClickCount == 1)
            {
                DragHandler.Default.BeginDrag(topLevelWindow, element, e);
                e.Handled = true;
                return true;
            }

            return false;
        }

        IWindow TopLevelWindow
        {
            get
            {
                IWindow topLevelWindow = this;
                for (DependencyObject parent = VisualTreeHelper.GetParent(this); parent != null; parent = VisualTreeHelper.GetParent(parent))
                {
                    if (parent is IWindow)
                        topLevelWindow = parent as IWindow;
                }
                return topLevelWindow;
            }
        }

        /// <summary>Gets the adjusted location to ensure the window is visible in its container.</summary>
        /// <param name="containerSize">The size of its container.</param>
        /// <param name="windowBounds">The window bounds.</param>
        /// <returns>The adjusted location.</returns>
        protected internal virtual Point EnsureVisibleLocation(Size containerSize, Rect windowBounds)
        {
            double x = windowBounds.X;
            if (x > containerSize.Width - SystemParameters.CaptionWidth)
                x = containerSize.Width - SystemParameters.CaptionWidth;
            if (x < SystemParameters.CaptionWidth - windowBounds.Width)
                x = SystemParameters.CaptionWidth - windowBounds.Width;

            double y = windowBounds.Y;
            if (y > containerSize.Height - SystemParameters.CaptionHeight)
                y = containerSize.Height - SystemParameters.CaptionHeight;
            if (y < 0)
                y = 0;

            return new Point(x, y);
        }

        Rect IWindow.Bounds
        {
            get { return new Rect(Left, Top, Width, Height); }
        }

        Rect IWindow.ActualBounds
        {
            get { return new Rect(ActualLeft, ActualTop, ActualWidth, ActualHeight); }
        }

        void IWindow.SetBounds(Rect bounds)
        {
            BeginInit();
            Left = bounds.Left;
            Top = bounds.Top;
            Width = bounds.Width;
            Height = bounds.Height;
            EndInit();
        }
    }
}
