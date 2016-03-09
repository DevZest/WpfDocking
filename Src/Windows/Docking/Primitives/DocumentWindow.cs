using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Represents the visual presentation of document <see cref="Docking.DockPane"/> object.</summary>
    /// <remarks>Set the <see cref="Docking.DockPane"/> object instance to <see cref="ContentControl.Content"/> property.</remarks>
    public class DocumentWindow : DockWindow
    {
        static DocumentWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentWindow), new FrameworkPropertyMetadata(typeof(DocumentWindow)));
        }
    }
}
