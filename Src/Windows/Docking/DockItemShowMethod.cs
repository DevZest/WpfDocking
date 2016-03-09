using System;
using System.ComponentModel;

namespace DevZest.Windows.Docking
{
    /// <summary>Specifies the methods to show a <see cref="DockItem"/>.</summary>
    public enum DockItemShowMethod
    {
        /// <summary>Shows the <see cref="DockItem"/> as activated.</summary>
        Activate,
        /// <summary>Shows the <see cref="DockItem"/> as selected.</summary>
        Select,
        /// <summary>Shows the <see cref="DockItem"/> as visible but not selected.</summary>
        Deselect,
        /// <summary>Shows the <see cref="DockItem"/> as invisible.</summary>
        Hide
    }
}
