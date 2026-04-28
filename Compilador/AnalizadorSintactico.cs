using Compilador.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compilador.UI.CORE
{
    public class AnalizadorSintactico
    {
        private List<Token> _tokens;
        private int _posicionActual;
        private Token _tokenActual;

        public List<string> Errores = new List<string>();

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
        private const int TKN_PLUS = 301;
        private const int TKN_DIV = 302;
        private const int TKN_MUL = 303;
        private const int TKN_ASSIGN = 304;
        private const int TKN_COLON = 311;
        private const int TKN_LPAREN = 312;
        private const int TKN_RPAREN = 313;
        private const int TKN_COMMA = 316;
        private const int TKN_MINUS = 317;
        private const int TKN_DOT = 320;

        private const int TWN_COMENTARIO = 400;

        // ═══════════════════════════════════════════════════════════════
        // ENTRADA
        // ═══════════════════════════════════════════════════════════════
        public void Parse(List<Token> tokensTotales)
        {
            _tokens = tokensTotales
                          .Where(t => t.Tipo != TWN_COMENTARIO)
                          .ToList();

            Errores.Clear();
            _posicionActual = 0;

            if (_tokens.Count == 0)
            {
                Errores.Add("El código fuente está vacío o no generó tokens válidos.");
                return;
            }

            _tokenActual = _tokens[_posicionActual];

            try
            {
                ParserPrograma();

                if (_posicionActual < _tokens.Count && _tokenActual != null)
                    Errores.Add($"Error en línea {_tokenActual.Linea}: " +
                                $"Tokens inesperados después del fin del programa " +
                                $"('{_tokenActual.Lexema}').");
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // CONTROL DE TOKENS
        // ═══════════════════════════════════════════════════════════════
        private void Avanzar()
        {
            _posicionActual++;
            _tokenActual = _posicionActual < _tokens.Count
                ? _tokens[_posicionActual]
                : null;
        }

        private bool CheckTipo(int tipo)
            => _tokenActual != null && _tokenActual.Tipo == tipo;

        private bool CheckLexema(string lexema)
            => _tokenActual != null &&
               string.Equals(_tokenActual.Lexema, lexema, StringComparison.OrdinalIgnoreCase);

        private void MatchTipo(int tipo, string mensaje)
        {
            if (_tokenActual != null && _tokenActual.Tipo == tipo)
            {
                Avanzar();
            }
            else
            {
                string encontrado = _tokenActual != null ? _tokenActual.Lexema : "EOF";
                int linea = _tokenActual?.Linea ?? 0;
                throw new Exception(
                    $"Error sintáctico en línea {linea}: {mensaje}. Encontrado '{encontrado}'.");
            }
        }

        private void MatchLexema(string esperado, string mensaje)
        {
            if (_tokenActual != null &&
                string.Equals(_tokenActual.Lexema, esperado, StringComparison.OrdinalIgnoreCase))
            {
                Avanzar();
            }
            else
            {
                string encontrado = _tokenActual != null ? _tokenActual.Lexema : "EOF";
                int linea = _tokenActual?.Linea ?? 0;
                throw new Exception(
                    $"Error sintáctico en línea {linea}: {mensaje}. Encontrado '{encontrado}'.");
            }
        }

        private void MatchTipoForzado(int tipo, string mensaje)
        {
            if (CheckTipo(tipo)) Avanzar();
            else
            {
                string encontrado = _tokenActual != null ? _tokenActual.Lexema : "EOF";
                Errores.Add($"[Error Sintáctico] {mensaje} — encontrado: '{encontrado}'");
                Avanzar();
            }
        }

        private void MatchLexemaForzado(string esperado, string mensaje)
        {
            if (CheckLexema(esperado)) Avanzar();
            else
            {
                string encontrado = _tokenActual != null ? _tokenActual.Lexema : "EOF";
                Errores.Add($"[Error Sintáctico] {mensaje} — encontrado: '{encontrado}'");
                Avanzar();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // PRODUCCIONES
        // ═══════════════════════════════════════════════════════════════

        private void ParserPrograma()
        {
            MatchTipo(TKN_PROGRAM, "Se esperaba 'PROGRAM'");
            MatchTipo(TKN_ID, "Se esperaba identificador del programa");
            MatchLexema(";", "Falta ';' después del identificador del programa");

            ParserBloque();

            MatchLexema(".", "Falta '.' al finalizar el programa");
        }

        private void ParserBloque()
        {
            if (CheckTipo(TKN_VAR))
                ParserDeclaracionesVariables();

            while (CheckTipo(TKN_PROCEDURE))
                ParserDeclaracionProcedimiento();

            MatchTipo(TKN_BEGIN, "Se esperaba 'BEGIN'");
            ParserInstrucciones();
            MatchTipo(TKN_END, "Se esperaba 'END'");
        }

        private void ParserDeclaracionesVariables()
        {
            MatchTipo(TKN_VAR, "Se esperaba 'VAR'");

            while (CheckTipo(TKN_ID))
            {
                MatchTipo(TKN_ID, "Se esperaba identificador de variable");

                while (CheckLexema(","))
                {
                    Avanzar();
                    MatchTipo(TKN_ID, "Se esperaba identificador después de la ','");
                }

                MatchLexema(":", "Falta ':' en la declaración de variable");
                ParserTipo();
                MatchLexema(";", "Falta ';' al final de la declaración de variable");
            }
        }

        private void ParserTipo()
        {
            if (_tokenActual.Tipo == TKN_INT)
            {
                Avanzar();
            }
            else if (_tokenActual.Tipo == TKN_FLOAT)
            {
                Avanzar();
            }
            else
            {
                throw new Exception(
                    $"Error sintáctico en línea {_tokenActual.Linea}: " +
                    $"Se esperaba un tipo de dato válido (INT o FLOAT).");
            }
        }

        private void ParserDeclaracionProcedimiento()
        {
            MatchTipo(TKN_PROCEDURE, "Se esperaba 'PROCEDURE'");
            MatchTipo(TKN_ID, "Se esperaba nombre del procedimiento");

            if (CheckLexema("("))
            {
                Avanzar();

                if (_tokenActual.Tipo == TKN_ID)
                {
                    MatchTipo(TKN_ID, "Identificador de parámetro");
                    MatchLexema(":", "Falta ':'");
                    ParserTipo();
                }

                MatchLexema(")", "Falta ')'");
            }

            MatchLexema(";", "Falta ';' después de cabecera de procedimiento");

            ParserBloque();

            MatchLexema(";", "Falta ';' después de cuerpo de procedimiento");
        }

        private void ParserInstrucciones()
        {
            ParserInstruccion();

            while (CheckLexema(";"))
            {
                Avanzar();
                if (_tokenActual.Tipo != TKN_END)
                    ParserInstruccion();
            }
        }

        private void ParserInstruccion()
        {
            if (_tokenActual == null) return;

            if (CheckTipo(TKN_ID))
            {
                Avanzar();
                MatchLexema(":=", "Se esperaba operador de asignación ':='");
                ParserExpresion();
            }
            else if (CheckTipo(TKN_IF))
            {
                Avanzar();
                ParserExpresion();
                MatchTipo(TKN_THEN, "Se esperaba 'THEN'");
                ParserInstruccion();

                if (CheckTipo(TKN_ELSE))
                {
                    Avanzar();
                    ParserInstruccion();
                }
            }
            else if (CheckTipo(TKN_WHILE))
            {
                Avanzar();
                ParserExpresion();
                MatchTipo(TKN_DO, "Se esperaba 'DO'");
                ParserInstruccion();
            }
            else if (CheckTipo(TKN_BEGIN))
            {
                Avanzar();
                ParserInstrucciones();
                MatchTipo(TKN_END, "Se esperaba 'END' en bloque anidado");
            }
            else if (CheckTipo(TKN_WRITE) || CheckTipo(TKN_WRITELN) || CheckTipo(TKN_PRINT))
            {
                Avanzar();
                MatchLexema("(", "Falta '(' para función de impresión");
                ParserExpresion();

                while (CheckLexema(","))
                {
                    Avanzar();
                    ParserExpresion();
                }

                MatchLexema(")", "Falta ')' en la función de impresión");
            }
        }

        private void ParserExpresion()
        {
            ParserTermino();

            while (CheckLexema("+") || CheckLexema("-") || CheckLexema("==") ||
                   CheckLexema(">") || CheckLexema("<") || CheckLexema(">=") ||
                   CheckLexema("<=") || CheckLexema("<>"))
            {
                Avanzar();
                ParserTermino();
            }
        }

        private void ParserTermino()
        {
            ParserFactor();

            while (CheckLexema("*") || CheckLexema("/"))
            {
                Avanzar();
                ParserFactor();
            }
        }

        private void ParserFactor()
        {
            if (_tokenActual.Tipo == TKN_ID)
            {
                Avanzar();
            }
            else if (_tokenActual.Tipo == TKN_ENTERO || _tokenActual.Tipo == TKN_REAL)
            {
                Avanzar();
            }
            else if (CheckLexema("("))
            {
                Avanzar();
                ParserExpresion();
                MatchLexema(")", "Se esperaba ')' tras expresión");
            }
            else
            {
                throw new Exception(
                    $"Error sintáctico en línea {_tokenActual.Linea}: " +
                    $"Se esperaba un FACTOR (Identificador, Número o '('), " +
                    $"pero se encontró '{_tokenActual.Lexema}'.");
            }
        }
    }
}