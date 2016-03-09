using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents the action to show source <see cref="DockItem"/> as specified dock position.</summary>
    /// <remarks>This show action corresponds <see cref="DockItem.Show(DockControl, DockPosition, DockItemShowMethod)"/> method.</remarks>
    public sealed class ShowAsDockPositionAction : ShowAction
    {
        private DockPosition _dockPosition = DockPosition.Unknown;

        /// <summary>Initializes a new instance of the <see cref="ShowAsDockPositionAction"/> class.</summary>
        public ShowAsDockPositionAction()
        {
        }

        internal ShowAsDockPositionAction(int source, DockPosition dockPosition, DockItemShowMethod showMethod)
            : base(source, showMethod)
        {
            _dockPosition = dockPosition;
        }

        /// <summary>Gets for sets a value for the specified dock position.</summary>
        /// <value>One of <see cref="DockPosition"/> values. The default value is <see cref="Docking.DockPosition.Unknown"/>.</value>
        [DefaultValue(DockPosition.Unknown)]
        public DockPosition DockPosition
        {
            get { return _dockPosition; }
            set { _dockPosition = value; }
        }

        internal override DockTreePosition? GetDockTreePosition(DockControl dockControl)
        {
            return DockPositionHelper.GetDockTreePosition(_dockPosition);
        }

        internal override bool GetIsAutoHide(DockControl dockControl)
        {
            return DockPositionHelper.IsAutoHide(_dockPosition);
        }

        internal sealed override void Run(DockItem dockItem, DockControl dockControl)
        {
            dockItem.Show(dockControl, DockPosition, ShowMethod);
        }
    }
}
