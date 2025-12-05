#nullable enable
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Models = TilesetEditor.Models;
using Utils = TilesetEditor.Utils;
using Actions = TilesetEditor.Actions;
using System.IO;
using System.Drawing.Drawing2D;
using System.Reflection;

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
        Bitmap? dragBitmap = null;
        int dragIndex = -1;
        Point lastMouse = Point.Empty;

        // potential drag start
        private Point mouseDownPos = Point.Empty;
        private bool potentialDrag = false;

        // selection
        int selectedIndex = -1;
        bool ignoreNextDoubleClick = false;

        // clipboard
        Bitmap? clipboardBitmap = null;
        int clipboardIndex = -1;

        // context
        int lastContextIndex = -1;

        // dirty flag and paths
        bool isDirty = false;
        string lastLoadPath = "";
        string lastSavePath = "";

        // state: whether grid has been applied (sliced)
        bool isSliced = false;

        public Form1()
        {
            InitializeComponent();

            // ensure "Custom" exists in preset combobox
            if (!cbPresetSize.Items.Contains("Custom")) cbPresetSize.Items.Add("Custom");

            // enable double buffering on panel to reduce flicker
            try
            {
                var pb = typeof(Panel).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                pb?.SetValue(panelTileset, true, null);
            }
            catch { /* ignore if not possible */ }

            // wire menu items and controls
            miLoad.Click += MiLoad_Click;
            miSave.Click += MiSave_Click;
            miSaveAs.Click += MiSaveAs_Click;
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
            btnSaveAs.Click += BtnSaveAs_Click;
            btnUndo.Click += (s, e) => DoUndo();
            btnRedo.Click += (s, e) => DoRedo();

            // Add/Remove column/row buttons (4 buttons)
            btnAddCol.Click += (s, e) => AddColumn();
            btnRemoveCol.Click += (s, e) => RemoveColumn();
            btnAddRow.Click += (s, e) => AddRow();
            btnRemoveRow.Click += (s, e) => RemoveRow();

            // tile size numeric fields (pixel sizes)
            numTileW.Minimum = 1;
            numTileH.Minimum = 1;
            numTileW.Maximum = 4096;
            numTileH.Maximum = 4096;
            numTileW.ValueChanged += (s, e) => { if (IsPresetCustom()) { tileSet.TileWidth = (int)numTileW.Value; tileSet.GridCols = 0; isSliced = false; UpdateDisplayAndInvalidate(); } };
            numTileH.ValueChanged += (s, e) => { if (IsPresetCustom()) { tileSet.TileHeight = (int)numTileH.Value; tileSet.GridRows = 0; isSliced = false; UpdateDisplayAndInvalidate(); } };

            cbPresetSize.SelectedIndexChanged += (s, e) => { OnPresetChanged(); };

            btnApplyGrid.Click += (s, e) => ApplyGridButton();

            trackZoom.Scroll += TrackZoom_Scroll;
            btnResetZoom.Click += (s, e) => ResetZoom();
            chkShowGrid.CheckedChanged += (s, e) => { showGrid = chkShowGrid.Checked; panelTileset.Invalidate(); };
            btnPickGridColor.Click += (s, e) => PickGridColor();

            // context menu actions
            miCopy.Click += MiCopy_Click;
            miCut.Click += MiCut_Click;
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
            UpdateUIState();
        }

        #region Helpers for preset/custom
        private bool IsPresetCustom() => string.Equals(cbPresetSize.Text, "Custom", StringComparison.OrdinalIgnoreCase);

        private void OnPresetChanged()
        {
            bool custom = IsPresetCustom();

            // enable tile size fields only when Custom
            numTileW.Enabled = custom;
            numTileH.Enabled = custom;

            if (!custom && tileSet.Image != null && int.TryParse(cbPresetSize.Text, out int presetVal))
            {
                // preset selected: set tile size and update tile size numeric controls (disabled)
                tileSet.TileWidth = presetVal;
                tileSet.TileHeight = presetVal;

                // update numeric displays (but disabled)
                numTileW.Value = Math.Min(numTileW.Maximum, Math.Max(numTileW.Minimum, presetVal));
                numTileH.Value = Math.Min(numTileH.Maximum, Math.Max(numTileH.Minimum, presetVal));

                // ensure GridCols/GridRows are derived from tile size
                tileSet.GridCols = 0;
                tileSet.GridRows = 0;
            }
            else if (custom)
            {
                // switching to custom: initialize tile size numeric from current tileSet values
                if (tileSet.TileWidth > 0) numTileW.Value = Math.Min(numTileW.Maximum, Math.Max(numTileW.Minimum, tileSet.TileWidth));
                if (tileSet.TileHeight > 0) numTileH.Value = Math.Min(numTileH.Maximum, Math.Max(numTileH.Minimum, tileSet.TileHeight));
                // keep GridCols/GridRows zero so Columns/Rows are derived from TileWidth/TileHeight
                tileSet.GridCols = 0;
                tileSet.GridRows = 0;
            }

            isSliced = false;
            UpdateDisplayAndInvalidate();
        }
        #endregion

        #region File / Edit / Help handlers
        private void MiLoad_Click(object? sender, EventArgs e) => BtnLoad_Click(sender, e);
        private void MiSave_Click(object? sender, EventArgs e) => BtnSave_Click(sender, e);
        private void MiSaveAs_Click(object? sender, EventArgs e) => BtnSaveAs_Click(sender, e);

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
            MessageBox.Show("Usage:\n1. File -> Load Tileset\n2. Choose preset or Custom (Tile W/H)\n3. Preview and Apply Grid\n4. Add columns/rows if needed, paste/edit tiles into empty space\n5. Save final image", "Usage Guide");
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
        private void BtnLoad_Click(object? sender, EventArgs e)
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

            // initialize tile size fields based on preset or fallback
            if (!IsPresetCustom() && int.TryParse(cbPresetSize.Text, out int presetVal))
            {
                tileSet.TileWidth = presetVal;
                tileSet.TileHeight = presetVal;
                numTileW.Value = Math.Min(numTileW.Maximum, Math.Max(numTileW.Minimum, presetVal));
                numTileH.Value = Math.Min(numTileH.Maximum, Math.Max(numTileH.Minimum, presetVal));
            }
            else
            {
                // custom or fallback
                int baseCell = GetPresetBaseCell();
                tileSet.TileWidth = baseCell;
                tileSet.TileHeight = baseCell;
                numTileW.Value = Math.Min(numTileW.Maximum, Math.Max(numTileW.Minimum, baseCell));
                numTileH.Value = Math.Min(numTileH.Maximum, Math.Max(numTileH.Minimum, baseCell));
            }

            undo.Clear();
            isDirty = false;
            isSliced = false;
            lblTilesetInfo.Text = Path.GetFileName(ofd.FileName);
            FitToView();
            UpdateUIState();
            UpdateStatus("Tileset loaded");
        }

        private void BtnSave_Click(object? sender, EventArgs e)
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

        private void BtnSaveAs_Click(object? sender, EventArgs e)
        {
            if (tileSet.Image == null) { MessageBox.Show("No tileset to save."); return; }

            using var sfd = new SaveFileDialog { Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap|*.bmp", InitialDirectory = lastSavePath, FileName = "tileset.png" };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            var ext = Path.GetExtension(sfd.FileName).ToLowerInvariant();
            ImageFormat fmt = ImageFormat.Png;
            if (ext == ".jpg" || ext == ".jpeg") fmt = ImageFormat.Jpeg;
            else if (ext == ".bmp") fmt = ImageFormat.Bmp;

            tileSet.Image.Save(sfd.FileName, fmt);
            lastSavePath = Path.GetDirectoryName(sfd.FileName) ?? lastSavePath;
            isDirty = false;
            UpdateStatus($"Saved As: {Path.GetFileName(sfd.FileName)}");
        }
        #endregion

        #region Grid controls (Add/Remove and Apply)
        private void AddColumn()
        {
            int currentCols = Math.Max(1, tileSet.Columns);
            int newCols = currentCols + 1;
            int currentRows = Math.Max(1, tileSet.Rows);

            var action = new Actions.GridResizeAction(tileSet, currentCols, currentRows, newCols, currentRows);
            undo.Do(action);

            tileSet.RebuildTiles();
            isSliced = true;
            UpdateScrollSize();
            panelTileset.Invalidate();
            MarkDirty();
            UpdateStatus($"Added column — now {tileSet.Columns} cols");
        }

        private void RemoveColumn()
        {
            int currentCols = Math.Max(1, tileSet.Columns);
            int newCols = Math.Max(1, currentCols - 1);
            int currentRows = Math.Max(1, tileSet.Rows);

            var action = new Actions.GridResizeAction(tileSet, currentCols, currentRows, newCols, currentRows);
            undo.Do(action);

            tileSet.RebuildTiles();
            isSliced = true;
            UpdateScrollSize();
            panelTileset.Invalidate();
            MarkDirty();
            UpdateStatus($"Removed column — now {tileSet.Columns} cols");
        }

        private void AddRow()
        {
            int currentRows = Math.Max(1, tileSet.Rows);
            int newRows = currentRows + 1;
            int currentCols = Math.Max(1, tileSet.Columns);

            var action = new Actions.GridResizeAction(tileSet, currentCols, currentRows, currentCols, newRows);
            undo.Do(action);

            tileSet.RebuildTiles();
            isSliced = true;
            UpdateScrollSize();
            panelTileset.Invalidate();
            MarkDirty();
            UpdateStatus($"Added row — now {tileSet.Rows} rows");
        }

        private void RemoveRow()
        {
            int currentRows = Math.Max(1, tileSet.Rows);
            int newRows = Math.Max(1, currentRows - 1);
            int currentCols = Math.Max(1, tileSet.Columns);

            var action = new Actions.GridResizeAction(tileSet, currentCols, currentRows, currentCols, newRows);
            undo.Do(action);

            tileSet.RebuildTiles();
            isSliced = true;
            UpdateScrollSize();
            panelTileset.Invalidate();
            MarkDirty();
            UpdateStatus($"Removed row — now {tileSet.Rows} rows");
        }

        private void ApplyGridButton()
        {
            if (!IsPresetCustom() && int.TryParse(cbPresetSize.Text, out int presetVal))
            {
                tileSet.TileWidth = presetVal;
                tileSet.TileHeight = presetVal;
                tileSet.GridCols = 0;
                tileSet.GridRows = 0;
            }
            else
            {
                // Custom: tile size comes from numTileW/numTileH (pixel sizes)
                tileSet.TileWidth = (int)numTileW.Value;
                tileSet.TileHeight = (int)numTileH.Value;
                tileSet.GridCols = 0;
                tileSet.GridRows = 0;
            }

            // Ensure canvas can contain the grid (pads if necessary)
            tileSet.EnsureCanvasForGrid(tileSet.Columns, tileSet.Rows);

            tileSet.RebuildTiles();
            isSliced = true;
            undo.Clear();
            UpdateScrollSize();
            panelTileset.Invalidate();
            UpdateUIState();
            UpdateStatus($"Grid applied: {tileSet.Columns} × {tileSet.Rows}");
        }
        #endregion

        #region Zoom / View / Preset
        private int GetPresetBaseCell()
        {
            if (int.TryParse(cbPresetSize.Text, out int v)) return v;
            return 32;
        }

        private void TrackZoom_Scroll(object? sender, EventArgs e)
        {
            zoomPercent = trackZoom.Value;
            panelTileset.Invalidate();
            UpdateStatus($"Zoom: {zoomPercent}%");
            UpdateScrollSize();
        }

        private void ResetZoom()
        {
            zoomPercent = 100f;
            trackZoom.Value = 100;
            panelTileset.Invalidate();
            UpdateStatus("Zoom reset");
            UpdateScrollSize();
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

        private void UpdateDisplayAndInvalidate()
        {
            if (!IsPresetCustom() && tileSet.Image != null && int.TryParse(cbPresetSize.Text, out int presetVal))
            {
                tileSet.TileWidth = presetVal;
                tileSet.TileHeight = presetVal;

                // update numeric displays (tile pixel sizes)
                numTileW.Value = Math.Min(numTileW.Maximum, Math.Max(numTileW.Minimum, presetVal));
                numTileH.Value = Math.Min(numTileH.Maximum, Math.Max(numTileH.Minimum, presetVal));

                tileSet.GridCols = 0;
                tileSet.GridRows = 0;
            }

            panelTileset.Invalidate();
            UpdateScrollSize();
        }
        #endregion

        #region Fit to view
        private void FitToView()
        {
            if (tileSet.Image == null) return;
            int cols = Math.Max(1, tileSet.Columns);
            int rows = Math.Max(1, tileSet.Rows);

            float tileW = tileSet.TileWidth > 0 ? tileSet.TileWidth : GetPresetBaseCell();
            float tileH = tileSet.TileHeight > 0 ? tileSet.TileHeight : GetPresetBaseCell();

            int availableW = Math.Max(100, panelTileset.ClientSize.Width - 32);
            int availableH = Math.Max(100, panelTileset.ClientSize.Height - panelTop.Height - 32);

            float neededW = cols * tileW;
            float neededH = rows * tileH;

            float scaleX = availableW / neededW;
            float scaleY = availableH / neededH;
            float fitScale = Math.Min(scaleX, scaleY);

            int newZoom = (int)Math.Round(fitScale * 100f);
            newZoom = Math.Max(ZoomMin, Math.Min(ZoomMax, newZoom));
            zoomPercent = newZoom;
            if (trackZoom.Value != newZoom) trackZoom.Value = newZoom;
            panelTileset.Invalidate();
            UpdateStatus($"Fitted to view ({zoomPercent}%)");
            UpdateScrollSize();
        }
        #endregion

        #region Paint & HitTest (zoom-aware, source-based)
        private void PanelTileset_Paint(object? sender, PaintEventArgs e)
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
            int cols = Math.Max(1, tileSet.Columns);
            int rows = Math.Max(1, tileSet.Rows);

            float tileW = tileSet.TileWidth > 0 ? tileSet.TileWidth : GetPresetBaseCell();
            float tileH = tileSet.TileHeight > 0 ? tileSet.TileHeight : GetPresetBaseCell();
            float dsW = tileW * (zoomPercent / 100f);
            float dsH = tileH * (zoomPercent / 100f);

            if (!isSliced)
            {
                float displayW = cols * dsW;
                float displayH = rows * dsH;

                float areaX = scroll.X + 8f;
                float areaY = scroll.Y + panelTop.Height + 8f;

                float offsetX = Math.Max(0f, (panelTileset.ClientSize.Width - displayW) / 2f);
                float offsetY = Math.Max(0f, (panelTileset.ClientSize.Height - panelTop.Height - displayH) / 2f);
                areaX += offsetX;
                areaY += offsetY;

                g.DrawImage(tileSet.Image, new RectangleF(areaX, areaY, displayW, displayH), new RectangleF(0, 0, tileSet.Image.Width, tileSet.Image.Height), GraphicsUnit.Pixel);

                if (showGrid)
                {
                    using var pen = new Pen(gridColor);
                    for (int r = 0; r <= rows; r++)
                    {
                        float y = areaY + r * dsH;
                        g.DrawLine(pen, areaX, y, areaX + displayW, y);
                    }
                    for (int c = 0; c <= cols; c++)
                    {
                        float x = areaX + c * dsW;
                        g.DrawLine(pen, x, areaY, x, areaY + displayH);
                    }
                }

                return;
            }

            for (int i = 0; i < tileSet.Tiles.Count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                float x = col * dsW + scroll.X + 8f;
                float y = row * dsH + scroll.Y + panelTop.Height + 8f;

                var src = tileSet.Tiles[i].SourceRect;

                if (tileSet.Image != null && src.Width > 0 && src.Height > 0)
                {
                    g.DrawImage(tileSet.Image, new RectangleF(x, y, dsW, dsH), src, GraphicsUnit.Pixel);
                }
                else
                {
                    using var br = new SolidBrush(Color.FromArgb(12, Color.Black));
                    g.FillRectangle(br, x, y, dsW, dsH);
                }

                if (showGrid)
                {
                    using var pen = new Pen(gridColor);
                    g.DrawRectangle(pen, x, y, dsW, dsH);
                }

                if (i == selectedIndex)
                {
                    using var selPen = new Pen(Color.Orange, 3);
                    g.DrawRectangle(selPen, x + 1, y + 1, Math.Max(0, dsW - 2), Math.Max(0, dsH - 2));
                }
            }

            if (isDragging && dragBitmap != null)
            {
                var p = lastMouse;
                int col = Math.Max(0, (int)((p.X - scroll.X - 8f) / dsW));
                int row = Math.Max(0, (int)((p.Y - scroll.Y - panelTop.Height - 8f) / dsH));
                col = Math.Min(col, cols - 1);
                row = Math.Min(row, rows - 1);

                int gx = (int)Math.Round(col * dsW + scroll.X + 8f);
                int gy = (int)Math.Round(row * dsH + scroll.Y + panelTop.Height + 8f);

                var cm = new ColorMatrix(); cm.Matrix33 = 0.6f;
                var ia = new ImageAttributes(); ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(dragBitmap, new Rectangle(gx, gy, (int)Math.Round(dsW), (int)Math.Round(dsH)), 0, 0, dragBitmap.Width, dragBitmap.Height, GraphicsUnit.Pixel, ia);
                g.DrawRectangle(Pens.Cyan, gx, gy, (int)Math.Round(dsW), (int)Math.Round(dsH));
            }
        }

        private int HitTestTileIndex(Point clientPoint)
        {
            if (tileSet.Tiles.Count == 0) return -1;
            var scroll = panelTileset.AutoScrollPosition;
            int cols = Math.Max(1, tileSet.Columns);

            float tileW = tileSet.TileWidth > 0 ? tileSet.TileWidth : GetPresetBaseCell();
            float tileH = tileSet.TileHeight > 0 ? tileSet.TileHeight : GetPresetBaseCell();
            float dsW = tileW * (zoomPercent / 100f);
            float dsH = tileH * (zoomPercent / 100f);

            float x = clientPoint.X - scroll.X - 8f;
            float y = clientPoint.Y - scroll.Y - panelTop.Height - 8f;
            if (x < 0 || y < 0) return -1;
            int col = (int)(x / dsW);
            int row = (int)(y / dsH);
            if (col < 0 || row < 0 || col >= cols || row >= tileSet.Rows) return -1;
            int idx = row * cols + col;
            return (idx >= 0 && idx < tileSet.Tiles.Count) ? idx : -1;
        }
        #endregion

        #region Drag / Swap (start after threshold) - only when sliced
        private void PanelTileset_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!isSliced) return;
            if (tileSet.Tiles.Count == 0) return;
            if (e.Button != MouseButtons.Left) return;

            int idx = HitTestTileIndex(e.Location);
            if (idx < 0) return;

            mouseDownPos = e.Location;
            potentialDrag = true;
            dragIndex = idx;
        }

        private void PanelTileset_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!isSliced) return;
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

        private void PanelTileset_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!isSliced)
            {
                potentialDrag = false;
                dragIndex = -1;
                return;
            }

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
                // perform swap action (overwrite)
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

        #region Selection & Context menu (only when sliced)
        private void PanelTileset_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (!isSliced) return;
            if (ignoreNextDoubleClick) { ignoreNextDoubleClick = false; return; }
            int idx = HitTestTileIndex(e.Location);
            if (idx >= 0)
            {
                selectedIndex = idx;
                panelTileset.Invalidate();
            }
        }

        private void PanelTileset_MouseClick(object? sender, MouseEventArgs e)
        {
            if (!isSliced) return;
            if (e.Button == MouseButtons.Right)
            {
                int idx = HitTestTileIndex(e.Location);
                if (idx >= 0)
                {
                    lastContextIndex = idx;
                    miPaste.Enabled = clipboardBitmap != null;
                    ctxTile.Show(panelTileset, e.Location);
                }
            }
        }
        #endregion

        #region Context actions (copy/paste/rotate/flip/delete/cut) - only when sliced
        private void MiCopy_Click(object? sender, EventArgs e)
        {
            if (!isSliced) return;
            if (lastContextIndex < 0) return;
            clipboardBitmap?.Dispose();
            clipboardBitmap = tileSet.ExtractTileBitmap(lastContextIndex);
            clipboardIndex = lastContextIndex;
            UpdateStatus("Copied tile");
        }

        private void MiCut_Click(object? sender, EventArgs e)
        {
            if (!isSliced) return;
            if (lastContextIndex < 0) return;

            // copy to clipboardBitmap
            clipboardBitmap?.Dispose();
            clipboardBitmap = tileSet.ExtractTileBitmap(lastContextIndex);
            clipboardIndex = lastContextIndex;

            // then clear the source tile by applying a transparent bitmap of tile size
            var rect = tileSet.Tiles[lastContextIndex].SourceRect;
            using var empty = new Bitmap(Math.Max(1, rect.Width), Math.Max(1, rect.Height), PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(empty)) g.Clear(Color.Transparent);

            var action = new Actions.TileImageAction(tileSet, lastContextIndex, empty);
            undo.Do(action);
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
            MarkDirty();
            UpdateStatus("Cut tile");
        }

        private void MiPaste_Click(object? sender, EventArgs e)
        {
            if (!isSliced) return;
            if (lastContextIndex < 0 || clipboardBitmap == null) return;

            // Paste: draw clipboardBitmap into the tile's SourceRect on the underlying image.
            var action = new Actions.TileImageAction(tileSet, lastContextIndex, clipboardBitmap);
            undo.Do(action);
            tileSet.RebuildTiles();
            panelTileset.Invalidate();
            MarkDirty();
            UpdateStatus("Pasted tile");
        }

        private void MiRotate_Click(object? sender, EventArgs e)
        {
            if (!isSliced) return;
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

        private void MiFlip_Click(object? sender, EventArgs e)
        {
            if (!isSliced) return;
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

        private void MiDelete_Click(object? sender, EventArgs e)
        {
            if (!isSliced) return;
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

        private void MiEdit_Click(object? sender, EventArgs e)
        {
            if (!isSliced) return;
            if (lastContextIndex < 0) return;
            string s = Microsoft.VisualBasic.Interaction.InputBox("Tile Index (info):", "Edit", lastContextIndex.ToString());
        }
        #endregion

        #region Mouse wheel zoom
        private void PanelTileset_MouseWheel(object? sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                int delta = e.Delta > 0 ? 10 : -10;
                zoomPercent = Math.Max(ZoomMin, Math.Min(ZoomMax, zoomPercent + delta));
                trackZoom.Value = (int)zoomPercent;
                panelTileset.Invalidate();
                UpdateStatus($"Zoom: {zoomPercent}%");
                UpdateScrollSize();
            }
        }
        #endregion

        #region Keyboard shortcuts
        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z) DoUndo();
            if (e.Control && e.KeyCode == Keys.Y) DoRedo();
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (!isSliced) return;
                var p = panelTileset.PointToClient(Cursor.Position);
                int idx = HitTestTileIndex(p);
                if (idx >= 0) { clipboardBitmap?.Dispose(); clipboardBitmap = tileSet.ExtractTileBitmap(idx); clipboardIndex = idx; UpdateStatus("Copied tile"); }
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (!isSliced) return;
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
                if (!isSliced) return;
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
                if (!isSliced) return;
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
            int cols = Math.Max(1, tileSet.Columns);
            int rows = Math.Max(1, tileSet.Rows);

            float tileW = tileSet.TileWidth > 0 ? tileSet.TileWidth : GetPresetBaseCell();
            float tileH = tileSet.TileHeight > 0 ? tileSet.TileHeight : GetPresetBaseCell();
            float dsW = tileW * (zoomPercent / 100f);
            float dsH = tileH * (zoomPercent / 100f);

            int w = (int)Math.Ceiling(cols * dsW) + 32;
            int h = (int)Math.Ceiling(rows * dsH) + panelTop.Height + 32;
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

        private void UpdateUIState()
        {
            bool editingEnabled = isSliced;
            btnUndo.Enabled = editingEnabled;
            btnRedo.Enabled = editingEnabled;
            miUndo.Enabled = editingEnabled;
            miRedo.Enabled = editingEnabled;
            miPaste.Enabled = editingEnabled && clipboardBitmap != null;
            btnSave.Enabled = tileSet.Image != null;
            btnSaveAs.Enabled = tileSet.Image != null;

            // tile size fields enabled only when preset == Custom
            numTileW.Enabled = IsPresetCustom();
            numTileH.Enabled = IsPresetCustom();
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
