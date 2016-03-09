using System;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Displays overlay (docking guide and preview) of <see cref="DockPaneControl"/>.</summary>
    /// <remarks><see cref="DockPaneControlOverlay"/> is used inside the control template of <see cref="DockPaneControl"/>.</remarks>
    public class DockPaneControlOverlay : Control
    {
        static DockPaneControlOverlay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockPaneControlOverlay), new FrameworkPropertyMetadata(typeof(DockPaneControlOverlay)));
        }
    }
}
