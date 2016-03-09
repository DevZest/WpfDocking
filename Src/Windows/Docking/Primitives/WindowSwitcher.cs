using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Switches between <see cref="DockItem"/> windows.</summary>
    /// <remarks>The switcher will be activated when the keyboard combination, that the <see cref="KeyGesture"/> value set to
    /// <see cref="DockControl"/> object's <b>Hotkey</b> attached property, is pressed.</remarks>
    public sealed class WindowSwitcher : Control
    {
        private static readonly DependencyPropertyKey DockControlPropertyKey;

        /// <summary>Identifies the <see cref="DockControl"/> dependency property.</summary>
        public static readonly DependencyProperty DockControlProperty;

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.WindowSwitcher.Hotkey"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets a value indicates the key combination that will activate the window switcher for a given <see cref="DockControl"/>.</summary>
        /// <value>The key combination. The default value is <see langword="null"/>.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty HotkeyProperty;

        private static WindowSwitcher s_current = null;

        static WindowSwitcher()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowSwitcher), new FrameworkPropertyMetadata(typeof(WindowSwitcher)));
            DockControlPropertyKey = DependencyProperty.RegisterReadOnly("DockControl", typeof(DockControl), typeof(WindowSwitcher),
                new FrameworkPropertyMetadata(null));
            DockControlProperty = DockControlPropertyKey.DependencyProperty;
            HotkeyProperty = DependencyProperty.RegisterAttached("Hotkey", typeof(KeyGesture), typeof(WindowSwitcher),
                new FrameworkPropertyMetadata(null));
            EventManager.RegisterClassHandler(typeof(DockItem), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown));
            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.KeyUpEvent, new KeyEventHandler(OnKeyUp));
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.WindowSwitcher.Hotkey" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.WindowSwitcher.Hotkey" /> attached property.</returns>
        public static KeyGesture GetHotkey(DockControl dockControl)
        {
            return (KeyGesture)dockControl.GetValue(HotkeyProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.Docking.Primitives..WindowSwitcher.Hotkey" /> attached property
        /// for a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetHotkey(DockControl dockControl, KeyGesture value)
        {
            dockControl.SetValue(HotkeyProperty, value);
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (s_current != null)
                return;

            DockItem dockItem = (DockItem)sender;
            DockControl dockControl = dockItem.DockControl;
            if (dockControl == null)
                return;

            KeyGesture hotkey = GetHotkey(dockControl);
            if (hotkey.Modifiers == ModifierKeys.None)
                return;

            if (hotkey.Key == e.Key && (hotkey.Modifiers & Keyboard.Modifiers) == hotkey.Modifiers)
            {
                e.Handled = true;
                s_current = new WindowSwitcher(dockControl);
                s_current.Show();
            }
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (s_current == null || e.Handled)
                return;

            KeyGesture hotkey = GetHotkey(s_current.DockControl);
            if ((Keyboard.Modifiers & hotkey.Modifiers) == ModifierKeys.None)
            {
                Close();
                e.Handled = true;
            }
        }

        private FloatingWindowAdorner _floatingWindowAdorner;

        private WindowSwitcher(DockControl dockControl)
        {
            if (dockControl == null)
                throw new ArgumentNullException("dockControl");

            DockControl = dockControl;
        }

        /// <summary>Gets the <see cref="Docking.DockControl"/> object associated with this <see cref="WindowSwitcher"/>.
        /// This is a dependency property.</summary>
        /// <value>The <see cref="Docking.DockControl"/> object.</value>
        public DockControl DockControl
        {
            get { return (DockControl)GetValue(DockControlProperty); }
            private set { SetValue(DockControlPropertyKey, value); }
        }

        /// <summary>Gets a collection of tool window <see cref="DockItem"/> objects, in the order of activation.</summary>
        /// <value>A collection of tool window <see cref="DockItem"/> objects, in the order of activation (last active first).</value>
        public IEnumerable<DockItem> ActiveToolWindows
        {
            get { return GetActiveToolWindows(DockControl); }
        }

        private static IEnumerable<DockItem> GetActiveToolWindows(DockControl dockControl)
        {
            DockPaneCollection panes = dockControl.Panes;
            for (int i = panes.Count - 1; i >= 0; i--)
            {
                DockPane pane = panes[i];
                DockPosition dockPosition = pane.DockPosition;
                Debug.Assert(dockPosition != DockPosition.Unknown);
                if (dockPosition == DockPosition.Hidden || dockPosition == DockPosition.Document)
                    continue;

                DockItemCollection activeItems = pane.ActiveItems;
                for (int j = activeItems.Count - 1; j >= 0; j--)
                    yield return activeItems[j];
            }
        }

        /// <summary>Gets a collection of document <see cref="DockItem"/> objects, in the order of activation.</summary>
        /// <value>A collection of document <see cref="DockItem"/> objects, in the order of activation (last active first).</value>
        public IEnumerable<DockItem> ActiveDocuments
        {
            get { return GetActiveDocuments(DockControl); }
        }

        private static IEnumerable<DockItem> GetActiveDocuments(DockControl dockControl)
        {
            DockPaneCollection panes = dockControl.Panes;
            for (int i = panes.Count - 1; i >= 0; i--)
            {
                DockPane pane = panes[i];
                DockPosition dockPosition = pane.DockPosition;
                if (dockPosition != DockPosition.Document)
                    continue;

                DockItemCollection activeItems = pane.ActiveItems;
                for (int j = activeItems.Count - 1; j >= 0; j--)
                    yield return activeItems[j];
            }
        }

        private void Show()
        {
            if (DockManager.GetFloatingWindowStrategy(DockControl) == FloatingWindowStrategy.Native)
            {
                Window window = new Window();
                window.WindowStyle = WindowStyle.None;
                window.ResizeMode = ResizeMode.NoResize;
                window.ShowInTaskbar = false;
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.Owner = Window.GetWindow(DockControl);
                window.AllowsTransparency = true;
                window.Background = null;
                window.Focusable = false;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Content = this;
                window.Show();
            }
            else
            {
                Grid grid = new Grid();
                VerticalAlignment = VerticalAlignment.Center;
                HorizontalAlignment = HorizontalAlignment.Center;
                grid.Children.Add(this);
                _floatingWindowAdorner = FloatingWindowAdorner.Add(DockControl, grid);
                UpdateLayout();
            }
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private static void Close()
        {
            if (s_current == null)
                return;

            WindowSwitcher current = s_current;
            s_current = null;
            current.DoClose();
        }

        private void DoClose()
        {
            if (IsKeyboardFocusWithin)
            {
                DockItem dockItemToActivate = DockControl.ActiveItem;
                FrameworkElement focusedElement = Keyboard.FocusedElement as FrameworkElement;
                if (focusedElement != null)
                {
                    DockItem item = focusedElement.DataContext as DockItem;
                    if (item != null)
                        dockItemToActivate = item;
                }
                if (dockItemToActivate != null)
                    dockItemToActivate.Activate();
            }

            if (_floatingWindowAdorner != null)
                FloatingWindowAdorner.Remove(DockControl, _floatingWindowAdorner);
            else
                Window.GetWindow(this).Close();
        }

        /// <exclude/>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            bool newValue = (bool)e.NewValue;
            if (!newValue)
                Close();
        }
    }
}
