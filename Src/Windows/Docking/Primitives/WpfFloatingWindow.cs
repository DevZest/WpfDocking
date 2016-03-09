using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the visual presentation of a <see cref="Docking.FloatingWindow"/> object in <see cref="WpfFloatingWindowClient"/>.</summary>
    /// <remarks><para><see cref="WpfFloatingWindow"/> is the item container of <see cref="WpfFloatingWindowClient"/>.
    /// Its <see cref="ContentControl.Content"/> property is set to a instance of <see cref="Docking.FloatingWindow"/> object.</para>
    /// </remarks>
    public class WpfFloatingWindow : WindowControl, IDragSource
    {
        /// <summary>Identifies the <b>DoubleClickCommand</b> dependency property.</summary>
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof(System.Windows.Input.ICommand), typeof(WpfFloatingWindow));

        static WpfFloatingWindow()
        {
            FocusableProperty.OverrideMetadata(typeof(WpfFloatingWindow), new FrameworkPropertyMetadata(BooleanBoxes.False));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpfFloatingWindow), new FrameworkPropertyMetadata(typeof(WpfFloatingWindow)));

            CommandBinding performCloseCommandBinding = new CommandBinding(DockCommands.PerformClose, new ExecutedRoutedEventHandler(OnPerformCloseExecuted));
            CommandBinding toggleFloatingCommandBinding = new CommandBinding(DockCommands.ToggleFloating, new ExecutedRoutedEventHandler(OnToggleFloatingExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(WpfFloatingWindow), performCloseCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(WpfFloatingWindow), toggleFloatingCommandBinding);
        }

        static void OnToggleFloatingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((WpfFloatingWindow)sender).OnToggleFloatingExecuted();
        }

        void OnToggleFloatingExecuted()
        {
            DockManager.ToggleFloating(FloatingWindow);
        }

        private static void OnPerformCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FloatingWindow floatingWindow = ((WpfFloatingWindow)sender).FloatingWindow;
            floatingWindow.PerformClose();
        }

        /// <summary>Gets or sets the double click command being invoked. This is a dependency property.</summary>
        /// <value>The double click command being invoked. The default value is <see langword="null"/>.</value>
        public System.Windows.Input.ICommand DoubleClickCommand
        {
            get { return (System.Windows.Input.ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        private FloatingWindow FloatingWindow
        {
            get { return Content as FloatingWindow; }
        }

        private DockControl DockControl
        {
            get { return FloatingWindow == null ? null : FloatingWindow.DockControl; }
        }

        /// <exclude />
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (FloatingWindow != null && FloatingWindow.ActivePanes.Count > 0)
            {
                DockPaneCollection activePanes = FloatingWindow.ActivePanes;
                DockItem itemToActivate = activePanes[activePanes.Count - 1].SelectedItem;
                itemToActivate.Activate();
            }
        }

        /// <exclude />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (FloatingWindow.CanBeNative || e.Handled || !DockManager.CanDrag(e))
                return;

            if (e.ClickCount == 1)
            {
                DockManager.BeginDrag(this, this, e);
                e.Handled = true;
            }
            else if (e.ClickCount == 2)
                e.Handled = DockCommands.Execute(this, DoubleClickCommand);
        }

        /// <exclude />
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            if (e.Handled || !DockManager.CanDrag(e))
                return;

            if (FloatingWindow.CountOfVisiblePanes == 1)
            {
                Point pt = e.GetPosition(this);
                ContextMenu menu = FloatingWindow.FirstVisiblePane.SelectedItem.TabContextMenu;
                if (menu != null)
                {
                    menu.Placement = PlacementMode.RelativePoint;
                    menu.PlacementRectangle = new Rect(pt, new Size(0, 0));
                    menu.PlacementTarget = this;
                    menu.IsOpen = true;
                }
                e.Handled = true;
            }
        }

        Rect IDragSource.GetFloatingWindowPreview(RelativePoint mousePosition)
        {
            return new Rect(ActualLeft, ActualTop, ActualWidth, ActualHeight);
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
    }
}
