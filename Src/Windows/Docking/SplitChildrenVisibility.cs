using System;

namespace DevZest.Windows.Docking
{
    /// <summary>Specifies the children visibility of <see cref="DockPaneSplit"/>.</summary>
    public enum SplitChildrenVisibility
    {
        /// <summary>None of the children is visible.</summary>
        None,
        /// <summary>Only child1 is visible.</summary>
        Child1Only,
        /// <summary>Only child2 is visible.</summary>
        Child2Only,
        /// <summary>Both child1 and child2 are visible.</summary>
        Both
    }
}
