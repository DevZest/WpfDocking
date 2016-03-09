using System;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Displays the preview overlay.</summary>
    public class PreviewOverlay : Control
    {
        static PreviewOverlay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PreviewOverlay), new FrameworkPropertyMetadata(typeof(PreviewOverlay)));
        }
    }
}
