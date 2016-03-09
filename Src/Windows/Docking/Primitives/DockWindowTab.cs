using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the tab in the <see cref="DockWindow"/>.</summary>
    public abstract class DockWindowTab : ContentControl, IDragSource
    {
        private static readonly DependencyPropertyKey IsContentTrimmedPropertyKey;

        /// <summary>Identifies the <see cref="IsContentTrimmed"/> dependency property.</summary>
        public static readonly DependencyProperty IsContentTrimmedProperty;

        /// <summary>Identifies the <b>DoubleClickCommand</b> dependency property.</summary>
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof(System.Windows.Input.ICommand), typeof(DockWindowTab));

        static DockWindowTab()
        {
            FocusableProperty.OverrideMetadata(typeof(DockWindowTab), new FrameworkPropertyMetadata(BooleanBoxes.False));
            DockWindow.IsTabProperty.OverrideMetadata(typeof(DockWindowTab), new FrameworkPropertyMetadata(BooleanBoxes.True));
            IsContentTrimmedPropertyKey = DependencyProperty.RegisterReadOnly("IsContentTrimmed", typeof(bool), typeof(DockWindowTab),
                new FrameworkPropertyMetadata(BooleanBoxes.False));
            IsContentTrimmedProperty = IsContentTrimmedPropertyKey.DependencyProperty;

            CommandBinding performCloseCommandBinding = new CommandBinding(DockCommands.PerformClose, new ExecutedRoutedEventHandler(OnPerformCloseExecuted));
            CommandBinding toggleFloatingCommandBinding = new CommandBinding(DockCommands.ToggleFloating,
                new ExecutedRoutedEventHandler(OnToggleFloatingExecuted), new CanExecuteRoutedEventHandler(CanExecuteToggleFloating));
            CommandBinding makeFloatingCommandBinding = new CommandBinding(DockCommands.MakeFloating,
                new ExecutedRoutedEventHandler(OnMakeFloatingExecuted), new CanExecuteRoutedEventHandler(CanExecuteMakeFloating));
            CommandManager.RegisterClassCommandBinding(typeof(DockWindowTab), performCloseCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DockWindowTab), toggleFloatingCommandBinding);
            CommandManager.RegisterClassCommandBinding(typeof(DockWindowTab), makeFloatingCommandBinding);

        }

        private static void OnPerformCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((DockWindowTab)sender).DockItem.PerformClose();
        }

        private static void OnToggleFloatingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DockManager.ToggleFloating(((DockWindowTab)sender).DockItem);
        }

        private static void CanExecuteToggleFloating(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((DockWindowTab)sender).DockItem.CanToggleFloating;
            e.Handled = true;
        }

        private static void OnMakeFloatingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DockItem dockItem = ((DockWindowTab)sender).DockItem;
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
            DockItem dockItem = ((DockWindowTab)sender).DockItem;
            e.CanExecute = (dockItem.AllowedDockTreePositions & AllowedDockTreePositions.Floating) == AllowedDockTreePositions.Floating;
            e.Handled = true;
        }

        /// <summary>Gets a value indicates whether the displaying content is trimmed. This is a dependency property.</summary>
        /// <value><see langword="true"/> if the displaying content is trimmed, otherwise <see langword="false"/>.</value>
        public bool IsContentTrimmed
        {
            get { return (bool)GetValue(IsContentTrimmedProperty); }
            internal set
            {
                if (value)
                    SetValue(IsContentTrimmedPropertyKey, BooleanBoxes.Box(value));
                else
                    ClearValue(IsContentTrimmedPropertyKey);
            }
        }

        /// <summary>Gets or sets the double click command being invoked. This is a dependency property.</summary>
        /// <value>The double click command being invoked. The default value is <see langword="null"/>.</value>
        public System.Windows.Input.ICommand DoubleClickCommand
        {
            get { return (System.Windows.Input.ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        internal DockItem DockItem
        {
            get { return Content as DockItem; }
        }

        private DockControl DockControl
        {
            get { return DockItem == null ? null : DockItem.DockControl; }
        }

        /// <exclude />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled)
                return;

            if (e.ClickCount == 1)
            {
                DockItem dockItem = DockItem;
                if (dockItem != null)
                {
                    dockItem.Activate();
                    UpdateLayout(); // Fix in 2.0.4846: this can avoid unnecessary mouse move event caused by layout change
                }
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
            get { return DockControl; }
        }
    }
}
