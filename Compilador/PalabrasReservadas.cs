using System;
using System.Collections.Generic;

namespace Compilador
{
    /// <summary>
    /// Tokens de palabras reservadas e identificadores.
    ///   101 = identificador
    ///   102 = var
    ///   103 = int / integer
    ///   104 = float
    ///   105 = if
    ///   106 = then
    ///   107 = else
    ///   108 = while
    ///   109 = print
    ///   110 = program
    ///   111 = begin
    ///   112 = end
    ///   113 = write
    ///   114 = writeln
    ///   115 = do
    /// </summary>
    public class PalabrasReservadas
    {
        private readonly Dictionary<string, int> _mapa = new Dictionary<string, int>
        {
            { "PROGRAM", 110 },
            { "VAR",     102 },
            { "INT",     103 },
            { "INTEGER", 103 },
            { "FLOAT",   104 },
            { "IF",      105 },
            { "THEN",    106 },
            { "ELSE",    107 },
            { "WHILE",   108 },
            { "DO",      115 },
            { "BEGIN",   111 },
            { "END",     112 },
            { "PRINT",   109 },
            { "WRITE",   113 },
            { "WRITELN", 114 }
        };

        public int ObtenerToken(string lexema)
        {
            if (string.IsNullOrWhiteSpace(lexema))
                return 101;

            var clave = lexema.ToUpperInvariant();
            return _mapa.TryGetValue(clave, out int token) ? token : 101;
        }
    }
}