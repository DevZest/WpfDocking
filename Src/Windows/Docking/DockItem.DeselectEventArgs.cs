using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class DeselectEventArgs : DockItemStateEventArgs
        {
            private bool _isHidden;

            public DeselectEventArgs(DockItem dockItem)
                : base(dockItem)
            {
                Debug.Assert(dockItem.DockPosition != DockPosition.Unknown);
                _isHidden = dockItem.IsHidden;
            }

            public override DockControl DockControl
            {
                get { return DockItem.DockControl; }
            }

            public override DockItemStateChangeMethod StateChangeMethod
            {
                get { return DockItemStateChangeMethod.Deselect; }
            }

            public override Nullable<DockItemShowMethod> ShowMethod
            {
                get { return DockItemShowMethod.Deselect; }
            }

            public override ShowAction ShowAction
            {
                get { return null; }
            }

            public override DockPosition OldDockPosition
            {
                get { return DockPositionHelper.GetDockPosition(OldDockTreePosition, OldIsAutoHide, _isHidden); }
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
                get { return DockPositionHelper.GetDockPosition(NewDockTreePosition, NewIsAutoHide, false); }
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
