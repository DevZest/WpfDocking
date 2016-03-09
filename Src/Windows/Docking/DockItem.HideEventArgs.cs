using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class HideEventArgs : DockItemStateEventArgs
        {
            public HideEventArgs(DockItem dockItem)
                : base(dockItem)
            {
            }

            public override DockControl DockControl
            {
                get { return DockItem.DockControl; }
            }

            public override DockItemStateChangeMethod StateChangeMethod
            {
                get { return DockItemStateChangeMethod.Hide; }
            }

            public override Nullable<DockItemShowMethod> ShowMethod
            {
                get { return DockItemShowMethod.Hide; }
            }

            public override ShowAction ShowAction
            {
                get { return null; }
            }

            public override DockPosition OldDockPosition
            {
                get { return DockPositionHelper.GetDockPosition(OldDockTreePosition, OldIsAutoHide); }
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
                get { return DockPosition.Hidden; }
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
