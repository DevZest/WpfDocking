using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents a resizable container consisting of one <see cref="DockTreeSplit"/> and one <see cref="DockTree"/> children.</summary>
    public sealed class DockTreeSplit : DependencyObject
    {
        private static readonly DependencyPropertyKey DockControlPropertyKey =
            DependencyProperty.RegisterReadOnly("DockControl", typeof(DockControl), typeof(DockTreeSplit),
                new FrameworkPropertyMetadata(null));
        private static readonly DependencyPropertyKey IsVisiblePropertyKey =
            DependencyProperty.RegisterReadOnly("IsVisible", typeof(Boolean), typeof(DockTreeSplit),
                new FrameworkPropertyMetadata(BooleanBoxes.False, new PropertyChangedCallback(OnIsVisibleChanged)));
        private static readonly DependencyPropertyKey PositionPropertyKey =
            DependencyProperty.RegisterReadOnly("Position", typeof(Dock), typeof(DockTreeSplit),
                new FrameworkPropertyMetadata());
        private static readonly DependencyPropertyKey OrientationPropertyKey =
            DependencyProperty.RegisterReadOnly("Orientation", typeof(Orientation), typeof(DockTreeSplit),
                new FrameworkPropertyMetadata(OrientationBoxes.Vertical));
        private static readonly DependencyPropertyKey IsSplitterTopLeftPropertyKey =
            DependencyProperty.RegisterReadOnly("IsSplitterTopLeft", typeof(bool), typeof(DockTreeSplit),
                new FrameworkPropertyMetadata(BooleanBoxes.True));
        private static readonly DependencyPropertyKey Child1PropertyKey =
            DependencyProperty.RegisterReadOnly("Child1", typeof(DependencyObject), typeof(DockTreeSplit),
                new FrameworkPropertyMetadata());
        private static readonly DependencyPropertyKey Child2PropertyKey =
            DependencyProperty.RegisterReadOnly("Child2", typeof(DependencyObject), typeof(DockTreeSplit),
                new FrameworkPropertyMetadata());

        /// <summary>Identifies the <see cref="DockControl"/> dependency property.</summary>
        public static readonly DependencyProperty DockControlProperty = DockControlPropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="IsVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsVisibleProperty = IsVisiblePropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="Position"/> dependency property.</summary>
        public static readonly DependencyProperty PositionProperty = PositionPropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty = OrientationPropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="IsSplitterTopLeft"/> dependency property.</summary>
        public static readonly DependencyProperty IsSplitterTopLeftProperty = IsSplitterTopLeftPropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="SplitterDistance"/> dependency property.</summary>
        public static readonly DependencyProperty SplitterDistanceProperty =
            DependencyProperty.Register("SplitterDistance", typeof(SplitterDistance), typeof(DockTreeSplit),
                new FrameworkPropertyMetadata(SplitterDistance.AutoPixel,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnSplitterDistanceChanged)),
                    new ValidateValueCallback(ValidateSplitterDistance));
        /// <summary>Identifies the <see cref="Child1"/> dependency property.</summary>
        public static readonly DependencyProperty Child1Property = Child1PropertyKey.DependencyProperty;
        /// <summary>Identifies the <see cref="Child2"/> dependency property.</summary>
        public static readonly DependencyProperty Child2Property = Child2PropertyKey.DependencyProperty;

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockTreeSplit)d).OnIsVisibleChanged();
        }

        private static void OnSplitterDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockTreeSplit)d).OnSplitterDistanceChanged((SplitterDistance)e.NewValue);
        }

        private static bool ValidateSplitterDistance(object value)
        {
            SplitterDistance distance = (SplitterDistance)value;
            return !distance.IsStar && !distance.IsAutoStar;
        }

        private DockTreeClient _dockTreeClient;
        private bool _isStateLocked = true;
        private bool _flagUpdateSplitterDistance;

        internal DockTreeSplit(DockTreeClient dockTreeClient, Dock position, DockTreeSplit nextSplit)
        {
            _dockTreeClient = dockTreeClient;
            DockControl = dockTreeClient.DockControl;
            Position = position;

            if (position == Dock.Left)
            {
                Child1 = DockControl.GetDockTree(DockPositionHelper.GetDockControlTreePosition(position));
                if (nextSplit != null)
                    Child2 = nextSplit;
                else
                    Child2 = DockControl.GetDockTree(DockControlTreePosition.Document);
                Orientation = Orientation.Horizontal;
                IsSplitterTopLeft = true;
            }
            else if (position == Dock.Right)
            {
                Child2 = DockControl.GetDockTree(DockPositionHelper.GetDockControlTreePosition(position));
                if (nextSplit != null)
                    Child1 = nextSplit;
                else
                    Child1 = DockControl.GetDockTree(DockControlTreePosition.Document);
                Orientation = Orientation.Horizontal;
                IsSplitterTopLeft = false;
            }
            else if (position == Dock.Top)
            {
                Child1 = DockControl.GetDockTree(DockPositionHelper.GetDockControlTreePosition(position));
                if (nextSplit != null)
                    Child2 = nextSplit;
                else
                    Child2 = DockControl.GetDockTree(DockControlTreePosition.Document);
                Orientation = Orientation.Vertical;
                IsSplitterTopLeft = true;
            }
            else
            {
                Debug.Assert(position == Dock.Bottom);
                Child2 = DockControl.GetDockTree(DockPositionHelper.GetDockControlTreePosition(position));
                if (nextSplit != null)
                    Child1 = nextSplit;
                else
                    Child1 = DockControl.GetDockTree(DockControlTreePosition.Document);
                Orientation = Orientation.Vertical;
                IsSplitterTopLeft = false;
            }

            UpdateSplitterDistance();
        }

        private DockTreeClient DockTreeClient
        {
            get { return _dockTreeClient; }
        }

        /// <summary>Gets the <see cref="Docking.DockControl"/> which this <see cref="DockTreeSplit"/> belongs to. This is a dependency property.</summary>
        /// <value>The <see cref="Docking.DockControl"/> which this <see cref="DockTreeSplit"/> belongs to.</value>
        public DockControl DockControl
        {
            get { return (DockControl)GetValue(DockControlProperty); }
            private set { SetValue(DockControlPropertyKey, value); }
        }

        /// <summary>Gets a value indicates whether this <see cref="DockTreeSplit"/> is visible. This is a dependency property.</summary>
        /// <value><see langword="true"/> if this <see cref="DockTreeSplit"/> is visible, otherwise <see langword="false"/>.</value>
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            private set { SetValue(IsVisiblePropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>Gets the position of the <see cref="DockTreeSplit"/>. This is a dependency property.</summary>
        /// <value>The position of the <see cref="DockTreeSplit"/>.</value>
        public Dock Position
        {
            get { return (Dock)GetValue(PositionProperty); }
            private set { SetValue(PositionPropertyKey, value); }
        }

        internal void Dispose()
        {
            ClearValue(Child1PropertyKey);
            ClearValue(Child2PropertyKey);
            _dockTreeClient = null;
            DockControl = null;
            ClearValue(OrientationPropertyKey);
            ClearValue(IsSplitterTopLeftPropertyKey);
            _flagUpdateSplitterDistance = true;
            ClearValue(SplitterDistanceProperty);
            _flagUpdateSplitterDistance = false;
        }

        internal void UpdateSplitterDistance()
        {
            if (_flagUpdateSplitterDistance)
                return;

            _flagUpdateSplitterDistance = true;
            try
            {
                Dock position = Position;
                SplitterDistance distance = GetSplitterDistance(position);
                SplitterDistance distanceToSet;
                if (distance.IsAutoStar)
                    distanceToSet = SplitterDistance.AutoPixel;
                else if (distance.IsStar)
                {
                    double proportion = distance.Value / (distance.Value + 1);
                    if (Orientation == Orientation.Horizontal)
                        distanceToSet = new SplitterDistance(DockTreeClient.ClientSize.Width * proportion, SplitterUnitType.Pixel);
                    else
                        distanceToSet = new SplitterDistance(DockTreeClient.ClientSize.Height * proportion, SplitterUnitType.Pixel);
                }
                else
                    distanceToSet = distance;

                SplitterDistance = distanceToSet;
            }
            finally
            {
                _flagUpdateSplitterDistance = false;
            }
        }

        private void OnIsVisibleChanged()
        {
            DockTreeSplit parentSplit = DockTreeClient.GetParentSplit(this);
            if (parentSplit != null)
                parentSplit.InvalidateState();
        }

        private void OnSplitterDistanceChanged(SplitterDistance newValue)
        {
            if (_flagUpdateSplitterDistance || DockControl == null)
                return;

            _flagUpdateSplitterDistance = true;
            try
            {
                Dock position = Position;
                SplitterDistance oldValue = GetSplitterDistance(position);
                SplitterDistance valueToSet;

                if (oldValue.IsAutoStar || oldValue.IsStar)
                {
                    if (newValue.IsAuto)
                        valueToSet = SplitterDistance.AutoStar;
                    else
                    {
                        Size clientSize = DockTreeClient.ClientSize;
                        double totalLength = Position == Dock.Left || Position == Dock.Right ? clientSize.Width : clientSize.Height;
                        double starValue = Math.Max(newValue.Value / (totalLength - newValue.Value), 0);
                        valueToSet = new SplitterDistance(starValue, SplitterUnitType.Star);
                    }
                }
                else
                    valueToSet = newValue;

                SetSplitterDistance(position, valueToSet);
            }
            finally
            {
                _flagUpdateSplitterDistance = false;
            }
        }

        private SplitterDistance GetSplitterDistance(Dock dock)
        {
            if (dock == Dock.Left)
                return DockControl.LeftDockTreeWidth;
            else if (dock == Dock.Right)
                return DockControl.RightDockTreeWidth;
            else if (dock == Dock.Top)
                return DockControl.TopDockTreeHeight;
            else
            {
                Debug.Assert(dock == Dock.Bottom);
                return DockControl.BottomDockTreeHeight;
            }
        }

        private void SetSplitterDistance(Dock dock, SplitterDistance value)
        {
            if (dock == Dock.Left)
                DockControl.LeftDockTreeWidth = value;
            else if (dock == Dock.Right)
                DockControl.RightDockTreeWidth = value;
            else if (dock == Dock.Top)
                DockControl.TopDockTreeHeight = value;
            else
            {
                Debug.Assert(dock == Dock.Bottom);
                DockControl.BottomDockTreeHeight = value;
            }
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
        public DependencyObject Child1
        {
            get { return (DependencyObject)GetValue(Child1Property); }
            private set { SetValue(Child1PropertyKey, value); }
        }

        /// <summary>Gets the right or bottom child, depending on <see cref="Orientation"/>. This is a dependency property.</summary>
        /// <value>If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Vertical"/>, the bottom child.
        /// If <see cref="Orientation"/> is <see cref="System.Windows.Controls.Orientation.Horizontal"/>, the right child.</value>
        public DependencyObject Child2
        {
            get { return (DependencyObject)GetValue(Child2Property); }
            private set { SetValue(Child2PropertyKey, value); }
        }

        internal void InvalidateState()
        {
            if (!_isStateLocked)
                RefreshState();
        }

        internal void RefreshState()
        {
            _isStateLocked = false;
            IsVisible = IsSplitterTopLeft ? GetIsVisible(Child1) : GetIsVisible(Child2);
        }

        private static bool GetIsVisible(DependencyObject child)
        {
            DockTree dockTree = child as DockTree;
            if (dockTree != null)
                return dockTree.IsVisible;

            DockTreeSplit dockTreeSplit = (DockTreeSplit)child;
            return dockTreeSplit.IsVisible;
        }

        /// <exclude/>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                "DockTreeSplit Position={0}, IsVisible={1}, Child1=({2}), Child2=({3})",
                Position, IsVisible, Child1, Child2);
        }
    }
}
