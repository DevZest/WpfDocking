using System;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Displays <see cref="DockPane"/>.</summary>
    /// <remarks><see cref="DockPaneControl"/> is used inside data template of <see cref="DockPane"/>.</remarks>
    public class DockPaneControl : Control
    {
        static DockPaneControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockPaneControl), new FrameworkPropertyMetadata(typeof(DockPaneControl)));
        }
    }
}
