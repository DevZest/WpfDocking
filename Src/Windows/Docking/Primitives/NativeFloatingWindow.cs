using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Globalization;
using System.Security.Permissions;
using System.ComponentModel;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the native floating window.</summary>
    /// <remarks>Since the <see cref="Window"/> class requires <see cref="UIPermissionWindow.AllWindows"/> of <see cref="System.Security.Permissions.UIPermission"/>,
    /// when the application is running under partial trust, for example XBAP, the <see cref="WpfFloatingWindow"/> class will be used.</remarks>
    public sealed class NativeFloatingWindow : Window, IDragSource, IWindow
    {
        private static readonly DependencyProperty NativeFloatingWindowProperty = DependencyProperty.RegisterAttached("NativeFloatingWindow", typeof(NativeFloatingWindow), typeof(NativeFloatingWindow),
            new FrameworkPropertyMetadata(null));
        internal static NativeFloatingWindow GetNativeFloatingWindow(FloatingWindow floatingWindow)
        {
            return (NativeFloatingWindow)floatingWindow.GetValue(NativeFloatingWindowProperty);
        }
        internal static void SetNativeFloatingWindow(FloatingWindow floatingWindow, NativeFloatingWindow value)
        {
            NativeFloatingWindow oldValue = GetNativeFloatingWindow(floatingWindow);
            if (oldValue != null)
            {
                oldValue.DataContext = null; // this should fix "Cannot set Visibility or call Show or ShowDialog after window has closed." exception
                oldValue.Close();
            }
            if (value == null)
                floatingWindow.ClearValue(NativeFloatingWindowProperty);
            else
                floatingWindow.SetValue(NativeFloatingWindowProperty, value);
        }

        /// <summary>Identifies the <b>DoubleClickCommand</b> dependency property.</summary>
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof(System.Windows.Input.ICommand), typeof(NativeFloatingWindow));

        static NativeFloatingWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NativeFloatingWindow), new FrameworkPropertyMetadata(typeof(NativeFloatingWindow)));
            ShowInTaskbarProperty.OverrideMetadata(typeof(NativeFloatingWindow), new FrameworkPropertyMetadata(BooleanBoxes.False));
            WindowStyleProperty.OverrideMetadata(typeof(NativeFloatingWindow), new FrameworkPropertyMetadata(WindowStyle.ToolWindow));
            ShowActivatedProperty.OverrideMetadata(typeof(NativeFloatingWindow), new FrameworkPropertyMetadata(BooleanBoxes.False));

            CommandBinding performCloseCommandBinding = new CommandBinding(DockCommands.PerformClose, new ExecutedRoutedEventHandler(OnPerformCloseExecuted));
            CommandBinding toggleFloatingCommandBinding = new CommandBinding(DockCommands.ToggleFloating, new ExecutedRoutedEventHandler(OnToggleFloatingExecuted));
            CommandBinding toggleWindowStateCommandBinding = new CommandBinding(DockCommands.ToggleWindowState, new ExecutedRoutedEventHandler(OnToggleWindowStateExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(NativeFloatingWindow), performCloseCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(NativeFloatingWindow), toggleFloatingCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(NativeFloatingWindow), toggleWindowStateCommandBinding);
        }

        static void OnToggleFloatingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((NativeFloatingWindow)sender).OnToggleFloatingExecuted();
        }

        void OnToggleFloatingExecuted()
        {
            DockManager.ToggleFloating(FloatingWindow);
        }

        static void OnPerformCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((NativeFloatingWindow)sender).PerformClose();
        }

        void PerformClose()
        {
            FloatingWindow.PerformClose();
        }

        static void OnToggleWindowStateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((NativeFloatingWindow)sender).OnToggleWindowStateExecuted();
        }

        void OnToggleWindowStateExecuted()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        internal NativeFloatingWindow(FloatingWindow floatingWindow)
        {
            // Call BeginInit/EndInit pair to make sure the default style applied.
            //BeginInit();

            Loaded += new RoutedEventHandler(OnLoaded);
            Unloaded += new RoutedEventHandler(OnUnloaded);

            Owner = Window.GetWindow(floatingWindow.DockControl);
            DataContext = floatingWindow;
            
            //EndInit();
        }

        /// <summary>Gets or sets the double click command being invoked. This is a dependency property.</summary>
        /// <value>The double click command being invoked. The default value is <see langword="null"/>.</value>
        public System.Windows.Input.ICommand DoubleClickCommand
        {
            get { return (System.Windows.Input.ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        HwndSource _hwndSource;
        HwndSourceHook _wndProcHandler;
        private const int WM_CLOSE = 0x0010;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int WM_NCLBUTTONUP = 0xA2;
        private const int WM_NCLBUTTONDBLCLK = 0xA3;
        private const int WM_NCRBUTTONDOWN = 0xA4;
        private const int HTCAPTION = 2;

        private void OnLoaded(object sender, EventArgs e)
        {
            FloatingWindow.Left = Left;
            FloatingWindow.Top = Top;
            WindowInteropHelper helper = new WindowInteropHelper(this);
            _hwndSource = HwndSource.FromHwnd(helper.Handle);
            _wndProcHandler = new HwndSourceHook(FilterMessage);
            _hwndSource.AddHook(_wndProcHandler);
        }

        private void OnUnloaded(object sender, EventArgs e)
        {
            if (_hwndSource != null)
                _hwndSource.RemoveHook(_wndProcHandler);
        }

        private IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CLOSE:
                    if (FloatingWindow != null && FloatingWindow.DockControl != null)
                    {
                        PerformClose();
                        handled = true;
                    }
                    break;
                case WM_NCLBUTTONDOWN:
                    if (wParam.ToInt32() == HTCAPTION && WindowState != WindowState.Maximized)
                    {
                        short x = (short)((lParam.ToInt32() & 0xFFFF));
                        short y = (short)((lParam.ToInt32() >> 16));

                        Point pt = new Point(x, y);
                        DockManager.BeginDrag(this, this, pt);
                    }
                    break;
                case WM_NCLBUTTONDBLCLK:
                    if (wParam.ToInt32() == HTCAPTION)
                        handled = DockCommands.Execute(this, DoubleClickCommand);
                    break;
                case WM_NCRBUTTONDOWN:
                    if (wParam.ToInt32() == HTCAPTION && FloatingWindow.CountOfVisiblePanes == 1)
                    {
                        short x = (short)((lParam.ToInt32() & 0xFFFF));
                        short y = (short)((lParam.ToInt32() >> 16));

                        ContextMenu menu = FloatingWindow.FirstVisiblePane.SelectedItem.TabContextMenu;
                        if (menu != null)
                        {
                            menu.Placement = PlacementMode.AbsolutePoint;
                            menu.PlacementRectangle = new Rect(new Point(x, y), new Size(0, 0));
                            menu.PlacementTarget = this;
                            menu.IsOpen = true;
                        }
                    }
                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }

        /// <exclude />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
                return;

            if (WindowControl.BeginDrag(this, this, e))
                return;

            if (!DockManager.CanDrag(e))
                return;

            if (e.ClickCount == 1 && WindowState != WindowState.Maximized)
            {
                DockManager.BeginDrag(this, this, e);
                e.Handled = true;
            }
            else if (e.ClickCount == 2)
                e.Handled = DockCommands.Execute(this, DoubleClickCommand);
        }

        private FloatingWindow FloatingWindow
        {
            get { return DataContext as FloatingWindow; }
        }

        private DockControl DockControl
        {
            get { return FloatingWindow == null ? null : FloatingWindow.DockControl; }
        }

        #region IDragSource
        Rect IDragSource.GetFloatingWindowPreview(RelativePoint startMousePosition)
        {
            return GetPreviewBounds(startMousePosition.Visual);
        }

        internal Rect GetPreviewBounds(Visual adornerWindow)
        {
            PresentationSource source = PresentationSource.FromVisual(adornerWindow);
            Point windowPos = source.CompositionTarget.TransformToDevice.Transform(new Point(Left, Top));
            return new Rect(adornerWindow.PointFromScreen(windowPos), new Size(ActualWidth, ActualHeight));
        }

        bool IDragSource.CanDrop(DockTreePosition dockTreePosition)
        {
            return DockManager.CanDrop(FloatingWindow, dockTreePosition);
        }

        bool IDragSource.CanDrop(DockPane targetPane)
        {
            return DockManager.CanDrop(FloatingWindow, targetPane);
        }

        bool IDragSource.CanDrop(DockItem targetItem)
        {
            return DockManager.CanDrop(FloatingWindow, targetItem);
        }

        void IDragSource.Drop(Dock dock, bool sendToBack)
        {
            DockManager.Drop(FloatingWindow, dock, sendToBack);
        }

        void IDragSource.Drop()
        {
            DockManager.Drop(FloatingWindow);
        }

        void IDragSource.Drop(Rect floatingWindowBounds)
        {
            DockManager.Drop(FloatingWindow, floatingWindowBounds);
        }

        void IDragSource.Drop(DockPane targetPane, DockPanePreviewPlacement placement)
        {
            DockManager.Drop(FloatingWindow, targetPane, placement);
        }

        void IDragSource.Drop(DockItem targetItem)
        {
            DockManager.Drop(FloatingWindow, targetItem);
        }

        DockControl IDragSource.DockControl
        {
            get { return DockControl; }
        }
        #endregion

        Rect IWindow.Bounds
        {
            get { return new Rect(Left, Top, Width, Height); }
        }

        Rect IWindow.ActualBounds
        {
            get { return new Rect(Left, Top, Width, Height); }
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

        /// <exclude />
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == DataContextProperty)
                OnFloatingWindowChanged(e.OldValue as FloatingWindow, e.NewValue as FloatingWindow);
        }

        // Workaround Issue #1 https://github.com/DevZest/WpfDocking/issues/1
        private void OnFloatingWindowChanged(FloatingWindow oldValue, FloatingWindow newValue)
        {
            if (oldValue != null)
                DetachIsVisibleChangedHandler(oldValue);
            if (newValue != null)
            {
                RefreshVisibility();
                AttachIsVisibleChangedHandler(newValue);
            }
        }

        private void AttachIsVisibleChangedHandler(FloatingWindow floatingWindow)
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(FloatingWindow.IsVisibleProperty, floatingWindow.GetType());
            dpd.AddValueChanged(floatingWindow, OnIsVisibleChanged);
        }

        private void DetachIsVisibleChangedHandler(FloatingWindow floatingWindow)
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(FloatingWindow.IsVisibleProperty, floatingWindow.GetType());
            dpd.RemoveValueChanged(floatingWindow, OnIsVisibleChanged);
        }

        private void OnIsVisibleChanged(object sender, EventArgs e)
        {
            RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            Visibility = FloatingWindow.IsVisible ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
