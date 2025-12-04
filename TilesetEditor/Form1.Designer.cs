namespace TilesetEditor
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Menu
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();

            // Top info panel (small)
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTilesetInfo = new System.Windows.Forms.Label();

            // Right side panel with tabs
            this.panelRight = new System.Windows.Forms.Panel();
            this.tabControlRight = new System.Windows.Forms.TabControl();
            this.tabSlice = new System.Windows.Forms.TabPage();
            this.tabActions = new System.Windows.Forms.TabPage();

            // Flow panels inside tabs (vertical layout)
            this.flowSlice = new System.Windows.Forms.FlowLayoutPanel();
            this.flowActions = new System.Windows.Forms.FlowLayoutPanel();

            // Slice controls
            this.groupSliceHeader = new System.Windows.Forms.Label();
            this.lblTileSize = new System.Windows.Forms.Label();
            this.cbPresetSize = new System.Windows.Forms.ComboBox();
            this.chkCustom = new System.Windows.Forms.CheckBox();
            this.numCustomWidth = new System.Windows.Forms.NumericUpDown();
            this.numCustomHeight = new System.Windows.Forms.NumericUpDown();
            this.btnSlice = new System.Windows.Forms.Button();

            // Grid controls
            this.groupGridHeader = new System.Windows.Forms.Label();
            this.chkUseCustomGrid = new System.Windows.Forms.CheckBox();
            this.lblGridCols = new System.Windows.Forms.Label();
            this.numGridCols = new System.Windows.Forms.NumericUpDown();
            this.lblGridRows = new System.Windows.Forms.Label();
            this.numGridRows = new System.Windows.Forms.NumericUpDown();

            // Zoom controls
            this.groupZoomHeader = new System.Windows.Forms.Label();
            this.chkZoom = new System.Windows.Forms.CheckBox();
            this.lblDisplaySize = new System.Windows.Forms.Label();
            this.numDisplaySize = new System.Windows.Forms.NumericUpDown();

            // Actions controls
            this.groupActionsHeader = new System.Windows.Forms.Label();
            this.btnLoadTileset = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnRedo = new System.Windows.Forms.Button();

            // Tileset panel (main)
            this.panelTileset = new System.Windows.Forms.Panel();

            // Status
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();

            // Context menu (manual show)
            this.ctxTile = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.miFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.miPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.miEdit = new System.Windows.Forms.ToolStripMenuItem();

            // --- MenuStrip ---
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fileToolStripMenu });
            this.fileToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.miSave, this.miExit });
            this.fileToolStripMenu.Text = "File";
            this.miSave.Text = "Save Tileset...";
            this.miExit.Text = "Exit";

            // --- panelTop ---
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 40;
            this.panelTop.Padding = new System.Windows.Forms.Padding(8);
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);

            this.lblTilesetInfo.Text = "No tileset loaded";
            this.lblTilesetInfo.AutoSize = true;
            this.lblTilesetInfo.Location = new System.Drawing.Point(12, 12);
            this.panelTop.Controls.Add(this.lblTilesetInfo);

            // --- panelRight ---
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Width = 360;
            this.panelRight.Padding = new System.Windows.Forms.Padding(8);
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            // --- tabControlRight ---
            this.tabControlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlRight.TabPages.AddRange(new System.Windows.Forms.TabPage[] { this.tabSlice, this.tabActions });

            // --- tabSlice ---
            this.tabSlice.Text = "Slice & Grid";
            this.tabSlice.Padding = new System.Windows.Forms.Padding(6);

            // flowSlice: vertical layout, auto scroll
            this.flowSlice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowSlice.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowSlice.WrapContents = false;
            this.flowSlice.AutoScroll = true;
            this.flowSlice.Padding = new System.Windows.Forms.Padding(6);
            this.flowSlice.AutoSize = false;

            // slice controls layout (use margins to create spacing)
            this.groupSliceHeader.Text = "Tile Size";
            this.groupSliceHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupSliceHeader.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);

            this.lblTileSize.Text = "Preset:";
            this.lblTileSize.AutoSize = true;
            this.lblTileSize.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);

            this.cbPresetSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPresetSize.Items.AddRange(new object[] { "8", "16", "24", "32", "48", "64", "128" });
            this.cbPresetSize.SelectedIndex = 3;
            this.cbPresetSize.Width = 120;
            this.cbPresetSize.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.chkCustom.Text = "Custom";
            this.chkCustom.AutoSize = true;
            this.chkCustom.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.numCustomWidth.Minimum = 1;
            this.numCustomWidth.Value = 32;
            this.numCustomWidth.Width = 80;
            this.numCustomWidth.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.numCustomHeight.Minimum = 1;
            this.numCustomHeight.Value = 32;
            this.numCustomHeight.Width = 80;
            this.numCustomHeight.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.btnSlice.Text = "Slice";
            this.btnSlice.Width = 120;
            this.btnSlice.Margin = new System.Windows.Forms.Padding(3, 6, 3, 12);

            // grid controls
            this.groupGridHeader.Text = "Grid";
            this.groupGridHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupGridHeader.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);

            this.chkUseCustomGrid.Text = "Use custom grid (cols × rows)";
            this.chkUseCustomGrid.AutoSize = true;
            this.chkUseCustomGrid.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.lblGridCols.Text = "Cols:";
            this.lblGridCols.AutoSize = true;
            this.lblGridCols.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);

            this.numGridCols.Minimum = 1;
            this.numGridCols.Value = 10;
            this.numGridCols.Width = 100;
            this.numGridCols.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.lblGridRows.Text = "Rows:";
            this.lblGridRows.AutoSize = true;
            this.lblGridRows.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);

            this.numGridRows.Minimum = 1;
            this.numGridRows.Value = 10;
            this.numGridRows.Width = 100;
            this.numGridRows.Margin = new System.Windows.Forms.Padding(3, 3, 3, 12);

            // zoom controls
            this.groupZoomHeader.Text = "Preview / Zoom";
            this.groupZoomHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupZoomHeader.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);

            this.chkZoom.Text = "Preview Zoom";
            this.chkZoom.AutoSize = true;
            this.chkZoom.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.lblDisplaySize.Text = "Display size:";
            this.lblDisplaySize.AutoSize = true;
            this.lblDisplaySize.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);

            this.numDisplaySize.Minimum = 4;
            this.numDisplaySize.Maximum = 512;
            this.numDisplaySize.Value = 32;
            this.numDisplaySize.Width = 100;
            this.numDisplaySize.Margin = new System.Windows.Forms.Padding(3, 3, 3, 12);

            // add slice tab children
            this.flowSlice.Controls.Add(this.groupSliceHeader);
            this.flowSlice.Controls.Add(this.lblTileSize);
            this.flowSlice.Controls.Add(this.cbPresetSize);
            this.flowSlice.Controls.Add(this.chkCustom);
            this.flowSlice.Controls.Add(this.numCustomWidth);
            this.flowSlice.Controls.Add(this.numCustomHeight);
            this.flowSlice.Controls.Add(this.btnSlice);

            this.flowSlice.Controls.Add(this.groupGridHeader);
            this.flowSlice.Controls.Add(this.chkUseCustomGrid);
            this.flowSlice.Controls.Add(this.lblGridCols);
            this.flowSlice.Controls.Add(this.numGridCols);
            this.flowSlice.Controls.Add(this.lblGridRows);
            this.flowSlice.Controls.Add(this.numGridRows);

            this.flowSlice.Controls.Add(this.groupZoomHeader);
            this.flowSlice.Controls.Add(this.chkZoom);
            this.flowSlice.Controls.Add(this.lblDisplaySize);
            this.flowSlice.Controls.Add(this.numDisplaySize);

            this.tabSlice.Controls.Add(this.flowSlice);

            // --- tabActions ---
            this.tabActions.Text = "Actions";
            this.tabActions.Padding = new System.Windows.Forms.Padding(6);

            this.flowActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowActions.WrapContents = false;
            this.flowActions.AutoScroll = true;
            this.flowActions.Padding = new System.Windows.Forms.Padding(6);

            this.groupActionsHeader.Text = "Main Actions";
            this.groupActionsHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupActionsHeader.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);

            this.btnLoadTileset.Text = "Load Tileset";
            this.btnLoadTileset.Width = 140;
            this.btnLoadTileset.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.btnUndo.Text = "Undo";
            this.btnUndo.Width = 100;
            this.btnUndo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.btnRedo.Text = "Redo";
            this.btnRedo.Width = 100;
            this.btnRedo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);

            this.flowActions.Controls.Add(this.groupActionsHeader);
            this.flowActions.Controls.Add(this.btnLoadTileset);
            this.flowActions.Controls.Add(this.btnUndo);
            this.flowActions.Controls.Add(this.btnRedo);

            this.tabActions.Controls.Add(this.flowActions);

            // add tabs to right tabcontrol
            this.panelRight.Controls.Add(this.tabControlRight);

            // --- panelTileset (main area) ---
            this.panelTileset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTileset.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTileset.AutoScroll = true;

            // --- statusStrip ---
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.toolStripStatusLabel });
            this.toolStripStatusLabel.Text = "Ready";

            // context menu
            this.ctxTile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.miRotate, this.miFlip, this.miCopy, this.miPaste, this.miDelete, this.miEdit
            });
            this.miRotate.Text = "Rotate 90°";
            this.miFlip.Text = "Flip Horizontal";
            this.miCopy.Text = "Copy";
            this.miPaste.Text = "Paste";
            this.miDelete.Text = "Delete";
            this.miEdit.Text = "Edit Index...";

            // add controls to form (z-order: menu, top, main, right, status)
            this.Controls.Add(this.panelTileset);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);

            this.MainMenuStrip = this.menuStrip;
            this.Text = "Tileset Editor";
            this.ClientSize = new System.Drawing.Size(1280, 800);
        }

        // Controls declarations
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miExit;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTilesetInfo;

        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.TabControl tabControlRight;
        private System.Windows.Forms.TabPage tabSlice;
        private System.Windows.Forms.TabPage tabActions;
        private System.Windows.Forms.FlowLayoutPanel flowSlice;
        private System.Windows.Forms.FlowLayoutPanel flowActions;

        private System.Windows.Forms.Label groupSliceHeader;
        private System.Windows.Forms.Label lblTileSize;
        private System.Windows.Forms.ComboBox cbPresetSize;
        private System.Windows.Forms.CheckBox chkCustom;
        private System.Windows.Forms.NumericUpDown numCustomWidth;
        private System.Windows.Forms.NumericUpDown numCustomHeight;
        private System.Windows.Forms.Button btnSlice;

        private System.Windows.Forms.Label groupGridHeader;
        private System.Windows.Forms.CheckBox chkUseCustomGrid;
        private System.Windows.Forms.Label lblGridCols;
        private System.Windows.Forms.NumericUpDown numGridCols;
        private System.Windows.Forms.Label lblGridRows;
        private System.Windows.Forms.NumericUpDown numGridRows;

        private System.Windows.Forms.Label groupZoomHeader;
        private System.Windows.Forms.CheckBox chkZoom;
        private System.Windows.Forms.Label lblDisplaySize;
        private System.Windows.Forms.NumericUpDown numDisplaySize;

        private System.Windows.Forms.Label groupActionsHeader;
        private System.Windows.Forms.Button btnLoadTileset;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnRedo;

        private System.Windows.Forms.Panel panelTileset;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;

        private System.Windows.Forms.ContextMenuStrip ctxTile;
        private System.Windows.Forms.ToolStripMenuItem miRotate;
        private System.Windows.Forms.ToolStripMenuItem miFlip;
        private System.Windows.Forms.ToolStripMenuItem miCopy;
        private System.Windows.Forms.ToolStripMenuItem miPaste;
        private System.Windows.Forms.ToolStripMenuItem miDelete;
        private System.Windows.Forms.ToolStripMenuItem miEdit;
    }
}
