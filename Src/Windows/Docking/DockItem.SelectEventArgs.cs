using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class SelectEventArgs : DockItemStateEventArgs
        {
            private bool _activate;
            private bool _isHidden;

            public SelectEventArgs(DockItem dockItem, bool activate)
                : base(dockItem)
            {
                _activate = activate;
                _isHidden = dockItem.DockPosition == DockPosition.Hidden;
            }

            public override DockControl DockControl
            {
                get { return DockItem.DockControl; }
            }

            public override DockItemStateChangeMethod StateChangeMethod
            {
                get { return DockItemStateChangeMethod.Select; }
            }

            public override Nullable<DockItemShowMethod> ShowMethod
            {
                get { return _activate ? DockItemShowMethod.Activate : DockItemShowMethod.Select; }
            }

            public override ShowAction ShowAction
            {
                get { return null; }
            }

            public override DockPosition OldDockPosition
            {
                get { return DockItem.GetDockPosition(_isHidden); }
            }

            public override DockTreePosition? OldDockTreePosition
            {
                get { return DockItem.DockTreePosition; }
            }

            public override bool OldIsAutoHide
            {
                get { return DockItem.IsAutoHide; }
            }

            public override DockPosition NewDockPosition
            {
                get { return DockItem.GetDockPosition(false); }
            }

            public override DockTreePosition? NewDockTreePosition
            {
                get { return DockItem.DockTreePosition; }
            }

            public override bool NewIsAutoHide
            {
                get { return DockItem.IsAutoHide; }
            }
        }
    }
}
