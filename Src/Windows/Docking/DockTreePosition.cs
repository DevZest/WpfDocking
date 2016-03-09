using System;

namespace DevZest.Windows.Docking
{
    /// <summary>Specifies the dock tree positions.</summary>
    public enum DockTreePosition
    {
        /// <summary>The dock tree is on the left of <see cref="DockControl"/>.</summary>
        Left,
        /// <summary>The dock tree is on the right of <see cref="DockControl"/>.</summary>
        Right,
        /// <summary>The dock tree is at the top of <see cref="DockControl"/>.</summary>
        Top,
        /// <summary>The dock tree is at the bottom of <see cref="DockControl"/>.</summary>
        Bottom,
        /// <summary>The dock tree fills the rest area of <see cref="DockControl"/>.</summary>
        Document,
        /// <summary>The dock tree is inside <see cref="FloatingWindow"/>.</summary>
        Floating
    }
}
