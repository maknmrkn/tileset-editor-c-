using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TilesetEditor.Actions
{
    public class SwapTileImagesAction : IAction
    {
        private readonly TilesetEditor.Models.TileSet _tileSet;
        private readonly Rectangle _rectA;
        private readonly Rectangle _rectB;
        private readonly Bitmap _bmpA;
        private readonly Bitmap _bmpB;

        public SwapTileImagesAction(TilesetEditor.Models.TileSet tileSet, int indexA, int indexB)
        {
            _tileSet = tileSet ?? throw new ArgumentNullException(nameof(tileSet));
            if (indexA < 0 || indexB < 0 || indexA >= tileSet.Tiles.Count || indexB >= tileSet.Tiles.Count)
                throw new ArgumentOutOfRangeException("Swap indices out of range");

            _rectA = tileSet.Tiles[indexA].SourceRect;
            _rectB = tileSet.Tiles[indexB].SourceRect;

            var ta = tileSet.ExtractTileBitmap(indexA);
            var tb = tileSet.ExtractTileBitmap(indexB);

            _bmpA = ta != null ? new Bitmap(ta) : new Bitmap(Math.Max(1, _rectA.Width), Math.Max(1, _rectA.Height));
            _bmpB = tb != null ? new Bitmap(tb) : new Bitmap(Math.Max(1, _rectB.Width), Math.Max(1, _rectB.Height));
        }

        public void Do()
        {
            Apply(_rectA, _bmpB);
            Apply(_rectB, _bmpA);
        }

        public void Undo()
        {
            Apply(_rectA, _bmpA);
            Apply(_rectB, _bmpB);
        }

        private void Apply(Rectangle destRect, Bitmap bmp)
        {
            if (_tileSet.Image == null) return;
            using var g = Graphics.FromImage(_tileSet.Image);
            var prev = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            g.DrawImage(bmp, destRect);
            g.CompositingMode = prev;
        }
    }
}
