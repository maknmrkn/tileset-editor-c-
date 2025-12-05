using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace TilesetEditor.Models
{
    public class Tile
    {
        public Rectangle SourceRect { get; set; }
        public Tile(Rectangle r) => SourceRect = r;
    }

    public class TileSet
    {
        // Image can be null when no tileset is loaded
        public Bitmap? Image { get; set; }

        // If GridCols/GridRows > 0 then explicit counts are used (custom).
        public int GridCols { get; set; } = 0;
        public int GridRows { get; set; } = 0;

        // Tile size in source pixels (set by Form1 when using preset or custom tile size).
        public int TileWidth { get; set; } = 0;
        public int TileHeight { get; set; } = 0;

        public List<Tile> Tiles { get; } = new();

        public int Columns
        {
            get
            {
                if (GridCols > 0) return GridCols;
                if (Image != null && TileWidth > 0) return Math.Max(1, (Image.Width + TileWidth - 1) / TileWidth);
                return (Image != null) ? Math.Max(1, Image.Width / 32) : 1;
            }
        }

        public int Rows
        {
            get
            {
                if (GridRows > 0) return GridRows;
                if (Image != null && TileHeight > 0) return Math.Max(1, (Image.Height + TileHeight - 1) / TileHeight);
                return (Image != null) ? Math.Max(1, Image.Height / 32) : 1;
            }
        }

        public void RebuildTiles()
        {
            Tiles.Clear();
            int cols = Math.Max(1, Columns);
            int rows = Math.Max(1, Rows);

            // If tile size not set but image exists, derive a reasonable tile size
            if ((TileWidth <= 0 || TileHeight <= 0) && Image != null)
            {
                if (TileWidth <= 0) TileWidth = Math.Max(1, Image.Width / cols);
                if (TileHeight <= 0) TileHeight = Math.Max(1, Image.Height / rows);
            }

            // Ensure tile size is at least 1
            int cellW = Math.Max(1, TileWidth > 0 ? TileWidth : 32);
            int cellH = Math.Max(1, TileHeight > 0 ? TileHeight : 32);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int sx = c * cellW;
                    int sy = r * cellH;
                    Tiles.Add(new Tile(new Rectangle(sx, sy, cellW, cellH)));
                }
            }
        }

        public Bitmap ExtractTileBitmap(int index)
        {
            if (index < 0 || index >= Tiles.Count) return new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            var rect = Tiles[index].SourceRect;

            int w = Math.Max(1, rect.Width);
            int h = Math.Max(1, rect.Height);
            var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                if (Image == null) return bmp;

                var imgRect = new Rectangle(0, 0, Image.Width, Image.Height);
                var srcIntersect = Rectangle.Intersect(rect, imgRect);
                if (srcIntersect.Width <= 0 || srcIntersect.Height <= 0) return bmp;

                int destX = srcIntersect.X - rect.X;
                int destY = srcIntersect.Y - rect.Y;

                g.DrawImage(Image,
                    new Rectangle(destX, destY, srcIntersect.Width, srcIntersect.Height),
                    srcIntersect,
                    GraphicsUnit.Pixel);
            }
            return bmp;
        }

        public bool ExpandCanvas(int extraWidth, int extraHeight)
        {
            if (extraWidth <= 0 && extraHeight <= 0) return false;
            int currentW = Image?.Width ?? 0;
            int currentH = Image?.Height ?? 0;
            int newW = Math.Max(1, currentW + extraWidth);
            int newH = Math.Max(1, currentH + extraHeight);

            var newBmp = new Bitmap(newW, newH, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(newBmp))
            {
                g.Clear(Color.Transparent);
                if (Image != null)
                {
                    g.DrawImage(Image, 0, 0, Image.Width, Image.Height);
                    Image.Dispose();
                }
            }
            Image = newBmp;
            return true;
        }

        public void EnsureCanvasForGrid(int cols, int rows)
        {
            if (TileWidth <= 0 || TileHeight <= 0) return;
            int requiredW = cols * TileWidth;
            int requiredH = rows * TileHeight;
            int currentW = Image?.Width ?? 0;
            int currentH = Image?.Height ?? 0;
            int extraW = Math.Max(0, requiredW - currentW);
            int extraH = Math.Max(0, requiredH - currentH);
            if (extraW > 0 || extraH > 0) ExpandCanvas(extraW, extraH);
        }
    }
}
