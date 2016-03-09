using System;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private abstract class ShowActionEventArgs<T> : DockItemStateEventArgs
            where T : ShowAction, new()
        {
            private DockControl _dockControl;
            private DockTreePosition? _oldDockTreePosition;
            private bool _oldIsAutoHide;
            private DockPosition _oldDockPosition;
            private T _showAction;

            protected ShowActionEventArgs(DockItem item, DockControl dockControl, DockItemShowMethod showMethod)
                : base(item)
            {
                _dockControl = dockControl;
                _oldDockTreePosition = DockItem.DockTreePosition;
                _oldIsAutoHide = DockItem.IsAutoHide;
                _oldDockPosition = DockItem.DockPosition;
                _showAction = new T();
                _showAction.Source = DockControl.DockItems.IndexOf(item);
                _showAction.ShowMethod = showMethod;
            }

            protected T StrongTypeShowAction
            {
                get { return _showAction; }
            }

            public sealed override DockControl DockControl
            {
                get { return _dockControl; }
            }

            public sealed override DockPosition OldDockPosition
            {
                get { return _oldDockPosition; }
            }

            public sealed override DockTreePosition? OldDockTreePosition
            {
                get { return _oldDockTreePosition; }
            }

            public sealed override bool OldIsAutoHide
            {
                get { return _oldIsAutoHide; }
            }

            public sealed override DockPosition NewDockPosition
            {
                get { return _showAction.GetDockPosition(DockControl); }
            }

            public sealed override DockTreePosition? NewDockTreePosition
            {
                get { return _showAction.GetDockTreePosition(DockControl); }
            }

            public sealed override bool NewIsAutoHide
            {
                get { return _showAction.GetIsAutoHide(DockControl); }
            }

            public sealed override DockItemShowMethod? ShowMethod
            {
                get { return _showAction.ShowMethod; }
            }

            public sealed override ShowAction ShowAction
            {
                get { return _showAction; }
            }
        }
    }
}
