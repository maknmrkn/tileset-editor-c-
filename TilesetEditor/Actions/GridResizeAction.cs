using System;
using System.Drawing;

namespace TilesetEditor.Actions
{
    /// <summary>
    /// Action that changes the logical grid size (cols/rows) and pads the underlying image canvas if needed.
    /// It captures the image before and after the operation so Undo/Redo restore the image state as well.
    /// </summary>
    public class GridResizeAction : IAction
    {
        private readonly TilesetEditor.Models.TileSet _tileSet;
        private readonly int _oldCols;
        private readonly int _oldRows;
        private readonly int _newCols;
        private readonly int _newRows;

        // snapshots of the image before/after (nullable)
        private Bitmap? _beforeImage;
        private Bitmap? _afterImage;

        public GridResizeAction(TilesetEditor.Models.TileSet tileSet, int oldCols, int oldRows, int newCols, int newRows)
        {
            _tileSet = tileSet ?? throw new ArgumentNullException(nameof(tileSet));
            _oldCols = Math.Max(1, oldCols);
            _oldRows = Math.Max(1, oldRows);
            _newCols = Math.Max(1, newCols);
            _newRows = Math.Max(1, newRows);

            // capture before image snapshot (clone) if exists
            _beforeImage = _tileSet.Image != null ? new Bitmap(_tileSet.Image) : null;
            _afterImage = null;
        }

        public void Do()
        {
            // Apply new counts
            _tileSet.GridCols = _newCols;
            _tileSet.GridRows = _newRows;

            // Ensure canvas is large enough for the new grid (this may modify _tileSet.Image)
            _tileSet.EnsureCanvasForGrid(_newCols, _newRows);

            // Rebuild tiles to reflect new grid
            _tileSet.RebuildTiles();

            // capture after image snapshot (clone) for redo
            _afterImage?.Dispose();
            _afterImage = _tileSet.Image != null ? new Bitmap(_tileSet.Image) : null;
        }

        public void Undo()
        {
            // Restore image snapshot (before)
            ApplyImageSnapshot(_beforeImage);

            // Restore counts
            _tileSet.GridCols = _oldCols;
            _tileSet.GridRows = _oldRows;

            // Rebuild tiles to reflect previous grid
            _tileSet.RebuildTiles();
        }

        private void ApplyImageSnapshot(Bitmap? snapshot)
        {
            // Dispose current image and replace with clone of snapshot (or null)
            _tileSet.Image?.Dispose();
            if (snapshot != null)
            {
                _tileSet.Image = new Bitmap(snapshot);
            }
            else
            {
                _tileSet.Image = null;
            }
        }
    }
}
