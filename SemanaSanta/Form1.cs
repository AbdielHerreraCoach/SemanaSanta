using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SemanaSanta
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCargarArchivo_Click(object sender, EventArgs e)
        {
            OpenFileDialog explorador = new OpenFileDialog();
            explorador.Title = "Selecciona tu archivo de datos";
            explorador.Filter = "Archivos Soportados (*.csv;*.json;*.xml;*.txt)|*.csv;*.json;*.xml;*.txt|Todos los archivos (*.*)|*.*";

            if (explorador.ShowDialog() == DialogResult.OK)
            {
                string rutaSeleccionada = explorador.FileName;
                string nombreArchivo = Path.GetFileName(rutaSeleccionada);
                string extension = Path.GetExtension(rutaSeleccionada).ToLower();

                List<RegistroDinamico> nuevosDatos = new List<RegistroDinamico>();

                try
                {
                    if (extension == ".csv")
                    {
                        nuevosDatos = LectorDatos.LeerCSV(rutaSeleccionada, nombreArchivo);
                    }
                    else if (extension == ".json")
                    {
                        nuevosDatos = LectorDatos.LeerJSON(rutaSeleccionada, nombreArchivo);
                    }
                    else if (extension == ".xml")
                    {
                        nuevosDatos = LectorDatos.LeerXML(rutaSeleccionada, nombreArchivo);
                    }
                    else if (extension == ".txt")
                    {
                        nuevosDatos = LectorDatos.LeerTXT(rutaSeleccionada, nombreArchivo);
                    }
                    else
                    {
                        MessageBox.Show("Formato no soportado.", "Error");
                        return;
                    }

                    MostrarDatosEnGrid(nuevosDatos);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al leer el archivo: {ex.Message}", "Evento Sorpresa");
                }
            }
        }

        private void MostrarDatosEnGrid(List<RegistroDinamico> datos)
        {
            dgvDatos.Columns.Clear();
            dgvDatos.Rows.Clear();

            if (datos.Count == 0) return;

            RegistroDinamico primerRegistro = datos[0];
            dgvDatos.Columns.Add("Origen", "Origen de Datos");

            foreach (var nombreColumna in primerRegistro.Campos.Keys)
            {
                dgvDatos.Columns.Add(nombreColumna, nombreColumna);
            }

            foreach (var registro in datos)
            {
                List<string> valoresFila = new List<string>();
                valoresFila.Add(registro.OrigenDatos);

                foreach (var nombreColumna in primerRegistro.Campos.Keys)
                {
                    if (registro.Campos.ContainsKey(nombreColumna))
                    {
                        valoresFila.Add(registro.Campos[nombreColumna]);
                    }
                    else
                    {
                        valoresFila.Add("");
                    }
                }
                dgvDatos.Rows.Add(valoresFila.ToArray());
            }
        }
    }
}