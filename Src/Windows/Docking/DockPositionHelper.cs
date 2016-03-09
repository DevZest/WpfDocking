using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    internal static class DockPositionHelper
    {
        internal static DockTreePosition? GetDockTreePosition(DockPosition dockPosition)
        {
            if (dockPosition == DockPosition.Unknown || dockPosition == DockPosition.Hidden)
                return null;
            else if (dockPosition == DockPosition.LeftAutoHide || dockPosition == DockPosition.Left)
                return DockTreePosition.Left;
            else if (dockPosition == DockPosition.RightAutoHide || dockPosition == DockPosition.Right)
                return DockTreePosition.Right;
            else if (dockPosition == DockPosition.TopAutoHide || dockPosition == DockPosition.Top)
                return DockTreePosition.Top;
            else if (dockPosition == DockPosition.BottomAutoHide || dockPosition == DockPosition.Bottom)
                return DockTreePosition.Bottom;
            else if (dockPosition == DockPosition.Floating)
                return DockTreePosition.Floating;
            else
            {
                Debug.Assert(dockPosition == DockPosition.Document);
                return DockTreePosition.Document;
            }
        }

        internal static DockTreePosition GetDockTreePosition(Dock dock)
        {
            if (dock == Dock.Left)
                return DockTreePosition.Left;
            else if (dock == Dock.Right)
                return DockTreePosition.Right;
            else if (dock == Dock.Top)
                return DockTreePosition.Top;
            else
            {
                Debug.Assert(dock == Dock.Bottom);
                return DockTreePosition.Bottom;
            }
        }

        internal static DockControlTreePosition GetDockControlTreePosition(DockPosition dockPosition)
        {
            if (dockPosition == DockPosition.LeftAutoHide || dockPosition == DockPosition.Left)
                return DockControlTreePosition.Left;
            else if (dockPosition == DockPosition.RightAutoHide || dockPosition == DockPosition.Right)
                return DockControlTreePosition.Right;
            else if (dockPosition == DockPosition.TopAutoHide || dockPosition == DockPosition.Top)
                return DockControlTreePosition.Top;
            else if (dockPosition == DockPosition.BottomAutoHide || dockPosition == DockPosition.Bottom)
                return DockControlTreePosition.Bottom;
            else
            {
                Debug.Assert(dockPosition == DockPosition.Document);
                return DockControlTreePosition.Document;
            }
        }

        internal static bool IsAutoHide(DockPosition dockPosition)
        {
            return (dockPosition == DockPosition.LeftAutoHide ||
                dockPosition == DockPosition.RightAutoHide ||
                dockPosition == DockPosition.TopAutoHide ||
                dockPosition == DockPosition.BottomAutoHide);
        }

        internal static bool IsVisible(DockPosition dockPosition)
        {
            return dockPosition != DockPosition.Unknown && dockPosition != DockPosition.Hidden;
        }

        internal static DockPosition GetDockPosition(DockTreePosition? dockTreePosition, bool isAutoHide, bool isHidden)
        {
            return isHidden ? DockPosition.Hidden : GetDockPosition(dockTreePosition, isAutoHide);
        }

        internal static DockPosition GetDockPosition(DockTreePosition? dockTreePosition, bool isAutoHide)
        {
            if (dockTreePosition == null)
            {
                Debug.Assert(!isAutoHide);
                return DockPosition.Unknown;    
            }
            else if (dockTreePosition == DockTreePosition.Floating)
            {
                Debug.Assert(!isAutoHide);
                return DockPosition.Floating;
            }
            else if (dockTreePosition == DockTreePosition.Document)
            {
                Debug.Assert(!isAutoHide);
                return DockPosition.Document;
            }
            else if (dockTreePosition == DockTreePosition.Left)
                return isAutoHide ? DockPosition.LeftAutoHide : DockPosition.Left;
            else if (dockTreePosition == DockTreePosition.Right)
                return isAutoHide ? DockPosition.RightAutoHide : DockPosition.Right;
            else if (dockTreePosition == DockTreePosition.Top)
                return isAutoHide ? DockPosition.TopAutoHide : DockPosition.Top;
            else
            {
                Debug.Assert(dockTreePosition == DockTreePosition.Bottom);
                return isAutoHide ? DockPosition.BottomAutoHide : DockPosition.Bottom;
            }
        }

        internal static DockControlTreePosition GetDockControlTreePosition(Dock dock)
        {
            if (dock == Dock.Left)
                return DockControlTreePosition.Left;
            else if (dock == Dock.Right)
                return DockControlTreePosition.Right;
            else if (dock == Dock.Top)
                return DockControlTreePosition.Top;
            else
            {
                Debug.Assert(dock == Dock.Bottom);
                return DockControlTreePosition.Bottom;
            }
        }

        internal static Dock GetDock(DockTreePosition dockTreePosition)
        {
            if (dockTreePosition == DockTreePosition.Left)
                return Dock.Left;
            else if (dockTreePosition == DockTreePosition.Right)
                return Dock.Right;
            else if (dockTreePosition == DockTreePosition.Top)
                return Dock.Top;
            else
            {
                Debug.Assert(dockTreePosition == DockTreePosition.Bottom);
                return Dock.Bottom;
            }
        }

        internal static bool IsValid(DockTreePosition? dockTreePosition, AllowedDockTreePositions allowedDockTreePositions)
        {
            if (dockTreePosition == null)
                return true;
            else if (dockTreePosition == DockTreePosition.Floating)
                return (allowedDockTreePositions & AllowedDockTreePositions.Floating) == AllowedDockTreePositions.Floating;
            else if (dockTreePosition == DockTreePosition.Left)
                return (allowedDockTreePositions & AllowedDockTreePositions.Left) == AllowedDockTreePositions.Left;
            else if (dockTreePosition == DockTreePosition.Right)
                return (allowedDockTreePositions & AllowedDockTreePositions.Right) == AllowedDockTreePositions.Right;
            else if (dockTreePosition == DockTreePosition.Top)
                return (allowedDockTreePositions & AllowedDockTreePositions.Top) == AllowedDockTreePositions.Top;
            else if (dockTreePosition == DockTreePosition.Bottom)
                return (allowedDockTreePositions & AllowedDockTreePositions.Bottom) == AllowedDockTreePositions.Bottom;
            else
            {
                Debug.Assert(dockTreePosition == DockTreePosition.Document);
                return (allowedDockTreePositions & AllowedDockTreePositions.Document) == AllowedDockTreePositions.Document;
            }
        }

        internal static bool IsValid(DockPosition dockPosition, AllowedDockTreePositions allowedDockTreePositions)
        {
            return IsValid(GetDockTreePosition(dockPosition), allowedDockTreePositions);
        }
    }
}
