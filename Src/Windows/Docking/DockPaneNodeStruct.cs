using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    internal struct DockPaneNodeStruct
    {
        public static DockPaneNodeStruct FromDockPaneNode(DockPaneNode paneNode)
        {
            DockControl dockControl = paneNode.DockControl;
            Debug.Assert(dockControl != null);

            int ancestorSplitLevel = 0;
            DockPaneNode node;
            for (node = paneNode; node is DockPaneSplit; node = (node as DockPaneSplit).Child1)
                ancestorSplitLevel++;
            DockPane pane = node as DockPane;
            Debug.Assert(pane != null);

            DockItem item = pane.Items[0];
            int itemIndex = dockControl.DockItems.IndexOf(item);
            bool isFloating = paneNode.IsFloating;
            return new DockPaneNodeStruct(itemIndex, isFloating, ancestorSplitLevel);
        }

        private int _itemIndex;
        private bool _isFloating;
        private int _ancestorLevel;

        private DockPaneNodeStruct(int targetIndex, bool isFloating, int ancestorLevel)
        {
            _itemIndex = targetIndex;
            _isFloating = isFloating;
            _ancestorLevel = ancestorLevel;
        }

        public int ItemIndex
        {
            get { return _itemIndex; }
        }

        public bool IsFloating
        {
            get { return _isFloating; }
        }

        public int AncestorLevel
        {
            get { return _ancestorLevel; }
        }

        public DockPaneNode ToDockPaneNode(DockControl dockControl)
        {
            DockItem item = dockControl.DockItems[ItemIndex];
            DockPaneNode paneNode = item.FirstPane.IsFloating == IsFloating ? item.FirstPane : item.SecondPane;
            for (int i = 0; i < AncestorLevel; i++)
                paneNode = paneNode.Parent;
            return paneNode;
        }
    }
}
