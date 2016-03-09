using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class ToggleAutoHideCommand : CommandBase
        {
            public static void Execute(DockItem dockItem, DockItemShowMethod showMethod)
            {
                DockControl dockControl = dockItem.DockControl;
                if (dockControl.CanEnterUndo)
                    dockControl.ExecuteCommand(new ToggleAutoHideCommand(dockItem, showMethod));
                else
                    dockItem.DoToggleAutoHide(showMethod);
            }

            private DockItemShowMethod _showMethod;
            private DockItemShowMethod _undoShowMethod;

            private ToggleAutoHideCommand(DockItem dockItem, DockItemShowMethod showMethod)
                : base(dockItem)
            {
                _showMethod = showMethod;
                _undoShowMethod = GetShowMethod(dockItem, dockItem.FirstPane);
            }

            public override void Execute(DockControl dockControl)
            {
                GetDockItem(dockControl).DoToggleAutoHide(_showMethod);
            }

            public override void UnExecute(DockControl dockControl)
            {
                GetDockItem(dockControl).DoToggleAutoHide(_undoShowMethod);
            }
        }
    }
}
