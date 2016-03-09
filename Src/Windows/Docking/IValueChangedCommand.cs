using System;

namespace DevZest.Windows.Docking
{
    internal interface IValueChangedCommand<T> : ICommand
    {
        bool Reset(DockControl dockControl, T oldValue, T newValue);
        bool ShouldRemove(DockControl dockControl, T value);
        bool Merge(DockControl dockControl, T value);
    }
}
