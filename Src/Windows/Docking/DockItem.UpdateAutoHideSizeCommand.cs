using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class UpdateAutoHideSizeCommand : IValueChangedCommand<UpdateAutoHideSizeData>
        {
            private int _dockItemIndex;
            private SplitterDistance _oldValue;
            private SplitterDistance _newValue;

            public UpdateAutoHideSizeCommand(DockItem dockItem, SplitterDistance oldValue, SplitterDistance newValue)
            {
                Initialize(dockItem, oldValue, newValue);
            }

            private void Initialize(DockItem dockItem, SplitterDistance oldValue, SplitterDistance newValue)
            {
                _dockItemIndex = dockItem.DockControl.DockItems.IndexOf(dockItem);
                _oldValue = oldValue;
                _newValue = newValue;
            }

            private DockItem GetDockItem(DockControl dockControl)
            {
                return dockControl.DockItems[_dockItemIndex];
            }

            public bool Reset(DockControl dockControl, UpdateAutoHideSizeData oldValue, UpdateAutoHideSizeData newValue)
            {
                Initialize(oldValue.DockItem, oldValue.Value, newValue.Value);
                return true;
            }

            public bool ShouldRemove(DockControl dockControl, UpdateAutoHideSizeData value)
            {
                return value.DockItem == GetDockItem(dockControl) && _oldValue == value.Value;
            }

            public bool Merge(DockControl dockControl, UpdateAutoHideSizeData value)
            {
                if (GetDockItem(dockControl) != value.DockItem)
                    return false;

                _newValue = value.Value;
                return true;
            }

            public void Execute(DockControl dockControl)
            {
                SetAutoHideSize(GetDockItem(dockControl), _newValue);
            }

            public void UnExecute(DockControl dockControl)
            {
                SetAutoHideSize(GetDockItem(dockControl), _oldValue);
            }

            private static void SetAutoHideSize(DockItem dockItem, SplitterDistance value)
            {
                dockItem.AutoHideSize = value;
                if (DockPositionHelper.IsAutoHide(dockItem.DockPosition) && !dockItem.IsSelected)
                    dockItem.Show(DockItemShowMethod.Select);
            }
        }
    }
}
