using System;

namespace DevZest.Windows.Docking
{
    /// <summary>Specifies the methods to change <see cref="DockItem"/> state.</summary>
    public enum DockItemStateChangeMethod
    {
        /// <summary>Shows the <see cref="DockItem"/> as selected.</summary>
        Select,
        /// <summary>Shows the <see cref="DockItem"/> as visible but not selected.</summary>
        Deselect,
        /// <summary>Hides the <see cref="DockItem"/>.</summary>
        Hide,
        /// <summary>Closes the <see cref="DockItem"/>.</summary>
        Close,
        /// <summary>Toggles the auto hide mode of the <see cref="DockItem"/>.</summary>
        ToggleAutoHide,
        /// <summary>Shows the <see cref="DockItem"/> as specified dock position.</summary>
        ShowAsDockPosition,
        /// <summary>Shows the <see cref="DockItem"/> as floating window.</summary>
        ShowAsFloating,
        /// <summary>Shows the <see cref="DockItem"/> as tabbed.</summary>
        ShowAsTabbed,
        /// <summary>Shows the <see cref="DockItem"/> as <see cref="DockPane"/>, side by side of target <see cref="DockPaneNode"/>.</summary>
        ShowAsSidePane
    }
}
