using System.Collections.Generic;
using TilesetEditor.Actions;

namespace TilesetEditor.Utils
{
    public class UndoManager
    {
        private readonly Stack<IAction> _undoStack = new();
        private readonly Stack<IAction> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// Perform an action and push it onto the undo stack.
        /// Clears redo stack.
        /// </summary>
        public void Do(IAction action)
        {
            if (action == null) return;
            action.Do();
            _undoStack.Push(action);
            _redoStack.Clear();
        }

        /// <summary>
        /// Undo last action.
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count == 0) return;
            var action = _undoStack.Pop();
            action.Undo();
            _redoStack.Push(action);
        }

        /// <summary>
        /// Redo last undone action.
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count == 0) return;
            var action = _redoStack.Pop();
            action.Do();
            _undoStack.Push(action);
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
