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
        public static List<RegistroDinamico> LeerCSV(string ruta, string origen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();
            string[] lineas = File.ReadAllLines(ruta);
            if (lineas.Length <= 1) return lista;
            string[] cabeceras = SepararFilaCSV(lineas[0]);
            for (int i = 1; i < lineas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lineas[i])) continue;
                string[] valores = SepararFilaCSV(lineas[i]);
                RegistroDinamico reg = new RegistroDinamico { OrigenDatos = origen };
                for (int j = 0; j < cabeceras.Length; j++)
                    reg.Campos.Add(cabeceras[j], j < valores.Length ? valores[j].Trim('"') : "");
                lista.Add(reg);
            }
            return lista;
        }

        private static string[] SepararFilaCSV(string linea)
        {
            List<string> columnas = new List<string>();
            bool comillas = false; string actual = "";
            foreach (char c in linea)
            {
                if (c == '"') comillas = !comillas;
                else if (c == ',' && !comillas) { columnas.Add(actual); actual = ""; }
                else actual += c;
            }
            columnas.Add(actual); return columnas.ToArray();
        }

        public static List<RegistroDinamico> LeerJSON(string ruta, string origen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();
            string json = File.ReadAllText(ruta);
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement el in doc.RootElement.EnumerateArray())
                    {
                        RegistroDinamico reg = new RegistroDinamico { OrigenDatos = origen };
                        foreach (JsonProperty prop in el.EnumerateObject())
                            reg.Campos.Add(prop.Name, prop.Value.ToString());
                        lista.Add(reg);
                    }
                }
            }
            return lista;
        }

        public static List<RegistroDinamico> LeerXML(string ruta, string origen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();
            XDocument doc = XDocument.Load(ruta);
            foreach (XElement nodo in doc.Root.Elements())
            {
                RegistroDinamico reg = new RegistroDinamico { OrigenDatos = origen };
                foreach (XElement campo in nodo.Elements())
                    reg.Campos.Add(campo.Name.LocalName, campo.Value);
                lista.Add(reg);
            }
            return lista;
        }

        public static List<RegistroDinamico> LeerTXT(string ruta, string origen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();
            string[] lineas = File.ReadAllLines(ruta);
            if (lineas.Length <= 1) return lista;
            char[] sep = { '|', ';', '\t', '^' };
            string[] cab = lineas[0].Split(sep);
            for (int i = 1; i < lineas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lineas[i])) continue;
                string[] val = lineas[i].Split(sep);
                RegistroDinamico reg = new RegistroDinamico { OrigenDatos = origen };
                for (int j = 0; j < cab.Length; j++)
                    reg.Campos.Add(cab[j].Trim(), j < val.Length ? val[j].Trim() : "");
                lista.Add(reg);
            }
            return lista;
        }

        public static List<RegistroDinamico> LeerSQL(string connection, string query, string origen)
        {
            List<RegistroDinamico> lista = new List<RegistroDinamico>();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        RegistroDinamico reg = new RegistroDinamico { OrigenDatos = origen };
                        for (int i = 0; i < rd.FieldCount; i++)
                            reg.Campos.Add(rd.GetName(i), rd.IsDBNull(i) ? "" : rd.GetValue(i).ToString());
                        lista.Add(reg);
                    }
                }
            }
            return lista;
        }
    }
}