using TilesetEditor.Models;

namespace TilesetEditor.Actions
{
    public class SetTileAction : IAction
    {
        TileMap map; int x, y; MapCell oldCell, newCell;
        public SetTileAction(TileMap map, int x, int y, MapCell newCell)
        {
            this.map = map; this.x = x; this.y = y; this.newCell = newCell;
            this.oldCell = map.Cells[x, y];
        }
        public void Do() => map.Cells[x, y] = newCell;
        public void Undo() => map.Cells[x, y] = oldCell;
    }
}
