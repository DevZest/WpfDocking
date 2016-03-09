using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the window to display the selected auto-hide <see cref="DockItem"/>.</summary>
    /// <remarks><see cref="AutoHideWindow"/> is used in the control template of <see cref="AutoHideClient"/>.
    /// The <see cref="ContentControl.Content"/> property is bind to the <see cref="DevZest.Windows.Docking.DockControl.SelectedAutoHideItem"/> property of
    /// <see cref="DockControl"/>.</remarks>
    public class AutoHideWindow : ContentControl, IDragSource
    {
        /// <summary>Identifies the <b>DoubleClickCommand</b> dependency property.</summary>
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof(System.Windows.Input.ICommand), typeof(AutoHideWindow));

        static AutoHideWindow()
        {
            FocusableProperty.OverrideMetadata(typeof(AutoHideWindow), new FrameworkPropertyMetadata(BooleanBoxes.False));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideWindow), new FrameworkPropertyMetadata(typeof(AutoHideWindow)));

            CommandBinding performCloseCommandBinding = new CommandBinding(DockCommands.PerformClose, new ExecutedRoutedEventHandler(OnPerformCloseExecuted));
            CommandBinding toggleFloatingCommandBinding = new CommandBinding(DockCommands.ToggleFloating,
                new ExecutedRoutedEventHandler(OnToggleFloatingExecuted), new CanExecuteRoutedEventHandler(CanExecuteToggleFloating));
            CommandBinding makeFloatingCommandBinding = new CommandBinding(DockCommands.MakeFloating,
                new ExecutedRoutedEventHandler(OnMakeFloatingExecuted), new CanExecuteRoutedEventHandler(CanExecuteMakeFloating));
            CommandManager.RegisterClassCommandBinding(typeof(AutoHideWindow), performCloseCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(AutoHideWindow), toggleFloatingCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(AutoHideWindow), makeFloatingCommandBinding);
        }

        private static void OnPerformCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((AutoHideWindow)sender).DockItem.PerformClose();
        }

        private static void OnToggleFloatingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DockManager.ToggleFloating(((AutoHideWindow)sender).DockItem);
        }

        private static void CanExecuteToggleFloating(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((AutoHideWindow)sender).DockItem.CanToggleFloating;
            e.Handled = true;
        }

        private static void OnMakeFloatingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DockItem dockItem = ((AutoHideWindow)sender).DockItem;
            DockControl dockControl = dockItem.DockControl;
            FloatingWindow floatingWindow = GetFloatingWindow(dockItem);
            Rect floatingWindowBounds;
            if (floatingWindow != null)
                floatingWindowBounds = new Rect(floatingWindow.Left, floatingWindow.Top, floatingWindow.Width, floatingWindow.Height);
            else
                floatingWindowBounds = new Rect(new Point(double.NaN, double.NaN), dockControl.DefaultFloatingWindowSize);
            dockItem.Show(dockItem.DockControl, floatingWindowBounds);
        }

        static FloatingWindow GetFloatingWindow(DockItem dockItem)
        {
            if (dockItem.FirstPane != null && dockItem.FirstPane.FloatingWindow != null)
                return dockItem.FirstPane.FloatingWindow;
            else if (dockItem.SecondPane != null && dockItem.SecondPane.FloatingWindow != null)
                return dockItem.SecondPane.FloatingWindow;
            return null;
        }

        private static void CanExecuteMakeFloating(object sender, CanExecuteRoutedEventArgs e)
        {
            DockItem dockItem = ((AutoHideWindow)sender).DockItem;
            e.CanExecute = (dockItem.AllowedDockTreePositions & AllowedDockTreePositions.Floating) == AllowedDockTreePositions.Floating;
            e.Handled = true;
        }


        /// <summary>Gets or sets the double click command being invoked. This is a dependency property.</summary>
        /// <value>The double click command being invoked. The default value is <see langword="null"/>.</value>
        public System.Windows.Input.ICommand DoubleClickCommand
        {
            get { return (System.Windows.Input.ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        private DockItem DockItem
        {
            get { return Content as DockItem; }
        }

        private DockControl DockControl
        {
            get { return DockItem == null ? null : DockItem.DockControl; }
        }

        /// <exclude/>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (DockItem != null)
                DockItem.Show(DockItemShowMethod.Activate);
        }

        /// <exclude />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled || !DockManager.CanDrag(e))
                return;

            if (e.ClickCount == 1)
            {
                DockManager.BeginDrag(this, this, e);
                e.Handled = true;
            }
            else if (e.ClickCount == 2)
                e.Handled = DockCommands.Execute(this, DoubleClickCommand);
        }

        Rect IDragSource.GetFloatingWindowPreview(RelativePoint mouseStartPosition)
        {
            return DockManager.GetDefaultFloatingPreview(DockControl, mouseStartPosition.Point);
        }

        bool IDragSource.CanDrop(DockTreePosition dockTreePosition)
        {
            return DockManager.CanDrop(DockItem, dockTreePosition);
        }

        bool IDragSource.CanDrop(DockPane targetPane)
        {
            return DockManager.CanDrop(DockItem, targetPane);
        }

        bool IDragSource.CanDrop(DockItem targetItem)
        {
            return DockManager.CanDrop(DockItem, targetItem);
        }

        void IDragSource.Drop()
        {
            DockManager.Drop(DockItem);
        }

        void IDragSource.Drop(Dock dock, bool sendToBack)
        {
            DockManager.Drop(DockItem, dock, sendToBack);
        }

        void IDragSource.Drop(Rect floatingWindowBounds)
        {
            DockManager.Drop(DockItem, floatingWindowBounds);
        }

        void IDragSource.Drop(DockPane targetPane, DockPanePreviewPlacement placement)
        {
            DockManager.Drop(DockItem, targetPane, placement);
        }

        void IDragSource.Drop(DockItem targetItem)
        {
            DockManager.Drop(DockItem, targetItem);
        }

        DockControl IDragSource.DockControl
        {
            get { return DockItem.DockControl; }
        }
    }
}
