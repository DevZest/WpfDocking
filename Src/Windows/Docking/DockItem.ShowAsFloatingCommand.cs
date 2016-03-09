using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private class ShowAsFloatingCommand : ShowCommandBase
        {
            public static void Execute(DockItem dockItem, DockControl dockControl, Rect floatingWindowBounds, DockItemShowMethod showMethod)
            {
                Debug.Assert(dockControl != null);
                if (TestUndo(dockItem, dockControl))
                    dockControl.ExecuteCommand(new ShowAsFloatingCommand(dockItem, floatingWindowBounds, showMethod));
                else
                    dockItem.DoShowAsFloating(dockControl, floatingWindowBounds, showMethod);
            }

            private Rect _floatingWindowBounds;

            private ShowAsFloatingCommand(DockItem dockItem, Rect floatingWindowBounds, DockItemShowMethod showMethod)
                : base(dockItem, showMethod)
            {
                _floatingWindowBounds = floatingWindowBounds;
            }

            protected override void ExecuteOverride(DockControl dockControl)
            {
                DockItem dockItem = GetDockItem(dockControl);
                dockItem.DoShowAsFloating(dockControl, _floatingWindowBounds, ShowMethod);
                if (double.IsNaN(_floatingWindowBounds.X))
                    _floatingWindowBounds.X = dockItem.FirstPane.FloatingWindow.Left;
                if (double.IsNaN(_floatingWindowBounds.Y))
                    _floatingWindowBounds.Y = dockItem.FirstPane.FloatingWindow.Top;
            }
        }
    }
}
