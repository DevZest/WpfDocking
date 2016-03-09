using System;

namespace DevZest.Windows.Docking
{
    /// <summary>Specifies the position of <see cref="DockTree"/> objects inside <see cref="DockControl"/>.</summary>
    public enum DockControlTreePosition
    {
        /// <summary>The <see cref="DockTree"/> is positioned on the left side of <see cref="DockControl"/>.</summary>
        Left,
        /// <summary>The <see cref="DockTree"/> is positioned on the right side of <see cref="DockControl"/>.</summary>
        Right,
        /// <summary>The <see cref="DockTree"/> is positioned at the top of <see cref="DockControl"/>.</summary>
        Top,
        /// <summary>The <see cref="DockTree"/> is positioned at the bottom of <see cref="DockControl"/>.</summary>
        Bottom,
        /// <summary>The <see cref="DockTree"/> takes all the rest available area of <see cref="DockControl"/>.</summary>
        Document
    }
}
