using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DevZest.Windows
{
    internal interface IWindow
    {
        Rect Bounds { get; }
        Rect ActualBounds { get; }
        double MinWidth { get; }
        double MaxWidth { get; }
        double MinHeight { get; }
        double MaxHeight { get; }
        void SetBounds(Rect bounds);
        void UpdateLayout();
    }
}
