namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Specifies the target drop position of drag and drop.</summary>
    public enum DropPosition
    {
        /// <summary>Not a target drop position.</summary>
        None = 0,
        /// <summary>Drop to the left side.</summary>
        Left,
        /// <summary>Drop to the right side.</summary>
        Right,
        /// <summary>Drop to the top side.</summary>
        Top,
        /// <summary>Drop to the bottom side.</summary>
        Bottom,
        /// <summary>Drop to fill the <see cref="DockPane"/> or document area.</summary>
        Fill,
        /// <summary>Drop to tab.</summary>
        Tab,
        /// <summary>Drop as floating.</summary>
        Floating
    }
}
