using System;
using System.Drawing;
using System.Drawing.Imaging;
using TilesetEditor.Models;

namespace TilesetEditor.Actions
{
    /// <summary>
    /// Replace the image content of a single tile (by index) with a provided bitmap.
    /// Stores the previous tile bitmap (only the tile-sized region) for Undo.
    /// Uses CompositingMode.SourceCopy so the destination pixels are fully overwritten.
    /// </summary>
    public class TileImageAction : IAction
    {
        private readonly TileSet _tileSet;
        private readonly int _index;
        private readonly Bitmap _after; // new content to apply (owned clone)
        private Bitmap? _before; // previous content (nullable clone)

        public TileImageAction(TileSet tileSet, int index, Bitmap newBitmap)
        {
            _tileSet = tileSet ?? throw new ArgumentNullException(nameof(tileSet));
            _index = index;
            if (newBitmap == null) throw new ArgumentNullException(nameof(newBitmap));
            _after = new Bitmap(newBitmap); // clone to own it
            _before = null;
        }

        public void Do()
        {
            if (_index < 0 || _index >= _tileSet.Tiles.Count) return;

            // capture before if not captured yet
            if (_before == null)
            {
                _before?.Dispose();
                _before = _tileSet.ExtractTileBitmap(_index);
            }

            ApplyBitmapToTile(_after);
        }

        public void Undo()
        {
            if (_index < 0 || _index >= _tileSet.Tiles.Count) return;
            if (_before != null)
            {
                ApplyBitmapToTile(_before);
            }
        }

        private void ApplyBitmapToTile(Bitmap bmp)
        {
            var rect = _tileSet.Tiles[_index].SourceRect;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Ensure canvas can contain the rect (pads if necessary)
            int colsNeeded = _tileSet.TileWidth > 0 ? Math.Max(_tileSet.Columns, (rect.Right + _tileSet.TileWidth - 1) / Math.Max(1, _tileSet.TileWidth)) : _tileSet.Columns;
            int rowsNeeded = _tileSet.TileHeight > 0 ? Math.Max(_tileSet.Rows, (rect.Bottom + _tileSet.TileHeight - 1) / Math.Max(1, _tileSet.TileHeight)) : _tileSet.Rows;
            _tileSet.EnsureCanvasForGrid(colsNeeded, rowsNeeded);

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

            // Draw bmp into the canvas at rect.X, rect.Y, overwriting destination pixels
            using (var g = Graphics.FromImage(_tileSet.Image))
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.DrawImage(bmp, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
    }
}
