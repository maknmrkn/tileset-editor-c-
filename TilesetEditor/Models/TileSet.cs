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
        public int TileWidth { get; set; } = 32;
        public int TileHeight { get; set; } = 32;
        public List<Tile> Tiles { get; } = new();

        // اگر GridCols/GridRows صفر باشند، از اندازهٔ تصویر محاسبه می‌شود
        public int GridCols { get; set; } = 0;
        public int GridRows { get; set; } = 0;

        public int Columns => (GridCols > 0) ? GridCols : (Image != null && TileWidth > 0 ? Image.Width / TileWidth : 0);
        public int Rows => (GridRows > 0) ? GridRows : (Image != null && TileHeight > 0 ? Image.Height / TileHeight : 0);

        public void RebuildTiles()
        {
            Tiles.Clear();
            if (TileWidth <= 0 || TileHeight <= 0) return;

            int cols = Columns;
            int rows = Rows;
            if (cols <= 0 || rows <= 0) return;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    // محاسبهٔ مستطیل منبع بر اساس مختصات در تصویر
                    var srcRect = new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
                    Tiles.Add(new Tile(srcRect));
                }
            }
        }

        public Bitmap? ExtractTileBitmap(int index)
        {
            if (Image == null) return null;
            if (index < 0 || index >= Tiles.Count) return null;

            var rect = Tiles[index].SourceRect;

            // اگر مستطیل کاملاً خارج از تصویر باشد، یک بیتی‌مپ خالی برگردان
            if (rect.X >= Image.Width || rect.Y >= Image.Height) 
            {
                var empty = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(empty)) g.Clear(Color.Transparent);
                return empty;
            }

            // محدود کردن مستطیل به محدودهٔ تصویر (برای لبه‌های ناقص)
            int w = Math.Min(rect.Width, Math.Max(0, Image.Width - rect.X));
            int h = Math.Min(rect.Height, Math.Max(0, Image.Height - rect.Y));
            var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                // ابتدا شفاف کن
                g.Clear(Color.Transparent);
                if (w > 0 && h > 0)
                {
                    var src = new Rectangle(rect.X, rect.Y, w, h);
                    var dest = new Rectangle(0, 0, w, h);
                    g.DrawImage(Image, dest, src, GraphicsUnit.Pixel);
                }
            }
            return bmp;
        }
    }
}
