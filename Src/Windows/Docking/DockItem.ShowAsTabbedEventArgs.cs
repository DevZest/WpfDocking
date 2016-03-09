using System;
using System.Diagnostics;
using System.Windows;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class ShowAsTabbedEventArgs : ShowActionEventArgs<ShowAsTabbedAction>
        {
            public ShowAsTabbedEventArgs(DockItem dockItem, DockPane pane, int index, DockItemShowMethod showMethod)
                : base(dockItem, pane.DockControl, showMethod)
            {
                ShowAsTabbedAction showAction = StrongTypeShowAction;
                DockPaneNodeStruct paneNodeStruct = DockPaneNodeStruct.FromDockPaneNode(pane);
                showAction.Target = paneNodeStruct.ItemIndex;
                showAction.IsFloating = paneNodeStruct.IsFloating;
                showAction.Index = index;
            }

            public override DockItemStateChangeMethod StateChangeMethod
            {
                get { return DockItemStateChangeMethod.ShowAsTabbed; }
            }
        }
    }
}
