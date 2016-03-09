using System;
using System.Diagnostics;

namespace DevZest.Windows.Docking
{
    /// <summary>Provides data for <see cref="Docking.DockItem"/> state change events.</summary>
    public abstract class DockItemStateEventArgs : EventArgs
    {
        private DockItem _dockItem;

        internal DockItemStateEventArgs(DockItem dockItem)
        {
            Debug.Assert(dockItem != null);
            _dockItem = dockItem;
        }

        /// <summary>Gets the <see cref="Docking.DockItem"/>.</summary>
        /// <value>The <see cref="Docking.DockItem"/>.</value>
        public DockItem DockItem
        {
            get { return _dockItem; }
        }

        /// <summary>Gets the <see cref="Docking.DockControl"/> associated with the <see cref="Docking.DockItem"/> state change.</summary>
        /// <value>The <see cref="Docking.DockControl"/> associated with the <see cref="Docking.DockItem"/> state change.</value>
        public abstract DockControl DockControl { get; }

        /// <summary>Gets the state change method.</summary>
        /// <value>The state change method.</value>
        public abstract DockItemStateChangeMethod StateChangeMethod { get; }

        /// <summary>Gets the dock position before state change.</summary>
        /// <value>The dock position before state change.</value>
        public abstract DockPosition OldDockPosition { get; }

        /// <summary>Gets the dock tree position before state change.</summary>
        /// <value>The dock tree position before state change.</value>
        public abstract DockTreePosition? OldDockTreePosition { get; }

        /// <summary>Gets the value indicates whether the <see cref="Docking.DockItem"/> is in auto-hide mode before state change.</summary>
        /// <value><see langword="true"/> if <see cref="Docking.DockItem"/> is in auto-hide mode before state change, otherwise <see langword="false"/>.</value>
        public abstract bool OldIsAutoHide { get; }

        /// <summary>Gets the dock position after state change.</summary>
        /// <value>The dock position after state change.</value>
        public abstract DockPosition NewDockPosition { get; }

        /// <summary>Gets the dock tree position after state change.</summary>
        /// <value>The dock tree position after state change.</value>
        public abstract DockTreePosition? NewDockTreePosition { get; }

        /// <summary>Gets the value indicates whether the <see cref="Docking.DockItem"/> is in auto-hide mode after state change.</summary>
        /// <value><see langword="true"/> if <see cref="Docking.DockItem"/> is in auto-hide mode after state change, otherwise <see langword="false"/>.</value>
        public abstract bool NewIsAutoHide { get; }

        /// <summary>Gets the show method for the state change.</summary>
        /// <value>The show method for the state change.</value>
        public abstract Nullable<DockItemShowMethod> ShowMethod { get; }

        /// <summary>Gets the show action for the state change.</summary>
        /// <value>The show action for the state change.</value>
        public abstract ShowAction ShowAction { get; }
    }
}
