using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private class ShowAsDockPositionCommand : ShowCommandBase
        {
            public static void Execute(DockItem dockItem, DockControl dockControl, DockPosition dockPosition, DockItemShowMethod showMethod)
            {
                Debug.Assert(dockControl != null);
                if (TestUndo(dockItem, dockControl))
                    dockControl.ExecuteCommand(new ShowAsDockPositionCommand(dockItem, dockPosition, showMethod));
                else
                    dockItem.DoShowAsDockPosition(dockControl, dockPosition, showMethod);
            }

            private DockPosition _dockPosition;

            private ShowAsDockPositionCommand(DockItem dockItem, DockPosition dockPosition, DockItemShowMethod showMethod)
                : base(dockItem, showMethod)
            {
                _dockPosition = dockPosition;
            }

            protected override void ExecuteOverride(DockControl dockControl)
            {
                GetDockItem(dockControl).DoShowAsDockPosition(dockControl, _dockPosition, ShowMethod);
            }
        }
    }
}
