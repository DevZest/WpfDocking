using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    partial class DockManager
    {
        private struct NestedDockEntry
        {
            private DockPane _sourcePane;
            private DockItem _targetItem;
            private int _ancestorSplitLevel;
            private Dock _dock;
            private SplitterDistance _splitterDistance;
            private bool _swapChildren;

            internal NestedDockEntry(DockPane sourcePane, DockItem targetItem, int ancestorSplitLevel, Dock dock, SplitterDistance splitterDistance, bool swapChildren)
            {
                Debug.Assert(targetItem != null);
                _sourcePane = sourcePane;
                _targetItem = targetItem;
                _ancestorSplitLevel = ancestorSplitLevel;
                _dock = dock;
                _splitterDistance = splitterDistance;
                _swapChildren = swapChildren;
            }

            public DockPane SourcePane
            {
                get { return _sourcePane; }
            }

            public DockItem TargetItem
            {
                get { return _targetItem; }
            }

            public DockPaneNode TargetPaneNode
            {
                get
                {
                    DockPaneNode pane = TargetItem.FirstPane;
                    for (int i = 0; i < AncestorSplitLevel; i++)
                        pane = pane.Parent;

                    Debug.Assert(pane != null);
                    return pane;
                }
            }

            public int AncestorSplitLevel
            {
                get { return _ancestorSplitLevel; }
            }

            public Dock Dock
            {
                get { return _dock; }
            }

            public SplitterDistance SplitterDistance
            {
                get { return _splitterDistance; }
            }

            public bool SwapChildren
            {
                get { return _swapChildren; }
            }
        }

        private static NestedDockEntry[] s_emptyNestedDockEntries = new NestedDockEntry[0];

        private static NestedDockEntry[] GetNestedDockEntries(DockPane[] panes)
        {
            if (panes.Length == 0 || panes.Length == 1)
                return s_emptyNestedDockEntries;

            NestedDockEntry[] entries = new NestedDockEntry[panes.Length - 1];
            DockPaneSplit[] splits = new DockPaneSplit[panes.Length - 1];
            for (int i = 1; i < panes.Length; i++)
            {
                DockPane sourcePane = panes[i];
                DockPane targetPane = null;
                DockPaneSplit split;
                for (split = sourcePane.Parent; split != null; split = split.Parent)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (split.IsParentOf(panes[j]))
                        {
                            targetPane = panes[j];
                            break;
                        }
                    }
                    if (targetPane != null)
                        break;
                }
                Debug.Assert(targetPane != null);
                Debug.Assert(split != null);
                int ancestorSplitLevel = 0;
                for (int j = 0; j < i - 1; j++)
                {
                    if (split.IsParentOf(splits[j]) && splits[j].IsParentOf(targetPane))
                        ancestorSplitLevel++;
                }
                entries[i - 1] = GetNestedDockEntry(sourcePane, targetPane, ancestorSplitLevel, split);
                splits[i - 1] = split;
            }

            return entries;
        }

        private static NestedDockEntry GetNestedDockEntry(DockPane sourcePane, DockPane targetPane, int ancestorSplitLevel, DockPaneSplit split)
        {
            Debug.Assert(sourcePane != null);
            Debug.Assert(targetPane != null);
            Debug.Assert(targetPane.SelectedItem != null);

            Dock dock;
            if (split.Orientation == Orientation.Horizontal)
            {
                if (split.IsSplitterTopLeft)
                    dock = Dock.Left;
                else
                    dock = Dock.Right;
            }
            else
            {
                if (split.IsSplitterTopLeft)
                    dock = Dock.Top;
                else
                    dock = Dock.Bottom;
            }

            SplitterDistance splitterDistance = split.SplitterDistance;

            DockPaneSplit child1Split = split.Child1 as DockPaneSplit;
            bool swapChildren = split.Child1 == targetPane || (child1Split != null && child1Split.IsParentOf(targetPane));

            return new NestedDockEntry(sourcePane, targetPane.SelectedItem, ancestorSplitLevel, dock, splitterDistance, swapChildren);
        }
    }
}
