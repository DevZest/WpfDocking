using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a node in <see cref="Docking.DockTree"/>.</summary>
    public abstract class DockPaneNode : DependencyObject
    {
        private static readonly DependencyPropertyKey DockPositionPropertyKey = DependencyProperty.RegisterReadOnly("DockPosition", typeof(DockPosition), typeof(DockPaneNode),
            new FrameworkPropertyMetadata(DockPosition.Unknown));
        /// <summary>Identifies the <see cref="DockPosition"/> dependency property.</summary>
        public static readonly DependencyProperty DockPositionProperty = DockPositionPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey DockControlPropertyKey = DependencyProperty.RegisterReadOnly("DockControl", typeof(DockControl), typeof(DockPaneNode),
            new FrameworkPropertyMetadata(null));
        /// <summary>Identifies the <see cref="DockControl"/> dependency property.</summary>
        public static readonly DependencyProperty DockControlProperty = DockControlPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey FloatingWindowPropertyKey = DependencyProperty.RegisterReadOnly("FloatingWindow", typeof(FloatingWindow), typeof(DockPaneNode),
            new FrameworkPropertyMetadata(null));
        /// <summary>Identifies the <see cref="FloatingWindow"/> dependency property.</summary>
        public static readonly DependencyProperty FloatingWindowProperty = FloatingWindowPropertyKey.DependencyProperty;

        private object _parent;
        private bool _isStateLocked = true;

        private void SetParentCore(object value)
        {
            if (_parent == value)
                return;

            _parent = value;
            InvalidateState();
        }

        internal void ClearParent()
        {
            SetParentCore(null);
        }

        internal void SetParent(DockTree dockTree)
        {
            Debug.Assert(dockTree != null);
            Debug.Assert(_parent == null);
            SetParentCore(dockTree);
        }

        internal void SetParent(DockPaneSplit split)
        {
            Debug.Assert(split != null);
            Debug.Assert(_parent == null);
            SetParentCore(split);
        }

        /// <summary>Gets the dock position. This is a dependency property.</summary>
        /// <value>The dock position.</value>
        public DockPosition DockPosition
        {
            get { return (DockPosition)GetValue(DockPositionProperty); }
            private set { SetValue(DockPositionPropertyKey, value); }
        }

        /// <summary>Gets the parent <see cref="DockPaneSplit"/> of this <see cref="DockPaneNode"/>.</summary>
        /// <value>The parent <see cref="DockPaneSplit"/> of this <see cref="DockPaneNode"/>.</value>
        public DockPaneSplit Parent
        {
            get { return _parent as DockPaneSplit; }
        }

        internal bool IsFloating
        {
            get { return FloatingWindow != null; }
        }

        /// <summary>Gets the <see cref="Docking.DockControl"/> which this <see cref="DockPaneNode"/> belongs to. This is a dependency property.</summary>
        /// <value>The <see cref="Docking.DockControl"/> which this <see cref="DockPaneNode"/> belongs to.</value>
        public DockControl DockControl
        {
            get { return (DockControl)GetValue(DockControlProperty); }
            private set { SetValue(DockControlPropertyKey, value); }
        }

        /// <summary>Gets the <see cref="Docking.FloatingWindow"/> which this <see cref="DockPaneNode"/> belongs to. This is a dependency property.</summary>
        /// <value>The <see cref="Docking.FloatingWindow"/> which this <see cref="DockPaneNode"/> belongs to.</value>
        public FloatingWindow FloatingWindow
        {
            get { return (FloatingWindow)GetValue(FloatingWindowProperty); }
            private set { SetValue(FloatingWindowPropertyKey, value); }
        }

        internal DockTree DockTree
        {
            get
            {
                DockPaneNode node = this;
                while (node.Parent != null)
                    node = node.Parent;

                return node._parent as DockTree;
            }
        }

        /// <summary>Gets the dock tree position.</summary>
        /// <value>The dock tree position.</value>
        public DockTreePosition? DockTreePosition
        {
            get
            {
                DockTree dockTree = DockTree;
                DockTreePosition? value;
                if (dockTree == null)
                    value = null;
                else
                    value = dockTree.Position;
                return value;
            }
        }

        internal void LockState()
        {
            _isStateLocked = true;
        }

        internal void InvalidateState()
        {
            if (!_isStateLocked)
                RefreshState();
        }

        internal void RefreshState()
        {
            _isStateLocked = false;
            DockTree dockTree = DockTree;
            DockControl = dockTree == null ? null : dockTree.DockControl;
            FloatingWindow = dockTree == null ? null : dockTree.FloatingWindow;
            RefreshStateCore();
        }

        internal virtual void RefreshStateCore()
        {
            RefreshDockPosition();
        }

        private void RefreshDockPosition()
        {
            DockPosition oldDockPosition = DockPosition;
            DockPosition newDockPosition = GetDockPosition();
            if (oldDockPosition != newDockPosition)
            {
                SetValue(DockPositionPropertyKey, newDockPosition);
                DockPaneSplit parent = Parent;
                if (parent != null)
                    parent.InvalidateState();
                OnDockPositionChanged(oldDockPosition, newDockPosition);
            }
        }

        internal virtual void OnDockPositionChanged(DockPosition oldDockPosition, DockPosition newDockPosition)
        {
        }

        internal abstract DockPosition GetDockPosition();
    }
}
