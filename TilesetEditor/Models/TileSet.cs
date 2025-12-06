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

    public class TileSet : IDisposable
    {
        // ----- Composite cache fields -----
        private Bitmap? _compositeBitmap = null;
        private bool _compositeDirty = true;
        private int _lastCompositeCols = 0;
        private int _lastCompositeRows = 0;
        private int _lastTileWidth = 0;
        private int _lastTileHeight = 0;

        // ----- Main image and grid properties -----
        public Bitmap? Image { get; set; }
        public int GridCols { get; set; } = 0;
        public int GridRows { get; set; } = 0;
        public int TileWidth { get; set; } = 0;
        public int TileHeight { get; set; } = 0;
        public List<Tile> Tiles { get; } = new();

        // ----- Tile render cache -----
        private Dictionary<int, Bitmap> _tileCache = new();
        private bool _cacheValid = false;

        // ----- HELPER: بررسی اعتبار بیت‌مپ -----
        private bool IsBitmapValid(Bitmap? bmp)
        {
            if (bmp == null) return false;
            try
            {
                var width = bmp.Width;
                var height = bmp.Height;
                return width > 0 && height > 0;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// دریافت یک کپی از bitmap تایل (برای استفاده در Actionها)
        /// </summary>
        public Bitmap GetTileBitmapCopy(int index)
        {
            if (index < 0 || index >= Tiles.Count)
                return new Bitmap(1, 1, PixelFormat.Format32bppArgb);

            try
            {
                Bitmap cached = GetCachedTileBitmap(index);
                return new Bitmap(cached); // بازگشت یک کپی مستقل
            }
            catch
            {
                return new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            }
        }

        public int Columns
        {
            get
            {
                if (GridCols > 0) return GridCols;
                if (Image != null && TileWidth > 0)
                    return Math.Max(1, (Image.Width + TileWidth - 1) / TileWidth);
                return (Image != null) ? Math.Max(1, Image.Width / 32) : 1;
            }
        }

        public int Rows
        {
            get
            {
                if (GridRows > 0) return GridRows;
                if (Image != null && TileHeight > 0)
                    return Math.Max(1, (Image.Height + TileHeight - 1) / TileHeight);
                return (Image != null) ? Math.Max(1, Image.Height / 32) : 1;
            }
        }

        /// <summary>
        /// بازسازی لیست تایل‌ها بر اساس grid/تصویر فعلی
        /// </summary>
        public void RebuildTiles()
        {
            Tiles.Clear();
            InvalidateCache();
            InvalidateComposite();

            int cols = Math.Max(1, Columns);
            int rows = Math.Max(1, Rows);

            // تنظیم اندازه tile اگر تنظیم نشده
            if ((TileWidth <= 0 || TileHeight <= 0) && Image != null)
            {
                if (TileWidth <= 0) TileWidth = Math.Max(1, Image.Width / cols);
                if (TileHeight <= 0) TileHeight = Math.Max(1, Image.Height / rows);
            }

            int cellW = Math.Max(1, TileWidth > 0 ? TileWidth : 32);
            int cellH = Math.Max(1, TileHeight > 0 ? TileHeight : 32);

            _lastTileWidth = cellW;
            _lastTileHeight = cellH;

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

        /// <summary>
        /// برگرداندن bitmap کش‌شده‌ی تایل (در صورت وجود) یا استخراج و کش کردن آن
        /// </summary>
        public Bitmap GetCachedTileBitmap(int index)
        {
            if (index < 0 || index >= Tiles.Count)
                return new Bitmap(1, 1, PixelFormat.Format32bppArgb);

            // اگر کش معتبر است و تایل در کش موجود است، بررسی و بازگشت کن
            if (_cacheValid && _tileCache.ContainsKey(index))
            {
                var cached = _tileCache[index];
                if (IsBitmapValid(cached))
                    return cached;
                else
                    _tileCache.Remove(index); // حذف ورودی نامعتبر
            }

            // استخراج و ذخیره در کش
            var bitmap = ExtractTileBitmap(index);
            _tileCache[index] = bitmap;
            return bitmap;
        }

        /// <summary>
        /// استخراج tile (بدون کش)
        /// </summary>
        private Bitmap ExtractTileBitmap(int index)
        {
            if (index < 0 || index >= Tiles.Count)
                return new Bitmap(1, 1, PixelFormat.Format32bppArgb);

            var rect = Tiles[index].SourceRect;
            int w = Math.Max(1, rect.Width);
            int h = Math.Max(1, rect.Height);

            var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                if (Image == null || !IsBitmapValid(Image))
                    return bmp;

                var imgRect = new Rectangle(0, 0, Image.Width, Image.Height);
                var srcIntersect = Rectangle.Intersect(rect, imgRect);
                if (srcIntersect.Width <= 0 || srcIntersect.Height <= 0)
                    return bmp;

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
        /// باطل کردن کش (وقتی tile یا تصویر تغییر کند)
        /// </summary>
        public void InvalidateCache()
        {
            foreach (var kvp in _tileCache)
                kvp.Value.Dispose();
            _tileCache.Clear();
            _cacheValid = false;
        }

        /// <summary>
        /// علامت‌گذاری کش به عنوان معتبر
        /// </summary>
        public void ValidateCache()
        {
            _cacheValid = true;
        }

        /// <summary>
        /// ساخت کامل کش برای همه تایل‌ها (بارگذاری اولیه)
        /// </summary>
        public void BuildCompleteCache()
        {
            InvalidateCache();
            for (int i = 0; i < Tiles.Count; i++)
            {
                _tileCache[i] = ExtractTileBitmap(i);
            }
            _cacheValid = true;
        }

        /// <summary>
        /// گسترش بوم (canvas) و انتقال تصویر قدیمی به بوم جدید
        /// </summary>
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

            InvalidateCache();
            InvalidateComposite();
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

            if (extraW > 0 || extraH > 0)
                ExpandCanvas(extraW, extraH);
        }

        /// <summary>
        /// ایجاد/بازگرداندن bitmap ترکیبی (composite) از همه تایل‌ها
        /// </summary>
        public Bitmap GetCompositeBitmap()
        {
            if (!_compositeDirty && _compositeBitmap != null &&
                _lastCompositeCols == Columns && _lastCompositeRows == Rows &&
                _lastTileWidth == TileWidth && _lastTileHeight == TileHeight)
            {
                return _compositeBitmap;
            }

            _compositeBitmap?.Dispose();

            int cols = Math.Max(1, Columns);
            int rows = Math.Max(1, Rows);
            int cellW = Math.Max(1, TileWidth > 0 ? TileWidth : 32);
            int cellH = Math.Max(1, TileHeight > 0 ? TileHeight : 32);

            int totalWidth = cols * cellW;
            int totalHeight = rows * cellH;

            _compositeBitmap = new Bitmap(totalWidth, totalHeight, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(_compositeBitmap))
            {
                g.Clear(Color.Transparent);

                for (int i = 0; i < Tiles.Count; i++)
                {
                    var rect = Tiles[i].SourceRect;

                    if (Image == null || rect.X >= Image.Width || rect.Y >= Image.Height)
                        continue;

                    int drawWidth = Math.Min(rect.Width, Image.Width - rect.X);
                    int drawHeight = Math.Min(rect.Height, Image.Height - rect.Y);

                    if (drawWidth <= 0 || drawHeight <= 0)
                        continue;

                    g.DrawImage(Image,
                        new Rectangle(rect.X, rect.Y, drawWidth, drawHeight),
                        new Rectangle(rect.X, rect.Y, drawWidth, drawHeight),
                        GraphicsUnit.Pixel);
                }
            }

            _lastCompositeCols = cols;
            _lastCompositeRows = rows;
            _lastTileWidth = cellW;
            _lastTileHeight = cellH;
            _compositeDirty = false;

            return _compositeBitmap;
        }

        /// <summary>
        /// علامت‌گذاری composite به عنوان منقضی (بعد از تغییر)
        /// </summary>
        public void InvalidateComposite()
        {
            _compositeDirty = true;
        }

        /// <summary>
        /// آزاد کردن منابع مرتبط با این TileSet
        /// </summary>
        public void Dispose()
        {
            // آزاد کردن composite bitmap
            _compositeBitmap?.Dispose();
            _compositeBitmap = null;

            // آزاد کردن کش tileها
            if (_tileCache != null)
            {
                foreach (var kvp in _tileCache)
                    kvp.Value?.Dispose();
                _tileCache.Clear();
            }

            // آزاد کردن تصویر اصلی
            Image?.Dispose();
            Image = null;

            _cacheValid = false;
        }
    }

    public class MapCell
    {
        public int TileIndex { get; set; } = -1;
        public bool FlipX { get; set; } = false;
        public byte Rotation { get; set; } = 0;

        public MapCell() { }
        public MapCell(int tileIndex, bool flipX = false, byte rotation = 0)
        {
            TileIndex = tileIndex;
            FlipX = flipX;
            Rotation = rotation;
        }
    }

    public class TileMap
    {
        public int Cols { get; private set; }
        public int Rows { get; private set; }
        public MapCell[,] Cells { get; private set; }

        public TileMap(int cols, int rows)
        {
            Cols = cols;
            Rows = rows;
            Cells = new MapCell[cols, rows];

            for (int x = 0; x < cols; x++)
                for (int y = 0; y < rows; y++)
                    Cells[x, y] = new MapCell(-1);
        }

        public void Resize(int newCols, int newRows)
        {
            var newCells = new MapCell[newCols, newRows];
            for (int x = 0; x < newCols; x++)
            {
                for (int y = 0; y < newRows; y++)
                {
                    newCells[x, y] = (x < Cols && y < Rows) ?
                        Cells[x, y] : new MapCell(-1);
                }
            }
            Cols = newCols;
            Rows = newRows;
            Cells = newCells;
        }
    }
}
