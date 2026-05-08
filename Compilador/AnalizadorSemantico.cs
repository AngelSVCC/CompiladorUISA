using Compilador.Core;
using System;
using System.Collections.Generic;

namespace Compilador.UI.CORE
{
    public class AnalizadorSemantico
    {
        private List<Token> _tokens;
        private int _pos;
        private Token _actual;

        private TablaSimbolos _tabla = new TablaSimbolos();
        public List<string> Errores { get; } = new List<string>();

        private const int TKN_ID = 101;
        private const int TKN_VAR = 102;
        private const int TKN_INT = 103;
        private const int TKN_FLOAT = 104;
        private const int TKN_IF = 105;
        private const int TKN_THEN = 106;
        private const int TKN_ELSE = 107;
        private const int TKN_WHILE = 108;
        private const int TKN_PRINT = 109;
        private const int TKN_PROGRAM = 110;
        private const int TKN_BEGIN = 111;
        private const int TKN_END = 112;
        private const int TKN_WRITE = 113;
        private const int TKN_WRITELN = 114;
        private const int TKN_DO = 115;
        private const int TKN_PROCEDURE = 116;

        private const int TKN_ENTERO = 200;
        private const int TKN_REAL = 201;

        private const int TKN_SEMICOL = 300;
        private const int TKN_ASSIGN = 304;
        private const int TKN_COLON = 311;
        private const int TKN_LPAREN = 312;
        private const int TKN_RPAREN = 313;
        private const int TKN_COMMA = 316;
        private const int TKN_DOT = 320;

        private const int TWN_COMENTARIO = 400;

        public TablaSimbolos Analizar(List<Token> tokensTotales)
        {
            _tokens = new List<Token>();
            foreach (var t in tokensTotales)
                if (t.Tipo != TWN_COMENTARIO)
                    _tokens.Add(t);

            Errores.Clear();
            _tabla.Limpiar();
            _pos = 0;
            _actual = _tokens.Count > 0 ? _tokens[0] : null;

            try
            {
                AnalizarPrograma();
            }
            catch (Exception ex)
            {
                Errores.Add("[Error Semántico] " + ex.Message);
            }

            Errores.AddRange(_tabla.Errores);
            return _tabla;
        }

        // ═══════════════════════════════════════════════════════════════
        // HELPERS
        // ═══════════════════════════════════════════════════════════════
        private void Avanzar()
        {
            _pos++;
            _actual = _pos < _tokens.Count ? _tokens[_pos] : null;
        }

        private bool EsTipo(int t)
            => _actual != null && _actual.Tipo == t;

        private bool EsLexema(string l)
            => _actual != null &&
               string.Equals(_actual.Lexema, l, StringComparison.OrdinalIgnoreCase);

        private void Consumir(int tipo)
        {
            if (EsTipo(tipo)) Avanzar();
            else Avanzar();
        }

        private void ConsumirLexema(string l)
        {
            if (EsLexema(l)) Avanzar();
            else Avanzar();
        }

        // Detecta si el lexema actual es un número entero
        private bool EsEntero()
        {
            if (_actual == null) return false;
            // Por tipo oficial
            if (_actual.Tipo == TKN_ENTERO) return true;
            // Por lexema cuando el léxico lo clasifica como ID
            return int.TryParse(_actual.Lexema, out _);
        }

        // Detecta si el lexema actual es un número real
        private bool EsReal()
        {
            if (_actual == null) return false;
            // Por tipo oficial
            if (_actual.Tipo == TKN_REAL) return true;
            // Por lexema cuando el léxico lo clasifica como ID
            if (_actual.Tipo == TKN_ID && _actual.Lexema.Contains("."))
                return double.TryParse(_actual.Lexema,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out _);
            return false;
        }

        // Detecta si el lexema es cualquier número (entero o real)
        private bool EsNumero() => EsReal() || EsEntero();

        // ═══════════════════════════════════════════════════════════════
        // RECORRIDO
        // ═══════════════════════════════════════════════════════════════
        private void AnalizarPrograma()
        {
            Consumir(TKN_PROGRAM);
            Consumir(TKN_ID);
            ConsumirLexema(";");
            AnalizarBloque();
            ConsumirLexema(".");
        }

        private void AnalizarBloque()
        {
            if (EsTipo(TKN_VAR))
                AnalizarDeclaraciones();

            while (EsTipo(TKN_PROCEDURE))
                AnalizarProcedimiento();

            Consumir(TKN_BEGIN);
            AnalizarInstrucciones();
            Consumir(TKN_END);
        }

        private void AnalizarDeclaraciones()
        {
            Consumir(TKN_VAR);

            while (EsTipo(TKN_ID))
            {
                var nombres = new List<(string nombre, int linea)>();
                nombres.Add((_actual.Lexema, _actual.Linea));
                Avanzar();

                while (EsLexema(","))
                {
                    Avanzar();
                    nombres.Add((_actual.Lexema, _actual.Linea));
                    Avanzar();
                }

                ConsumirLexema(":");

                string tipo = _actual?.Lexema ?? "?";
                int linea = _actual?.Linea ?? 0;
                Avanzar();

                ConsumirLexema(";");

                foreach (var (nombre, ln) in nombres)
                    _tabla.Declarar(nombre, tipo, ln);
            }
        }

        private void AnalizarProcedimiento()
        {
            Consumir(TKN_PROCEDURE);
            Consumir(TKN_ID);

            if (EsLexema("("))
            {
                Avanzar();
                if (EsTipo(TKN_ID))
                {
                    Avanzar();
                    ConsumirLexema(":");
                    Avanzar();
                }
                ConsumirLexema(")");
            }

            ConsumirLexema(";");
            AnalizarBloque();
            ConsumirLexema(";");
        }

        private void AnalizarInstrucciones()
        {
            AnalizarInstruccion();

            while (EsLexema(";"))
            {
                Avanzar();
                if (EsTipo(TKN_END) || _actual == null) break;
                AnalizarInstruccion();
            }
        }

        private void AnalizarInstruccion()
        {
            if (_actual == null || EsTipo(TKN_END)) return;

            if (EsTipo(TKN_ID) && !EsNumero())
            {
                string nombreVar = _actual.Lexema;
                int linea = _actual.Linea;
                Avanzar();

                ConsumirLexema(":=");

                string tipoValor = AnalizarExpresion();
                _tabla.VerificarAsignacion(nombreVar, tipoValor, linea);
            }
            else if (EsTipo(TKN_IF))
            {
                Avanzar();
                AnalizarExpresion();
                Consumir(TKN_THEN);
                AnalizarInstruccion();

                if (EsTipo(TKN_ELSE))
                {
                    Avanzar();
                    AnalizarInstruccion();
                }
            }
            else if (EsTipo(TKN_WHILE))
            {
                Avanzar();
                AnalizarExpresion();
                Consumir(TKN_DO);
                AnalizarInstruccion();
            }
            else if (EsTipo(TKN_BEGIN))
            {
                Avanzar();
                AnalizarInstrucciones();
                Consumir(TKN_END);
            }
            else if (EsTipo(TKN_WRITE) || EsTipo(TKN_WRITELN) || EsTipo(TKN_PRINT))
            {
                Avanzar();
                ConsumirLexema("(");
                AnalizarExpresion();

                while (EsLexema(","))
                {
                    Avanzar();
                    AnalizarExpresion();
                }

                ConsumirLexema(")");
            }
            else
            {
                Avanzar();
            }
        }

        private string AnalizarExpresion()
        {
            string tipo = AnalizarTermino();

            while (_actual != null &&
                   (EsLexema("+") || EsLexema("-") || EsLexema("==") ||
                    EsLexema(">") || EsLexema("<") || EsLexema(">=") ||
                    EsLexema("<=") || EsLexema("<>")))
            {
                Avanzar();
                string tipoDer = AnalizarTermino();

                if (tipo == "FLOAT" || tipoDer == "FLOAT")
                    tipo = "FLOAT";
                else if (tipo == "INT" || tipoDer == "INT")
                    tipo = "INT";
            }

            return tipo;
        }

        private string AnalizarTermino()
        {
            string tipo = AnalizarFactor();

            while (EsLexema("*") || EsLexema("/"))
            {
                Avanzar();
                string tipoDer = AnalizarFactor();

                if (tipo == "FLOAT" || tipoDer == "FLOAT")
                    tipo = "FLOAT";
                else if (tipo == "INT" || tipoDer == "INT")
                    tipo = "INT";
            }

            return tipo;
        }

        private string AnalizarFactor()
        {
            if (_actual == null) return "?";

            // Número real — verificar antes que entero porque contiene punto
            if (EsReal())
            {
                Avanzar();
                return "FLOAT";
            }

            // Número entero
            if (EsEntero())
            {
                Avanzar();
                return "INT";
            }

            // Variable — verificar que fue declarada
            if (EsTipo(TKN_ID))
            {
                string nombre = _actual.Lexema;
                int linea = _actual.Linea;
                Avanzar();
                return _tabla.ObtenerTipo(nombre, linea) ?? "?";
            }

            // Expresión entre paréntesis
            if (EsLexema("("))
            {
                Avanzar();
                string tipo = AnalizarExpresion();
                ConsumirLexema(")");
                return tipo;
            }

            Avanzar();
            return "?";
        }
    }
}