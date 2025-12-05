using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TilesetEditor.Actions
{
    public class TileImageAction : IAction
    {
        TilesetEditor.Models.TileSet tileSet;
        Rectangle destRect;
        Bitmap before;
        Bitmap after;

        public TileImageAction(TilesetEditor.Models.TileSet tileSet, int index, Bitmap newBitmap)
        {
            this.tileSet = tileSet ?? throw new ArgumentNullException(nameof(tileSet));
            if (index < 0 || index >= tileSet.Tiles.Count) throw new ArgumentOutOfRangeException(nameof(index));

            destRect = tileSet.Tiles[index].SourceRect;
            var b = tileSet.ExtractTileBitmap(index);
            before = b != null ? new Bitmap(b) : new Bitmap(Math.Max(1, destRect.Width), Math.Max(1, destRect.Height));
            after = new Bitmap(newBitmap);
        }

        public void Do() => Apply(after);
        public void Undo() => Apply(before);

        private void Apply(Bitmap bmp)
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
