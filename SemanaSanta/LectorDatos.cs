using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace SemanaSanta
{
    public class LectorDatos
    {
        // ==========================================
        // 1. LECTOR DE CSV (Con parser inteligente)
        // ==========================================
        public static List<RegistroDinamico> LeerCSV(string rutaArchivo, string nombreOrigen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();
            string[] lineas = File.ReadAllLines(rutaArchivo);

            if (lineas.Length <= 1) return lista;

            string[] cabeceras = SepararFilaCSV(lineas[0]);

            for (int i = 1; i < lineas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lineas[i])) continue;

                string[] valores = SepararFilaCSV(lineas[i]);
                RegistroDinamico nuevoRegistro = new RegistroDinamico();
                nuevoRegistro.OrigenDatos = nombreOrigen;

                for (int j = 0; j < cabeceras.Length; j++)
                {
                    string valorCelda = (j < valores.Length) ? valores[j] : "";
                    valorCelda = valorCelda.Trim('"');
                    nuevoRegistro.Campos.Add(cabeceras[j], valorCelda);
                }
                lista.Add(nuevoRegistro);
            }
            return lista;
        }

        private static string[] SepararFilaCSV(string linea)
        {
            List<string> columnas = new List<string>();
            bool dentroDeComillas = false;
            string valorActual = "";

            foreach (char letra in linea)
            {
                if (letra == '"') dentroDeComillas = !dentroDeComillas;
                else if (letra == ',' && !dentroDeComillas)
                {
                    columnas.Add(valorActual);
                    valorActual = "";
                }
                else valorActual += letra;
            }
            columnas.Add(valorActual);
            return columnas.ToArray();
        }

        // ==========================================
        // 2. LECTOR DE JSON
        // ==========================================
        public static List<RegistroDinamico> LeerJSON(string rutaArchivo, string nombreOrigen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();
            string jsonString = File.ReadAllText(rutaArchivo);

            using (JsonDocument documento = JsonDocument.Parse(jsonString))
            {
                JsonElement raiz = documento.RootElement;
                if (raiz.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement elemento in raiz.EnumerateArray())
                    {
                        RegistroDinamico nuevoRegistro = new RegistroDinamico();
                        nuevoRegistro.OrigenDatos = nombreOrigen;
                        foreach (JsonProperty propiedad in elemento.EnumerateObject())
                        {
                            nuevoRegistro.Campos.Add(propiedad.Name, propiedad.Value.ToString());
                        }
                        lista.Add(nuevoRegistro);
                    }
                }
            }
            return lista;
        }

        // ==========================================
        // 3. LECTOR DE XML
        // ==========================================
        public static List<RegistroDinamico> LeerXML(string rutaArchivo, string nombreOrigen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();
            XDocument doc = XDocument.Load(rutaArchivo);

            foreach (XElement nodo in doc.Root.Elements())
            {
                RegistroDinamico nuevoRegistro = new RegistroDinamico();
                nuevoRegistro.OrigenDatos = nombreOrigen;

                foreach (XElement campo in nodo.Elements())
                {
                    nuevoRegistro.Campos.Add(campo.Name.LocalName, campo.Value);
                }
                lista.Add(nuevoRegistro);
            }
            return lista;
        }

        // ==========================================
        // 4. LECTOR DE TXT (Con múltiples separadores)
        // ==========================================
        public static List<RegistroDinamico> LeerTXT(string rutaArchivo, string nombreOrigen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();
            string[] lineas = File.ReadAllLines(rutaArchivo);

            if (lineas.Length <= 1) return lista;

            char[] separadores = new char[] { '|', ';', '\t', '^' };
            string[] cabeceras = lineas[0].Split(separadores);

            for (int i = 1; i < lineas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lineas[i])) continue;

                string[] valores = lineas[i].Split(separadores);
                RegistroDinamico nuevoRegistro = new RegistroDinamico();
                nuevoRegistro.OrigenDatos = nombreOrigen;

                for (int j = 0; j < cabeceras.Length; j++)
                {
                    string valorCelda = (j < valores.Length) ? valores[j].Trim() : "";
                    nuevoRegistro.Campos.Add(cabeceras[j].Trim(), valorCelda);
                }
                lista.Add(nuevoRegistro);
            }
            return lista;
        }

        // ==========================================
        // 5. LECTOR DE SQL SERVER (NIVEL 3)
        // ==========================================
        public static List<RegistroDinamico> LeerSQL(string connectionString, string consulta, string nombreOrigen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RegistroDinamico nuevoRegistro = new RegistroDinamico();
                            nuevoRegistro.OrigenDatos = nombreOrigen;

                            // Recorremos todas las columnas dinámicamente
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string nombreColumna = reader.GetName(i);
                                // Evitamos errores si en la base de datos hay valores NULL
                                string valor = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString();

                                nuevoRegistro.Campos.Add(nombreColumna, valor);
                            }
                            lista.Add(nuevoRegistro);
                        }
                    }
                }
            }
            return lista;
        }
    }
}