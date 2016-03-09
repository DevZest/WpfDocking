using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private abstract class CommandBase : ICommand
        {
            private int _dockItemIndex = -1;

            protected CommandBase(DockItem dockItem)
            {
                Debug.Assert(dockItem != null);
                DockControl dockControl = dockItem.DockControl;
                _dockItemIndex = dockControl == null ? -1 : dockControl.DockItems.IndexOf(dockItem);
            }

            protected int DockItemIndex
            {
                get { return _dockItemIndex; }
            }

            protected virtual DockItem GetDockItem(DockControl dockControl)
            {
                return dockControl.DockItems[_dockItemIndex];
            }

            public abstract void Execute(DockControl dockControl);

            public abstract void UnExecute(DockControl dockControl);

            protected static ShowAction GetShowAction(DockItem item, DockPane pane)
            {
                Debug.Assert(item != null);
                Debug.Assert(pane != null && pane.Items.Contains(item));
                Debug.Assert(item.FirstPane == pane || item.SecondPane == pane);

                if (pane.Items.Count > 1)
                    return GetShowAsTabbedAction(item, pane);
                else if (pane.Parent == null)
                    return GetShowAsDockTreeRootAction(item, pane);
                else
                    return GetShowAsSidePaneAction(item, pane);
            }

            protected static DockItemShowMethod GetShowMethod(DockItem item, DockPane pane)
            {
                Debug.Assert(item != null);
                Debug.Assert(pane != null && pane.Items.Contains(item));
                Debug.Assert(item.FirstPane == pane || item.SecondPane == pane);

                DockControl dockControl = item.DockControl;
                DockItemShowMethod showMethod;
                if (item.SecondPane == pane || item.IsHidden)
                    showMethod = DockItemShowMethod.Hide;
                else if (dockControl.FocusedItem == item)
                    showMethod = DockItemShowMethod.Activate;
                else if (pane.SelectedItem == item)
                    showMethod = DockItemShowMethod.Select;
                else
                    showMethod = DockItemShowMethod.Deselect;

                return showMethod;
            }

            private static ShowAction GetShowAsTabbedAction(DockItem item, DockPane pane)
            {
                DockControl dockControl = item.DockControl;
                int source = dockControl.DockItems.IndexOf(item);
                DockItemShowMethod showMethod = GetShowMethod(item, pane);

                int index = pane.Items.IndexOf(item);
                DockItem targetItem = pane.Items[index == 0 ? 1 : 0];
                int targetIndex = dockControl.DockItems.IndexOf(targetItem);
                if (index == pane.Items.Count - 1)
                    index = -1;
                return new ShowAsTabbedAction(source, targetIndex, pane.IsFloating, index, showMethod);
            }

            private static ShowAction GetShowAsDockTreeRootAction(DockItem item, DockPane pane)
            {
                DockControl dockControl = item.DockControl;
                int source = dockControl.DockItems.IndexOf(item);
                DockItemShowMethod showMethod = GetShowMethod(item, pane);

                DockTree dockTree = pane.DockTree;
                if (dockTree.IsFloating)
                {
                    FloatingWindow floatingWindow = dockTree.FloatingWindow;
                    Rect bounds = new Rect(floatingWindow.Left, floatingWindow.Top, floatingWindow.Width, floatingWindow.Height);
                    return new ShowAsFloatingAction(source, bounds, showMethod);
                }
                else
                {
                    DockPosition dockPosition = DockPositionHelper.GetDockPosition(pane.DockTreePosition, pane.IsAutoHide);
                    return new ShowAsDockPositionAction(source, dockPosition, showMethod);
                }
            }

            private static ShowAction GetShowAsSidePaneAction(DockItem item, DockPane pane)
            {
                DockPaneSplit split = pane.Parent;
                Debug.Assert(split != null);
                DockPaneNode targetPaneNode = split.Child1 == pane ? split.Child2 : split.Child1;
                Debug.Assert(targetPaneNode != null);
                DockPaneNodeStruct targetPaneNodeStruct = DockPaneNodeStruct.FromDockPaneNode(targetPaneNode);
                bool swapChildren = split.Child1 == targetPaneNode;
                Dock side;
                if (split.Orientation == Orientation.Horizontal)
                {
                    if (split.IsSplitterTopLeft)
                        side = Dock.Left;
                    else
                        side = Dock.Right;
                }
                else
                {
                    if (split.IsSplitterTopLeft)
                        side = Dock.Top;
                    else
                        side = Dock.Bottom;
                }
                SplitterDistance splitterDistance = split.SplitterDistance;

                int source = pane.DockControl.DockItems.IndexOf(item);
                int targetIndex = targetPaneNodeStruct.ItemIndex;
                bool isFloating = targetPaneNodeStruct.IsFloating;
                int ancestorLevel = targetPaneNodeStruct.AncestorLevel;
                bool isAutoHide = pane.IsAutoHide;
                DockItemShowMethod showMethod = GetShowMethod(item, pane);
                return new ShowAsSidePaneAction(source, targetIndex, isFloating, ancestorLevel, isAutoHide, side, splitterDistance, swapChildren, showMethod);
            }
        }
    }
}
