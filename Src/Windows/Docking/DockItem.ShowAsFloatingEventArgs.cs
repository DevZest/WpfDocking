using System;
using System.Diagnostics;
using System.Windows;

namespace DevZest.Windows.Docking
{
    partial class DockItem
    {
        private sealed class ShowAsFloatingEventArgs : ShowActionEventArgs<ShowAsFloatingAction>
        {
            public ShowAsFloatingEventArgs(DockItem dockItem, DockControl dockControl, Rect floatingWindowBounds, DockItemShowMethod showMethod)
                : base(dockItem, dockControl, showMethod)
            {
                ShowAsFloatingAction showAction = StrongTypeShowAction;
                showAction.Left = floatingWindowBounds.Left;
                showAction.Top = floatingWindowBounds.Top;
                showAction.Width = floatingWindowBounds.Width;
                showAction.Height = floatingWindowBounds.Height;
            }

            public override DockItemStateChangeMethod StateChangeMethod
            {
                get { return DockItemStateChangeMethod.ShowAsFloating; }
            }
        }
    }
}
