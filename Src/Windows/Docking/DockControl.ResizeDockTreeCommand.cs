using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockControl
    {
        private abstract class ResizeDockTreeCommandBase : IValueChangedCommand<ResizeDockTreeData>
        {
            private SplitterDistance _oldValue;
            private SplitterDistance _newValue;

            protected ResizeDockTreeCommandBase(SplitterDistance oldValue, SplitterDistance newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            protected SplitterDistance OldValue
            {
                get { return _oldValue; }
            }

            protected SplitterDistance NewValue
            {
                get { return _newValue; }
            }

            protected abstract Dock Dock { get; }

            public bool Reset(DockControl dockControl, ResizeDockTreeData oldValue, ResizeDockTreeData newValue)
            {
                Debug.Assert(oldValue.Dock == newValue.Dock);
                if (oldValue.Dock != Dock)
                    return false;

                _oldValue = oldValue.Value;
                _newValue = newValue.Value;
                return true;
            }

            public bool ShouldRemove(DockControl dockControl, ResizeDockTreeData newValue)
            {
                return (newValue.Dock == Dock && newValue.Value == OldValue);
            }

            public bool Merge(DockControl dockControl, ResizeDockTreeData newValue)
            {
                if (newValue.Dock != Dock)
                    return false;
                _newValue = newValue.Value;
                return true;
            }

            public abstract void Execute(DockControl dockControl);
            public abstract void UnExecute(DockControl dockControl);
        }

        private sealed class ResizeLeftDockTreeCommand : ResizeDockTreeCommandBase
        {
            public ResizeLeftDockTreeCommand(SplitterDistance oldValue, SplitterDistance newValue)
                : base(oldValue, newValue)
            {
            }

            protected sealed override Dock Dock
            {
                get { return Dock.Left; }
            }

            public sealed override void Execute(DockControl dockControl)
            {
                dockControl.LeftDockTreeWidth = NewValue;
            }

            public sealed override void UnExecute(DockControl dockControl)
            {
                dockControl.LeftDockTreeWidth = OldValue;
            }
        }

        private sealed class ResizeRightDockTreeCommand : ResizeDockTreeCommandBase
        {
            public ResizeRightDockTreeCommand(SplitterDistance oldValue, SplitterDistance newValue)
                : base(oldValue, newValue)
            {
            }

            protected sealed override Dock Dock
            {
                get { return Dock.Right; }
            }

            public sealed override void Execute(DockControl dockControl)
            {
                dockControl.RightDockTreeWidth = NewValue;
            }

            public sealed override void UnExecute(DockControl dockControl)
            {
                dockControl.RightDockTreeWidth = OldValue;
            }
        }

        private sealed class ResizeTopDockTreeCommand : ResizeDockTreeCommandBase
        {
            public ResizeTopDockTreeCommand(SplitterDistance oldValue, SplitterDistance newValue)
                : base(oldValue, newValue)
            {
            }

            protected sealed override Dock Dock
            {
                get { return Dock.Top; }
            }

            public sealed override void Execute(DockControl dockControl)
            {
                dockControl.TopDockTreeHeight = NewValue;
            }

            public sealed override void UnExecute(DockControl dockControl)
            {
                dockControl.TopDockTreeHeight = OldValue;
            }
        }

        private sealed class ResizeBottomDockTreeCommand : ResizeDockTreeCommandBase
        {
            public ResizeBottomDockTreeCommand(SplitterDistance oldValue, SplitterDistance newValue)
                : base(oldValue, newValue)
            {
            }

            protected sealed override Dock Dock
            {
                get { return Dock.Bottom; }
            }

            public sealed override void Execute(DockControl dockControl)
            {
                dockControl.BottomDockTreeHeight = NewValue;
            }

            public sealed override void UnExecute(DockControl dockControl)
            {
                dockControl.BottomDockTreeHeight = OldValue;
            }
        }
    }
}
