using System;
using System.ComponentModel;

namespace DevZest.Windows.Docking
{
    /// <summary>Specifies the dock position.</summary>
    public enum DockPosition
    {
        /// <summary>The dock position is unknown.</summary>
        Unknown,
        /// <summary>Shown as hidden.</summary>
        Hidden,
        /// <summary>Shown on the left of <see cref="DockControl"/>.</summary>
        Left,
        /// <summary>Shown in auto-hide mode, on the left of <see cref="DockControl"/>.</summary>
        LeftAutoHide,
        /// <summary>Shown on the right of <see cref="DockControl"/>.</summary>
        Right,
        /// <summary>Shown in auto-hide mode, on the right of <see cref="DockControl"/>.</summary>
        RightAutoHide,
        /// <summary>Shown at the top of <see cref="DockControl"/>.</summary>
        Top,
        /// <summary>Shown in auto-hide mode, at the top of <see cref="DockControl"/>.</summary>
        TopAutoHide,
        /// <summary>Shown at the bottom of <see cref="DockControl"/>.</summary>
        Bottom,
        /// <summary>Shown in auto-hide mode, at the bottom of <see cref="DockControl"/>.</summary>
        BottomAutoHide,
        /// <summary>Shown as document.</summary>
        Document,
        /// <summary>Shown as floating.</summary>
        Floating
    }
}
