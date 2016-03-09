using System;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    internal interface IDragSource
    {
        DockControl DockControl { get; }
        Rect GetFloatingWindowPreview(RelativePoint mouseStartPosition);
        bool CanDrop(DockTreePosition dockTreePosition);
        bool CanDrop(DockPane targetPane);
        bool CanDrop(DockItem targetItem);
        void Drop(Dock dock, bool sendToBack);
        void Drop();
        void Drop(Rect floatingWindowBounds);
        void Drop(DockPane targetPane, DockPanePreviewPlacement placement);
        void Drop(DockItem targetItem);
    }
}
