#nullable disable
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Models = TilesetEditor.Models;
using Utils = TilesetEditor.Utils;
using Actions = TilesetEditor.Actions;

namespace TilesetEditor
{
    public partial class Form1 : Form
    {
        Models.TileSet tileSet = new Models.TileSet();
        Utils.UndoManager undo = new Utils.UndoManager();

        // constants: no gap between cells by default
        private readonly int TilePadding = 0;
        private readonly int ToolbarHeight = 48;

        // drag state
        bool isDragging = false;
        Bitmap dragBitmap = null;
        int dragIndex = -1;
        Point lastMouse = Point.Empty;

        // potential drag start
        private Point mouseDownPos = Point.Empty;
        private bool potentialDrag = false;

        // selection / double click
        int selectedIndex = -1;
        bool ignoreNextDoubleClick = false;

        // clipboard
        Bitmap clipboardBitmap = null;
        int clipboardIndex = -1;

        // context
        int lastContextIndex = -1;

        // display size (for zoom). If zoom disabled, displaySize == tileSet.TileWidth
        private int displaySize => chkZoom.Checked ? (int)numDisplaySize.Value : tileSet.TileWidth;

        public Form1()
        {
            InitializeComponent();

            // enable double buffering for panelTileset to reduce flicker
            var pi = typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            pi.SetValue(panelTileset, true, null);

            // wire events
            btnLoadTileset.Click += BtnLoadTileset_Click;
            btnSlice.Click += BtnSlice_Click;

            cbPresetSize.SelectedIndexChanged += (s, e) => { if (!chkZoom.Checked) ApplyTileSizeAndRefresh(); };
            numCustomWidth.ValueChanged += (s, e) => { if (chkCustom.Checked && !chkZoom.Checked) ApplyTileSizeAndRefresh(); };
            numCustomHeight.ValueChanged += (s, e) => { if (chkCustom.Checked && !chkZoom.Checked) ApplyTileSizeAndRefresh(); };
            chkCustom.CheckedChanged += (s, e) => { if (!chkZoom.Checked) ApplyTileSizeAndRefresh(); };

            // grid controls
            chkUseCustomGrid.CheckedChanged += (s, e) =>
            {
                if (chkUseCustomGrid.Checked)
                {
                    tileSet.GridCols = (int)numGridCols.Value;
                    tileSet.GridRows = (int)numGridRows.Value;
                }
                else
                {
                    tileSet.GridCols = 0;
                    tileSet.GridRows = 0;
                }
                ApplyTileSizeAndRefresh();
            };
            numGridCols.ValueChanged += (s, e) => { if (chkUseCustomGrid.Checked) { tileSet.GridCols = (int)numGridCols.Value; ApplyTileSizeAndRefresh(); } };
            numGridRows.ValueChanged += (s, e) => { if (chkUseCustomGrid.Checked) { tileSet.GridRows = (int)numGridRows.Value; ApplyTileSizeAndRefresh(); } };

            // zoom controls
            chkZoom.CheckedChanged += (s, e) =>
            {
                if (chkZoom.Checked)
                {
                    numDisplaySize.Value = Math.Max(4, Math.Min(512, tileSet.TileWidth));
                }
                else
                {
                    ApplyTileSizeAndRefresh();
                }
                UpdateScrollSize();
                panelTileset.Invalidate();
            };
            numDisplaySize.ValueChanged += (s, e) =>
            {
                if (chkZoom.Checked)
                {
                    UpdateScrollSize();
                    panelTileset.Invalidate();
                }
            };

            btnUndo.Click += (s, e) => { undo.Undo(); tileSet.RebuildTiles(); panelTileset.Invalidate(); };
            btnRedo.Click += (s, e) => { undo.Redo(); tileSet.RebuildTiles(); panelTileset.Invalidate(); };

            panelTileset.Paint += PanelTileset_Paint;
            panelTileset.MouseDown += PanelTileset_MouseDown;
            panelTileset.MouseMove += PanelTileset_MouseMove;
            panelTileset.MouseUp += PanelTileset_MouseUp;
            panelTileset.MouseClick += PanelTileset_MouseClick;
            panelTileset.MouseDoubleClick += PanelTileset_MouseDoubleClick;

            miCopy.Click += MiCopy_Click;
            miPaste.Click += MiPaste_Click;
            miRotate.Click += MiRotate_Click;
            miFlip.Click += MiFlip_Click;
            miDelete.Click += MiDelete_Click;
            miEdit.Click += MiEdit_Click;

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        #region Load / Slice / Refresh
        private void BtnLoadTileset_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.bmp;*.jpeg" };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            tileSet.Image?.Dispose();
            tileSet.Image = new Bitmap(ofd.FileName);

            // clear undo/redo because this is a new base image
            undo.Clear();

            // if not using custom grid, reset grid counts so they derive from image
            if (!chkUseCustomGrid.Checked) { tileSet.GridCols = 0; tileSet.GridRows = 0; }

            ApplyTileSizeAndRefresh();
        }

        private void BtnSlice_Click(object sender, EventArgs e)
        {
            if (tileSet.Image == null) { MessageBox.Show("ابتدا تایل‌ست را بارگذاری کنید."); return; }
            // If using zoom mode, Slice should still rebuild tiles based on tileSet.TileWidth (not display)
            ApplyTileSizeAndRefresh();
        }

        private void ApplyTileSizeAndRefresh()
        {
            ApplyTileSize();
            tileSet.RebuildTiles();
            UpdateScrollSize();
            EndDrag();
            selectedIndex = -1;
            panelTileset.Invalidate();
        }

        private void ApplyTileSize()
        {
            if (chkCustom.Checked)
            {
                tileSet.TileWidth = (int)numCustomWidth.Value;
                tileSet.TileHeight = (int)numCustomHeight.Value;
            }
            else if (int.TryParse(cbPresetSize.Text, out int v))
            {
                tileSet.TileWidth = tileSet.TileHeight = v;
            }
        }

        private void UpdateScrollSize()
        {
            if (tileSet.Image == null || tileSet.TileWidth <= 0 || tileSet.TileHeight <= 0)
            {
                panelTileset.AutoScrollMinSize = Size.Empty;
                return;
            }
            int cols = tileSet.Columns;
            int rows = tileSet.Rows;
            int cell = displaySize; // use display size for scroll extents
            int w = cols * (cell + TilePadding) + TilePadding;
            int h = rows * (cell + TilePadding) + TilePadding + ToolbarHeight;
            panelTileset.AutoScrollMinSize = new Size(w, h);
        }
        #endregion

        #region Paint & HitTest
        private void PanelTileset_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(panelTileset.BackColor);

            if (tileSet.Image == null)
            {
                using var f = new Font("Segoe UI", 12);
                using var b = new SolidBrush(Color.Black);
                g.DrawString("Load tileset and Slice to view tiles", f, b, 8, ToolbarHeight - 32);
                return;
            }

            var scroll = panelTileset.AutoScrollPosition;
            int tw = tileSet.TileWidth;      // source tile size
            int th = tileSet.TileHeight;
            int ds = displaySize;           // display cell size (zoom or actual)

            int cols = tileSet.Columns;
            int rows = tileSet.Rows;

            for (int i = 0; i < tileSet.Tiles.Count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                float x = col * (ds + TilePadding) + TilePadding + scroll.X;
                float y = row * (ds + TilePadding) + TilePadding + ToolbarHeight + scroll.Y;
                var src = tileSet.Tiles[i].SourceRect;

                // draw scaled or 1:1 depending on displaySize
                g.DrawImage(tileSet.Image, new RectangleF(x, y, ds, ds), src, GraphicsUnit.Pixel);

                // grid rect (no gap)
                g.DrawRectangle(Pens.LightGray, x, y, ds, ds);

                // selected highlight
                if (i == selectedIndex)
                {
                    using var selPen = new Pen(Color.Orange, 3);
                    g.DrawRectangle(selPen, x + 1, y + 1, ds - 2, ds - 2);
                }
            }

            // ghost drag
            if (isDragging && dragBitmap != null)
            {
                var p = lastMouse;
                int col = Math.Max(0, (int)((p.X - scroll.X - TilePadding) / (ds + TilePadding)));
                int row = Math.Max(0, (int)((p.Y - scroll.Y - TilePadding - ToolbarHeight) / (ds + TilePadding)));
                col = Math.Min(col, cols - 1);
                row = Math.Min(row, rows - 1);

                int gx = col * (ds + TilePadding) + TilePadding + scroll.X;
                int gy = row * (ds + TilePadding) + TilePadding + ToolbarHeight + scroll.Y;

                var cm = new ColorMatrix(); cm.Matrix33 = 0.6f;
                var ia = new ImageAttributes(); ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(dragBitmap, new Rectangle(gx, gy, ds, ds), 0, 0, dragBitmap.Width, dragBitmap.Height, GraphicsUnit.Pixel, ia);
                g.DrawRectangle(Pens.Cyan, gx, gy, ds, ds);
            }
        }

        private int HitTestTileIndex(Point clientPoint)
        {
            if (tileSet.Image == null) return -1;
            var scroll = panelTileset.AutoScrollPosition;
            int ds = displaySize;
            int cols = tileSet.Columns;
            int rows = tileSet.Rows;

            float x = clientPoint.X - scroll.X - TilePadding;
            float y = clientPoint.Y - scroll.Y - TilePadding - ToolbarHeight;
            if (x < 0 || y < 0) return -1;
            int col = (int)(x / (ds + TilePadding));
            int row = (int)(y / (ds + TilePadding));
            if (col < 0 || row < 0 || col >= cols || row >= rows) return -1;
            int idx = row * cols + col;
            return (idx >= 0 && idx < tileSet.Tiles.Count) ? idx : -1;
        }
        #endregion

        #region Drag / Swap (start drag after threshold)
        private void PanelTileset_MouseDown(object sender, MouseEventArgs e)
        {
            if (tileSet.Image == null) return;
            if (e.Button != MouseButtons.Left) return;

            int idx = HitTestTileIndex(e.Location);
            if (idx < 0) return;

            // register potential drag start, but don't start dragging yet
            mouseDownPos = e.Location;
            potentialDrag = true;
            dragIndex = idx; // remember which tile user pressed on
        }

        private void PanelTileset_MouseMove(object sender, MouseEventArgs e)
        {
            if (potentialDrag && !isDragging)
            {
                var dx = Math.Abs(e.X - mouseDownPos.X);
                var dy = Math.Abs(e.Y - mouseDownPos.Y);
                var dragThreshold = SystemInformation.DragSize;
                if (dx >= dragThreshold.Width || dy >= dragThreshold.Height)
                {
                    // start actual drag
                    isDragging = true;
                    potentialDrag = false;
                    dragBitmap?.Dispose();
                    dragBitmap = tileSet.ExtractTileBitmap(dragIndex);
                    lastMouse = e.Location;
                    // now cancel double-click selection to avoid conflict
                    ignoreNextDoubleClick = true;
                    selectedIndex = -1;
                    panelTileset.Invalidate();
                }
            }
            else if (isDragging)
            {
                lastMouse = e.Location;
                panelTileset.Invalidate();
            }
        }

        private void PanelTileset_MouseUp(object sender, MouseEventArgs e)
        {
            if (potentialDrag)
            {
                // it was a click without drag
                potentialDrag = false;
                dragIndex = -1;
                return;
            }

            if (!isDragging) return;
            int target = HitTestTileIndex(e.Location);
            if (target >= 0 && dragIndex >= 0 && target != dragIndex)
            {
                // perform swap with undo
                var action = new Actions.SwapTileImagesAction(tileSet, dragIndex, target);
                undo.Do(action);
                tileSet.RebuildTiles();
                UpdateScrollSize();
            }
            EndDrag();
            panelTileset.Invalidate();
        }

        private void EndDrag()
        {
            isDragging = false;
            dragIndex = -1;
            dragBitmap?.Dispose();
            dragBitmap = null;
            potentialDrag = false;
            ignoreNextDoubleClick = false;
        }
        #endregion

        #region Double click selection & context menu show
        private void PanelTileset_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ignoreNextDoubleClick) { ignoreNextDoubleClick = false; return; }
            int idx = HitTestTileIndex(e.Location);
            if (idx >= 0)
            {
                selectedIndex = idx;
                panelTileset.Invalidate();
            }
        }

        private void PanelTileset_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int idx = HitTestTileIndex(e.Location);
                if (idx >= 0 && idx == selectedIndex)
                {
                    lastContextIndex = idx;
                    // enable/disable menu items based on clipboard or selection
                    miPaste.Enabled = clipboardBitmap != null;
                    miCopy.Enabled = true;
                    miRotate.Enabled = true;
                    miFlip.Enabled = true;
                    miDelete.Enabled = true;
                    miEdit.Enabled = true;

                    // show menu at mouse location
                    ctxTile.Show(panelTileset, e.Location);
                }
            }
        }
        #endregion

        #region Context actions (with Undo)
        private void MiCopy_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0) return;
            clipboardBitmap?.Dispose();
            clipboardBitmap = tileSet.ExtractTileBitmap(lastContextIndex);
            clipboardIndex = lastContextIndex;
        }

        private void MiPaste_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0 || clipboardBitmap == null) return;
            var action = new Actions.TileImageAction(tileSet, lastContextIndex, clipboardBitmap);
            undo.Do(action);
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
        }

        private void MiRotate_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0) return;
            using var bmp = tileSet.ExtractTileBitmap(lastContextIndex);
            if (bmp == null) return;
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var action = new Actions.TileImageAction(tileSet, lastContextIndex, bmp);
            undo.Do(action);
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
        }

        private void MiFlip_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0) return;
            using var bmp = tileSet.ExtractTileBitmap(lastContextIndex);
            if (bmp == null) return;
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
            var action = new Actions.TileImageAction(tileSet, lastContextIndex, bmp);
            undo.Do(action);
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
        }

        private void MiDelete_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0) return;
            var rect = tileSet.Tiles[lastContextIndex].SourceRect;
            using var empty = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(empty)) g.Clear(Color.Transparent);
            var action = new Actions.TileImageAction(tileSet, lastContextIndex, empty);
            undo.Do(action);
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
        }

        private void MiEdit_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0) return;
            string s = Microsoft.VisualBasic.Interaction.InputBox("Tile Index (info):", "Edit", lastContextIndex.ToString());
            // no direct effect; kept for compatibility
        }
        #endregion

        #region Keyboard shortcuts
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z) { undo.Undo(); tileSet.RebuildTiles(); panelTileset.Invalidate(); }
            if (e.Control && e.KeyCode == Keys.Y) { undo.Redo(); tileSet.RebuildTiles(); panelTileset.Invalidate(); }
            if (e.Control && e.KeyCode == Keys.C)
            {
                var p = panelTileset.PointToClient(Cursor.Position);
                int idx = HitTestTileIndex(p);
                if (idx >= 0) { clipboardBitmap?.Dispose(); clipboardBitmap = tileSet.ExtractTileBitmap(idx); clipboardIndex = idx; }
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                var p = panelTileset.PointToClient(Cursor.Position);
                int idx = HitTestTileIndex(p);
                if (idx >= 0 && clipboardBitmap != null)
                {
                    var action = new Actions.TileImageAction(tileSet, idx, clipboardBitmap);
                    undo.Do(action);
                    tileSet.RebuildTiles();
                    panelTileset.Invalidate();
                }
            }
            if (e.KeyCode == Keys.R)
            {
                if (selectedIndex >= 0)
                {
                    using var bmp = tileSet.ExtractTileBitmap(selectedIndex);
                    if (bmp != null)
                    {
                        bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        var action = new Actions.TileImageAction(tileSet, selectedIndex, bmp);
                        undo.Do(action);
                        tileSet.RebuildTiles();
                        panelTileset.Invalidate();
                    }
                }
            }
            if (e.KeyCode == Keys.F)
            {
                if (selectedIndex >= 0)
                {
                    using var bmp = tileSet.ExtractTileBitmap(selectedIndex);
                    if (bmp != null)
                    {
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        var action = new Actions.TileImageAction(tileSet, selectedIndex, bmp);
                        undo.Do(action);
                        tileSet.RebuildTiles();
                        panelTileset.Invalidate();
                    }
                }
            }
        }
        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            clipboardBitmap?.Dispose();
            dragBitmap?.Dispose();
            tileSet.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
