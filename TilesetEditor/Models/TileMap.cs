namespace TilesetEditor.Models
{
    public class MapCell
    {
        public int TileIndex { get; set; } = -1;
        public bool FlipX { get; set; } = false;
        public byte Rotation { get; set; } = 0;
        public MapCell() { }
        public MapCell(int tileIndex, bool flipX = false, byte rotation = 0)
        {
            TileIndex = tileIndex; FlipX = flipX; Rotation = rotation;
        }
    }

    public class TileMap
    {
        public int Cols { get; private set; }
        public int Rows { get; private set; }
        public MapCell[,] Cells { get; private set; }

        public TileMap(int cols, int rows)
        {
            Cols = cols; Rows = rows;
            Cells = new MapCell[cols, rows];
            for (int x = 0; x < cols; x++)
                for (int y = 0; y < rows; y++)
                    Cells[x, y] = new MapCell(-1);
        }

        public void Resize(int newCols, int newRows)
        {
            var newCells = new MapCell[newCols, newRows];
            for (int x = 0; x < newCols; x++)
                for (int y = 0; y < newRows; y++)
                    newCells[x, y] = (x < Cols && y < Rows) ? Cells[x, y] : new MapCell(-1);
            Cols = newCols; Rows = newRows; Cells = newCells;
        }
    }
}
