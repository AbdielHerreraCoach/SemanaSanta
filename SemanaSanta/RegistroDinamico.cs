using System.Collections.Generic;

namespace SemanaSanta
{
    public class RegistroDinamico
    {
        public string OrigenDatos { get; set; }
        public Dictionary<string, string> Campos { get; set; }

        public RegistroDinamico()
        {
            Campos = new Dictionary<string, string>();
        }
    }
}