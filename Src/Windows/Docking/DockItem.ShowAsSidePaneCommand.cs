using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private class ShowAsSidePaneCommand : ShowCommandBase
        {
            public static void Execute(DockItem dockItem, DockPaneNode paneNode, bool isAutoHide, Dock side, SplitterDistance size, bool isSizeForTarget, DockItemShowMethod showMethod)
            {
                DockControl dockControl = paneNode.DockControl;
                Debug.Assert(dockControl != null);
                if (TestUndo(dockItem, dockControl))
                    dockControl.ExecuteCommand(new ShowAsSidePaneCommand(dockItem, paneNode, isAutoHide, side, size, isSizeForTarget, showMethod));
                else // if (dockItem.DockControl == dockControl) Fix for 1.1.3730: this condition will prevent unattached DockItem from showing
                    dockItem.DoShowAsSidePane(paneNode, isAutoHide, side, size, isSizeForTarget, showMethod);
            }

            private DockPaneNodeStruct _paneNodeStruct;
            private bool _isAutoHide;
            private Dock _side;
            private SplitterDistance _size;
            private bool _isSizeForTarget;

            private ShowAsSidePaneCommand(DockItem dockItem, DockPaneNode paneNode, bool isAutoHide, Dock side, SplitterDistance size, bool isSizeForTarget, DockItemShowMethod showMethod)
                : base(dockItem, showMethod)
            {
                TargetPaneNode = paneNode;
                _isAutoHide = isAutoHide;
                _side = side;
                _size = size;
                _isSizeForTarget = isSizeForTarget;
            }

            private DockPaneNode GetTargetPaneNode(DockControl dockControl)
            {
                return _paneNodeStruct.ToDockPaneNode(dockControl);
            }

            private DockPaneNode TargetPaneNode
            {
                set { _paneNodeStruct = DockPaneNodeStruct.FromDockPaneNode(value); }
            }

            protected override void ExecuteOverride(DockControl dockControl)
            {
                GetDockItem(dockControl).DoShowAsSidePane(GetTargetPaneNode(dockControl), _isAutoHide, _side, _size, _isSizeForTarget, ShowMethod);
            }
        }
    }
}
