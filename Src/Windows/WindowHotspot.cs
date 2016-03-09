namespace DevZest.Windows
{
    /// <summary>Specifies values for <see cref="P:DevZest.Windows.WindowControl.Hotspot"/> attached property.</summary>
    public enum WindowHotspot
    {
        /// <summary>The given dependency object is not a hotspot.</summary>
        None,
        /// <summary>The given dependency object is hotspot to move the window.</summary>
        Move,
        /// <summary>The given dependency object is hotspot to resize the top edge of the window.</summary>
        ResizeTop,
        /// <summary>The given dependency object is hotspot to resize the bottom edge of the window.</summary>
        ResizeBottom,
        /// <summary>The given dependency object is hotspot to resize the left edge of the window.</summary>
        ResizeLeft,
        /// <summary>The given dependency object is hotspot to resize the right edge of the window.</summary>
        ResizeRight,
        /// <summary>The given dependency object is hotspot to resize the left top edge of the window.</summary>
        ResizeLeftTop,
        /// <summary>The given dependency object is hotspot to resize the right top edge of the window.</summary>
        ResizeRightTop,
        /// <summary>The given dependency object is hotspot to resize the left bottom edge of the window.</summary>
        ResizeLeftBottom,
        /// <summary>The given dependency object is hotspot to resize the right bottom edge of the window.</summary>
        ResizeRightBottom
    }
}
