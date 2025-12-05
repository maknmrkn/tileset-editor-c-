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
        public Bitmap? Image { get; set; }

        // Grid definition: number of columns and rows (grid size)
        // If zero, derived from image (default fallback)
        public int GridCols { get; set; } = 0;
        public int GridRows { get; set; } = 0;

        public List<Tile> Tiles { get; } = new();

        public int Columns => (GridCols > 0) ? GridCols : (Image != null ? Math.Max(1, Image.Width / 32) : 0);
        public int Rows => (GridRows > 0) ? GridRows : (Image != null ? Math.Max(1, Image.Height / 32) : 0);

        public void RebuildTiles()
        {
            Tiles.Clear();
            if (Columns <= 0 || Rows <= 0) return;

            if (Image == null)
            {
                // create empty tiles (all empty)
                for (int r = 0; r < Rows; r++)
                    for (int c = 0; c < Columns; c++)
                        Tiles.Add(new Tile(new Rectangle(0, 0, 0, 0)));
                return;
            }

            // compute floating cell size so we can map grid to image area
            float cellW = (float)Image.Width / Columns;
            float cellH = (float)Image.Height / Rows;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    int sx = (int)Math.Round(c * cellW);
                    int sy = (int)Math.Round(r * cellH);
                    int sw = (int)Math.Ceiling((c + 1) * cellW) - sx;
                    int sh = (int)Math.Ceiling((r + 1) * cellH) - sy;

                    // If the computed source rect is outside image bounds, mark as empty rect
                    if (sx >= Image.Width || sy >= Image.Height || sw <= 0 || sh <= 0)
                    {
                        Tiles.Add(new Tile(new Rectangle(0, 0, 0, 0)));
                    }
                    else
                    {
                        // clamp width/height to image bounds
                        sw = Math.Min(sw, Image.Width - sx);
                        sh = Math.Min(sh, Image.Height - sy);
                        Tiles.Add(new Tile(new Rectangle(sx, sy, sw, sh)));
                    }
                }
            }
        }

        public Bitmap ExtractTileBitmap(int index)
        {
            if (index < 0 || index >= Tiles.Count) return new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            var rect = Tiles[index].SourceRect;

            // If no image or rect empty -> return transparent bitmap sized to logical cell (use 1x1 fallback)
            if (Image == null || rect.Width <= 0 || rect.Height <= 0)
            {
                var empty = new Bitmap(Math.Max(1, rect.Width), Math.Max(1, rect.Height), PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(empty)) g.Clear(Color.Transparent);
                return empty;
            }

            var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(Image, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            }
            return bmp;
        }
    }
}
