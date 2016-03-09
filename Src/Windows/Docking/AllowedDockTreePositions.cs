using System;
using System.ComponentModel;

namespace DevZest.Windows.Docking
{
    /// <summary>Specifies the allowed dock tree positions for <see cref="DockItem" />.
    /// </summary>
    /// <remarks>This enumeration has a <see cref="System.FlagsAttribute" /> attribute that allows a bitwise combination of its member values.</remarks>
    /// <seealso cref="DockItem.AllowedDockTreePositions">DockItem.AllowedDockTreePositions</seealso>
    [Flags]
    [Serializable]
    public enum AllowedDockTreePositions
    {
        /// <summary>Floating above <see cref="DockControl" /> allowed.</summary>
        Floating = 1,
        /// <summary>Dock to left side of <see cref="DockControl" /> allowed.</summary>
        Left = 2,
        /// <summary>Dock to right side of <see cref="DockControl" /> allowed.</summary>
        Right = 4,
        /// <summary>Docked to top side of <see cref="DockControl" /> allowed.</summary>
        Top = 8,
        /// <summary>Docked to bottom side of <see cref="DockControl" /> allowed.</summary>
        Bottom = 16,
        /// <summary>Docked to center of <see cref="DockControl" /> allowed.</summary>
        Document = 32,
        /// <summary>The combination of Left, Right, Top, Bottom and Floating.</summary>
        ToolWindow = Left | Right | Top | Bottom | Floating,
        /// <summary>The combination of Left, Right, Top, Bottom, Document and Floating.</summary>
        All = Floating | Left | Right | Top | Bottom | Document
    }
}
