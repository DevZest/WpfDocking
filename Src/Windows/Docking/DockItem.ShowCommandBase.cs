using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private abstract class ShowCommandBase : ShowCloseCommandBase
        {
            protected static bool TestUndo(DockItem dockItem, DockControl dockControl)
            {
                if (!dockControl.CanEnterUndo)
                    return false;

                if (dockItem.FirstPane != null)
                    return true;

                if (dockItem.UndoRedoReference == null)
                {
                    dockControl.ClearUndo();
                    return false;
                }

                return true;
            }

            private DockItemShowMethod _showMethod;

            protected ShowCommandBase(DockItem dockItem, DockItemShowMethod showMethod)
                : base(dockItem)
            {
                _showMethod = showMethod;
            }

            protected sealed override DockItem GetDockItem(DockControl dockControl)
            {
                if (DockItemIndex == -1)
                    return UndoRedoReference.DockItem;
                else
                    return base.GetDockItem(dockControl);
            }

            protected DockItemShowMethod ShowMethod
            {
                get { return _showMethod; }
            }

            public sealed override void Execute(DockControl dockControl)
            {
                if (DockItemIndex != -1)
                    GetDockItem(dockControl).EnsureAttached(dockControl, DockItemIndex);
                ExecuteOverride(dockControl);
            }

            protected abstract void ExecuteOverride(DockControl dockControl);

            public sealed override void UnExecute(DockControl dockControl)
            {
                DockItem item = GetDockItem(dockControl);
                item.DoClose();
                if (DockItemIndex != -1)
                    item.EnsureAttached(dockControl, DockItemIndex);

                if (SecondPaneShowAction != null)
                    SecondPaneShowAction.Run(dockControl);

                if (FirstPaneShowAction != null)
                    FirstPaneShowAction.Run(dockControl);
            }
        }
    }
}
