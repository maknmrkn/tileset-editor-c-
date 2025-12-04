namespace TilesetEditor.Actions
{
    public interface IAction
    {
        void Do();
        void Undo();
    }
}
