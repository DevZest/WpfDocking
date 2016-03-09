using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class FloatingWindow
    {
        private sealed class UpdateBoundsCommand : IValueChangedCommand<UpdateBoundsData>
        {
            private int _dockItemIndex;
            private Rect _oldValue;
            private Rect _newValue;

            public UpdateBoundsCommand(FloatingWindow floatingWindow, Rect oldValue, Rect newValue)
            {
                Initialize(floatingWindow, oldValue, newValue);
            }

            private void Initialize(FloatingWindow floatingWindow, Rect oldValue, Rect newValue)
            {
                DockControl dockControl = floatingWindow.DockControl;
                DockItem dockItem = floatingWindow.Panes[0].Items[0];
                _dockItemIndex = dockControl.DockItems.IndexOf(dockItem);
                _oldValue = oldValue;
                _newValue = newValue;
            }

            private FloatingWindow GetFloatingWindow(DockControl dockControl)
            {
                return GetDockItem(dockControl).GetPane(true).FloatingWindow;
            }

            private DockItem GetDockItem(DockControl dockControl)
            {
                return dockControl.DockItems[_dockItemIndex];
            }

            public bool Reset(DockControl dockControl, UpdateBoundsData oldValue, UpdateBoundsData newValue)
            {
                Debug.Assert(oldValue.FloatingWindow == newValue.FloatingWindow);
                Debug.Assert(dockControl == oldValue.FloatingWindow.DockControl);
                Initialize(oldValue.FloatingWindow, oldValue.Value, newValue.Value);
                return true;
            }


            public bool ShouldRemove(DockControl dockControl, UpdateBoundsData value)
            {
                return value.FloatingWindow == GetFloatingWindow(dockControl) && value.Value == _oldValue;
            }

            public bool Merge(DockControl dockControl, UpdateBoundsData value)
            {
                if (value.FloatingWindow != GetFloatingWindow(dockControl))
                    return false;

                _newValue = value.Value;
                return true;
            }

            public void Execute(DockControl dockControl)
            {
                SetBounds(GetFloatingWindow(dockControl), _newValue);
            }

            public void UnExecute(DockControl dockControl)
            {
                SetBounds(GetFloatingWindow(dockControl), _oldValue);
            }

            private static void SetBounds(FloatingWindow floatingWindow, Rect bounds)
            {
                floatingWindow.Left = bounds.Left;
                floatingWindow.Top = bounds.Top;
                floatingWindow.Width = bounds.Width;
                floatingWindow.Height = bounds.Height;
            }
        }
    }
}
