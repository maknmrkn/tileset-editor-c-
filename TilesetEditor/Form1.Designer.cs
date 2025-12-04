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

            this.panelTileset = new System.Windows.Forms.Panel();
            this.btnLoadTileset = new System.Windows.Forms.Button();
            this.btnSlice = new System.Windows.Forms.Button();
            this.cbPresetSize = new System.Windows.Forms.ComboBox();
            this.chkCustom = new System.Windows.Forms.CheckBox();
            this.numCustomWidth = new System.Windows.Forms.NumericUpDown();
            this.numCustomHeight = new System.Windows.Forms.NumericUpDown();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnRedo = new System.Windows.Forms.Button();

            // Zoom controls
            this.chkZoom = new System.Windows.Forms.CheckBox();
            this.numDisplaySize = new System.Windows.Forms.NumericUpDown();

            // Grid controls (new)
            this.chkUseCustomGrid = new System.Windows.Forms.CheckBox();
            this.numGridCols = new System.Windows.Forms.NumericUpDown();
            this.numGridRows = new System.Windows.Forms.NumericUpDown();

            this.ctxTile = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.miFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.miPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.miEdit = new System.Windows.Forms.ToolStripMenuItem();

            // Form
            this.ClientSize = new System.Drawing.Size(1100, 760);
            this.Text = "Tileset Editor - Single Panel";
            this.BackColor = System.Drawing.Color.White;
            this.ForeColor = System.Drawing.Color.Black;

            // panelTileset (fills form)
            this.panelTileset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTileset.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTileset.AutoScroll = true;

            // toolbar controls (placed inside panelTileset at top)
            int x = 8, y = 8, h = 30, gap = 8;

            this.btnLoadTileset.Text = "Load Tileset";
            this.btnLoadTileset.SetBounds(x, y, 110, h);
            x += 110 + gap;

            this.btnSlice.Text = "Slice";
            this.btnSlice.SetBounds(x, y, 80, h);
            x += 80 + gap;

            this.cbPresetSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPresetSize.Items.AddRange(new object[] { "8", "16", "24", "32", "48", "64", "128" });
            this.cbPresetSize.SelectedIndex = 3;
            this.cbPresetSize.SetBounds(x, y, 70, h);
            x += 70 + gap;

            this.chkCustom.Text = "Custom";
            this.chkCustom.SetBounds(x, y + 4, 70, 20);
            x += 70 + gap;

            this.numCustomWidth.Minimum = 1;
            this.numCustomWidth.Value = 32;
            this.numCustomWidth.SetBounds(x, y, 60, h);
            x += 60 + gap;

            this.numCustomHeight.Minimum = 1;
            this.numCustomHeight.Value = 32;
            this.numCustomHeight.SetBounds(x, y, 60, h);
            x += 60 + gap;

            this.btnUndo.Text = "Undo";
            this.btnUndo.SetBounds(x, y, 70, h);
            x += 70 + gap;

            this.btnRedo.Text = "Redo";
            this.btnRedo.SetBounds(x, y, 70, h);
            x += 70 + gap;

            // Zoom controls
            this.chkZoom.Text = "Preview Zoom";
            this.chkZoom.SetBounds(x, y + 4, 110, 20);
            x += 110 + gap;

            this.numDisplaySize.Minimum = 4;
            this.numDisplaySize.Maximum = 512;
            this.numDisplaySize.Value = 32;
            this.numDisplaySize.SetBounds(x, y, 60, h);
            x += 60 + gap;

            // Grid controls (new)
            this.chkUseCustomGrid.Text = "Use custom grid";
            this.chkUseCustomGrid.SetBounds(x, y + 4, 120, 20);
            x += 120 + gap;

            this.numGridCols.Minimum = 1;
            this.numGridCols.Value = 10;
            this.numGridCols.SetBounds(x, y, 60, h);
            x += 60 + gap;

            this.numGridRows.Minimum = 1;
            this.numGridRows.Value = 10;
            this.numGridRows.SetBounds(x, y, 60, h);
            x += 60 + gap;

            // add toolbar controls into panelTileset (so they stay visible)
            this.panelTileset.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnLoadTileset, this.btnSlice, this.cbPresetSize, this.chkCustom,
                this.numCustomWidth, this.numCustomHeight, this.btnUndo, this.btnRedo,
                this.chkZoom, this.numDisplaySize, this.chkUseCustomGrid, this.numGridCols, this.numGridRows
            });

            // context menu (do NOT assign to panel automatically; we'll show it manually)
            this.ctxTile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.miRotate, this.miFlip, this.miCopy, this.miPaste, this.miDelete, this.miEdit
            });
            this.miRotate.Text = "Rotate 90°";
            this.miFlip.Text = "Flip Horizontal";
            this.miCopy.Text = "Copy";
            this.miPaste.Text = "Paste";
            this.miDelete.Text = "Delete";
            this.miEdit.Text = "Edit Index...";

            // add main control
            this.Controls.Add(this.panelTileset);
        }

        private System.Windows.Forms.Panel panelTileset;
        private System.Windows.Forms.Button btnLoadTileset;
        private System.Windows.Forms.Button btnSlice;
        private System.Windows.Forms.ComboBox cbPresetSize;
        private System.Windows.Forms.CheckBox chkCustom;
        private System.Windows.Forms.NumericUpDown numCustomWidth;
        private System.Windows.Forms.NumericUpDown numCustomHeight;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnRedo;

        private System.Windows.Forms.CheckBox chkZoom;
        private System.Windows.Forms.NumericUpDown numDisplaySize;

        // Grid controls
        private System.Windows.Forms.CheckBox chkUseCustomGrid;
        private System.Windows.Forms.NumericUpDown numGridCols;
        private System.Windows.Forms.NumericUpDown numGridRows;

        private System.Windows.Forms.ContextMenuStrip ctxTile;
        private System.Windows.Forms.ToolStripMenuItem miRotate;
        private System.Windows.Forms.ToolStripMenuItem miFlip;
        private System.Windows.Forms.ToolStripMenuItem miCopy;
        private System.Windows.Forms.ToolStripMenuItem miPaste;
        private System.Windows.Forms.ToolStripMenuItem miDelete;
        private System.Windows.Forms.ToolStripMenuItem miEdit;
    }
}
