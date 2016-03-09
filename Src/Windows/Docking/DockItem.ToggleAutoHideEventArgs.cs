using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class ToggleAutoHideEventArgs : DockItemStateEventArgs
        {
            private bool _isAutoHide;
            private DockItemShowMethod _showMethod;

            public ToggleAutoHideEventArgs(DockItem dockItem, bool isAutoHide, DockItemShowMethod showMethod)
                : base(dockItem)
            {
                _isAutoHide = isAutoHide;
                _showMethod = showMethod;
            }

            public override DockControl DockControl
            {
                get { return DockItem.DockControl; }
            }

            public override DockItemStateChangeMethod StateChangeMethod
            {
                get { return DockItemStateChangeMethod.ToggleAutoHide; }
            }

            public override Nullable<DockItemShowMethod> ShowMethod
            {
                get { return _showMethod; }
            }

            public override ShowAction ShowAction
            {
                get { return null; }
            }

            public override DockPosition OldDockPosition
            {
                get { return DockPositionHelper.GetDockPosition(DockItem.FirstPane.DockTreePosition, _isAutoHide); }
            }

            public override DockTreePosition? OldDockTreePosition
            {
                get { return DockItem.FirstPane.DockTreePosition; }
            }

            public override bool OldIsAutoHide
            {
                get { return _isAutoHide; }
            }

            public override DockPosition NewDockPosition
            {
                get
                {
                    if (_showMethod == DockItemShowMethod.Hide)
                        return DockPosition.Hidden;
                    else
                        return DockPositionHelper.GetDockPosition(DockItem.FirstPane.DockTreePosition, !_isAutoHide);
                }
            }

            public override DockTreePosition? NewDockTreePosition
            {
                get { return DockItem.FirstPane.DockTreePosition; }
            }

            public override bool NewIsAutoHide
            {
                get { return !_isAutoHide; }
            }
        }
    }
}
