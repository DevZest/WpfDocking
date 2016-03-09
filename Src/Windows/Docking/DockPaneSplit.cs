using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a container consisting of two resizable <see cref="DockPaneNode"/> objects.</summary>
    public sealed class DockPaneSplit : DockPaneNode
    {
        private static readonly DependencyPropertyKey OrientationPropertyKey = DependencyProperty.RegisterReadOnly("Orientation", typeof(Orientation), typeof(DockPaneSplit),
                new PropertyMetadata(OrientationBoxes.Vertical));
        private static readonly DependencyPropertyKey IsSplitterTopLeftPropertyKey = DependencyProperty.RegisterReadOnly("IsSplitterTopLeft", typeof(bool), typeof(DockPaneSplit),
                new PropertyMetadata(BooleanBoxes.True));
        private static readonly DependencyPropertyKey Child1PropertyKey = DependencyProperty.RegisterReadOnly("Child1", typeof(DockPaneNode), typeof(DockPaneSplit),
                new PropertyMetadata(null));
        private static readonly DependencyPropertyKey Child2PropertyKey = DependencyProperty.RegisterReadOnly("Child2", typeof(DockPaneNode), typeof(DockPaneSplit),
                new PropertyMetadata(null));
        private static readonly DependencyPropertyKey ChildrenVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("ChildrenVisibility", typeof(SplitChildrenVisibility), typeof(DockPaneSplit),
                new PropertyMetadata(SplitChildrenVisibility.None));

        /// <summary>Identifies the <see cref="SplitterDistance"/> dependency property.</summary>
        public static readonly DependencyProperty SplitterDistanceProperty = DependencyProperty.Register("SplitterDistance", typeof(SplitterDistance), typeof(DockPaneSplit),
                new FrameworkPropertyMetadata(new SplitterDistance(1d, SplitterUnitType.Star), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty = OrientationPropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="IsSplitterTopLeft"/> dependency property.</summary>
        public static readonly DependencyProperty IsSplitterTopLeftProperty = IsSplitterTopLeftPropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="Child1"/> dependency property.</summary>
        public static readonly DependencyProperty Child1Property = Child1PropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="Child2"/> dependency property.</summary>
        public static readonly DependencyProperty Child2Property = Child2PropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="ChildrenVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty ChildrenVisibilityProperty = ChildrenVisibilityPropertyKey.DependencyProperty;

        internal DockPaneSplit(DockPaneNode child1, DockPaneNode child2, Orientation orientation, bool isSplitterTopLeft, SplitterDistance splitterDistance)
        {
            Child1 = child1;
            Child2 = child2;
            Orientation = orientation;
            IsSplitterTopLeft = isSplitterTopLeft;
            SplitterDistance = splitterDistance;
        }

        /// <summary>Gets a value indicating the horizontal or vertical orientation of the children. This is a dependency property.</summary>
        /// <value>The value indicating the horizontal or vertical orientation of the children.</value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            private set { SetValue(OrientationPropertyKey, OrientationBoxes.Box(value)); }
        }

        /// <summary>Gets a value indicating whether the <see cref="SplitterDistance"/> property specifies the size of <see cref="Child1"/> or <see cref="Child2"/>. This is a dependency property.</summary>
        /// <value><see langword="true"/> if <see cref="SplitterDistance"/> specifies the size of <see cref="Child1"/>; <see langword="false"/> if <see cref="SplitterDistance"/> specifies the size of <see cref="Child2"/>.</value>
        public bool IsSplitterTopLeft
        {
            get { return (bool)GetValue(IsSplitterTopLeftProperty); }
            private set { SetValue(IsSplitterTopLeftPropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets a value indicating the size of <see cref="Child1"/> or <see cref="Child2"/>, depending on the value of <see cref="IsSplitterTopLeft"/>. This is a dependency property.</summary>
        /// <value>If <see cref="IsSplitterTopLeft"/> is <see langword="true"/>, size of <see cref="Child1"/>;
        /// otherwise, size of <see cref="Child2"/>.</value>
        public SplitterDistance SplitterDistance
        {
            get { return (SplitterDistance)GetValue(SplitterDistanceProperty); }
            set { SetValue(SplitterDistanceProperty, value); }
        }

        /// <summary>Gets the left or top child, depending on <see cref="Orientation"/>. This is a dependency property.</summary>
        /// <value>If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Vertical"/>, the top child.
        /// If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Horizontal"/>, the left child.</value>
        public DockPaneNode Child1
        {
            get { return (DockPaneNode)GetValue(Child1Property); }
            private set
            {
                DockPaneNode oldValue = Child1;
                if (oldValue == value)
                    return;

                SetValue(Child1PropertyKey, value);
                if (oldValue != null)
                    oldValue.ClearParent();
                if (value != null)
                    value.SetParent(this);
                InvalidateState();
            }
        }

        /// <summary>Gets the right or bottom child, depending on <see cref="Orientation"/>. This is a dependency property.</summary>
        /// <value>If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Vertical"/>, the bottom child.
        /// If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Horizontal"/>, the right child.</value>
        public DockPaneNode Child2
        {
            get { return (DockPaneNode)GetValue(Child2Property); }
            private set
            {
                DockPaneNode oldValue = Child2;
                if (oldValue == value)
                    return;

                SetValue(Child2PropertyKey, value);
                if (oldValue != null)
                    oldValue.ClearParent();
                if (value != null)
                    value.SetParent(this);
                InvalidateState();
            }
        }

        /// <summary>Gets a value indicates the visibility of the children. This is a dependency property.</summary>
        /// <value>The visibility of the children.</value>
        public SplitChildrenVisibility ChildrenVisibility
        {
            get { return (SplitChildrenVisibility)GetValue(ChildrenVisibilityProperty); }
            private set { SetValue(ChildrenVisibilityPropertyKey, value); }
        }

        internal void ClearChildren()
        {
            Child1 = Child2 = null;
        }

        internal DockPaneNode GetSibling(DockPaneNode node)
        {
            Debug.Assert(Child1 == node || Child2 == node);
            if (Child1 == node)
                return Child2;
            else
                return Child1;
        }

        internal void SetSibling(DockPaneNode node, DockPaneNode newValue)
        {
            Debug.Assert(Child1 == node || Child2 == node);
            Debug.Assert(newValue != null);
            Debug.Assert(newValue.Parent == null);

            if (Child1 == node)
                SetValue(Child2PropertyKey, newValue);
            else
                SetValue(Child1PropertyKey, newValue);
            newValue.SetParent(this);
        }

        private void RefreshChildrenVisibility()
        {
            bool showChild1, showChild2;

            showChild1 = showChild2 = true;
            if (Child1 == null || Child1.DockPosition == DockPosition.Hidden || DockPositionHelper.IsAutoHide(Child1.DockPosition))
                showChild1 = false;

            if (Child2 == null || Child2.DockPosition == DockPosition.Hidden || DockPositionHelper.IsAutoHide(Child2.DockPosition))
                showChild2 = false;

            SplitChildrenVisibility newChildrenVisibility;
            if (showChild1 && showChild2)
                newChildrenVisibility = SplitChildrenVisibility.Both;
            else if (showChild1)
                newChildrenVisibility = SplitChildrenVisibility.Child1Only;
            else if (showChild2)
                newChildrenVisibility = SplitChildrenVisibility.Child2Only;
            else
                newChildrenVisibility = SplitChildrenVisibility.None;

            SplitChildrenVisibility oldChildrenVisibility = ChildrenVisibility;            
            if (oldChildrenVisibility != newChildrenVisibility)
                ChildrenVisibility = newChildrenVisibility;
        }

        internal override void OnDockPositionChanged(DockPosition oldDockPosition, DockPosition newDockPosition)
        {
            if (Parent != null)
                Parent.InvalidateState();
        }

        internal override DockPosition GetDockPosition()
        {
            DockTreePosition? dockTreePosition = DockTreePosition;

            if (ChildrenVisibility == SplitChildrenVisibility.None)
                return DockPosition.Hidden;
            else
                return DockPositionHelper.GetDockPosition(dockTreePosition, false);
        }

        internal override void RefreshStateCore()
        {
            RefreshChildrenVisibility();
            base.RefreshStateCore();
            Debug.Assert(Child1 == null || (Child1 != null && Child1.Parent == this));
            Debug.Assert(Child2 == null || (Child2 != null && Child2.Parent == this));
        }

        internal bool IsParentOf(DockPaneNode paneNode)
        {
            for (DockPaneSplit split = paneNode.Parent; split != null; split = split.Parent)
            {
                if (split == this)
                    return true;
            }
            return false;
        }

        /// <exclude/>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                "DockPaneSplit: ChildrenVisibility={0}, Child1=({1}), Child2=({2})",
                ChildrenVisibility, Child1, Child2);
        }
    }
}
