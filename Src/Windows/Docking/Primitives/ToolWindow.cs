using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the visual presentation of tool window <see cref="Docking.DockPane"/> object.</summary>
    /// <remarks>Set the <see cref="Docking.DockPane"/> object instance to <see cref="ContentControl.Content"/> property.</remarks>
    public class ToolWindow : DockWindow
    {
        static ToolWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolWindow), new FrameworkPropertyMetadata(typeof(ToolWindow)));
        }
    }
}
