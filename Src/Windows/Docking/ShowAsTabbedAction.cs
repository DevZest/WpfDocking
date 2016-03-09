using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents the action to show source <see cref="DockItem"/> as tabbed in target <see cref="DockPane"/>.</summary>
    /// <remarks>This show action corresponds <see cref="DockItem.Show(DockPane, int, DockItemShowMethod)"/> method.
    /// The target <see cref="DockPane"/> is determined by the combination of <see cref="ShowInDockTreeAction.Target"/> 
    /// and <see cref="ShowInDockTreeAction.IsFloating"/> properties.</remarks>
    public sealed class ShowAsTabbedAction : ShowInDockTreeAction
    {
        private int _index = -1;

        /// <summary>Initializes a new instance of the <see cref="ShowAsTabbedAction"/> class.</summary>
        public ShowAsTabbedAction()
        {
        }

        internal ShowAsTabbedAction(int source, int target, bool isFloating, int index, DockItemShowMethod showMethod)
            : base(source, target, isFloating, showMethod)
        {
            _index = index;
        }

        /// <summary>Gets or sets the value indicates the position within target <see cref="DockPane.Items">DockPane.Items</see> at which the source <see cref="DockItem"/> is inserted before.</summary>
        /// <value>The zero-based index at which the source <see cref="DockItem"/> is inserted before. -1 adds source <see cref="DockItem"/> to the end. The default value is -1.</value>
        [DefaultValue(-1)]
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        internal override bool GetIsAutoHide(DockControl dockControl)
        {
            return GetTargetPane(dockControl).IsAutoHide;
        }

        internal sealed override void Run(DockItem dockItem, DockPane targetPane)
        {
            dockItem.Show(targetPane, Index, ShowMethod);
        }
    }
}
