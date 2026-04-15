using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SemanaSanta
{
    public partial class Form1 : Form
    {
        // 🌟 NUESTRA LISTA MAESTRA GLOBAL (NIVEL 4)
        // Aquí se acumulará todo lo que vayamos leyendo
        private List<RegistroDinamico> listaMaestra = new List<RegistroDinamico>();

        public Form1()
        {
            InitializeComponent();
        }

        // ==========================================
        // EVENTO 1: CARGAR DESDE ARCHIVOS LOCALES
        // ==========================================
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
                    if (extension == ".csv") nuevosDatos = LectorDatos.LeerCSV(rutaSeleccionada, nombreArchivo);
                    else if (extension == ".json") nuevosDatos = LectorDatos.LeerJSON(rutaSeleccionada, nombreArchivo);
                    else if (extension == ".xml") nuevosDatos = LectorDatos.LeerXML(rutaSeleccionada, nombreArchivo);
                    else if (extension == ".txt") nuevosDatos = LectorDatos.LeerTXT(rutaSeleccionada, nombreArchivo);
                    else
                    {
                        MessageBox.Show("Formato no soportado.", "Error");
                        return;
                    }

                    // 🌟 NIVEL 4: En lugar de reemplazar, AGREGAMOS a la lista maestra
                    listaMaestra.AddRange(nuevosDatos);

                    // Mostramos toda la lista acumulada
                    MostrarDatosEnGrid(listaMaestra);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al leer el archivo: {ex.Message}", "Evento Sorpresa");
                }
            }
        }

        // ==========================================
        // EVENTO 2: CARGAR DESDE SQL SERVER
        // ==========================================
        private void btnCargarSQL_Click(object sender, EventArgs e)
        {
            // OJO: Recuerda poner aquí el nombre correcto de tu servidor
            string connString = "Server=DULSERVICE\\SQLEXPRESS;Database=TiendaSanta;Trusted_Connection=True;TrustServerCertificate=True;";
            string query = "SELECT * FROM Productos";

            try
            {
                List<RegistroDinamico> datosSQL = LectorDatos.LeerSQL(connString, query, "SQL Server - Productos");

                // 🌟 NIVEL 4: Agregamos lo de SQL a la lista maestra junto con los archivos
                listaMaestra.AddRange(datosSQL);
                MostrarDatosEnGrid(listaMaestra);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar o consultar SQL Server:\n\n{ex.Message}", "Error de Base de Datos");
            }
        }

        // ==========================================
        // EVENTO 3: AGRUPAR DATOS (NIVEL 4)
        // ==========================================
        private void btnAgrupar_Click(object sender, EventArgs e)
        {
            if (listaMaestra.Count == 0)
            {
                MessageBox.Show("Primero carga algunos datos.", "Aviso");
                return;
            }

            // 🌟 EL DICCIONARIO AGRUPADOR
            Dictionary<string, List<RegistroDinamico>> datosAgrupados = new Dictionary<string, List<RegistroDinamico>>();

            foreach (RegistroDinamico registro in listaMaestra)
            {
                string llaveOrigen = registro.OrigenDatos;

                if (!datosAgrupados.ContainsKey(llaveOrigen))
                {
                    datosAgrupados.Add(llaveOrigen, new List<RegistroDinamico>());
                }
                datosAgrupados[llaveOrigen].Add(registro);
            }

            string mensaje = "Tus datos están organizados así:\n\n";
            foreach (var grupo in datosAgrupados)
            {
                mensaje += $"📂 {grupo.Key}: {grupo.Value.Count} registros.\n";
            }

            MessageBox.Show(mensaje, "Nivel 4: Organización Completada");
        }

        // ==========================================
        // EVENTO 4: BUSCADOR EN TIEMPO REAL (NIVEL 5 - Filtrar)
        // ==========================================
        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            // 1. Convertimos lo que el usuario escribió a minúsculas para que la búsqueda no sea sensible a mayúsculas
            string textoBusqueda = txtBuscar.Text.ToLower();

            // 2. Si borró todo y el buscador está vacío, volvemos a mostrar TODA la lista maestra
            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                MostrarDatosEnGrid(listaMaestra);
                return;
            }

            // 3. Creamos una lista temporal solo para los que coincidan
            List<RegistroDinamico> resultadosFiltrados = new List<RegistroDinamico>();

            // 4. Recorremos nuestra caja gigante de datos
            foreach (RegistroDinamico registro in listaMaestra)
            {
                bool coincide = false;

                // Buscamos si ALGÚN valor de las columnas de este registro contiene el texto
                foreach (string valor in registro.Campos.Values)
                {
                    if (valor.ToLower().Contains(textoBusqueda))
                    {
                        coincide = true;
                        break; // Con una coincidencia basta, dejamos de revisar las demás columnas
                    }
                }

                // Si encontramos el texto en alguna columna, agregamos todo el registro a los resultados
                if (coincide)
                {
                    resultadosFiltrados.Add(registro);
                }
            }

            // 5. Le mandamos a nuestra función gráfica SOLO los datos que filtrados
            MostrarDatosEnGrid(resultadosFiltrados);
        }

        // ==========================================
        // LÓGICA GRÁFICA: DIBUJAR LA SÚPER TABLA
        // ==========================================
        private void MostrarDatosEnGrid(List<RegistroDinamico> datos)
        {
            dgvDatos.Columns.Clear();
            dgvDatos.Rows.Clear();

            if (datos.Count == 0) return;

            // 1. Obtener TODAS las columnas únicas (Para mezclar bases distintas sin que explote)
            List<string> todasLasColumnas = new List<string>();
            foreach (var registro in datos)
            {
                foreach (var llave in registro.Campos.Keys)
                {
                    if (!todasLasColumnas.Contains(llave))
                    {
                        todasLasColumnas.Add(llave);
                    }
                }
            }

            // 2. Crear las columnas en el Grid
            dgvDatos.Columns.Add("Origen", "Origen de Datos");
            foreach (string col in todasLasColumnas)
            {
                dgvDatos.Columns.Add(col, col);
            }

            // 3. Llenar las filas
            foreach (var registro in datos)
            {
                List<string> valoresFila = new List<string>();
                valoresFila.Add(registro.OrigenDatos);

                foreach (string col in todasLasColumnas)
                {
                    if (registro.Campos.ContainsKey(col))
                    {
                        valoresFila.Add(registro.Campos[col]);
                    }
                    else
                    {
                        valoresFila.Add(""); // Si el registro no tiene esa columna, se queda en blanco
                    }
                }
                dgvDatos.Rows.Add(valoresFila.ToArray());
            }
        }

        private void txtBuscar_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}