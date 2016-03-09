using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Security;
using System.Security.Permissions;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a floating window which contains a <see cref="Docking.DockTree"/>.</summary>
    public partial class FloatingWindow : DependencyObject
    {
        private static readonly DependencyPropertyKey DockControlPropertyKey = DependencyProperty.RegisterReadOnly("DockControl", typeof(DockControl), typeof(FloatingWindow),
            new FrameworkPropertyMetadata(null));
        /// <summary>Identifies the <see cref="DockControl"/> dependency property.</summary>
        public static readonly DependencyProperty DockControlProperty = DockControlPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey DockTreePropertyKey = DependencyProperty.RegisterReadOnly("DockTree", typeof(DockTree), typeof(FloatingWindow),
            new FrameworkPropertyMetadata(null));
        /// <summary>Identifies the <see cref="DockTree"/> dependency property.</summary>
        public static readonly DependencyProperty DockTreeProperty = DockTreePropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsVisible", typeof(bool), typeof(FloatingWindow),
            new FrameworkPropertyMetadata(BooleanBoxes.False));
        /// <summary>Identifies the <see cref="IsVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsVisibleProperty = IsVisiblePropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey CountOfVisiblePanesPropertyKey = DependencyProperty.RegisterReadOnly("CountOfVisiblePanes", typeof(int), typeof(FloatingWindow),
            new FrameworkPropertyMetadata(-1));
        /// <summary>Identifies the <see cref="CountOfVisiblePanes"/> dependency property.</summary>
        public static readonly DependencyProperty CountOfVisiblePanesProperty = CountOfVisiblePanesPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey FirstVisiblePanePropertyKey = DependencyProperty.RegisterReadOnly("FirstVisiblePane", typeof(DockPane), typeof(FloatingWindow),
            new FrameworkPropertyMetadata(null));
        /// <summary>Identifies the <see cref="FirstVisiblePane"/> dependency property.</summary>
        public static readonly DependencyProperty FirstVisiblePaneProperty = FirstVisiblePanePropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="Left"/> dependency property.</summary>
        public static readonly DependencyProperty LeftProperty = WindowControl.LeftProperty.AddOwner(typeof(FloatingWindow), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBoundsChanged)));
        /// <summary>Identifies the <see cref="Top"/> dependency property.</summary>
        public static readonly DependencyProperty TopProperty = WindowControl.TopProperty.AddOwner(typeof(FloatingWindow), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBoundsChanged)));
        /// <summary>Identifies the <see cref="Width"/> dependency property.</summary>
        public static readonly DependencyProperty WidthProperty = WindowControl.WidthProperty.AddOwner(typeof(FloatingWindow), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBoundsChanged)));
        /// <summary>Identifies the <see cref="Height"/> dependency property.</summary>
        public static readonly DependencyProperty HeightProperty = WindowControl.HeightProperty.AddOwner(typeof(FloatingWindow), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBoundsChanged)));

        private static void OnBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FloatingWindow floatingWindow = (FloatingWindow)d;
            DockControl dockControl = floatingWindow.DockControl;
            if (dockControl == null)
                return;

            Rect oldBounds = floatingWindow.GetBounds(e.Property, (double)e.OldValue);
            Rect newBounds = floatingWindow.GetBounds(e.Property, (double)e.NewValue);
            UpdateBoundsData oldValue = new UpdateBoundsData(floatingWindow, oldBounds);
            UpdateBoundsData newValue = new UpdateBoundsData(floatingWindow, newBounds);
            dockControl.OnValueChanged(oldValue, newValue,
                delegate(UpdateBoundsData oldData, UpdateBoundsData newData)
                {
                    return new UpdateBoundsCommand(oldData.FloatingWindow, oldData.Value, newData.Value);
                });
        }

        private Rect GetBounds(DependencyProperty dp, double value)
        {
            if (dp == LeftProperty)
                return new Rect(value, Top, Width, Height);
            else if (dp == TopProperty)
                return new Rect(Left, value, Width, Height);
            else if (dp == WidthProperty)
                return new Rect(Left, Top, value, Height);
            else
            {
                Debug.Assert(dp == HeightProperty);
                return new Rect(Left, Top, Width, value);
            }
        }

        internal FloatingWindow(DockControl dockControl, Rect bounds)
        {
            Debug.Assert(dockControl != null);
            DockControl = dockControl;
            Left = bounds.Left;
            Top = bounds.Top;
            Width = bounds.Width;
            Height = bounds.Height;
            DockTree = new DockTree(this);
        }

        /// <summary>Gets the <see cref="Docking.DockControl"/> which this <see cref="FloatingWindow"/> belongs to. This is a dependency property.</summary>
        /// <value>The <see cref="Docking.DockControl"/> which this <see cref="FloatingWindow"/> belongs to.</value>
        public DockControl DockControl
        {
            get { return (DockControl)GetValue(DockControlProperty); }
            private set { SetValue(DockControlPropertyKey, value); }
        }

        /// <summary>Gets the <see cref="Docking.DockTree"/> which this <see cref="FloatingWindow"/> contains. This is a dependency property.</summary>
        /// <value>The <see cref="Docking.DockTree"/> which this <see cref="FloatingWindow"/> contains.</value>
        public DockTree DockTree
        {
            get { return (DockTree)GetValue(DockTreeProperty); }
            private set { SetValue(DockTreePropertyKey, value); }
        }

        /// <summary>Gets a value indicates this <see cref="FloatingWindow"/> is visible. This is a dependency property.</summary>
        /// <value><see langword="true"/> if this <see cref="FloatingWindow"/> is visible, otherwise <see langword="false"/>.</value>
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            internal set { SetValue(IsVisiblePropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets the left position of this <see cref="FloatingWindow"/>. This is a dependency property.</summary>
        /// <value>The left position of this <see cref="FloatingWindow"/>.</value>
        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        /// <summary>Gets the top position of this <see cref="FloatingWindow"/>. This is a dependency property.</summary>
        /// <value>The top position of this <see cref="FloatingWindow"/>.</value>
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        /// <summary>Gets the width of this <see cref="FloatingWindow"/>. This is a dependency property.</summary>
        /// <value>The width of this <see cref="FloatingWindow"/>.</value>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>Gets the height of this <see cref="FloatingWindow"/>. This is a dependency property.</summary>
        /// <value>The height of this <see cref="FloatingWindow"/>.</value>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>Gets a collection of <see cref="DockPane"/> objects contained by this <see cref="FloatingWindow"/>.</summary>
        /// <value>A collection of <see cref="DockPane"/> objects contained by this <see cref="FloatingWindow"/>, in order of
        /// creation (last created last).</value>
        public DockPaneCollection Panes
        {
            get { return DockTree.Panes; }
        }

        /// <summary>Gets a collection of visible <see cref="DockPane"/> objects contained by this <see cref="FloatingWindow"/>.</summary>
        /// <value>A collection of visible <see cref="DockPane"/> objects contained by this <see cref="FloatingWindow"/>, in order of
        /// creation (last created last).</value>
        public DockPaneCollection VisiblePanes
        {
            get { return DockTree.VisiblePanes; }
        }

        /// <summary>Gets a collection of active <see cref="DockPane"/> objects contained by this <see cref="FloatingWindow"/>.</summary>
        /// <value>A collection of active <see cref="DockPane"/> objects contained by this <see cref="FloatingWindow"/>, in order of
        /// activation (last activated last).</value>
        public DockPaneCollection ActivePanes
        {
            get { return DockTree.ActivePanes; }
        }

        /// <summary>Gets the number of visible <see cref="DockPane"/> objects contained by this <see cref="FloatingWindow"/>. This is a dependency property.</summary>
        /// <value>The number of visible <see cref="DockPane"/> objects contained by this <see cref="FloatingWindow"/>.</value>
        public int CountOfVisiblePanes
        {
            get { return (int)GetValue(CountOfVisiblePanesProperty); }
            internal set { SetValue(CountOfVisiblePanesPropertyKey, value); }
        }

        /// <summary>Gets the first visible <see cref="DockPane"/> object contained by this <see cref="FloatingWindow"/>. This is a dependency property.</summary>
        /// <value>The first visible <see cref="DockPane"/> object contained by this <see cref="FloatingWindow"/>.</value>
        public DockPane FirstVisiblePane
        {
            get { return (DockPane)GetValue(FirstVisiblePaneProperty); }
            internal set { SetValue(FirstVisiblePanePropertyKey, value); }
        }

        internal void PerformClose()
        {
            DockPaneCollection srcPanes = ActivePanes;
            DockPane[] panes = new DockPane[srcPanes.Count];
            srcPanes.CopyTo(panes, 0);

            for (int i = panes.Length - 1; i >= 0; i--)
            {
                DockPane pane = panes[i];
                DockItemCollection srcItems = pane.ActiveItems;
                DockItem[] items = new DockItem[srcItems.Count];
                srcItems.CopyTo(items, 0);
                for (int j = items.Length - 1; j >= 0; j--)
                    items[j].PerformClose();
            }
        }

        static bool? s_canBeNative;
        /// <summary>Gets a value indicates whether the application has UIPermission to show native floating window.</summary>
        /// <value><see langword="true"/> if application has UIPermission to show native floating window, otherwise <see langword="false" /></value>
        public static bool CanBeNative
        {
            get
            {
                if (!s_canBeNative.HasValue)
                {
                    try
                    {
                        IPermission permission = new UIPermission(UIPermissionWindow.AllWindows);
                        permission.Demand();
                        s_canBeNative = true;
                    }
                    catch (SecurityException)
                    {
                        s_canBeNative = false;
                    }
                }

                return s_canBeNative.Value;
            }
        }
    }
}
