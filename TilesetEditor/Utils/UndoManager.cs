using System.Collections.Generic;
using TilesetEditor.Actions;

namespace TilesetEditor.Utils
{
    public class UndoManager
    {
        Stack<IAction> undo = new Stack<IAction>();
        Stack<IAction> redo = new Stack<IAction>();

        public void Do(IAction action)
        {
            action.Do();
            undo.Push(action);
            redo.Clear();
        }

        public void Undo()
        {
            if (undo.Count == 0) return;
            var a = undo.Pop();
            a.Undo();
            redo.Push(a);
        }

        public void Redo()
        {
            if (redo.Count == 0) return;
            var a = redo.Pop();
            a.Do();
            undo.Push(a);
        }

        public void Clear()
        {
            undo.Clear(); redo.Clear();
        }

        public int UndoCount => undo.Count;
        public int RedoCount => redo.Count;
    }
}
