using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private class HideCommand : CommandBase
        {
            public static void Execute(DockItem dockItem)
            {
                DockControl dockControl = dockItem.DockControl;
                if (dockControl.CanEnterUndo)
                    dockControl.ExecuteCommand(new HideCommand(dockItem));
                else
                    dockItem.DoHide();
            }

            private DockItemShowMethod _showMethod;

            private HideCommand(DockItem dockItem)
                : base(dockItem)
            {
                Debug.Assert(dockItem.DockPosition != DockPosition.Hidden);
                Debug.Assert(dockItem.FirstPane != null);
                _showMethod = GetShowMethod(dockItem, dockItem.FirstPane);
            }

            public override void Execute(DockControl dockControl)
            {
                GetDockItem(dockControl).Hide();
            }

            public override void UnExecute(DockControl dockControl)
            {
                GetDockItem(dockControl).Show(_showMethod);
            }
        }
    }
}
