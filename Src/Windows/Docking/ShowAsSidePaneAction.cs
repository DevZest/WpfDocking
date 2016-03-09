using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents the action to show source <see cref="DockItem"/> as <see cref="DockPane"/>, side by side of target <see cref="DockPaneNode"/>.</summary>
    /// <remarks>This show action corresponds <see cref="DockItem.Show(DockPaneNode, bool, Dock, SplitterDistance, bool, DockItemShowMethod)"/> method.
    /// The target <see cref="DockPaneNode"/> is determined by the combination of <see cref="ShowInDockTreeAction.Target"/>,
    /// <see cref="ShowInDockTreeAction.IsFloating"/> and <see cref="AncestorLevel"/> properties: <see cref="ShowInDockTreeAction.Target"/> and <see cref="ShowInDockTreeAction.IsFloating"/>
    /// properties determines the leaf <see cref="DockPane"/> node in the <see cref="DockTree"/>, the optional
    /// <see cref="AncestorLevel"/> property determines the extra level(s) to walk up the tree.</remarks>
    public sealed class ShowAsSidePaneAction : ShowInDockTreeAction
    {
        private int _ancestorLevel;
        private bool _isAutoHide;
        private Dock _side;
        private SplitterDistance _size = new SplitterDistance(1.0, SplitterUnitType.Star);
        private bool _isSizeForTarget;

        /// <summary>Initializes a new instance of the <see cref="ShowAsSidePaneAction"/> class.</summary>
        public ShowAsSidePaneAction()
        {
        }

        internal ShowAsSidePaneAction(int source, int target, bool isFloating, int ancestorLevel, bool isAutoHide, Dock side, SplitterDistance size, bool isSizeForTarget, DockItemShowMethod showMethod)
            : base(source, target, isFloating, showMethod)
        {
            _ancestorLevel = ancestorLevel;
            _isAutoHide = isAutoHide;
            _side = side;
            _size = size;
            _isSizeForTarget = isSizeForTarget;
        }

        /// <summary>Gets or sets a value indicates the source <see cref="DockItem"/> shows on which side of target <see cref="DockPaneNode"/>.</summary>
        /// <value>The side of target <see cref="DockPaneNode"/>.</value>
        public Dock Side
        {
            get { return _side; }
            set { _side = value; }
        }

        /// <summary>Gets or sets the size of to be created <see cref="DockPane"/> or target <see cref="DockPaneNode"/>, depending on the
        /// value of <see cref="IsSizeForTarget"/>.</summary>
        /// <value>The size of to be created <see cref="DockPane"/> or target <see cref="DockPaneNode"/>, depending on the
        /// value of <see cref="IsSizeForTarget"/>.</value>
        public SplitterDistance Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>Gets or sets the value indicates whether to be created <see cref="DockPane"/> is auto hide.</summary>
        /// <value><see langword="true"/> if to be created <see cref="DockPane"/> is auto hide, otherwise, <see langword="false"/>. The default value is <see langword="false"/>.</value>
        [DefaultValue(false)]
        public bool IsAutoHide
        {
            get { return _isAutoHide; }
            set { _isAutoHide = value; }
        }

        /// <summary>Gets or sets the value indicates whether <see cref="Size"/> is for to be created <see cref="DockPane"/> or target <see cref="DockPaneNode"/>.</summary>
        /// <value><see langword="true"/> if <see cref="Size"/> is for target <see cref="DockPaneNode"/>, otherwise, <see langword="false"/>. The default value is <see langword="false"/>.</value>
        [DefaultValue(false)]
        public bool IsSizeForTarget
        {
            get { return _isSizeForTarget; }
            set { _isSizeForTarget = value; }
        }

        /// <summary>Gets or sets the level of ancestor to look for target <see cref="DockPaneNode"/>.</summary>
        /// <value>The level of ancestor to look for target <see cref="DockPaneNode"/>. The default value is 0.</value>
        [DefaultValue(0)]
        public int AncestorLevel
        {
            get { return _ancestorLevel; }
            set { _ancestorLevel = value; }
        }

        internal override bool GetIsAutoHide(DockControl dockControl)
        {
            return _isAutoHide;
        }

        internal sealed override void Run(DockItem dockItem, DockPane targetPane)
        {
            DockPaneNode targetPaneNode = targetPane;
            for (int i = 0; i < AncestorLevel; i++)
                targetPaneNode = targetPaneNode.Parent;
            dockItem.Show(targetPaneNode, IsAutoHide, Side, Size, IsSizeForTarget, ShowMethod);
        }
    }
}
