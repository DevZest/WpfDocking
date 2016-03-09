using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the client area to display the left, right, top, bottom and document <see cref="DockTree"/> objects.</summary>
    /// <remarks><para><see cref="DockTreeClient"/> converts the left, right, top, bottom and document <see cref="DockTree"/> objects
    /// into a tree of <see cref="DockTreeSplit"/> (non-leaf node) objects and <see cref="DockTree"/> (leaf node) objects.
    /// The <see cref="Split"/> property returns the root <see cref="DockTreeSplit"/> tree node.</para>
    /// <para>Use <see cref="DockTreeClient"/> in the control template of <see cref="Docking.DockControl"/>, bind the <see cref="DockControl"/>
    /// property to its template parent.</para></remarks>
    public class DockTreeClient : Control
    {
        /// <summary>Identifies the <see cref="DockControl"/> dependency property.</summary>
        public static readonly DependencyProperty DockControlProperty;
        private static readonly DependencyPropertyKey SplitPropertyKey;
        /// <summary>Identifies the <see cref="Split"/> dependency property.</summary>
        public static readonly DependencyProperty SplitProperty;
        private static readonly DependencyProperty LeftDockTreeIsVisibleProperty;
        private static readonly DependencyProperty TopDockTreeIsVisibleProperty;
        private static readonly DependencyProperty RightDockTreeIsVisibleProperty;
        private static readonly DependencyProperty BottomDockTreeIsVisibleProperty;
        private static readonly DependencyProperty DocumentDockTreeIsVisibleProperty;
        private static readonly DependencyProperty LeftDockTreeWidthProperty;
        private static readonly DependencyProperty RightDockTreeWidthProperty;
        private static readonly DependencyProperty TopDockTreeHeightProperty;
        private static readonly DependencyProperty BottomDockTreeHeightProperty;
        private static readonly DependencyProperty DockTreeZOrderProperty;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DockTreeClient()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockTreeClient), new FrameworkPropertyMetadata(typeof(DockTreeClient)));
            DockControlProperty = DependencyProperty.Register("DockControl", typeof(DockControl), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDockControlChanged)));
            SplitPropertyKey = DependencyProperty.RegisterReadOnly("Split", typeof(DockTreeSplit), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(null));
            SplitProperty = SplitPropertyKey.DependencyProperty;
            LeftDockTreeIsVisibleProperty = DependencyProperty.Register("LeftDockTreeIsVisible", typeof(Boolean), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnIsVisibleChanged)));
            RightDockTreeIsVisibleProperty = DependencyProperty.Register("RightDockTreeIsVisible", typeof(Boolean), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnIsVisibleChanged)));
            TopDockTreeIsVisibleProperty = DependencyProperty.Register("TopDockTreeIsVisible", typeof(Boolean), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnIsVisibleChanged)));
            BottomDockTreeIsVisibleProperty = DependencyProperty.Register("BottomDockTreeIsVisible", typeof(Boolean), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnIsVisibleChanged)));
            DocumentDockTreeIsVisibleProperty = DependencyProperty.Register("DocumentDockTreeIsVisible", typeof(Boolean), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnIsVisibleChanged)));
            LeftDockTreeWidthProperty = DependencyProperty.Register("LeftDockTreeWidth", typeof(SplitterDistance), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(new SplitterDistance(1d / 3d, SplitterUnitType.Star),
                new PropertyChangedCallback(OnDockTreeSizeChanged)));
            RightDockTreeWidthProperty = DependencyProperty.Register("RightDockTreeWidth", typeof(SplitterDistance), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(new SplitterDistance(1d / 3d, SplitterUnitType.Star),
                new PropertyChangedCallback(OnDockTreeSizeChanged)));
            TopDockTreeHeightProperty = DependencyProperty.Register("TopDockTreeHeight", typeof(SplitterDistance), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(new SplitterDistance(1d / 3d, SplitterUnitType.Star),
                new PropertyChangedCallback(OnDockTreeSizeChanged)));
            BottomDockTreeHeightProperty = DependencyProperty.Register("BottomDockTreeHeight", typeof(SplitterDistance), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(new SplitterDistance(1d / 3d, SplitterUnitType.Star),
                new PropertyChangedCallback(OnDockTreeSizeChanged)));
            DockTreeZOrderProperty = DependencyProperty.Register("DockTreeZOrder", typeof(DockTreeZOrder), typeof(DockTreeClient),
                new FrameworkPropertyMetadata(DockTreeZOrder.Default, new PropertyChangedCallback(OnDockTreeZOrderChanged)));
        }

        private static void OnDockControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockTreeClient)d).OnDockControlChanged((DockControl)e.NewValue);
        }

        private void OnDockControlChanged(DockControl newValue)
        {
            _isInitBinding = true;
            if (newValue == null)
            {
                BindingOperations.ClearBinding(this, LeftDockTreeIsVisibleProperty);
                BindingOperations.ClearBinding(this, RightDockTreeIsVisibleProperty);
                BindingOperations.ClearBinding(this, TopDockTreeIsVisibleProperty);
                BindingOperations.ClearBinding(this, BottomDockTreeIsVisibleProperty);
                BindingOperations.ClearBinding(this, DocumentDockTreeIsVisibleProperty);
                BindingOperations.ClearBinding(this, LeftDockTreeWidthProperty);
                BindingOperations.ClearBinding(this, RightDockTreeWidthProperty);
                BindingOperations.ClearBinding(this, TopDockTreeHeightProperty);
                BindingOperations.ClearBinding(this, BottomDockTreeHeightProperty);
                BindingOperations.ClearBinding(this, DockTreeZOrderProperty);
            }
            else
            {
                SetBinding(LeftDockTreeIsVisibleProperty, new Binding() { Source = DockControl.LeftDockTree, Path = new PropertyPath(DockTree.IsVisibleProperty) });
                SetBinding(RightDockTreeIsVisibleProperty, new Binding() { Source = DockControl.RightDockTree, Path = new PropertyPath(DockTree.IsVisibleProperty) });
                SetBinding(TopDockTreeIsVisibleProperty, new Binding() { Source = DockControl.TopDockTree, Path = new PropertyPath(DockTree.IsVisibleProperty) });
                SetBinding(BottomDockTreeIsVisibleProperty, new Binding() { Source = DockControl.BottomDockTree, Path = new PropertyPath(DockTree.IsVisibleProperty) });
                SetBinding(DocumentDockTreeIsVisibleProperty, new Binding() { Source = DockControl.DocumentDockTree, Path = new PropertyPath(DockTree.IsVisibleProperty) });
                SetBinding(LeftDockTreeWidthProperty, new Binding() { Source = DockControl, Path = new PropertyPath(DockControl.LeftDockTreeWidthProperty) });
                SetBinding(RightDockTreeWidthProperty, new Binding() { Source = DockControl, Path = new PropertyPath(DockControl.RightDockTreeWidthProperty) });
                SetBinding(TopDockTreeHeightProperty, new Binding() { Source = DockControl, Path = new PropertyPath(DockControl.TopDockTreeHeightProperty) });
                SetBinding(BottomDockTreeHeightProperty, new Binding() { Source = DockControl, Path = new PropertyPath(DockControl.BottomDockTreeHeightProperty) });
                SetBinding(DockTreeZOrderProperty, new Binding() { Source = DockControl, Path = new PropertyPath(DockControl.DockTreeZOrderProperty) });
            }
            InitializeSplits();
            _isInitBinding = false;
        }

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockTreeClient dockTreeClient = (DockTreeClient)d;

            if (dockTreeClient._isInitBinding)
                return;

            DockTree dockTree;
            DependencyProperty dp = e.Property;
            if (dp == LeftDockTreeIsVisibleProperty)
                dockTree = dockTreeClient.DockControl.LeftDockTree;
            else if (dp == RightDockTreeIsVisibleProperty)
                dockTree = dockTreeClient.DockControl.RightDockTree;
            else if (dp == TopDockTreeIsVisibleProperty)
                dockTree = dockTreeClient.DockControl.TopDockTree;
            else if (dp == BottomDockTreeIsVisibleProperty)
                dockTree = dockTreeClient.DockControl.BottomDockTree;
            else
            {
                Debug.Assert(dp == DocumentDockTreeIsVisibleProperty);
                    dockTree = dockTreeClient.DockControl.DocumentDockTree;
            }

            dockTreeClient.GetParentSplit(dockTree).InvalidateState();
        }

        private static void OnDockTreeZOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockTreeClient dockTreeClient = (DockTreeClient)d;

            if (dockTreeClient._isInitBinding)
                return;

            dockTreeClient.DockControl.SaveFocus();
            dockTreeClient.InitializeSplits();
            dockTreeClient.DockControl.RestoreFocus();
        }

        private static void OnDockTreeSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockTreeClient dockTreeClient = (DockTreeClient)d;

            if (dockTreeClient._isInitBinding)
                return;

            Dock dock;
            DependencyProperty dp = e.Property;
            if (dp == LeftDockTreeWidthProperty)
                dock = Dock.Left;
            else if (dp == RightDockTreeWidthProperty)
                dock = Dock.Right;
            else if (dp == TopDockTreeHeightProperty)
                dock = Dock.Top;
            else
            {
                Debug.Assert(dp == BottomDockTreeHeightProperty);
                dock = Dock.Bottom;
            }

            dockTreeClient.GetDockTreeSplit(dock).UpdateSplitterDistance();
        }

        private bool _isInitBinding;
        private DockTreeSplit[] _splits;
        private Size _clientSize;

        /// <summary>Gets or sets the <see cref="Docking.DockControl"/> with this <see cref="DockTreeClient"/>. This is a dependency property.</summary>
        /// <remarks><para>Use <see cref="DockTreeClient"/> in the control template of <see cref="Docking.DockControl"/>, bind the
        /// <see cref="DockControl"/> property to its template parent.</para></remarks>
        /// <value>The <see cref="Docking.DockControl"/> with this <see cref="DockTreeClient"/>.</value>
        public DockControl DockControl
        {
            get { return (DockControl)GetValue(DockControlProperty); }
            set { SetValue(DockControlProperty, value); }
        }

        /// <summary>Gets the root node of the tree converted from <see cref="DockTree"/> objects.</summary>
        /// <remarks><para><see cref="DockTreeClient"/> converts the left, right, top, bottom and document <see cref="DockTree"/> objects
        /// into a tree of <see cref="DockTreeSplit"/> (non-leaf node) objects and <see cref="DockTree"/> (leaf node) objects.
        /// The <see cref="Split"/> property returns the root <see cref="DockTreeSplit"/> tree node.</para></remarks>
        /// <value>The root node of the tree converted from <see cref="DockTree"/> objects.</value>
        public DockTreeSplit Split
        {
            get { return (DockTreeSplit)GetValue(SplitProperty); }
            private set { SetValue(SplitPropertyKey, value); }
        }

        /// <exclude/>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size finalSize = base.ArrangeOverride(arrangeBounds);
            ClientSize = arrangeBounds;

            return finalSize;
        }

        internal Size ClientSize
        {
            get { return _clientSize; }
            set
            {
                if (_clientSize == value)
                    return;

                _clientSize = value;
                foreach (DockTreeSplit split in Splits)
                    split.UpdateSplitterDistance();
            }
        }

        private DockTreeSplit[] Splits
        {
            get { return _splits; }
        }

        private void InitializeSplits()
        {
            if (DockControl == null)
            {
                if (_splits != null)
                {
                    for (int i = 3; i >= 0; i--)
                        _splits[i].Dispose();
                    _splits = null;
                }
                return;
            }

            Split = null;
            UpdateLayout();
            DockTreeZOrder zorder = DockControl.DockTreeZOrder;
            if (_splits == null)
                _splits = new DockTreeSplit[4];
            DockTreeSplit[] splits = _splits;
            DockTreeSplit nextSplit = null;
            for (int i = 3; i >= 0; i--)
            {
                if (splits[i] != null)
                    splits[i].Dispose();
                nextSplit = splits[i] = new DockTreeSplit(this, zorder[i], nextSplit);
            }

            for (int i = 3; i >= 0; i--)
                splits[i].RefreshState();

            Split = splits[0];
        }

        private DockTreeSplit GetDockTreeSplit(Dock position)
        {
            foreach (DockTreeSplit split in Splits)
            {
                if (split.Position == position)
                    return split;
            }

            return null;
        }

        private DockTreeSplit GetParentSplit(DockTree dockTree)
        {
            Debug.Assert(dockTree.DockControl == DockControl);
            if (dockTree.Position == DockTreePosition.Document)
                return Splits[3];
            int index = DockControl.DockTreeZOrder.IndexOf(DockPositionHelper.GetDock(dockTree.Position));
            Debug.Assert(index != -1);
            return Splits[index];
        }

        internal DockTreeSplit GetParentSplit(DockTreeSplit split)
        {
            Debug.Assert(split != null);
            int index = DockControl.DockTreeZOrder.IndexOf(split.Position);
            Debug.Assert(index != -1);
            return index == 0 ? null : Splits[index - 1];
        }
    }
}
