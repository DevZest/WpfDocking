using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private class DeselectCommand : CommandBase
        {
            public static void Execute(DockItem dockItem)
            {
                DockControl dockControl = dockItem.DockControl;
                if (dockControl.CanEnterUndo && dockItem.IsHidden)
                    dockControl.ExecuteCommand(new DeselectCommand(dockItem));
                else
                    dockItem.DoDeselect();
            }

            private DeselectCommand(DockItem dockItem)
                : base(dockItem)
            {
                Debug.Assert(dockItem.DockPosition != DockPosition.Hidden);
                Debug.Assert(dockItem.FirstPane != null);
            }

            public override void Execute(DockControl dockControl)
            {
                GetDockItem(dockControl).DoDeselect();
            }

            public override void UnExecute(DockControl dockControl)
            {
                GetDockItem(dockControl).DoHide();
            }
        }
    }
}
