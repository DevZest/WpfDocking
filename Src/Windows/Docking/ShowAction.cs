using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents base class for the action to show a <see cref="DockItem"/>.</summary>
    public abstract class ShowAction : MarkupExtension
    {
        private int _source = -1;
        private DockItemShowMethod _showMethod = DockItemShowMethod.Activate;

        internal ShowAction()
        {
        }

        internal ShowAction(int source, DockItemShowMethod showMethod)
        {
            _source = source;
            _showMethod = showMethod;
        }

        /// <summary>Gets or sets the source <see cref="DockItem"/>.</summary>
        /// <value><see cref="System.Int32"/> that represents the index of source <see cref="DockItem"/> in <see cref="DockControl.DockItems">DockControl.DockItems</see>. The default value is -1, which represents a null <see cref="DockItem"/>.</value>
        /// <remarks>When use with <see cref="DockItem.ShowAction"/>, this property will be
        /// properly set.</remarks>
        [DefaultValue(-1)]
        public int Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>Gets or sets the show method for the action.</summary>
        // <value><see cref="DockItemShowMethod"/> for the action. The default value is <see cref="DockItemShowMethod.Activate"/>.</value>
        [DefaultValue(DockItemShowMethod.Activate)]
        public DockItemShowMethod ShowMethod
        {
            get { return _showMethod; }
            set { _showMethod = value; }
        }

        internal DockPosition GetDockPosition(DockControl dockControl)
        {
            return ShowMethod == DockItemShowMethod.Hide ? DockPosition.Hidden : DockPositionHelper.GetDockPosition(GetDockTreePosition(dockControl), GetIsAutoHide(dockControl));
        }

        internal abstract DockTreePosition? GetDockTreePosition(DockControl dockControl);

        internal abstract bool GetIsAutoHide(DockControl dockControl);

        internal void Run(DockControl dockControl)
        {
            DockItem dockItem = dockControl.DockItems[Source];
            Run(dockItem, dockControl);
        }

        internal abstract void Run(DockItem dockItem, DockControl dockControl);

        /// <exclude />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
