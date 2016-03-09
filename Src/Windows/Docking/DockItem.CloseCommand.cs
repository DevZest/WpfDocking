using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class CloseCommand : ShowCloseCommandBase
        {
            public static void Execute(DockItem dockItem)
            {
                DockControl dockControl = dockItem.DockControl;
                
                if (TestUndo(dockItem))
                    dockControl.ExecuteCommand(new CloseCommand(dockItem));
                else
                    dockItem.DoClose();
            }

            private static bool TestUndo(DockItem dockItem)
            {
                DockControl dockControl = dockItem.DockControl;

                if (!dockControl.CanEnterUndo)
                    return false;

                if (dockItem.FirstPane == null)
                    return false;

                if (dockItem.UndoRedoReference == null)
                {
                    dockControl.ClearUndo();
                    return false;
                }

                return true;
            }

            private CloseCommand(DockItem dockItem)
                : base(dockItem)
            {
                Debug.Assert(UndoRedoReference != null);
                Debug.Assert(FirstPaneShowAction != null);
            }

            public override void Execute(DockControl dockControl)
            {
                GetDockItem(dockControl).DoClose();
            }

            public override void UnExecute(DockControl dockControl)
            {
                DockItem item = UndoRedoReference.DockItem;
                item.EnsureAttached(dockControl, DockItemIndex);
                if (SecondPaneShowAction != null)
                    SecondPaneShowAction.Run(dockControl);
                FirstPaneShowAction.Run(dockControl);
            }
        }
    }
}
