namespace Compilador.UI.Forms
{
    partial class FrmCompilador
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            toolStrip1 = new ToolStrip();
            btnNuevo = new ToolStripButton();
            btnAbrir = new ToolStripButton();
            btnGuardar = new ToolStripButton();
            btnCompilar = new ToolStripButton();
            btnTema = new ToolStripButton();
            btnSalir = new ToolStripButton();
            splitMain = new SplitContainer();
            splitEditor = new SplitContainer();
            txtTokens = new TextBox();
            splitBottom = new SplitContainer();
            txtEstatus = new TextBox();
            gridSimbolos = new DataGridView();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();

            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitEditor).BeginInit();
            splitEditor.Panel2.SuspendLayout();
            splitEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitBottom).BeginInit();
            splitBottom.Panel1.SuspendLayout();
            splitBottom.Panel2.SuspendLayout();
            splitBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridSimbolos).BeginInit();
            SuspendLayout();

            // ── toolStrip1 ──────────────────────────────────────────────────
            toolStrip1.BackColor = Color.FromArgb(245, 247, 252);
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.ImageScalingSize = new Size(28, 28);
            toolStrip1.Items.AddRange(new ToolStripItem[]
            {
                btnNuevo, btnAbrir, btnGuardar, btnCompilar, btnTema, btnSalir
            });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(10, 4, 10, 4);
            toolStrip1.RenderMode = ToolStripRenderMode.Professional;
            toolStrip1.Size = new Size(1200, 62);
            toolStrip1.TabIndex = 1;

            // ── Botones ─────────────────────────────────────────────────────
            ConfigurarBoton(btnNuevo, "Nuevo", Properties.Resources.new_document);
            ConfigurarBoton(btnAbrir, "Abrir", Properties.Resources.folder);
            ConfigurarBoton(btnGuardar, "Guardar", Properties.Resources.diskette);
            ConfigurarBoton(btnCompilar, "Compilar", Properties.Resources.compiler);
            ConfigurarBoton(btnTema, "Tema", Properties.Resources.icon_nuevo);
            ConfigurarBoton(btnSalir, "Salir", Properties.Resources.logout);

            btnCompilar.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            btnCompilar.ForeColor = Color.FromArgb(37, 99, 235);

            btnNuevo.Click += btnNuevo_Click;
            btnAbrir.Click += btnAbrir_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnCompilar.Click += btnCompilar_Click;

            // ── splitMain ───────────────────────────────────────────────────
            splitMain.BackColor = Color.FromArgb(226, 232, 245);
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 62);
            splitMain.Name = "splitMain";
            splitMain.Orientation = Orientation.Horizontal;
            splitMain.Panel1.Controls.Add(splitEditor);
            splitMain.Panel2.Controls.Add(splitBottom);
            splitMain.Size = new Size(1200, 638);
            splitMain.SplitterDistance = 440;
            splitMain.SplitterWidth = 4;
            splitMain.TabIndex = 0;

            // ── splitEditor ─────────────────────────────────────────────────
            splitEditor.BackColor = Color.FromArgb(226, 232, 245);
            splitEditor.Dock = DockStyle.Fill;
            splitEditor.Location = new Point(0, 0);
            splitEditor.Name = "splitEditor";
            splitEditor.Panel2.Controls.Add(txtTokens);
            splitEditor.Size = new Size(1200, 440);
            splitEditor.SplitterDistance = 860;
            splitEditor.SplitterWidth = 4;
            splitEditor.TabIndex = 0;

            // ── txtTokens ───────────────────────────────────────────────────
            txtTokens.BackColor = Color.FromArgb(241, 245, 255);
            txtTokens.BorderStyle = BorderStyle.None;
            txtTokens.Dock = DockStyle.Fill;
            txtTokens.Font = new Font("Consolas", 10.5F);
            txtTokens.ForeColor = Color.FromArgb(37, 99, 235);
            txtTokens.Location = new Point(0, 0);
            txtTokens.Multiline = true;
            txtTokens.Name = "txtTokens";
            txtTokens.Padding = new Padding(10);
            txtTokens.ReadOnly = true;
            txtTokens.ScrollBars = ScrollBars.Vertical;
            txtTokens.Size = new Size(335, 440);
            txtTokens.TabIndex = 0;
            txtTokens.Text = "— tokens —";

            // ── splitBottom ─────────────────────────────────────────────────
            splitBottom.BackColor = Color.FromArgb(226, 232, 245);
            splitBottom.Dock = DockStyle.Fill;
            splitBottom.Location = new Point(0, 0);
            splitBottom.Name = "splitBottom";
            splitBottom.Panel1.Controls.Add(txtEstatus);
            splitBottom.Panel2.Controls.Add(gridSimbolos);
            splitBottom.Size = new Size(1200, 193);
            splitBottom.SplitterDistance = 860;
            splitBottom.SplitterWidth = 4;
            splitBottom.TabIndex = 0;

            // ── txtEstatus ──────────────────────────────────────────────────
            txtEstatus.BackColor = Color.FromArgb(248, 250, 255);
            txtEstatus.BorderStyle = BorderStyle.None;
            txtEstatus.Dock = DockStyle.Fill;
            txtEstatus.Font = new Font("Consolas", 10F);
            txtEstatus.ForeColor = Color.FromArgb(51, 65, 85);
            txtEstatus.Location = new Point(0, 0);
            txtEstatus.Multiline = true;
            txtEstatus.Name = "txtEstatus";
            txtEstatus.Padding = new Padding(10, 8, 10, 8);
            txtEstatus.ReadOnly = true;
            txtEstatus.ScrollBars = ScrollBars.Vertical;
            txtEstatus.Size = new Size(860, 193);
            txtEstatus.TabIndex = 0;
            txtEstatus.Text = "Estatus del compilador";

            // ── gridSimbolos ────────────────────────────────────────────────
            gridSimbolos.AllowUserToAddRows = false;
            gridSimbolos.AllowUserToDeleteRows = false;
            gridSimbolos.BackgroundColor = Color.FromArgb(241, 245, 255);
            gridSimbolos.BorderStyle = BorderStyle.None;
            gridSimbolos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridSimbolos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            gridSimbolos.ColumnHeadersHeight = 32;

            gridSimbolos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(219, 228, 248);
            gridSimbolos.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(37, 99, 235);
            gridSimbolos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            gridSimbolos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            gridSimbolos.DefaultCellStyle.BackColor = Color.FromArgb(241, 245, 255);
            gridSimbolos.DefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            gridSimbolos.DefaultCellStyle.Font = new Font("Consolas", 9.5f);
            gridSimbolos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(147, 197, 253);
            gridSimbolos.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);

            gridSimbolos.GridColor = Color.FromArgb(203, 213, 235);
            gridSimbolos.Dock = DockStyle.Fill;
            gridSimbolos.EnableHeadersVisualStyles = false;
            gridSimbolos.Location = new Point(0, 0);
            gridSimbolos.Name = "gridSimbolos";
            gridSimbolos.ReadOnly = true;
            gridSimbolos.RowHeadersVisible = false;
            gridSimbolos.RowTemplate.Height = 28;
            gridSimbolos.Size = new Size(335, 193);
            gridSimbolos.TabIndex = 0;

            // ── FrmCompilador ───────────────────────────────────────────────
            BackColor = Color.FromArgb(245, 247, 252);
            ClientSize = new Size(1200, 700);
            Controls.Add(splitMain);
            Controls.Add(toolStrip1);
            Font = new Font("Segoe UI", 9.5F);
            ForeColor = Color.FromArgb(30, 41, 59);
            Name = "FrmCompilador";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Compilador  —  Analizador";
            WindowState = FormWindowState.Maximized;

            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            splitEditor.Panel2.ResumeLayout(false);
            splitEditor.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitEditor).EndInit();
            splitEditor.ResumeLayout(false);
            splitBottom.Panel1.ResumeLayout(false);
            splitBottom.Panel1.PerformLayout();
            splitBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitBottom).EndInit();
            splitBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridSimbolos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void ConfigurarBoton(ToolStripButton btn, string texto, Image imagen)
        {
            btn.Font = new Font("Segoe UI", 8.5F);
            btn.ForeColor = Color.FromArgb(51, 65, 85);
            btn.Image = imagen;
            btn.Margin = new Padding(4, 0, 4, 0);
            btn.Padding = new Padding(8, 4, 8, 4);
            btn.Size = new Size(72, 56);
            btn.Text = texto;
            btn.TextImageRelation = TextImageRelation.ImageAboveText;
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripButton btnNuevo;
        private ToolStripButton btnAbrir;
        private ToolStripButton btnGuardar;
        private ToolStripButton btnCompilar;
        private ToolStripButton btnTema;
        private ToolStripButton btnSalir;
        private SplitContainer splitMain;
        private SplitContainer splitEditor;
        private SplitContainer splitBottom;
        private TextBox txtTokens;
        private TextBox txtEstatus;
        private DataGridView gridSimbolos;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
    }
}