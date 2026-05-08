using System;
using System.Collections.Generic;

namespace Compilador.UI.CORE
{
    public class Simbolo
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public int Linea { get; set; }

        public Simbolo(string nombre, string tipo, int linea)
        {
            Nombre = nombre;
            Tipo = tipo;
            Linea = linea;
        }
    }

    public class TablaSimbolos
    {
        private Dictionary<string, Simbolo> _tabla = new Dictionary<string, Simbolo>();

        public List<string> Errores { get; } = new List<string>();

        public void Declarar(string nombre, string tipo, int linea)
        {
            if (_tabla.ContainsKey(nombre.ToLower()))
            {
                Errores.Add($"Error semántico en línea {linea}: " +
                            $"La variable '{nombre}' ya fue declarada.");
                return;
            }

            string tipoNorm = NormalizarTipo(tipo);
            if (tipoNorm == null)
            {
                Errores.Add($"Error semántico en línea {linea}: " +
                            $"Tipo de dato '{tipo}' no válido. Solo se permite INT o FLOAT.");
                return;
            }

            _tabla[nombre.ToLower()] = new Simbolo(nombre, tipoNorm, linea);
        }

        public Simbolo Buscar(string nombre, int linea)
        {
            if (_tabla.TryGetValue(nombre.ToLower(), out Simbolo s))
                return s;

            Errores.Add($"Error semántico en línea {linea}: " +
                        $"La variable '{nombre}' no ha sido declarada.");
            return null;
        }

        public void VerificarAsignacion(string nombreVar, string tipoValor, int linea)
        {
            Simbolo s = Buscar(nombreVar, linea);
            if (s == null) return;

            if (s.Tipo != tipoValor)
            {
                if (s.Tipo == "FLOAT" && tipoValor == "INT")
                    return;

                Errores.Add($"Error semántico en línea {linea}: " +
                            $"No se puede asignar un valor {tipoValor} " +
                            $"a la variable '{nombreVar}' de tipo {s.Tipo}.");
            }
        }

        public string ObtenerTipo(string nombre, int linea)
        {
            Simbolo s = Buscar(nombre, linea);
            return s?.Tipo;
        }

        public void Limpiar()
        {
            _tabla.Clear();
            Errores.Clear();
        }

        public IEnumerable<Simbolo> ObtenerTodos() => _tabla.Values;

        private string NormalizarTipo(string tipo)
        {
            switch (tipo.ToUpper())
            {
                case "INT":
                case "INTEGER": return "INT";
                case "FLOAT":
                case "REAL": return "FLOAT";
                default: return null;
            }
        }
    }
}