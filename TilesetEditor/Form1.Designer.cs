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

            // main controls
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();

            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTilesetInfo = new System.Windows.Forms.Label();
            this.groupSlice = new System.Windows.Forms.GroupBox();
            this.lblTileSize = new System.Windows.Forms.Label();
            this.cbPresetSize = new System.Windows.Forms.ComboBox();
            this.chkCustom = new System.Windows.Forms.CheckBox();
            this.numCustomWidth = new System.Windows.Forms.NumericUpDown();
            this.numCustomHeight = new System.Windows.Forms.NumericUpDown();
            this.btnSlice = new System.Windows.Forms.Button();

            this.groupGrid = new System.Windows.Forms.GroupBox();
            this.chkUseCustomGrid = new System.Windows.Forms.CheckBox();
            this.lblGridCols = new System.Windows.Forms.Label();
            this.numGridCols = new System.Windows.Forms.NumericUpDown();
            this.lblGridRows = new System.Windows.Forms.Label();
            this.numGridRows = new System.Windows.Forms.NumericUpDown();

            this.groupActions = new System.Windows.Forms.GroupBox();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnRedo = new System.Windows.Forms.Button();
            this.btnLoadTileset = new System.Windows.Forms.Button();

            this.chkZoom = new System.Windows.Forms.CheckBox();
            this.lblDisplaySize = new System.Windows.Forms.Label();
            this.numDisplaySize = new System.Windows.Forms.NumericUpDown();

            this.panelTileset = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();

            // context menu (manual show)
            this.ctxTile = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.miFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.miPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.miEdit = new System.Windows.Forms.ToolStripMenuItem();

            // MenuStrip
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fileToolStripMenu });
            this.fileToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.miSave, this.miExit });
            this.fileToolStripMenu.Text = "File";
            this.miSave.Text = "Save Tileset...";
            this.miExit.Text = "Exit";

            // panelTop layout
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 96;
            this.panelTop.Padding = new System.Windows.Forms.Padding(8);
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);

            // lblTilesetInfo
            this.lblTilesetInfo.Text = "No tileset loaded";
            this.lblTilesetInfo.AutoSize = true;
            this.lblTilesetInfo.Location = new System.Drawing.Point(12, 8);

            // groupSlice
            this.groupSlice.Text = "Tile Size";
            this.groupSlice.SetBounds(12, 28, 360, 60);
            this.lblTileSize.Text = "Preset:";
            this.lblTileSize.SetBounds(8, 18, 48, 20);
            this.cbPresetSize.SetBounds(60, 16, 70, 24);
            this.cbPresetSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPresetSize.Items.AddRange(new object[] { "8", "16", "24", "32", "48", "64", "128" });
            this.cbPresetSize.SelectedIndex = 3;
            this.chkCustom.Text = "Custom";
            this.chkCustom.SetBounds(140, 18, 64, 20);
            this.numCustomWidth.SetBounds(210, 14, 60, 24);
            this.numCustomWidth.Minimum = 1;
            this.numCustomWidth.Value = 32;
            this.numCustomHeight.SetBounds(276, 14, 60, 24);
            this.numCustomHeight.Minimum = 1;
            this.numCustomHeight.Value = 32;
            this.btnSlice.Text = "Slice";
            this.btnSlice.SetBounds(340, 14, 60, 24);

            this.groupSlice.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTileSize, this.cbPresetSize, this.chkCustom, this.numCustomWidth, this.numCustomHeight, this.btnSlice
            });

            // groupGrid
            this.groupGrid.Text = "Grid (cols × rows)";
            this.groupGrid.SetBounds(380, 28, 320, 60);
            this.chkUseCustomGrid.Text = "Use custom grid";
            this.chkUseCustomGrid.SetBounds(8, 16, 120, 20);
            this.lblGridCols.Text = "Cols:";
            this.lblGridCols.SetBounds(136, 16, 36, 20);
            this.numGridCols.SetBounds(176, 14, 60, 24);
            this.numGridCols.Minimum = 1;
            this.numGridCols.Value = 10;
            this.lblGridRows.Text = "Rows:";
            this.lblGridRows.SetBounds(240, 16, 36, 20);
            this.numGridRows.SetBounds(276, 14, 36, 24);
            this.numGridRows.Minimum = 1;
            this.numGridRows.Value = 10;
            this.groupGrid.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.chkUseCustomGrid, this.lblGridCols, this.numGridCols, this.lblGridRows, this.numGridRows
            });

            // groupActions
            this.groupActions.Text = "Actions";
            this.groupActions.SetBounds(712, 28, 360, 60);
            this.btnLoadTileset.Text = "Load Tileset";
            this.btnLoadTileset.SetBounds(8, 16, 110, 28);
            this.btnUndo.Text = "Undo";
            this.btnUndo.SetBounds(126, 16, 70, 28);
            this.btnRedo.Text = "Redo";
            this.btnRedo.SetBounds(204, 16, 70, 28);
            this.groupActions.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnLoadTileset, this.btnUndo, this.btnRedo
            });

            // zoom controls
            this.chkZoom.Text = "Preview Zoom";
            this.chkZoom.SetBounds(12, 92, 110, 20);
            this.lblDisplaySize.Text = "Display size:";
            this.lblDisplaySize.SetBounds(132, 92, 80, 20);
            this.numDisplaySize.SetBounds(212, 90, 60, 24);
            this.numDisplaySize.Minimum = 4;
            this.numDisplaySize.Maximum = 512;
            this.numDisplaySize.Value = 32;

            // panelTileset
            this.panelTileset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTileset.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTileset.AutoScroll = true;

            // statusStrip
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

            // assemble top panel
            this.panelTop.Controls.Add(this.lblTilesetInfo);
            this.panelTop.Controls.Add(this.groupSlice);
            this.panelTop.Controls.Add(this.groupGrid);
            this.panelTop.Controls.Add(this.groupActions);
            this.panelTop.Controls.Add(this.chkZoom);
            this.panelTop.Controls.Add(this.lblDisplaySize);
            this.panelTop.Controls.Add(this.numDisplaySize);

            // add to form
            this.Controls.Add(this.panelTileset);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);

            // layout z-order
            this.MainMenuStrip = this.menuStrip;
        }

        // controls
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miExit;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTilesetInfo;
        private System.Windows.Forms.GroupBox groupSlice;
        private System.Windows.Forms.Label lblTileSize;
        private System.Windows.Forms.ComboBox cbPresetSize;
        private System.Windows.Forms.CheckBox chkCustom;
        private System.Windows.Forms.NumericUpDown numCustomWidth;
        private System.Windows.Forms.NumericUpDown numCustomHeight;
        private System.Windows.Forms.Button btnSlice;

        private System.Windows.Forms.GroupBox groupGrid;
        private System.Windows.Forms.CheckBox chkUseCustomGrid;
        private System.Windows.Forms.Label lblGridCols;
        private System.Windows.Forms.NumericUpDown numGridCols;
        private System.Windows.Forms.Label lblGridRows;
        private System.Windows.Forms.NumericUpDown numGridRows;

        private System.Windows.Forms.GroupBox groupActions;
        private System.Windows.Forms.Button btnLoadTileset;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnRedo;

        private System.Windows.Forms.CheckBox chkZoom;
        private System.Windows.Forms.Label lblDisplaySize;
        private System.Windows.Forms.NumericUpDown numDisplaySize;

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
