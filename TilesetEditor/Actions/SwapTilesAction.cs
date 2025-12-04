using TilesetEditor.Models;

namespace TilesetEditor.Actions
{
    public class SwapTilesAction : IAction
    {
        TileMap map;
        int x1, y1, x2, y2;
        MapCell a, b;
        public SwapTilesAction(TileMap map, int x1, int y1, int x2, int y2)
        {
            this.map = map; this.x1 = x1; this.y1 = y1; this.x2 = x2; this.y2 = y2;
            a = map.Cells[x1, y1];
            b = map.Cells[x2, y2];
        }

        public void Do()
        {
            map.Cells[x1, y1] = b;
            map.Cells[x2, y2] = a;
        }

        public void Undo()
        {
            map.Cells[x1, y1] = a;
            map.Cells[x2, y2] = b;
        }
    }
}
