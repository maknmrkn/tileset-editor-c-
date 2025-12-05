#nullable disable
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Models = TilesetEditor.Models;
using Utils = TilesetEditor.Utils;
using Actions = TilesetEditor.Actions;
using System.IO;

namespace TilesetEditor
{
    public partial class Form1 : Form
    {
        Models.TileSet tileSet = new Models.TileSet();
        Utils.UndoManager undo = new Utils.UndoManager();

        // UI state
        private Color gridColor = Color.LightGray;
        private bool showGrid = true;

        // zoom state (percentage)
        private float zoomPercent = 100f; // 100% default
        private const int ZoomMin = 10;
        private const int ZoomMax = 400;

        // drag state
        bool isDragging = false;
        Bitmap dragBitmap = null;
        int dragIndex = -1;
        Point lastMouse = Point.Empty;

        // potential drag start
        private Point mouseDownPos = Point.Empty;
        private bool potentialDrag = false;

        // selection
        int selectedIndex = -1;
        bool ignoreNextDoubleClick = false;

        // clipboard
        Bitmap clipboardBitmap = null;
        int clipboardIndex = -1;

        // context
        int lastContextIndex = -1;

        // dirty flag and paths
        bool isDirty = false;
        string lastLoadPath = "";
        string lastSavePath = "";

        public Form1()
        {
            InitializeComponent();

            // wire menu items and controls
            miLoad.Click += MiLoad_Click;
            miSave.Click += MiSave_Click;
            miExit.Click += (s, e) => Close();

            miUndo.Click += (s, e) => DoUndo();
            miRedo.Click += (s, e) => DoRedo();

            miToggleGrid.Click += (s, e) => ToggleGridMenu();
            miGridColor.Click += (s, e) => PickGridColor();
            miResetZoom.Click += (s, e) => ResetZoom();

            miUsage.Click += (s, e) => ShowUsage();
            miHotkeys.Click += (s, e) => ShowHotkeys();
            miCredits.Click += (s, e) => ShowCredits();

            // right panel controls
            btnLoad.Click += BtnLoad_Click;
            btnSave.Click += BtnSave_Click;
            btnUndo.Click += (s, e) => DoUndo();
            btnRedo.Click += (s, e) => DoRedo();

            btnAddCol.Click += (s, e) => ChangeCols(1);
            btnRemoveCol.Click += (s, e) => ChangeCols(-1);
            btnAddRow.Click += (s, e) => ChangeRows(1);
            btnRemoveRow.Click += (s, e) => ChangeRows(-1);
            chkUseCustomGrid.CheckedChanged += (s, e) => ToggleCustomGrid();
            numGridCols.ValueChanged += (s, e) => { if (chkUseCustomGrid.Checked) { tileSet.GridCols = (int)numGridCols.Value; ApplyGridAndRefresh(); } };
            numGridRows.ValueChanged += (s, e) => { if (chkUseCustomGrid.Checked) { tileSet.GridRows = (int)numGridRows.Value; ApplyGridAndRefresh(); } };

            trackZoom.Scroll += TrackZoom_Scroll;
            btnResetZoom.Click += (s, e) => ResetZoom();
            chkShowGrid.CheckedChanged += (s, e) => { showGrid = chkShowGrid.Checked; panelTileset.Invalidate(); };
            btnPickGridColor.Click += (s, e) => PickGridColor();

            // context menu actions
            miCopy.Click += MiCopy_Click;
            miPaste.Click += MiPaste_Click;
            miRotate.Click += MiRotate_Click;
            miFlip.Click += MiFlip_Click;
            miDelete.Click += MiDelete_Click;
            miEdit.Click += MiEdit_Click;

            // panel events
            panelTileset.Paint += PanelTileset_Paint;
            panelTileset.MouseDown += PanelTileset_MouseDown;
            panelTileset.MouseMove += PanelTileset_MouseMove;
            panelTileset.MouseUp += PanelTileset_MouseUp;
            panelTileset.MouseClick += PanelTileset_MouseClick;
            panelTileset.MouseDoubleClick += PanelTileset_MouseDoubleClick;
            panelTileset.MouseEnter += (s, e) => panelTileset.Focus();
            panelTileset.MouseWheel += PanelTileset_MouseWheel;

            // keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            UpdateStatus("Ready");
            UpdateZoomControls();
        }

        #region File / Edit / Help handlers
        private void MiLoad_Click(object sender, EventArgs e) => BtnLoad_Click(sender, e);
        private void MiSave_Click(object sender, EventArgs e) => BtnSave_Click(sender, e);

        private void DoUndo()
        {
            undo.Undo();
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
            MarkDirty();
        }

        private void DoRedo()
        {
            undo.Redo();
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
            MarkDirty();
        }

        private void ShowUsage()
        {
            MessageBox.Show("Usage:\n1. File -> Load Tileset\n2. Use grid controls to set cols/rows\n3. Drag tiles to swap, right-click for actions\n4. Save when done", "Usage Guide");
        }

        private void ShowHotkeys()
        {
            MessageBox.Show("Hotkeys:\nCtrl+Z Undo\nCtrl+Y Redo\nCtrl+C Copy\nCtrl+V Paste\nCtrl+MouseWheel Zoom\nR Rotate\nF Flip", "Hotkeys");
        }

        private void ShowCredits()
        {
            MessageBox.Show("Created by: Your Name\nAssistant: Copilot", "Credits");
        }
        #endregion

        #region Load / Save / Paths / Dirty
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (isDirty)
            {
                var r = MessageBox.Show("You have unsaved changes. Load new tileset and discard changes?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (r != DialogResult.Yes) return;
            }

            using var ofd = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.bmp;*.jpeg", InitialDirectory = lastLoadPath };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            tileSet.Image?.Dispose();
            tileSet.Image = new Bitmap(ofd.FileName);
            lastLoadPath = Path.GetDirectoryName(ofd.FileName) ?? lastLoadPath;
            lastSavePath = lastLoadPath;

            // default grid if not set
            if (!chkUseCustomGrid.Checked)
            {
                tileSet.GridCols = Math.Max(1, tileSet.Image.Width / 32);
                tileSet.GridRows = Math.Max(1, tileSet.Image.Height / 32);
            }

            undo.Clear();
            isDirty = false;
            lblTilesetInfo.Text = Path.GetFileName(ofd.FileName);
            ApplyGridAndRefresh();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (tileSet.Image == null) { MessageBox.Show("No tileset to save."); return; }

            if (isDirty)
            {
                var r = MessageBox.Show("There are unsaved changes. Save now?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Cancel) return;
                if (r == DialogResult.No) return;
            }

            using var sfd = new SaveFileDialog { Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap|*.bmp", InitialDirectory = lastSavePath, FileName = "tileset.png" };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            var ext = Path.GetExtension(sfd.FileName).ToLowerInvariant();
            ImageFormat fmt = ImageFormat.Png;
            if (ext == ".jpg" || ext == ".jpeg") fmt = ImageFormat.Jpeg;
            else if (ext == ".bmp") fmt = ImageFormat.Bmp;

            tileSet.Image.Save(sfd.FileName, fmt);
            lastSavePath = Path.GetDirectoryName(sfd.FileName) ?? lastSavePath;
            isDirty = false;
            UpdateStatus($"Saved: {Path.GetFileName(sfd.FileName)}");
        }
        #endregion

        #region Grid controls (cols/rows +/-)
        private void ToggleCustomGrid()
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
            ApplyGridAndRefresh();
        }

        private void ChangeCols(int delta)
        {
            int current = tileSet.GridCols > 0 ? tileSet.GridCols : tileSet.Columns;
            int cols = Math.Max(1, current + delta);
            tileSet.GridCols = cols;
            if (chkUseCustomGrid.Checked) numGridCols.Value = cols;
            ApplyGridAndRefresh();
        }

        private void ChangeRows(int delta)
        {
            int current = tileSet.GridRows > 0 ? tileSet.GridRows : tileSet.Rows;
            int rows = Math.Max(1, current + delta);
            tileSet.GridRows = rows;
            if (chkUseCustomGrid.Checked) numGridRows.Value = rows;
            ApplyGridAndRefresh();
        }

        private void ApplyGridAndRefresh()
        {
            tileSet.RebuildTiles();
            UpdateScrollSize();
            panelTileset.Invalidate();
            UpdateStatus($"Grid: {tileSet.Columns} × {tileSet.Rows}");
        }
        #endregion

        #region Zoom / View
        private void TrackZoom_Scroll(object sender, EventArgs e)
        {
            zoomPercent = trackZoom.Value;
            panelTileset.Invalidate();
            UpdateStatus($"Zoom: {zoomPercent}%");
        }

        private void ResetZoom()
        {
            zoomPercent = 100f;
            trackZoom.Value = 100;
            panelTileset.Invalidate();
            UpdateStatus("Zoom reset");
        }

        private void UpdateZoomControls()
        {
            trackZoom.Minimum = ZoomMin;
            trackZoom.Maximum = ZoomMax;
            trackZoom.Value = (int)zoomPercent;
        }

        private void ToggleGridMenu()
        {
            showGrid = !showGrid;
            chkShowGrid.Checked = showGrid;
            panelTileset.Invalidate();
        }

        private void PickGridColor()
        {
            using var cd = new ColorDialog { Color = gridColor };
            if (cd.ShowDialog() == DialogResult.OK)
            {
                gridColor = cd.Color;
                panelTileset.Invalidate();
            }
        }
        #endregion

        #region Paint & HitTest (zoom-aware)
        private void PanelTileset_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(panelTileset.BackColor);

            if (tileSet.Image == null)
            {
                using var f = new Font("Segoe UI", 12);
                using var b = new SolidBrush(Color.Black);
                g.DrawString("Load tileset from File -> Load Tileset", f, b, 8, 8);
                return;
            }

            var scroll = panelTileset.AutoScrollPosition;
            int cols = tileSet.Columns;
            int rows = tileSet.Rows;

            // compute display cell size based on zoom and a base cell size
            float baseCell = 64f;
            float ds = baseCell * (zoomPercent / 100f);

            for (int i = 0; i < tileSet.Tiles.Count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                float x = col * ds + scroll.X + 8;
                float y = row * ds + scroll.Y + panelTop.Height + 8;

                var src = tileSet.Tiles[i].SourceRect;

                // draw tile image or empty cell
                if (tileSet.Image != null && src.Width > 0 && src.Height > 0)
                {
                    g.DrawImage(tileSet.Image, new RectangleF(x, y, ds, ds), src, GraphicsUnit.Pixel);
                }
                else
                {
                    using var br = new SolidBrush(Color.FromArgb(10, Color.Black));
                    g.FillRectangle(br, x, y, ds, ds);
                }

                // grid lines
                if (showGrid)
                {
                    using var pen = new Pen(gridColor);
                    g.DrawRectangle(pen, x, y, ds, ds);
                }

                // selection highlight
                if (i == selectedIndex)
                {
                    using var selPen = new Pen(Color.Orange, 3);
                    g.DrawRectangle(selPen, x + 1, y + 1, ds - 2, ds - 2);
                }
            }
        }

        private int HitTestTileIndex(Point clientPoint)
        {
            if (tileSet.Tiles.Count == 0) return -1;
            var scroll = panelTileset.AutoScrollPosition;
            int cols = tileSet.Columns;
            float baseCell = 64f;
            float ds = baseCell * (zoomPercent / 100f);

            float x = clientPoint.X - scroll.X - 8;
            float y = clientPoint.Y - scroll.Y - panelTop.Height - 8;
            if (x < 0 || y < 0) return -1;
            int col = (int)(x / ds);
            int row = (int)(y / ds);
            if (col < 0 || row < 0 || col >= cols || row >= tileSet.Rows) return -1;
            int idx = row * cols + col;
            return (idx >= 0 && idx < tileSet.Tiles.Count) ? idx : -1;
        }
        #endregion

        #region Drag / Swap (start after threshold)
        private void PanelTileset_MouseDown(object sender, MouseEventArgs e)
        {
            if (tileSet.Tiles.Count == 0) return;
            if (e.Button != MouseButtons.Left) return;

            int idx = HitTestTileIndex(e.Location);
            if (idx < 0) return;

            mouseDownPos = e.Location;
            potentialDrag = true;
            dragIndex = idx;
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
                    isDragging = true;
                    potentialDrag = false;
                    dragBitmap?.Dispose();
                    dragBitmap = tileSet.ExtractTileBitmap(dragIndex);
                    lastMouse = e.Location;
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
                potentialDrag = false;
                dragIndex = -1;
                return;
            }

            if (!isDragging) return;
            int target = HitTestTileIndex(e.Location);
            if (target >= 0 && dragIndex >= 0 && target != dragIndex)
            {
                var action = new Actions.SwapTileImagesAction(tileSet, dragIndex, target);
                undo.Do(action);
                tileSet.RebuildTiles();
                UpdateScrollSize();
                MarkDirty();
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

        #region Selection & Context menu
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
                    miPaste.Enabled = clipboardBitmap != null;
                    ctxTile.Show(panelTileset, e.Location);
                }
            }
        }
        #endregion

        #region Context actions (copy/paste/rotate/flip/delete)
        private void MiCopy_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0) return;
            clipboardBitmap?.Dispose();
            clipboardBitmap = tileSet.ExtractTileBitmap(lastContextIndex);
            clipboardIndex = lastContextIndex;
            UpdateStatus("Copied tile");
        }

        private void MiPaste_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0 || clipboardBitmap == null) return;
            var action = new Actions.TileImageAction(tileSet, lastContextIndex, clipboardBitmap);
            undo.Do(action);
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
            MarkDirty();
            UpdateStatus("Pasted tile");
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
            MarkDirty();
            UpdateStatus("Rotated tile");
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
            MarkDirty();
            UpdateStatus("Flipped tile");
        }

        private void MiDelete_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0) return;
            var rect = tileSet.Tiles[lastContextIndex].SourceRect;
            using var empty = new Bitmap(Math.Max(1, rect.Width), Math.Max(1, rect.Height), PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(empty)) g.Clear(Color.Transparent);
            var action = new Actions.TileImageAction(tileSet, lastContextIndex, empty);
            undo.Do(action);
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
            MarkDirty();
            UpdateStatus("Deleted tile");
        }

        private void MiEdit_Click(object sender, EventArgs e)
        {
            if (lastContextIndex < 0) return;
            string s = Microsoft.VisualBasic.Interaction.InputBox("Tile Index (info):", "Edit", lastContextIndex.ToString());
        }
        #endregion

        #region Mouse wheel zoom
        private void PanelTileset_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                int delta = e.Delta > 0 ? 10 : -10;
                zoomPercent = Math.Max(ZoomMin, Math.Min(ZoomMax, zoomPercent + delta));
                trackZoom.Value = (int)zoomPercent;
                panelTileset.Invalidate();
                UpdateStatus($"Zoom: {zoomPercent}%");
            }
        }
        #endregion

        #region Keyboard shortcuts
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z) DoUndo();
            if (e.Control && e.KeyCode == Keys.Y) DoRedo();
            if (e.Control && e.KeyCode == Keys.C)
            {
                var p = panelTileset.PointToClient(Cursor.Position);
                int idx = HitTestTileIndex(p);
                if (idx >= 0) { clipboardBitmap?.Dispose(); clipboardBitmap = tileSet.ExtractTileBitmap(idx); clipboardIndex = idx; UpdateStatus("Copied tile"); }
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
                    MarkDirty();
                    UpdateStatus("Pasted tile");
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
                        MarkDirty();
                        UpdateStatus("Rotated tile");
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
                        MarkDirty();
                        UpdateStatus("Flipped tile");
                    }
                }
            }
        }
        #endregion

        #region Utilities
        private void UpdateScrollSize()
        {
            if (tileSet == null) { panelTileset.AutoScrollMinSize = Size.Empty; return; }
            int cols = tileSet.Columns;
            int rows = tileSet.Rows;
            float baseCell = 64f;
            float ds = baseCell * (zoomPercent / 100f);
            int w = (int)(cols * ds) + 32;
            int h = (int)(rows * ds) + panelTop.Height + 32;
            panelTileset.AutoScrollMinSize = new Size(w, h);
        }

        private void UpdateStatus(string text)
        {
            toolStripStatusLabel.Text = text;
        }

        private void MarkDirty()
        {
            isDirty = true;
            UpdateStatus("Modified");
        }
        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isDirty)
            {
                var r = MessageBox.Show("You have unsaved changes. Exit and discard changes?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (r != DialogResult.Yes) { e.Cancel = true; return; }
            }
            clipboardBitmap?.Dispose();
            dragBitmap?.Dispose();
            tileSet.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
