using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SemanaSanta
{
    public partial class Form1 : Form
    {
        private List<RegistroDinamico> listaMaestra = new List<RegistroDinamico>();
        private string columnaOrdenAnterior = "";
        private bool ordenAscendente = true;

        public Form1()
        {
            InitializeComponent();
            // Conectamos eventos manualmente para asegurar funcionamiento
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            dgvDatos.ColumnHeaderMouseClick += dgvDatos_ColumnHeaderMouseClick;
        }

        // ==========================================
        // 🛡️ EL ESCUDO PREVENTIVO (Nivel 5 Mejorado)
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

            // Creamos un set de firmas actuales para búsqueda rápida
            HashSet<string> firmasExistentes = new HashSet<string>();
            foreach (var r in listaMaestra) firmasExistentes.Add(ObtenerFirma(r));

            foreach (var nuevo in entrantes)
            {
                if (!firmasExistentes.Contains(ObtenerFirma(nuevo)))
                {
                    listaMaestra.Add(nuevo);
                    firmasExistentes.Add(ObtenerFirma(nuevo)); // Evita duplicados dentro del mismo archivo
                    agregados++;
                }
                else bloqueados++;
            }

            MostrarDatosEnGrid(listaMaestra);
            if (bloqueados > 0)
                MessageBox.Show($"🛡️ Escudo Activo: Se agregaron {agregados} registros y se bloquearon {bloqueados} duplicados.");
        }

        // ==========================================
        // CARGA DE DATOS
        // ==========================================
        private void btnCargarArchivo_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Soportados|*.csv;*.json;*.xml;*.txt";
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
            // CAMBIA 'server' POR EL NOMBRE DE TU INSTANCIA
            string cs = "Server=DULSERVICE\\SQLEXPRESS;Database=TiendaSanta;Trusted_Connection=True;TrustServerCertificate=True;";
            try
            {
                var sql = LectorDatos.LeerSQL(cs, "SELECT * FROM Productos", "SQL Server");
                IntegrarDatosSeguros(sql);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // ==========================================
        // PROCESAMIENTO (Agrupar, Buscar, Ordenar)
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
            string res = "Resumen:\n";
            foreach (var g in grupos) res += $"- {g.Key}: {g.Value.Count}\n";
            MessageBox.Show(res, "Nivel 4: Diccionario");
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string bus = txtBuscar.Text.ToLower();
            if (string.IsNullOrEmpty(bus)) { MostrarDatosEnGrid(listaMaestra); return; }
            var fil = listaMaestra.FindAll(r => {
                foreach (var v in r.Campos.Values) if (v.ToLower().Contains(bus)) return true;
                return false;
            });
            MostrarDatosEnGrid(fil);
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
                    int comp = string.Compare(v1, v2, StringComparison.OrdinalIgnoreCase);
                    if (ordenAscendente ? comp > 0 : comp < 0)
                    {
                        var temp = listaMaestra[j]; listaMaestra[j] = listaMaestra[j + 1]; listaMaestra[j + 1] = temp;
                    }
                }
            }
            MostrarDatosEnGrid(listaMaestra);
        }

        private void MostrarDatosEnGrid(List<RegistroDinamico> datos)
        {
            dgvDatos.Columns.Clear(); dgvDatos.Rows.Clear();
            if (datos.Count == 0) return;
            List<string> cols = new List<string>();
            foreach (var r in datos) foreach (var k in r.Campos.Keys) if (!cols.Contains(k)) cols.Add(k);
            dgvDatos.Columns.Add("Origen", "Origen");
            foreach (var c in cols) dgvDatos.Columns.Add(c, c);
            foreach (var r in datos)
            {
                List<string> f = new List<string> { r.OrigenDatos };
                foreach (var c in cols) f.Add(r.Campos.ContainsKey(c) ? r.Campos[c] : "");
                dgvDatos.Rows.Add(f.ToArray());
            }
        }
    }
}