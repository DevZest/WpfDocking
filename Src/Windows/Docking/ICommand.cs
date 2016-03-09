using System;

namespace DevZest.Windows.Docking
{
    internal interface ICommand
    {
        void Execute(DockControl dockControl);
        void UnExecute(DockControl dockControl);
    }
}
