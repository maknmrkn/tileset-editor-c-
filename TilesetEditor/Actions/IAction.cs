namespace TilesetEditor.Actions
{
    /// <summary>
    /// Minimal action interface for Undo/Redo.
    /// Implementations must make Do() apply the change and Undo() revert it.
    /// UndoManager will call Do() when performing the action and call Undo() to revert.
    /// </summary>
    public interface IAction
    {
        void Do();
        void Undo();
    }
}
