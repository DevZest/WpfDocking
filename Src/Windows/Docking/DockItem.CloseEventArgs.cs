using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private class CloseEventArgs : DockItemStateEventArgs
        {
            private DockControl _dockControl;
            private DockTreePosition? _oldDockTreePosition;
            private bool _oldIsAutoHide;
            private DockPosition _oldDockPosition;

            public CloseEventArgs(DockItem dockItem)
                : base(dockItem)
            {
                Debug.Assert(dockItem.DockControl != null);
                _dockControl = dockItem.DockControl;
                _oldDockTreePosition = dockItem.DockTreePosition;
                _oldIsAutoHide = dockItem.IsAutoHide;
                _oldDockPosition = dockItem.DockPosition;
            }

            public override DockItemStateChangeMethod StateChangeMethod
            {
                get { return DockItemStateChangeMethod.Close; }
            }

            public override DockControl DockControl
            {
                get { return _dockControl; }
            }

            public override Nullable<DockItemShowMethod> ShowMethod
            {
                get { return null; }
            }

            public override ShowAction ShowAction
            {
                get { return null; }
            }

            public override DockPosition OldDockPosition
            {
                get { return _oldDockPosition; }
            }

            public override DockTreePosition? OldDockTreePosition
            {
                get { return _oldDockTreePosition; }
            }

            public override bool OldIsAutoHide
            {
                get { return _oldIsAutoHide; }
            }

            public override DockPosition NewDockPosition
            {
                get { return DockPosition.Unknown; }
            }

            public override DockTreePosition? NewDockTreePosition
            {
                get { return null; }
            }

            public override bool NewIsAutoHide
            {
                get { return false; }
            }
        }
    }
}
