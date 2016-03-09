using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class FloatingWindow
    {
        private struct UpdateBoundsData
        {
            public FloatingWindow FloatingWindow;
            public Rect Value;

            public UpdateBoundsData(FloatingWindow floatingWindow, Rect value)
            {
                FloatingWindow = floatingWindow;
                Value = value;
            }
        }
    }
}
