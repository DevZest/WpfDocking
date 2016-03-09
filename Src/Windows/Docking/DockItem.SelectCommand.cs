using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class SelectCommand : CommandBase
        {
            public static void Execute(DockItem dockItem, bool activate)
            {
                DockControl dockControl = dockItem.DockControl;
                if (dockControl.CanEnterUndo && dockItem.IsHidden)
                    dockControl.ExecuteCommand(new SelectCommand(dockItem, activate));
                else
                    dockItem.DoSelect(activate);
            }

            private bool _activate;

            private SelectCommand(DockItem dockItem, bool activate)
                : base(dockItem)
            {
                Debug.Assert(dockItem.IsHidden);
                _activate = activate;
            }

            public override void Execute(DockControl dockControl)
            {
                GetDockItem(dockControl).DoSelect(_activate);
            }

            public override void UnExecute(DockControl dockControl)
            {
                GetDockItem(dockControl).DoHide();
            }
        }
    }
}
