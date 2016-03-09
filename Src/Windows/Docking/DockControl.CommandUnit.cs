using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;

namespace DevZest.Windows.Docking
{
    partial class DockControl
    {
        private class CompositeCommand : ICommand
        {
            private ICommand[] _commands;

            public CompositeCommand(List<ICommand> commands)
            {
                Debug.Assert(commands.Count > 0);
                _commands = new ICommand[commands.Count];
                commands.CopyTo(_commands);
            }

            public void Execute(DockControl dockControl)
            {
                for (int i = 0; i < _commands.Length; i++)
                    _commands[i].Execute(dockControl);
            }

            public void UnExecute(DockControl dockControl)
            {
                for (int i = _commands.Length - 1; i >= 0; i--)
                    _commands[i].UnExecute(dockControl);
            }
        }
    }
}
