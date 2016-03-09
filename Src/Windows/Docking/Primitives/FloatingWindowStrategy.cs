using System;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Specifies how the <see cref="FloatingWindow"/> is hosted.</summary>
    public enum FloatingWindowStrategy
    {
        /// <summary>The <see cref="DockManager" /> is not enabled or <see cref="Docking.DockControl"/> is not initialized yet.</summary>
        Unknown,
        /// <summary>The <see cref="FloatingWindow"/> is hosted in a native window.</summary>
        Native,
        /// <summary>The <see cref="FloatingWindow"/> is hosted on the adorner of <see cref="Docking.DockControl"/>.</summary>
        Wpf
    }
}
