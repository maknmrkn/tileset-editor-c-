using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TilesetEditor.Actions
{
    public class SwapTileImagesAction : IAction
    {
        TilesetEditor.Models.TileSet tileSet;
        Rectangle aRect, bRect;
        Bitmap aBmp, bBmp;

        public SwapTileImagesAction(TilesetEditor.Models.TileSet tileSet, int a, int b)
        {
            this.tileSet = tileSet ?? throw new ArgumentNullException(nameof(tileSet));
            if (a < 0 || b < 0 || a >= tileSet.Tiles.Count || b >= tileSet.Tiles.Count)
                throw new ArgumentOutOfRangeException("Swap indices out of range");

            aRect = tileSet.Tiles[a].SourceRect;
            bRect = tileSet.Tiles[b].SourceRect;

            var ta = tileSet.ExtractTileBitmap(a);
            var tb = tileSet.ExtractTileBitmap(b);
            aBmp = ta != null ? new Bitmap(ta) : new Bitmap(Math.Max(1, aRect.Width), Math.Max(1, aRect.Height));
            bBmp = tb != null ? new Bitmap(tb) : new Bitmap(Math.Max(1, bRect.Width), Math.Max(1, bRect.Height));
        }

        public void Do()
        {
            Apply(aRect, bBmp);
            Apply(bRect, aBmp);
        }

        public void Undo()
        {
            Apply(aRect, aBmp);
            Apply(bRect, bBmp);
        }

        private void Apply(Rectangle destRect, Bitmap bmp)
        {
            if (tileSet.Image == null) return;
            using var g = Graphics.FromImage(tileSet.Image);
            var prev = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            g.DrawImage(bmp, destRect);
            g.CompositingMode = prev;
        }
    }
}
