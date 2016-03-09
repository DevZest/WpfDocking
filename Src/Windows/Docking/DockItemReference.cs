using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents a reference of <see cref="DockItem"/> instance.</summary>
    [ContentProperty("Target")]
    public sealed class DockItemReference
    {
        private static SplitterDistance DefaultAutoHideSize = new SplitterDistance(1d / 3d, SplitterUnitType.Star);
        private SplitterDistance _autoHideSize = DefaultAutoHideSize;
        private object _target;

        /// <summary>Gets or sets the size of <see cref="DockItem"/> when in auto hide mode. This is a dependency property.</summary>
        /// <value>The size of <see cref="DockItem"/> when in auto hide mode.</value>
        public SplitterDistance AutoHideSize
        {
            get { return _autoHideSize; }
            set { _autoHideSize = value; }
        }

        private bool ShouldSerializeAutoHideSize()
        {
            return _autoHideSize != DefaultAutoHideSize;
        }

        /// <summary>Gets or sets the target <see cref="DockItem"/>.</summary>
        /// <value>The target <see cref="DockItem"/>.</value>
        /// <remarks>When saving window layout by calling
        /// <see cref="Docking.DockControl.Save">DockControl.Save</see>, the returned object instance
        /// of <see cref="DockItem.Save">DockItem.Save</see> method
        /// is set as <see cref="DockItemReference.Target"/> property of <see cref="DockItemReference"/>.
        /// This provides the flexibility that any object instance can be saved for the <see cref="DockItem"/>.</remarks>
        public object Target
        {
            get { return _target; }
            set { _target = value; }
        }
    }
}
