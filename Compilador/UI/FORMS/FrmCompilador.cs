using Compilador.Core;
using Compilador.UI.CORE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compilador.UI.Forms
{
    public partial class FrmCompilador : Form
    {
        private RichTextBox txtEditor;
        private Panel pnlLineNumbers;
        private bool isDarkTheme = false;

        public FrmCompilador()
        {
            InitializeComponent();
            InicializarEditor();
            InicializarGridSimbolos();
            AplicarTemaClaro();

            btnTema.Click += BtnTema_Click;
            btnSalir.Click += btnSalir_Click;
        }

        private void InicializarGridSimbolos()
        {
            gridSimbolos.Columns.Clear();
            gridSimbolos.Columns.Add("colNombre", "Nombre");
            gridSimbolos.Columns.Add("colTipo", "Tipo");
            gridSimbolos.Columns.Add("colLinea", "Linea");

            gridSimbolos.Columns["colNombre"].Width = 120;
            gridSimbolos.Columns["colTipo"].Width = 80;
            gridSimbolos.Columns["colLinea"].Width = 60;

            gridSimbolos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridSimbolos.AllowUserToAddRows = false;
            gridSimbolos.ReadOnly = true;
        }

        private void InicializarEditor()
        {
            pnlLineNumbers = new Panel
            {
                Dock = DockStyle.Left,
                Width = 50
            };
            pnlLineNumbers.Paint += PnlLineNumbers_Paint;

            txtEditor = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 11F),
                WordWrap = false,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            txtEditor.VScroll += (s, e) => pnlLineNumbers.Invalidate();
            txtEditor.TextChanged += (s, e) => pnlLineNumbers.Invalidate();
            txtEditor.Resize += (s, e) => pnlLineNumbers.Invalidate();

            splitEditor.Panel1.Controls.Add(txtEditor);
            splitEditor.Panel1.Controls.Add(pnlLineNumbers);
        }

        private void PnlLineNumbers_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(pnlLineNumbers.BackColor);

            int firstLine = txtEditor.GetLineFromCharIndex(
                txtEditor.GetCharIndexFromPosition(new Point(0, 0)));

            int lastLine = txtEditor.GetLineFromCharIndex(
                txtEditor.GetCharIndexFromPosition(new Point(0, txtEditor.Height)));

            int lineHeight = txtEditor.Font.Height;
            int y = 2;

            for (int i = firstLine; i <= lastLine + 1; i++)
            {
                string line = (i + 1).ToString();
                SizeF size = e.Graphics.MeasureString(line, txtEditor.Font);

                e.Graphics.DrawString(
                    line,
                    txtEditor.Font,
                    Brushes.Gray,
                    pnlLineNumbers.Width - size.Width - 5,
                    y
                );

                y += lineHeight;
            }
        }

        private void BtnTema_Click(object sender, EventArgs e)
        {
            isDarkTheme = !isDarkTheme;

            if (isDarkTheme)
                AplicarTemaOscuro();
            else
                AplicarTemaClaro();
        }

        private void AplicarTemaClaro()
        {
            txtEditor.BackColor = Color.White;
            txtEditor.ForeColor = Color.Black;
            txtTokens.BackColor = Color.White;
            txtTokens.ForeColor = Color.Black;
            txtEstatus.BackColor = Color.White;
            txtEstatus.ForeColor = Color.Black;
            gridSimbolos.BackgroundColor = Color.White;
            gridSimbolos.DefaultCellStyle.BackColor = Color.White;
            gridSimbolos.DefaultCellStyle.ForeColor = Color.Black;
            pnlLineNumbers.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void AplicarTemaOscuro()
        {
            txtEditor.BackColor = Color.FromArgb(30, 30, 30);
            txtEditor.ForeColor = Color.Gainsboro;
            txtTokens.BackColor = Color.FromArgb(30, 30, 30);
            txtTokens.ForeColor = Color.Gainsboro;
            txtEstatus.BackColor = Color.FromArgb(30, 30, 30);
            txtEstatus.ForeColor = Color.Gainsboro;
            gridSimbolos.BackgroundColor = Color.FromArgb(30, 30, 30);
            gridSimbolos.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            gridSimbolos.DefaultCellStyle.ForeColor = Color.Gainsboro;
            pnlLineNumbers.BackColor = Color.FromArgb(45, 45, 48);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            try
            {
                txtEditor.Clear();
                txtTokens.Clear();
                txtEstatus.Clear();
                gridSimbolos.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al limpiar los campos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Limpiar();
                    string filePath = openFileDialog1.FileName;
                    string fileContent = File.ReadAllText(filePath);
                    txtEditor.Text = fileContent;

                    txtEstatus.AppendText("Archivo abierto: " + filePath + Environment.NewLine);
                }
                else
                {
                    txtEstatus.AppendText("Operación de apertura cancelada." + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el archivo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog1.FileName;
                    File.WriteAllText(filePath, txtEditor.Text);
                    txtEstatus.AppendText("Archivo guardado: " + filePath + Environment.NewLine);
                }
                else
                {
                    txtEstatus.AppendText("Operación de guardado cancelada." + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el archivo: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {
            txtEstatus.Clear();
            txtTokens.Clear();
            gridSimbolos.Rows.Clear();

            // ── FASE 1: LÉXICO ────────────────────────────────────────
            txtEstatus.AppendText("Fase 1 [Léxico ] INICIADO" + Environment.NewLine);
            txtEstatus.AppendText("Ha iniciado el analizador léxico" + Environment.NewLine);

            var fuente = CodigoFuente.DesdeTexto(txtEditor.Text);
            var analizador = new AnalizadorLexico();
            var resultado = analizador.Analizar(fuente);

            foreach (var aviso in resultado.Avisos)
                txtEstatus.AppendText(aviso + Environment.NewLine);

            txtEstatus.AppendText($"Líneas procesadas: {fuente.NumeroLineas}" + Environment.NewLine);
            txtEstatus.AppendText($"Total de tokens: {resultado.Tokens.Count}" + Environment.NewLine);

            txtTokens.Text = resultado.ObtenerTokensAgrupados();

            txtEstatus.AppendText("Analisis Léxico finalizado con éxito" + Environment.NewLine);

            // ── FASE 2: SINTÁCTICO ────────────────────────────────────
            txtEstatus.AppendText("Fase 2 [Sintáctico] INICIADO" + Environment.NewLine);

            var analizadorSintactico = new AnalizadorSintactico();
            analizadorSintactico.Parse(resultado.Tokens);

            if (analizadorSintactico.Errores.Count > 0)
            {
                foreach (var error in analizadorSintactico.Errores)
                    txtEstatus.AppendText(error + Environment.NewLine);

                // Si hay errores sintácticos no tiene sentido continuar
                return;
            }

            txtEstatus.AppendText("Análisis Sintáctico finalizado con éxito" + Environment.NewLine);

            // ── FASE 3: SEMÁNTICO ─────────────────────────────────────
            txtEstatus.AppendText("Fase 3 [Semántico] INICIADO" + Environment.NewLine);

            var analizadorSemantico = new AnalizadorSemantico();
            var tablaSimbolos = analizadorSemantico.Analizar(resultado.Tokens);

            // Mostrar errores semánticos
            if (analizadorSemantico.Errores.Count > 0)
            {
                foreach (var error in analizadorSemantico.Errores)
                    txtEstatus.AppendText(error + Environment.NewLine);
            }
            else
            {
                txtEstatus.AppendText("Análisis Semántico finalizado con éxito" + Environment.NewLine);
            }

            // Llenar tabla de símbolos en el DataGridView
            foreach (var simbolo in tablaSimbolos.ObtenerTodos())
                gridSimbolos.Rows.Add(simbolo.Nombre, simbolo.Tipo, simbolo.Linea);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}