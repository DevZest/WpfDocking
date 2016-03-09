using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockControl
    {
        private sealed class UpdateDockTreeZOrderCommand : IValueChangedCommand<DockTreeZOrder>
        {
            private DockTreeZOrder _oldValue;
            private DockTreeZOrder _newValue;

            public UpdateDockTreeZOrderCommand(DockTreeZOrder oldValue, DockTreeZOrder newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public bool Reset(DockControl dockControl, DockTreeZOrder oldValue, DockTreeZOrder newValue)
            {
                return false;
            }

            public bool ShouldRemove(DockControl dockControl, DockTreeZOrder value)
            {
                return false;
            }

            public bool Merge(DockControl dockControl, DockTreeZOrder value)
            {
                return false;
            }

            public void Execute(DockControl dockControl)
            {
                dockControl.DockTreeZOrder = _newValue;
            }

            public void UnExecute(DockControl dockControl)
            {
                dockControl.DockTreeZOrder = _oldValue;
            }
        }
    }
}
