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
            this.miLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();

            this.editToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.miUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.miRedo = new System.Windows.Forms.ToolStripMenuItem();

            this.viewToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.miToggleGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.miGridColor = new System.Windows.Forms.ToolStripMenuItem();
            this.miResetZoom = new System.Windows.Forms.ToolStripMenuItem();

            this.helpToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.miUsage = new System.Windows.Forms.ToolStripMenuItem();
            this.miHotkeys = new System.Windows.Forms.ToolStripMenuItem();
            this.miCredits = new System.Windows.Forms.ToolStripMenuItem();

            // Top info panel
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTilesetInfo = new System.Windows.Forms.Label();
            this.btnApplyGrid = new System.Windows.Forms.Button();

            // Right side panel (controls)
            this.panelRight = new System.Windows.Forms.Panel();

            // Grid controls area
            this.grpGrid = new System.Windows.Forms.GroupBox();
            this.lblGridHeader = new System.Windows.Forms.Label();

            // Column controls
            this.lblCols = new System.Windows.Forms.Label();
            this.btnAddCol = new System.Windows.Forms.Button();
            this.btnRemoveCol = new System.Windows.Forms.Button();

            // Row controls
            this.lblRows = new System.Windows.Forms.Label();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.btnRemoveRow = new System.Windows.Forms.Button();

            // Tile size (custom) controls
            this.lblTileW = new System.Windows.Forms.Label();
            this.numTileW = new System.Windows.Forms.NumericUpDown();
            this.lblTileH = new System.Windows.Forms.Label();
            this.numTileH = new System.Windows.Forms.NumericUpDown();

            // Grid preset (display cell size presets)
            this.lblPreset = new System.Windows.Forms.Label();
            this.cbPresetSize = new System.Windows.Forms.ComboBox();

            // Zoom / View controls
            this.grpView = new System.Windows.Forms.GroupBox();
            this.lblZoom = new System.Windows.Forms.Label();
            this.trackZoom = new System.Windows.Forms.TrackBar();
            this.btnResetZoom = new System.Windows.Forms.Button();
            this.chkShowGrid = new System.Windows.Forms.CheckBox();
            this.btnPickGridColor = new System.Windows.Forms.Button();

            // Actions
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnRedo = new System.Windows.Forms.Button();

            // Main tileset panel
            this.panelTileset = new System.Windows.Forms.Panel();

            // Status strip
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();

            // Context menu (manual show)
            this.ctxTile = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.miFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.miCut = new System.Windows.Forms.ToolStripMenuItem();
            this.miPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.miEdit = new System.Windows.Forms.ToolStripMenuItem();

            // MenuStrip setup
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileToolStripMenu, this.editToolStripMenu, this.viewToolStripMenu, this.helpToolStripMenu
            });

            this.fileToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.miLoad, this.miSave, this.miSaveAs, this.miExit
            });
            this.fileToolStripMenu.Text = "File";
            this.miLoad.Text = "Load Tileset...";
            this.miSave.Text = "Save";
            this.miSaveAs.Text = "Save As...";
            this.miExit.Text = "Exit";

            this.editToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.miUndo, this.miRedo
            });
            this.editToolStripMenu.Text = "Edit";
            this.miUndo.Text = "Undo";
            this.miRedo.Text = "Redo";

            this.viewToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.miToggleGrid, this.miGridColor, this.miResetZoom
            });
            this.viewToolStripMenu.Text = "View";
            this.miToggleGrid.Text = "Show Grid";
            this.miGridColor.Text = "Grid Color...";
            this.miResetZoom.Text = "Reset Zoom";

            this.helpToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.miUsage, this.miHotkeys, this.miCredits
            });
            this.helpToolStripMenu.Text = "Help";
            this.miUsage.Text = "Usage Guide";
            this.miHotkeys.Text = "Hotkeys";
            this.miCredits.Text = "Credits";

            // panelTop
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 56;
            this.panelTop.Padding = new System.Windows.Forms.Padding(10);
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.lblTilesetInfo.Text = "No tileset loaded";
            this.lblTilesetInfo.AutoSize = true;
            this.lblTilesetInfo.Location = new System.Drawing.Point(12, 18);
            this.lblTilesetInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.btnApplyGrid.Text = "Apply Grid / Slice";
            this.btnApplyGrid.SetBounds(240, 12, 140, 32);
            this.panelTop.Controls.Add(this.lblTilesetInfo);
            this.panelTop.Controls.Add(this.btnApplyGrid);

            // panelRight
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Width = 360;
            this.panelRight.Padding = new System.Windows.Forms.Padding(12);
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            // grpGrid
            this.grpGrid.Text = "Grid Controls";
            this.grpGrid.SetBounds(8, 8, 340, 300);
            this.grpGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);

            this.lblGridHeader.Text = "Grid (cols × rows)";
            this.lblGridHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblGridHeader.SetBounds(12, 18, 200, 20);

            // Column controls (labels + buttons)
            this.lblCols.Text = "Columns";
            this.lblCols.SetBounds(12, 48, 80, 20);
            this.btnAddCol.Text = "Add Column";
            this.btnAddCol.SetBounds(100, 44, 100, 26);
            this.btnRemoveCol.Text = "Remove Column";
            this.btnRemoveCol.SetBounds(206, 44, 120, 26);

            // Row controls
            this.lblRows.Text = "Rows";
            this.lblRows.SetBounds(12, 80, 80, 20);
            this.btnAddRow.Text = "Add Row";
            this.btnAddRow.SetBounds(100, 76, 100, 26);
            this.btnRemoveRow.Text = "Remove Row";
            this.btnRemoveRow.SetBounds(206, 76, 120, 26);

            // Tile size (custom) controls (pixel sizes)
            this.lblTileW.Text = "Tile W (px)";
            this.lblTileW.SetBounds(12, 112, 80, 20);
            this.numTileW.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numTileW.Value = new decimal(new int[] { 32, 0, 0, 0 });
            this.numTileW.SetBounds(100, 108, 120, 26);

            this.lblTileH.Text = "Tile H (px)";
            this.lblTileH.SetBounds(12, 144, 80, 20);
            this.numTileH.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numTileH.Value = new decimal(new int[] { 32, 0, 0, 0 });
            this.numTileH.SetBounds(100, 140, 120, 26);

            // Preset combobox
            this.lblPreset.Text = "Grid cell preset (px):";
            this.lblPreset.SetBounds(12, 176, 140, 20);
            this.cbPresetSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPresetSize.Items.AddRange(new object[] { "8", "16", "24", "32", "48", "64", "96", "128", "Custom" });
            this.cbPresetSize.SelectedIndex = 3;
            this.cbPresetSize.SetBounds(160, 172, 120, 26);

            // Add controls to grpGrid
            this.grpGrid.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblGridHeader,
                this.lblCols, this.btnAddCol, this.btnRemoveCol,
                this.lblRows, this.btnAddRow, this.btnRemoveRow,
                this.lblTileW, this.numTileW, this.lblTileH, this.numTileH,
                this.lblPreset, this.cbPresetSize
            });

            // grpView
            this.grpView.Text = "View / Zoom";
            this.grpView.SetBounds(8, 316, 340, 160);
            this.grpView.Font = new System.Drawing.Font("Segoe UI", 9F);

            this.lblZoom.Text = "Zoom:";
            this.lblZoom.SetBounds(12, 20, 40, 20);

            this.trackZoom.SetBounds(12, 44, 316, 45);
            this.trackZoom.Minimum = 10;
            this.trackZoom.Maximum = 400;
            this.trackZoom.Value = 100;
            this.trackZoom.TickFrequency = 10;

            this.btnResetZoom.Text = "Reset Zoom";
            this.btnResetZoom.SetBounds(12, 96, 120, 28);

            this.chkShowGrid.Text = "Show Grid";
            this.chkShowGrid.SetBounds(12, 132, 120, 20);
            this.chkShowGrid.Checked = true;

            this.btnPickGridColor.Text = "Grid Color";
            this.btnPickGridColor.SetBounds(144, 128, 100, 28);

            this.grpView.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblZoom, this.trackZoom, this.btnResetZoom, this.chkShowGrid, this.btnPickGridColor
            });

            // grpActions
            this.grpActions.Text = "Actions";
            this.grpActions.SetBounds(8, 484, 340, 140);
            this.grpActions.Font = new System.Drawing.Font("Segoe UI", 9F);

            this.btnLoad.Text = "Load Tileset";
            this.btnLoad.SetBounds(12, 24, 150, 34);

            this.btnSave.Text = "Save";
            this.btnSave.SetBounds(12, 64, 150, 34);

            this.btnSaveAs.Text = "Save As...";
            this.btnSaveAs.SetBounds(172, 64, 150, 34);

            this.btnUndo.Text = "Undo";
            this.btnUndo.SetBounds(12, 104, 80, 28);

            this.btnRedo.Text = "Redo";
            this.btnRedo.SetBounds(100, 104, 80, 28);

            this.grpActions.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnLoad, this.btnSave, this.btnSaveAs, this.btnUndo, this.btnRedo
            });

            // panelRight add groups
            this.panelRight.Controls.Add(this.grpGrid);
            this.panelRight.Controls.Add(this.grpView);
            this.panelRight.Controls.Add(this.grpActions);

            // panelTileset
            this.panelTileset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTileset.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTileset.AutoScroll = true;

            // statusStrip
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.toolStripStatusLabel });
            this.toolStripStatusLabel.Text = "Ready";

            // context menu
            this.ctxTile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.miRotate, this.miFlip, this.miCopy, this.miCut, this.miPaste, this.miDelete, this.miEdit
            });
            this.miRotate.Text = "Rotate 90°";
            this.miFlip.Text = "Flip Horizontal";
            this.miCopy.Text = "Copy";
            this.miCut.Text = "Cut";
            this.miPaste.Text = "Paste";
            this.miDelete.Text = "Delete";
            this.miEdit.Text = "Edit Index...";

            // add to form
            this.Controls.Add(this.panelTileset);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);

            this.MainMenuStrip = this.menuStrip;
            this.Text = "Tileset Editor";
            this.ClientSize = new System.Drawing.Size(1280, 860);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
        }

        // Controls declarations
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem miLoad;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miSaveAs;
        private System.Windows.Forms.ToolStripMenuItem miExit;

        private System.Windows.Forms.ToolStripMenuItem editToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem miUndo;
        private System.Windows.Forms.ToolStripMenuItem miRedo;

        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem miToggleGrid;
        private System.Windows.Forms.ToolStripMenuItem miGridColor;
        private System.Windows.Forms.ToolStripMenuItem miResetZoom;

        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem miUsage;
        private System.Windows.Forms.ToolStripMenuItem miHotkeys;
        private System.Windows.Forms.ToolStripMenuItem miCredits;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTilesetInfo;
        private System.Windows.Forms.Button btnApplyGrid;

        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.GroupBox grpGrid;
        private System.Windows.Forms.Label lblGridHeader;

        private System.Windows.Forms.Label lblCols;
        private System.Windows.Forms.Button btnAddCol;
        private System.Windows.Forms.Button btnRemoveCol;

        private System.Windows.Forms.Label lblRows;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Button btnRemoveRow;

        private System.Windows.Forms.Label lblTileW;
        private System.Windows.Forms.NumericUpDown numTileW;
        private System.Windows.Forms.Label lblTileH;
        private System.Windows.Forms.NumericUpDown numTileH;

        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.ComboBox cbPresetSize;

        private System.Windows.Forms.GroupBox grpView;
        private System.Windows.Forms.Label lblZoom;
        private System.Windows.Forms.TrackBar trackZoom;
        private System.Windows.Forms.Button btnResetZoom;
        private System.Windows.Forms.CheckBox chkShowGrid;
        private System.Windows.Forms.Button btnPickGridColor;

        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnRedo;

        private System.Windows.Forms.Panel panelTileset;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;

        private System.Windows.Forms.ContextMenuStrip ctxTile;
        private System.Windows.Forms.ToolStripMenuItem miRotate;
        private System.Windows.Forms.ToolStripMenuItem miFlip;
        private System.Windows.Forms.ToolStripMenuItem miCopy;
        private System.Windows.Forms.ToolStripMenuItem miCut;
        private System.Windows.Forms.ToolStripMenuItem miPaste;
        private System.Windows.Forms.ToolStripMenuItem miDelete;
        private System.Windows.Forms.ToolStripMenuItem miEdit;
    }
}
