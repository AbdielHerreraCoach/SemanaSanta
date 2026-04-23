using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SemanaSanta
{
    public partial class Form1 : Form
    {
        // 🌟 ALMACENAMIENTO GLOBAL (NIVEL 4)
        private List<RegistroDinamico> listaMaestra = new List<RegistroDinamico>();

        // Variables para el ordenamiento (Nivel 5)
        private string columnaOrdenAnterior = "";
        private bool ordenAscendente = true;

        public Form1()
        {
            InitializeComponent();

            // 🔌 CONECTAMOS LOS CABLES
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            dgvDatos.ColumnHeaderMouseClick += dgvDatos_ColumnHeaderMouseClick;

            // Configuración inicial de la gráfica (opcional)
            chartDatos.Series.Clear();
        }

        // ==========================================
        // 🛡️ ESCUDO PREVENTIVO (Validación de Entrada)
        // ==========================================
        private string ObtenerFirma(RegistroDinamico reg)
        {
            string firma = reg.OrigenDatos;
            foreach (var val in reg.Campos.Values) firma += "|" + val;
            return firma;
        }

        private void IntegrarDatosSeguros(List<RegistroDinamico> entrantes)
        {
            int agregados = 0; int bloqueados = 0;

            // Creamos un set temporal para saber qué tenemos ya en memoria
            HashSet<string> firmasExistentes = new HashSet<string>();
            foreach (var r in listaMaestra) firmasExistentes.Add(ObtenerFirma(r));

            foreach (var nuevo in entrantes)
            {
                if (!firmasExistentes.Contains(ObtenerFirma(nuevo)))
                {
                    listaMaestra.Add(nuevo);
                    firmasExistentes.Add(ObtenerFirma(nuevo));
                    agregados++;
                }
                else bloqueados++;
            }

            MostrarDatosEnGrid(listaMaestra);
            if (bloqueados > 0)
                MessageBox.Show($"🛡️ Bloqueo Preventivo: Se agregaron {agregados} y se rebotaron {bloqueados} duplicados.");
        }

        // ==========================================
        // 📂 CARGA DE DATOS (NIVELES 1, 2, 3)
        // ==========================================
        private void btnCargarArchivo_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Archivos Soportados|*.csv;*.json;*.xml;*.txt";
            if (op.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(op.FileName).ToLower();
                string nom = Path.GetFileName(op.FileName);
                List<RegistroDinamico> datos = new List<RegistroDinamico>();
                try
                {
                    if (ext == ".csv") datos = LectorDatos.LeerCSV(op.FileName, nom);
                    else if (ext == ".json") datos = LectorDatos.LeerJSON(op.FileName, nom);
                    else if (ext == ".xml") datos = LectorDatos.LeerXML(op.FileName, nom);
                    else if (ext == ".txt") datos = LectorDatos.LeerTXT(op.FileName, nom);

                    IntegrarDatosSeguros(datos);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void btnCargarSQL_Click(object sender, EventArgs e)
        {
            string cs = "Server=DULSERVICE\\SQLEXPRESS;Database=TiendaSanta;Trusted_Connection=True;TrustServerCertificate=True;";
            try
            {
                var sql = LectorDatos.LeerSQL(cs, "SELECT * FROM Productos", "SQL Server");
                IntegrarDatosSeguros(sql);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // ==========================================
        // 🧠 PROCESAMIENTO (AGRUPAR, BUSCAR, ORDENAR)
        // ==========================================
        private void btnAgrupar_Click(object sender, EventArgs e)
        {
            if (listaMaestra.Count == 0) return;
            var grupos = new Dictionary<string, List<RegistroDinamico>>();
            foreach (var r in listaMaestra)
            {
                if (!grupos.ContainsKey(r.OrigenDatos)) grupos.Add(r.OrigenDatos, new List<RegistroDinamico>());
                grupos[r.OrigenDatos].Add(r);
            }
            string res = "Resumen de Diccionario:\n";
            foreach (var g in grupos) res += $"- {g.Key}: {g.Value.Count} registros\n";
            MessageBox.Show(res, "Nivel 4");
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string bus = txtBuscar.Text.ToLower();
            if (string.IsNullOrEmpty(bus)) { MostrarDatosEnGrid(listaMaestra); return; }
            var filtrados = listaMaestra.FindAll(r => {
                foreach (var v in r.Campos.Values) if (v.ToLower().Contains(bus)) return true;
                return false;
            });
            MostrarDatosEnGrid(filtrados);
        }

        private void dgvDatos_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (listaMaestra.Count < 2) return;
            string col = dgvDatos.Columns[e.ColumnIndex].Name;
            ordenAscendente = (col == columnaOrdenAnterior) ? !ordenAscendente : true;
            columnaOrdenAnterior = col;

            // MÉTODO BURBUJA (Nivel 5)
            for (int i = 0; i < listaMaestra.Count - 1; i++)
            {
                for (int j = 0; j < listaMaestra.Count - i - 1; j++)
                {
                    string v1 = col == "Origen" ? listaMaestra[j].OrigenDatos : (listaMaestra[j].Campos.ContainsKey(col) ? listaMaestra[j].Campos[col] : "");
                    string v2 = col == "Origen" ? listaMaestra[j + 1].OrigenDatos : (listaMaestra[j + 1].Campos.ContainsKey(col) ? listaMaestra[j + 1].Campos[col] : "");
                    if (ordenAscendente ? string.Compare(v1, v2) > 0 : string.Compare(v1, v2) < 0)
                    {
                        var temp = listaMaestra[j]; listaMaestra[j] = listaMaestra[j + 1]; listaMaestra[j + 1] = temp;
                    }
                }
            }
            MostrarDatosEnGrid(listaMaestra);
        }

        // ==========================================
        // 💾 EXPORTACIÓN (NIVEL 6)
        // ==========================================
        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (listaMaestra.Count == 0) return;
            SaveFileDialog sd = new SaveFileDialog { Filter = "CSV|*.csv" };
            if (sd.ShowDialog() == DialogResult.OK)
            {
                List<string> lineas = new List<string>();
                List<string> cols = ObtenerColumnasUnicas(listaMaestra);
                lineas.Add("Origen," + string.Join(",", cols));
                foreach (var r in listaMaestra)
                {
                    List<string> f = new List<string> { $"\"{r.OrigenDatos}\"" };
                    foreach (var c in cols) f.Add($"\"{(r.Campos.ContainsKey(c) ? r.Campos[c] : "")}\"");
                    lineas.Add(string.Join(",", f));
                }
                File.WriteAllLines(sd.FileName, lineas);
                MessageBox.Show("Exportación exitosa.");
            }
        }

        // ==========================================
        // 📊 PANEL VISUAL (CHART)
        // ==========================================
        private void ActualizarGrafico(List<RegistroDinamico> datos)
        {
            chartDatos.Series.Clear();
            if (datos.Count == 0) return;

            Series s = new Series("Datos") { ChartType = SeriesChartType.Pie };
            s.IsValueShownAsLabel = true;

            Dictionary<string, int> conteo = new Dictionary<string, int>();
            foreach (var r in datos)
            {
                if (!conteo.ContainsKey(r.OrigenDatos)) conteo.Add(r.OrigenDatos, 0);
                conteo[r.OrigenDatos]++;
            }

            foreach (var par in conteo) s.Points.AddXY(par.Key, par.Value);
            chartDatos.Series.Add(s);
        }

        // ==========================================
        // LÓGICA GRÁFICA (GRID)
        // ==========================================
        private void MostrarDatosEnGrid(List<RegistroDinamico> datos)
        {
            dgvDatos.Columns.Clear(); dgvDatos.Rows.Clear();
            if (datos.Count == 0) return;
            List<string> cols = ObtenerColumnasUnicas(datos);
            dgvDatos.Columns.Add("Origen", "Origen");
            foreach (var c in cols) dgvDatos.Columns.Add(c, c);
            foreach (var r in datos)
            {
                List<string> f = new List<string> { r.OrigenDatos };
                foreach (var c in cols) f.Add(r.Campos.ContainsKey(c) ? r.Campos[c] : "");
                dgvDatos.Rows.Add(f.ToArray());
            }
            // 📈 Al final, refrescamos la gráfica automáticamente
            ActualizarGrafico(datos);
        }

        private List<string> ObtenerColumnasUnicas(List<RegistroDinamico> lista)
        {
            List<string> c = new List<string>();
            foreach (var r in lista) foreach (var k in r.Campos.Keys) if (!c.Contains(k)) c.Add(k);
            return c;
        }
    }
}