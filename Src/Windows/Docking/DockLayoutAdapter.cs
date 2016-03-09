using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DevZest.Windows.Docking
{
    internal class DockLayoutAdapter
    {
        private static DockLayoutAdapter s_default;
        Collection<Collection<DockItem>> _itemsCollections = new Collection<Collection<DockItem>>();
        Collection<DockPane> _panes = new Collection<DockPane>();
        Collection<DockPaneSplit> _splits = new Collection<DockPaneSplit>();

        private static DockLayoutAdapter Default
        {
            get
            {
                if (s_default == null)
                    s_default = new DockLayoutAdapter();
                return s_default;
            }
        }

        private DockLayoutAdapter()
        {
        }

        internal static DockLayout Save(DockControl dockControl)
        {
            DockLayout layout = new DockLayout();
            layout.DockTreeZOrder = dockControl.DockTreeZOrder;
            layout.LeftDockTreeWidth = dockControl.LeftDockTreeWidth;
            layout.RightDockTreeWidth = dockControl.RightDockTreeWidth;
            layout.TopDockTreeHeight = dockControl.TopDockTreeHeight;
            layout.BottomDockTreeHeight = dockControl.BottomDockTreeHeight;
            layout.DockItems.Clear();
            layout.ShowActions.Clear();

            foreach (DockItem item in dockControl.DockItems)
            {
                DockItemReference itemRef = new DockItemReference();
                itemRef.AutoHideSize = item.AutoHideSize;
                itemRef.Target = item.Save();
                layout.DockItems.Add(itemRef);
            }

            Default.FillDockActions(dockControl, layout);

            return layout;
        }

        internal static void Load(DockControl dockControl, DockLayout layout, Func<object, DockItem> loadDockItemCallback)
        {
            Debug.Assert(dockControl.DockItems.Count == 0);

            dockControl.DockTreeZOrder = layout.DockTreeZOrder;
            dockControl.LeftDockTreeWidth = layout.LeftDockTreeWidth;
            dockControl.RightDockTreeWidth = layout.RightDockTreeWidth;
            dockControl.TopDockTreeHeight = layout.TopDockTreeHeight;
            dockControl.BottomDockTreeHeight = layout.BottomDockTreeHeight;
            DockItemCollection items = dockControl.DockItems;
            foreach (DockItemReference itemRef in layout.DockItems)
            {
                DockItem item = loadDockItemCallback == null ? (DockItem)itemRef.Target : loadDockItemCallback(itemRef.Target);
                item.AutoHideSize = itemRef.AutoHideSize;
                items.AddInternal(item);
            }

            foreach (ShowAction action in layout.ShowActions)
                action.Run(dockControl);
        }

        private void FillDockActions(DockControl dockControl, DockLayout layout)
        {
            lock (this)
            {
                foreach (DockItem item in GetDockItems(dockControl, delegate(DockItem item, DockPane pane) { return item.SecondPane == pane; }))
                    layout.ShowActions.Add(GetShowAction(item, item.SecondPane));

                foreach (DockItem item in GetDockItems(dockControl, delegate(DockItem item, DockPane pane) { return item.FirstPane == pane; }))
                    layout.ShowActions.Add(GetShowAction(item, item.FirstPane));

                _itemsCollections.Clear();
                _panes.Clear();
                _splits.Clear();
            }
        }

        private static int GetIndex(DockItem item)
        {
            return item.DockControl.DockItems.IndexOf(item);
        }

        private Collection<DockItem> GetItems(DockPane pane)
        {
            return _itemsCollections[_panes.IndexOf(pane)];
        }

        private ShowAction GetShowAction(DockItem item, DockPane pane)
        {
            Debug.Assert(item != null);
            Debug.Assert(pane != null && pane.Items.Contains(item));
            Debug.Assert(item.FirstPane == pane || item.SecondPane == pane);

            int source = GetIndex(item);
            DockItemShowMethod showMethod = item.SecondPane == pane || item.IsHidden ? DockItemShowMethod.Hide : DockItemShowMethod.Activate;

            Collection<DockItem> items;
            ShowAction action;
            if (_panes.Contains(pane))
            {
                items = GetItems(pane);
                DockItem targetItem = items[0];
                int targetIndex = GetIndex(targetItem);
                DockItem itemToInsert = FindItemToInsert(item, pane);
                int index = itemToInsert == null ? -1 : items.IndexOf(itemToInsert);
                action = new ShowAsTabbedAction(source, targetIndex, pane.IsFloating, index, showMethod);
            }
            else
            {
                if (IsDockTreeEmpty(pane))
                {
                    DockTree dockTree = pane.DockTree;
                    if (dockTree.IsFloating)
                    {
                        FloatingWindow floatingWindow = dockTree.FloatingWindow;
                        Rect bounds = new Rect(floatingWindow.Left, floatingWindow.Top, floatingWindow.Width, floatingWindow.Height);
                        action = new ShowAsFloatingAction(source, bounds, showMethod);
                    }
                    else
                    {
                        DockPosition dockPosition = DockPositionHelper.GetDockPosition(pane.DockTreePosition, pane.IsAutoHide);
                        action = new ShowAsDockPositionAction(source, dockPosition, showMethod);
                    }
                }
                else
                {
                    DockPane targetPane = null;
                    DockPaneSplit split;
                    for (split = pane.Parent; split != null; split = split.Parent)
                    {
                        foreach (DockPane pane1 in _panes)
                        {
                            if (split.IsParentOf(pane1))
                            {
                                targetPane = pane1;
                                break;
                            }
                        }
                        if (targetPane != null)
                            break;
                    }
                    Debug.Assert(targetPane != null);
                    Debug.Assert(split != null);

                    int ancestorSplitLevel = 0;
                    foreach (DockPaneSplit split1 in _splits)
                    {
                        if (split.IsParentOf(split1) && split1.IsParentOf(targetPane))
                            ancestorSplitLevel++;
                    }
                    action = GetShowAsSidePaneAction(item, pane, targetPane, ancestorSplitLevel, split, showMethod);
                    _splits.Add(split);
                }
                _panes.Add(pane);
                items = new Collection<DockItem>();
                _itemsCollections.Add(items);
            }
            items.Add(item);
            return action;
        }

        private DockItem FindItemToInsert(DockItem item, DockPane pane)
        {
            Debug.Assert(pane.Items.Contains(item));

            Collection<DockItem> items = GetItems(pane);
            for (int i = pane.Items.IndexOf(item) + 1; i < pane.Items.Count; i++)
            {
                DockItem nextItem = pane.Items[i];
                if (items.Contains(nextItem))
                    return nextItem;
            }
            return null;
        }

        private bool IsDockTreeEmpty(DockPane dockPane)
        {
            foreach (DockPane pane in _panes)
            {
                if (pane.DockTree == dockPane.DockTree)
                    return false;
            }

            return true;
        }

        private ShowAsSidePaneAction GetShowAsSidePaneAction(DockItem sourceItem, DockPane sourcePane, DockPane targetPane, int ancestorSplitLevel, DockPaneSplit split, DockItemShowMethod showMethod)
        {
            Debug.Assert(sourceItem != null);
            Debug.Assert(targetPane != null);

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

            DockPaneSplit child1Split = split.Child1 as DockPaneSplit;
            bool swapChildren = split.Child1 == targetPane || (child1Split != null && child1Split.IsParentOf(targetPane));

            int source = GetIndex(sourceItem);
            DockItem targetItem = GetItems(targetPane)[0];
            Debug.Assert(targetPane.Items.Contains(targetItem));
            int targetIndex = GetIndex(targetItem);
            bool isAutoHide = sourcePane.IsAutoHide;
            return new ShowAsSidePaneAction(source, targetIndex, targetPane.IsFloating, ancestorSplitLevel, isAutoHide, side, splitterDistance, swapChildren, showMethod);
        }

        private static IEnumerable<DockItem> GetDockItems(DockControl dockControl, Func<DockItem, DockPane, bool> predicate)
        {
            foreach (DockPane pane in dockControl.Panes)
            {
                foreach (DockItem item in GetDockItems(pane))
                {
                    if (predicate(item, pane))
                        yield return item;
                }
            }
        }

        private static IEnumerable<DockItem> GetDockItems(DockPane dockPane)
        {
            foreach (DockItem item in dockPane.Items)
            {
                if (!dockPane.VisibleItems.Contains(item))
                    yield return item;
            }

            foreach (DockItem item in dockPane.ActiveItems)
                yield return item;
        }
    }
}