using Compilador.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compilador
{
    public class ResultadoLexico
    {
        public List<Token> Tokens { get; } = new List<Token>();
        public List<string> Avisos { get; } = new List<string>();

        public void AgregarAviso(string mensaje)
        {
            Avisos.Add(mensaje);
        }

        /// 
        /// Devuelve los tokens agrupados por línea en el formato requerido:
        ///   [numLinea] [token1] [token2] [token3] ...
        ///
        /// Ejemplo:
        ///   [1] [102] [101] [311] [103] [300]
        ///   [2] [109] [312] [101] [313]
        ///
        public string ObtenerTokensAgrupados()
        {
            if (Tokens.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            var porLinea = Tokens
                .GroupBy(t => t.Linea)
                .OrderBy(g => g.Key);

            foreach (var grupo in porLinea)
            {
                sb.Append($"[{grupo.Key}]");
                foreach (var tok in grupo)
                    sb.Append($" [{tok.Tipo}]");
                sb.AppendLine();
            }

            return sb.ToString().TrimEnd();
        }
    }
}