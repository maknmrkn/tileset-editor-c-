using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace TilesetEditor.Models
{
    public class Tile
    {
        // SourceRect is the logical source rectangle in the tileset image.
        // It may lie partially or fully outside the current image bounds (for padded areas).
        public Rectangle SourceRect { get; set; }
        public Tile(Rectangle r) => SourceRect = r;
    }

    public class TileSet
    {
        public Bitmap? Image { get; set; }

        // If GridCols/GridRows > 0 then explicit counts are used (custom).
        // Otherwise Columns/Rows are derived from TileWidth/TileHeight (preset).
        public int GridCols { get; set; } = 0;
        public int GridRows { get; set; } = 0;

        // Tile size in source pixels (set by Form1 when using preset or custom tile size).
        // If zero, fallback logic will be used.
        public int TileWidth { get; set; } = 0;
        public int TileHeight { get; set; } = 0;

        public List<Tile> Tiles { get; } = new();

        public int Columns
        {
            get
            {
                if (GridCols > 0) return GridCols;
                if (Image != null && TileWidth > 0) return Math.Max(1, (Image.Width + TileWidth - 1) / TileWidth);
                return (Image != null) ? Math.Max(1, Image.Width / 32) : 0;
            }
        }

        public int Rows
        {
            get
            {
                if (GridRows > 0) return GridRows;
                if (Image != null && TileHeight > 0) return Math.Max(1, (Image.Height + TileHeight - 1) / TileHeight);
                return (Image != null) ? Math.Max(1, Image.Height / 32) : 0;
            }
        }

        /// <summary>
        /// Rebuild Tiles list based on current Image and either TileWidth/TileHeight (preferred) or Columns/Rows.
        /// Tiles may have SourceRect that extend beyond Image bounds (for padded areas).
        /// </summary>
        public void RebuildTiles()
        {
            Tiles.Clear();
            if (Columns <= 0 || Rows <= 0) return;

            if (Image == null)
            {
                for (int r = 0; r < Rows; r++)
                    for (int c = 0; c < Columns; c++)
                        Tiles.Add(new Tile(new Rectangle(0, 0, 0, 0)));
                return;
            }

            // Determine tile size in source pixels
            int cellW = TileWidth > 0 ? TileWidth : (Image.Width / Columns);
            int cellH = TileHeight > 0 ? TileHeight : (Image.Height / Rows);

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    int sx = c * cellW;
                    int sy = r * cellH;
                    int sw = cellW;
                    int sh = cellH;

                    // If the source rectangle is fully outside the image, we still create a rect
                    // so UI can render an empty tile area and user can paste into it.
                    // ExtractTileBitmap will handle partial intersections.
                    Tiles.Add(new Tile(new Rectangle(sx, sy, sw, sh)));
                }
            }
        }

        /// <summary>
        /// Extracts a bitmap for the tile at index. If the tile's SourceRect extends outside the image,
        /// the returned bitmap will be the full tile size with transparent areas where image data is missing.
        /// </summary>
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

                // Source intersection with image bounds
                var imgRect = new Rectangle(0, 0, Image.Width, Image.Height);
                var srcIntersect = Rectangle.Intersect(rect, imgRect);
                if (srcIntersect.Width <= 0 || srcIntersect.Height <= 0) return bmp;

                // Destination point inside bmp where the intersected portion should be drawn
                int destX = srcIntersect.X - rect.X;
                int destY = srcIntersect.Y - rect.Y;

                g.DrawImage(Image,
                    new Rectangle(destX, destY, srcIntersect.Width, srcIntersect.Height),
                    srcIntersect,
                    GraphicsUnit.Pixel);
            }
            return bmp;
        }

        /// <summary>
        /// Expand the underlying image canvas by extraWidth and extraHeight.
        /// The existing image is drawn at (0,0) and new areas are left transparent.
        /// Returns true if expansion occurred.
        /// </summary>
        public bool ExpandCanvas(int extraWidth, int extraHeight)
        {
            if (extraWidth <= 0 && extraHeight <= 0) return false;
            int newW = Math.Max(1, (Image?.Width ?? 0) + extraWidth);
            int newH = Math.Max(1, (Image?.Height ?? 0) + extraHeight);

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

        /// <summary>
        /// Pads canvas to ensure it can contain at least cols * TileWidth and rows * TileHeight.
        /// If TileWidth/TileHeight are zero, no padding is performed.
        /// </summary>
        public void EnsureCanvasForGrid(int cols, int rows)
        {
            if (TileWidth <= 0 || TileHeight <= 0) return;
            int requiredW = cols * TileWidth;
            int requiredH = rows * TileHeight;
            int extraW = Math.Max(0, requiredW - (Image?.Width ?? 0));
            int extraH = Math.Max(0, requiredH - (Image?.Height ?? 0));
            if (extraW > 0 || extraH > 0) ExpandCanvas(extraW, extraH);
        }
    }
}
