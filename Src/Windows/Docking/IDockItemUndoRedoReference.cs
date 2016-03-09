namespace DevZest.Windows.Docking
{
    /// <summary>Provides property to reference a <see cref="DockItem"/> instance for undo/redo.</summary>
    public interface IDockItemUndoRedoReference
    {
        /// <summary>Gets the referenced <see cref="DockItem"/> for undo/redo.</summary>
        /// <value>The referenced <see cref="DockItem"/> for undo/redo.</value>
        DockItem DockItem { get; }
    }
}
