using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockControl
    {
        private struct ResizeDockTreeData
        {
            public Dock Dock;
            public SplitterDistance Value;

            public ResizeDockTreeData(Dock dock, SplitterDistance value)
            {
                Dock = dock;
                Value = value;
            }
        }
    }
}
