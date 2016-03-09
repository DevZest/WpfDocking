using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the visual presentation of <see cref="Docking.DockPane"/> object.</summary>
    /// <remarks>Set the <see cref="Docking.DockPane"/> object instance to <see cref="ContentControl.Content"/> property.</remarks>
    public abstract class DockWindow : Control, IDragSource
    {
        internal static readonly DependencyProperty IsTabProperty;

        /// <summary>Identifies the <b>DoubleClickCommand</b> dependency property.</summary>
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof(System.Windows.Input.ICommand), typeof(DockWindow));

        static DockWindow()
        {
            IsHitTestVisibleProperty.OverrideMetadata(typeof(DockWindow), new FrameworkPropertyMetadata(true));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(DockWindow), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(DockWindow), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(DockWindow), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            FocusableProperty.OverrideMetadata(typeof(DockWindow), new FrameworkPropertyMetadata(BooleanBoxes.False));
            IsTabProperty = DependencyProperty.RegisterAttached("IsTab", typeof(bool), typeof(DockWindow),
                new FrameworkPropertyMetadata(BooleanBoxes.False));

            CommandBinding performCloseCommandBinding = new CommandBinding(DockCommands.PerformClose, new ExecutedRoutedEventHandler(OnPerformCloseExecuted));
            CommandBinding toggleFloatingCommandBinding = new CommandBinding(DockCommands.ToggleFloating,
                new ExecutedRoutedEventHandler(OnToggleFloatingExecuted), new CanExecuteRoutedEventHandler(CanExecuteToggleFloating));
            CommandManager.RegisterClassCommandBinding(typeof(DockWindow), performCloseCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DockWindow), toggleFloatingCommandBinding);
        }

        private static void OnPerformCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((DockWindow)sender).PerformClose();
        }

        private static void OnToggleFloatingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DockManager.ToggleFloating(((DockWindow)sender).DockPane);
        }

        private static void CanExecuteToggleFloating(object sender, CanExecuteRoutedEventArgs e)
        {
            ((DockWindow)sender).CanExecuteToggleFloating(e);
        }

        void CanExecuteToggleFloating(CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            foreach (DockItem dockItem in DockPane.ActiveItems)
            {
                if (dockItem.CanToggleFloating)
                {
                    e.CanExecute = true;
                    break;
                }
            }
            e.Handled = true;
        }

        private void PerformClose()
        {
            DockControl dockControl = DockPane.DockControl;
            dockControl.BeginUndoUnit();
            var visibleItems = DockPane.VisibleItems;
            DockItem[] dockItems = new DockItem[visibleItems.Count];
            visibleItems.CopyTo(dockItems, 0);
            foreach (DockItem dockItem in dockItems)
                dockItem.PerformClose();
            dockControl.EndUndoUnit();
        }

        private static bool GetIsTab(DependencyObject element)
        {
            return (bool)element.GetValue(IsTabProperty);
        }

        /// <summary>Gets or sets the double click command being invoked. This is a dependency property.</summary>
        /// <value>The double click command being invoked. The default value is <see langword="null"/>.</value>
        public System.Windows.Input.ICommand DoubleClickCommand
        {
            get { return (System.Windows.Input.ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        private DockPane DockPane
        {
            get { return DataContext as DockPane; }
        }

        private bool IsOnlyVisibleFloatingPane
        {
            get { return DockPane.DockPosition == DockPosition.Floating && DockPane.FloatingWindow.CountOfVisiblePanes == 1 && DockPane.FloatingWindow.FirstVisiblePane == DockPane; }
        }

        private DockItem SelectedItem
        {
            get { return DockPane == null ? null : DockPane.SelectedItem; }
        }

        private DockControl DockControl
        {
            get { return DockPane == null ? null : DockPane.DockControl; }
        }

        /// <exclude />
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            DockItem selectedItem = SelectedItem;
            if (selectedItem != null && !HitTestTab(e.GetPosition(this)))
                selectedItem.Activate();
        }

        private bool HitTestTab(Point pt)
        {
            bool flag = false;
            VisualTreeHelper.HitTest(this,
                (DependencyObject target) =>
                {
                    if (GetIsTab(target))
                    {
                        flag = true;
                        return HitTestFilterBehavior.Stop;
                    }
                    return HitTestFilterBehavior.Continue;
                },
                (HitTestResult result) => HitTestResultBehavior.Stop,
                new PointHitTestParameters(pt));

            return flag;
        }

        /// <exclude />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled || !DockManager.CanDrag(e) || IsOnlyVisibleFloatingPane)
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
            return DockManager.CanDrop(DockPane, dockTreePosition);
        }

        bool IDragSource.CanDrop(DockPane targetPane)
        {
            return DockManager.CanDrop(DockPane, targetPane);
        }

        bool IDragSource.CanDrop(DockItem targetItem)
        {
            return DockManager.CanDrop(DockPane, targetItem);
        }

        void IDragSource.Drop()
        {
            DockManager.Drop(DockPane);
        }

        void IDragSource.Drop(Dock dock, bool sendToBack)
        {
            DockManager.Drop(DockPane, dock, sendToBack);
        }

        void IDragSource.Drop(Rect floatingWindowBounds)
        {
            DockManager.Drop(DockPane, floatingWindowBounds);
        }

        void IDragSource.Drop(DockPane targetPane, DockPanePreviewPlacement placement)
        {
            DockManager.Drop(DockPane, targetPane, placement);
        }

        void IDragSource.Drop(DockItem targetItem)
        {
            DockManager.Drop(DockPane, targetItem);
        }

        DockControl IDragSource.DockControl
        {
            get { return DockControl; }
        }
    }
}
