using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a tree of <see cref="DockPaneNode"/> objects.</summary>
    public sealed class DockTree : DependencyObject
    {
        private static readonly DependencyPropertyKey DockControlPropertyKey =
            DependencyProperty.RegisterReadOnly("DockControl", typeof(DockControl), typeof(DockTree),
                new FrameworkPropertyMetadata(null));
        private static readonly DependencyPropertyKey IsVisiblePropertyKey =
            DependencyProperty.RegisterReadOnly("IsVisible", typeof(Boolean), typeof(DockTree),
                new FrameworkPropertyMetadata(BooleanBoxes.False));
        private static DependencyPropertyKey PositionPropertyKey =
            DependencyProperty.RegisterReadOnly("Position", typeof(DockTreePosition), typeof(DockTree),
                new FrameworkPropertyMetadata());
        private static DependencyPropertyKey RootNodePropertyKey =
            DependencyProperty.RegisterReadOnly("RootNode", typeof(DockPaneNode), typeof(DockTree),
                new PropertyMetadata(null));
        /// <summary>Identifies the <see cref="DockControl"/> dependency property.</summary>
        public static readonly DependencyProperty DockControlProperty = DockControlPropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="IsVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsVisibleProperty = IsVisiblePropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="RootNode"/> dependency property.</summary>
        public static readonly DependencyProperty RootNodeProperty = RootNodePropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="Position"/> dependency property.</summary>
        public static readonly DependencyProperty PositionProperty = PositionPropertyKey.DependencyProperty;

        private FloatingWindow _floatingWindow;
        private DockPaneCollection _panes;
        private DockPaneCollection _visiblePanes;
        private DockPaneCollection _activePanes;
        private DockPaneCollection _autoHidePanes;
        private DockItemCollection _autoHideItems;
        private List<DockPaneNode> _dirtyNodes;
        private bool _isDirty;

        internal DockTree(DockControl dockControl, DockTreePosition dockTreePosition)
            : this(dockControl, null, dockTreePosition)
        {
        }

        internal DockTree(FloatingWindow floatingWindow)
            : this(floatingWindow.DockControl, floatingWindow, DockTreePosition.Floating)
        {
        }

        private DockTree(DockControl dockControl, FloatingWindow floatingWindow, DockTreePosition dockTreePosition)
        {
            DockControl = dockControl;
            _floatingWindow = floatingWindow;
            _panes = new DockPaneCollection();
            _visiblePanes = new DockPaneCollection();
            _activePanes = new DockPaneCollection();
            _autoHidePanes = new DockPaneCollection();
            _autoHideItems = new DockItemCollection();
            _dirtyNodes = new List<DockPaneNode>();
            Position = dockTreePosition;
        }

        /// <summary>Gets the <see cref="Docking.DockControl"/> which this <see cref="DockTree"/> belongs to. This is a dependency property.</summary>
        /// <value>The <see cref="Docking.DockControl"/> which this <see cref="DockTree"/> belongs to.</value>
        public DockControl DockControl
        {
            get { return (DockControl)GetValue(DockControlProperty); }
            private set { SetValue(DockControlPropertyKey, value); }
        }

        /// <summary>Gets a value indicates whether this <see cref="DockTree"/> is visible. This is a dependency property.</summary>
        /// <value><see langword="true"/> if this <see cref="DockTree"/> is visible, otherwise <see langword="false"/>.</value>
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            private set { SetValue(IsVisiblePropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets the position of the <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The position of the <see cref="DockTree"/>.</value>
        public DockTreePosition Position
        {
            get { return (DockTreePosition)GetValue(PositionProperty); }
            private set { SetValue(PositionPropertyKey, value); }
        }

        /// <summary>Gets the root node of the <see cref="DockTree"/>. This is a dependency property.</summary>
        /// <value>The root node of the <see cref="DockTree"/>.</value>
        public DockPaneNode RootNode
        {
            get { return (DockPaneNode)GetValue(RootNodeProperty) ; }
        }

        private void SetRootNode(DockPaneNode newValue)
        {
            SetRootNode(newValue, true);
        }

        private void SetRootNode(DockPaneNode newValue, bool clearOldValueParent)
        {
            DockPaneNode oldValue = RootNode;
            if (oldValue == newValue)
                return;

            SetValue(RootNodePropertyKey, newValue);

            if (oldValue != null && clearOldValueParent)
                oldValue.ClearParent();

            if (newValue != null)
                newValue.SetParent(this);
        }

        /// <summary>Gets a collection of <see cref="DockPane"/> objects contained by this <see cref="DockTree"/>.</summary>
        /// <value>A collection of <see cref="DockPane"/> objects contained by this <see cref="DockTree"/>, in the order of
        /// creation (last created last).</value>
        public DockPaneCollection Panes
        {
            get { return _panes; }
        }

        /// <summary>Gets a collection of visible <see cref="DockPane"/> objects contained by this <see cref="DockTree"/>.</summary>
        /// <value>A collection of visible <see cref="DockPane"/> objects contained by this <see cref="DockTree"/>, in the order of
        /// creation (last created last).</value>
        public DockPaneCollection VisiblePanes
        {
            get { return _visiblePanes; }
        }

        /// <summary>Gets a collection of active <see cref="DockPane"/> objects contained by this <see cref="DockTree"/>.</summary>
        /// <value>A collection of active <see cref="DockPane"/> objects contained by this <see cref="DockTree"/>, in the order of
        /// activation (last activated last).</value>
        public DockPaneCollection ActivePanes
        {
            get { return _activePanes; }
        }

        /// <summary>Gets a collection of auto-hide <see cref="DockPane"/> objects contained by this <see cref="DockTree"/>.</summary>
        /// <value>A collection of <see cref="DockPane"/> objects contained by this <see cref="DockTree"/>, in the order of
        /// creation (last created last).</value>
        public DockPaneCollection AutoHidePanes
        {
            get { return _autoHidePanes; }
        }

        /// <summary>Gets a collection of auto-hide <see cref="DockItem"/> objects contained by this <see cref="DockTree"/>.</summary>
        /// <value>A collection of <see cref="DockItem"/> objects contained by this <see cref="DockTree"/>.</value>
        public DockItemCollection AutoHideItems
        {
            get { return _autoHideItems; }
        }

        internal bool IsFloating
        {
            get { return Position == DockTreePosition.Floating; }
        }

        /// <summary>Gets the <see cref="FloatingWindow"/> that contains this <see cref="DockTree"/>.</summary>
        /// <value>The <see cref="FloatingWindow"/> that contains this <see cref="DockTree"/>.</value>
        public FloatingWindow FloatingWindow
        {
            get { return _floatingWindow; }
        }

        internal DockPane FindPane(bool isAutoHide)
        {
            return FindPane(RootNode, isAutoHide);
        }

        private static DockPane FindPane(DockPaneNode node, bool isAutoHide)
        {
            DockPane pane = node as DockPane;
            if (pane != null && pane.IsAutoHide == isAutoHide)
                return pane;

            DockPaneSplit split = node as DockPaneSplit;
            if (split != null)
            {
                pane = FindPane(split.Child1, isAutoHide);
                if (pane != null)
                    return pane;
                pane = FindPane(split.Child2, isAutoHide);
                if (pane != null)
                    return pane;
            }

            return null;
        }

        internal DockPane FirstPane
        {
            get
            {
                DockPaneNode node = RootNode;
                while (node != null)
                {
                    DockPane pane = node as DockPane;
                    if (pane != null)
                        return pane;
                    else
                        node = ((DockPaneSplit)node).Child1;
                }
                return null;
            }

        }

        private void AddPaneToCollection(DockPane pane)
        {
            Debug.Assert(!_panes.Contains(pane));
            _panes.Insert(_panes.Count, pane);
        }

        private void RemovePaneFromCollection(DockPane pane)
        {
            Debug.Assert(_panes.Contains(pane));
            _panes.Remove(pane);
            if (_visiblePanes.Contains(pane))
                RemoveVisiblePane(pane);
            if (_autoHidePanes.Contains(pane))
                RemoveAutoHidePane(pane);
        }

        private static bool IsPaneVisible(DockPane pane)
        {
            return !pane.IsAutoHide && pane.DockPosition != DockPosition.Hidden;
        }

        private static bool IsPaneAutoHide(DockPane pane)
        {
            return pane.IsAutoHide && pane.DockPosition != DockPosition.Hidden;
        }

        private void RemoveVisiblePane(DockPane pane)
        {
            Debug.Assert(_visiblePanes.Contains(pane));
            _visiblePanes.Remove(pane);
        }

        private void RemoveAutoHidePane(DockPane pane)
        {
            Debug.Assert(_autoHidePanes.Contains(pane));
            _autoHidePanes.Remove(pane);
        }

        internal void AddDirtyNode(DockPaneNode node)
        {
            if (_dirtyNodes.Contains(node))
                return;

            node.LockState();
            _dirtyNodes.Add(node);
            _isDirty = true;
        }

        internal void CommitChanges()
        {
            CommitChanges(false);
        }

        private void CommitChanges(bool force)
        {
            if (!_isDirty && !force)
                return;

            foreach (DockPaneNode node in _dirtyNodes)
                node.RefreshState();

            _dirtyNodes.Clear();

            DockPaneNode rootNode = RootNode;
            IsVisible = (rootNode != null && rootNode.DockPosition == DockPositionHelper.GetDockPosition(Position, false));
            UpdateVisiblePanes();
            UpdateAutoHidePanes();
            UpdateAutoHideItems();
            if (_floatingWindow != null)
            {
                _floatingWindow.CountOfVisiblePanes = _visiblePanes.Count;
                _floatingWindow.FirstVisiblePane = _visiblePanes.Count == 0 ? null : _visiblePanes[0];
                _floatingWindow.IsVisible = IsVisible;
                if (_panes.Count == 0) // Fix bug #1 https://github.com/DevZest/WpfDocking/issues/1
                    _floatingWindow.DockControl.RemoveFloatingWindow(_floatingWindow);
            }
            _isDirty = false;
        }

        private void UpdateVisiblePanes()
        {
            CollectionUtil.Synchronize(_panes,
                delegate(DockPane pane) { return IsPaneVisible(pane); },
                _visiblePanes,
                delegate(int index, DockPane pane) { _visiblePanes.Insert(index, pane); },
                delegate(DockPane pane) { _visiblePanes.Remove(pane); });
        }

        private void UpdateAutoHidePanes()
        {
            CollectionUtil.Synchronize(_panes,
                delegate(DockPane pane) { return IsPaneAutoHide(pane); },
                _autoHidePanes,
                delegate(int index, DockPane pane) { _autoHidePanes.Insert(index, pane); },
                delegate(DockPane pane) { _autoHidePanes.Remove(pane); });
        }

        private void UpdateAutoHideItems()
        {
            CollectionUtil.Synchronize(GetAutoHideItems(),
                _autoHideItems,
                delegate(int index, DockItem item) { _autoHideItems.Insert(index, item); },
                delegate(DockItem item) { _autoHideItems.Remove(item); });
        }

        private IEnumerable<DockItem> GetAutoHideItems()
        {
            foreach (DockPane pane in _autoHidePanes)
            {
                if (!DockPositionHelper.IsAutoHide(pane.DockPosition))
                    continue;

                foreach (DockItem item in pane.VisibleItems)
                    yield return item;
            }
        }

        internal DockPane AddItem(DockItem item, bool isAutoHide)
        {
            Debug.Assert(RootNode == null);

            DockPane pane = DockControl.PaneManager.CreatePane(item, isAutoHide);
            AddDirtyNode(pane);
            SetRootNode(pane);
            AddPaneToCollection(pane);
            return pane;
        }

        internal void AddItem(DockItem item, DockPane pane, int index)
        {
            Debug.Assert(pane.DockTree == this);
            Debug.Assert(index >= 0 && index <= pane.Items.Count);

            AddDirtyNode(pane);
            pane.InsertItem(index, item);
        }

        internal DockPane AddItem(DockItem item, DockPaneNode targetPaneNode, bool isAutoHide, Dock side, SplitterDistance size, bool isSizeForTarget)
        {
            Debug.Assert(targetPaneNode.DockTree == this);

            DockPane pane = DockControl.PaneManager.CreatePane(item, isAutoHide);
            AddDirtyNode(pane);
            AddDirtyNode(targetPaneNode);

            DockPaneSplit parent = targetPaneNode.Parent;
            DockPaneNode child1 = side == Dock.Left || side == Dock.Top ? pane : targetPaneNode;
            DockPaneNode child2 = side == Dock.Left || side == Dock.Top ? targetPaneNode : pane;
            Orientation orientation = side == Dock.Left || side == Dock.Right ? Orientation.Horizontal : Orientation.Vertical;
            bool isSplitterTopLeft = isSizeForTarget ? child1 == targetPaneNode : child2 == targetPaneNode;
            targetPaneNode.ClearParent();
            DockPaneSplit newSplit;
            newSplit = new DockPaneSplit(child1, child2, orientation, isSplitterTopLeft, size);
            AddDirtyNode(newSplit);
            if (parent != null)
            {
                AddDirtyNode(parent);
                parent.SetSibling(parent.GetSibling(targetPaneNode), newSplit);
            }
            else
                SetRootNode(newSplit, false);
            AddPaneToCollection(pane);
            Debug.Assert(newSplit.Child1 != null);
            Debug.Assert(newSplit.Child1.Parent == newSplit);
            Debug.Assert(newSplit.Child2 != null);
            Debug.Assert(newSplit.Child2.Parent == newSplit);
            return pane;
        }

        internal void RemovePane(DockPane pane)
        {
            Debug.Assert(pane.DockTree == this);
            Debug.Assert(pane.Items.Count == 0);

            RemovePaneFromCollection(pane);

            DockPaneSplit splitToRemove = pane.Parent;
            if (splitToRemove == null)
                SetRootNode(null);
            else
            {
                AddDirtyNode(splitToRemove);

                DockPaneNode siblingNode = splitToRemove.GetSibling(pane);
                Debug.Assert(siblingNode != null);
                AddDirtyNode(siblingNode);
                DockPaneSplit grandParent = splitToRemove.Parent;
                splitToRemove.ClearChildren();
                if (grandParent == null)
                    SetRootNode(siblingNode);
                else
                {
                    AddDirtyNode(grandParent);
                    grandParent.SetSibling(grandParent.GetSibling(splitToRemove), siblingNode);
                }

                splitToRemove.ClearParent();
            }
        }

        /// <exclude/>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                "DockTree: Position={0} IsVisible={1} RootNode={2}",
                Position, IsVisible, RootNode);
        }
    }
}
