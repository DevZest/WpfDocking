using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents the action to show <see cref="DockItem"/> in target <see cref="DockTree"/>.</summary>
    /// <remarks><see cref="ShowInDockTreeAction"/> has two properties, <see cref="Target"/> and <see cref="IsFloating"/>,
    /// to specify target <see cref="DockTree"/>.</remarks>
    public abstract class ShowInDockTreeAction : ShowAction
    {
        private int _target = -1;
        private bool _isFloating;

        internal ShowInDockTreeAction()
        {
        }

        internal ShowInDockTreeAction(int source, int target, bool isFloating, DockItemShowMethod showMethod)
            : base(source, showMethod)
        {
            _target = target;
            _isFloating = isFloating;
        }

        /// <summary>Gets or sets the target <see cref="DockItem"/>.</summary>
        /// <value><see cref="System.Int32"/> that represents the index of target <see cref="DockItem"/> in <see cref="DockControl.DockItems">DockControl.DockItems</see>. The default value is -1, which represents a null <see cref="DockItem"/>.</value>
        [DefaultValue(-1)]
        public int Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>Gets or sets a value indicates whether target <see cref="DockTree"/> is floating.</summary>
        /// <value><see langword="true"/> if target <see cref="DockTree"/> is floating, otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        [DefaultValue(false)]
        public bool IsFloating
        {
            get { return _isFloating; }
            set { _isFloating = value; }
        }

        internal DockPane GetTargetPane(DockControl dockControl)
        {
            DockItem targetItem = dockControl.DockItems[Target];
            return targetItem.FirstPane.IsFloating == IsFloating ? targetItem.FirstPane : targetItem.SecondPane;
        }

        internal sealed override DockTreePosition? GetDockTreePosition(DockControl dockControl)
        {
            return GetTargetPane(dockControl).DockTreePosition;
        }

        internal sealed override void Run(DockItem dockItem, DockControl dockControl)
        {
            Run(dockItem, GetTargetPane(dockControl));
        }

        internal abstract void Run(DockItem dockItem, DockPane targetPane);
    }
}
