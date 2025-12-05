using System;
using System.Drawing;
using System.Drawing.Imaging;
using TilesetEditor.Models;

namespace TilesetEditor.Actions
{
    /// <summary>
    /// Swap the image content of two tiles (by index). Stores before snapshots for Undo.
    /// Uses SourceCopy so swap overwrites pixels cleanly.
    /// </summary>
    public class SwapTileImagesAction : IAction
    {
        private readonly TileSet _tileSet;
        private readonly int _a;
        private readonly int _b;
        private Bitmap? _beforeA;
        private Bitmap? _beforeB;

        public SwapTileImagesAction(TileSet tileSet, int indexA, int indexB)
        {
            _tileSet = tileSet ?? throw new ArgumentNullException(nameof(tileSet));
            _a = indexA;
            _b = indexB;
            _beforeA = null;
            _beforeB = null;
        }

        public void Do()
        {
            if (_a < 0 || _b < 0 || _a >= _tileSet.Tiles.Count || _b >= _tileSet.Tiles.Count) return;

            // capture before snapshots if not captured
            if (_beforeA == null)
            {
                _beforeA?.Dispose();
                _beforeA = _tileSet.ExtractTileBitmap(_a);
            }
            if (_beforeB == null)
            {
                _beforeB?.Dispose();
                _beforeB = _tileSet.ExtractTileBitmap(_b);
            }

            // Apply swap: draw beforeA into B and beforeB into A (overwrite)
            if (_beforeB != null) ApplyBitmapToTile(_a, _beforeB);
            if (_beforeA != null) ApplyBitmapToTile(_b, _beforeA);
        }

        public void Undo()
        {
            if (_a < 0 || _b < 0 || _a >= _tileSet.Tiles.Count || _b >= _tileSet.Tiles.Count) return;

            // restore original snapshots (overwrite)
            if (_beforeA != null) ApplyBitmapToTile(_a, _beforeA);
            if (_beforeB != null) ApplyBitmapToTile(_b, _beforeB);
        }

        private void ApplyBitmapToTile(int index, Bitmap bmp)
        {
            if (index < 0 || index >= _tileSet.Tiles.Count) return;
            var rect = _tileSet.Tiles[index].SourceRect;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Ensure canvas large enough for the grid (tile size based)
            _tileSet.EnsureCanvasForGrid(_tileSet.Columns, _tileSet.Rows);

            // If image is null, create a canvas large enough
            if (_tileSet.Image == null)
            {
                int newW = Math.Max(1, rect.Right);
                int newH = Math.Max(1, rect.Bottom);
                _tileSet.Image = new Bitmap(newW, newH, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(_tileSet.Image)) g.Clear(Color.Transparent);
            }
            else
            {
                // If rect extends beyond image, expand canvas
                int extraW = Math.Max(0, rect.Right - _tileSet.Image.Width);
                int extraH = Math.Max(0, rect.Bottom - _tileSet.Image.Height);
                if (extraW > 0 || extraH > 0) _tileSet.ExpandCanvas(extraW, extraH);
            }

            using (var g = Graphics.FromImage(_tileSet.Image))
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.DrawImage(bmp, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
    }
}
