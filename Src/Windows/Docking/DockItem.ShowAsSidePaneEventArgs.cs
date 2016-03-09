using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DevZest.Windows.Docking.Primitives;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class ShowAsSidePaneEventArgs : ShowActionEventArgs<ShowAsSidePaneAction>
        {
            public ShowAsSidePaneEventArgs(DockItem dockItem, DockPaneNode paneNode, bool isAutoHide, Dock side, SplitterDistance size, bool isSizeForTarget, DockItemShowMethod showMethod)
                : base(dockItem, paneNode.DockControl, showMethod)
            {
                ShowAsSidePaneAction showAction = StrongTypeShowAction;
                DockPaneNodeStruct paneNodeStruct = DockPaneNodeStruct.FromDockPaneNode(paneNode);
                showAction.Target = paneNodeStruct.ItemIndex;
                showAction.IsFloating = paneNodeStruct.IsFloating;
                showAction.AncestorLevel = paneNodeStruct.AncestorLevel;
                showAction.IsAutoHide = isAutoHide;
                showAction.Side = side;
                showAction.Size = size;
                showAction.IsSizeForTarget = isSizeForTarget;
            }

            public override DockItemStateChangeMethod StateChangeMethod
            {
                get { return DockItemStateChangeMethod.ShowAsSidePane; }
            }
        }
    }
}
