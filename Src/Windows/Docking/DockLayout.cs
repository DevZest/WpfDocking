using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents the window layout of <see cref="DockControl"/>.</summary>
    /// <remarks><see cref="DockLayout"/> is used to save and load <see cref="DockControl"/>.
    /// Calling <see cref="DockControl.Save">DockControl.Save</see> saves the window layout to a <see cref="DockLayout"/> instance;
    /// calling <see cref="DockControl.Load">DockControl.Load</see> loads the window layout from a <see cref="DockLayout"/> instance. 
    /// The <see cref="DockLayout"/> instance can be persisted to XAML using <see cref="XamlWriter.Save(object)">XamlWriter.Save</see>.</remarks>
    public sealed class DockLayout
    {
        private static SplitterDistance DefaultSplitterDistance = new SplitterDistance(1d / 3d, SplitterUnitType.Star);
        private static DockTreeZOrder DefaultDockTreeZOrder = DockTreeZOrder.Default;
        private SplitterDistance _leftDockTreeWidth = DefaultSplitterDistance;
        private SplitterDistance _rightDockTreeWidth = DefaultSplitterDistance;
        private SplitterDistance _topDockTreeHeight = DefaultSplitterDistance;
        private SplitterDistance _bottomDockTreeHeight = DefaultSplitterDistance;
        private DockTreeZOrder _dockTreeZOrder = DefaultDockTreeZOrder;
        private Collection<DockItemReference> _dockItems = new Collection<DockItemReference>();
        private Collection<ShowAction> _showActions = new Collection<ShowAction>();

        /// <summary>Gets or sets the width of left <see cref="DockTree"/>.</summary>
        /// <value>The width of the left <see cref="DockTree"/>. The default value is "1/3*".</value>
        public SplitterDistance LeftDockTreeWidth
        {
            get { return _leftDockTreeWidth; }
            set { _leftDockTreeWidth = value; }
        }

        private bool ShouldSerializeLeftDockTreeWidth()
        {
            return _leftDockTreeWidth != DefaultSplitterDistance;
        }

        /// <summary>Gets or sets the width of right <see cref="DockTree"/>.</summary>
        /// <value>The width of the right <see cref="DockTree"/>. The default value is "1/3*".</value>
        public SplitterDistance RightDockTreeWidth
        {
            get { return _rightDockTreeWidth; }
            set { _rightDockTreeWidth = value; }
        }

        private bool ShouldSerializeRightDockTreeWidth()
        {
            return _rightDockTreeWidth != DefaultSplitterDistance;
        }

        /// <summary>Gets or sets the height of top <see cref="DockTree"/>.</summary>
        /// <value>The height of the top <see cref="DockTree"/>. The default value is "1/3*".</value>
        public SplitterDistance TopDockTreeHeight
        {
            get { return _topDockTreeHeight; }
            set { _topDockTreeHeight = value; }
        }

        private bool ShouldSerializeTopDockTreeHeight()
        {
            return _topDockTreeHeight != DefaultSplitterDistance;
        }

        /// <summary>Gets or sets the height of bottom <see cref="DockTree"/>.</summary>
        /// <value>The height of the bottom <see cref="DockTree"/>. The default value is "1/3*".</value>
        public SplitterDistance BottomDockTreeHeight
        {
            get { return _bottomDockTreeHeight; }
            set { _bottomDockTreeHeight = value; }
        }

        private bool ShouldSerializeBottomDockTreeHeight()
        {
            return _bottomDockTreeHeight != DefaultSplitterDistance;
        }

        /// <summary>Gets or sets the z-order of <see cref="DockTree"/> objects.</summary>
        /// <value>The z-order of left, right, top and bottom <see cref="DockTree"/> objects. The document
        /// <see cref="DockTree"/> is always on top of the Z-order.</value>
        public DockTreeZOrder DockTreeZOrder
        {
            get { return _dockTreeZOrder; }
            set { _dockTreeZOrder = value; }
        }

        private bool ShouldSerializeDockTreeZOrder()
        {
            return !_dockTreeZOrder.Equals(DefaultDockTreeZOrder);
        }

        /// <summary>Gets a collection of <see cref="DockItemReference"/> objects.</summary>
        /// <value>A collection of <see cref="DockItemReference"/> objects.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<DockItemReference> DockItems
        {
            get { return _dockItems; }
        }

        /// <summary>Gets a collection of <see cref="ShowAction"/> objects.</summary>
        /// <value>A collection of <see cref="ShowAction"/> objects.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<ShowAction> ShowActions
        {
            get { return _showActions; }
        }
    }
}
