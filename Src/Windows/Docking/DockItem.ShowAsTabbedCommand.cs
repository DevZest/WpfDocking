using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private class ShowAsTabbedCommand : ShowCommandBase
        {
            public static void Execute(DockItem dockItem, DockPane pane, int index, DockItemShowMethod showMethod)
            {
                DockControl dockControl = pane.DockControl;
                Debug.Assert(dockControl != null);
                if (TestUndo(dockItem, dockControl))
                    dockControl.ExecuteCommand(new ShowAsTabbedCommand(dockItem, pane, index, showMethod));
                else
                    dockItem.DoShowAsTabbed(pane, index, showMethod);
            }

            private DockPaneNodeStruct _paneNodeStruct;
            private int _index;

            private ShowAsTabbedCommand(DockItem dockItem, DockPane pane, int index, DockItemShowMethod showMethod)
                : base(dockItem, showMethod)
            {
                TargetPane = pane;
                _index = index;
            }

            private DockPane GetTargetPane(DockControl dockControl)
            {
                return _paneNodeStruct.ToDockPaneNode(dockControl) as DockPane;
            }

            private DockPane TargetPane
            {
                set { _paneNodeStruct = DockPaneNodeStruct.FromDockPaneNode(value); }
            }

            protected override void ExecuteOverride(DockControl dockControl)
            {
                GetDockItem(dockControl).DoShowAsTabbed(GetTargetPane(dockControl), _index, ShowMethod);
            }
        }
    }
}
