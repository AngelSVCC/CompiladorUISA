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

        private const int TKN_ENTERO = 200;
        private const int TKN_REAL = 201;

        private const int TKN_SEMICOL = 300;
        private const int TKN_PLUS = 301;
        private const int TKN_MINUS = 317;
        private const int TKN_DIV = 302;
        private const int TKN_MUL = 303;
        private const int TKN_ASSIGN = 304;
        private const int TKN_COLON = 311;
        private const int TKN_LPAREN = 312;
        private const int TKN_RPAREN = 313;
        private const int TKN_COMMA = 316;
        private const int TKN_DOT = 320;

        private const int TWN_COMENTARIO = 400;

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
                ParsePrograma();

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

        private void ParsePrograma()
        {
            MatchTipo(TKN_PROGRAM, "Se esperaba 'PROGRAM'");
            MatchTipo(TKN_ID, "Se esperaba identificador del programa");
            MatchLexema(";", "Falta ';' después del identificador del programa");

            ParseBloque();

            MatchLexema(".", "Falta '.' al finalizar el programa");
        }

        private void ParseBloque()
        {
            if (CheckTipo(TKN_VAR))
                ParseSeccionVar();

            ParseSentenciaCompuesta();
        }

        private void ParseSeccionVar()
        {
            MatchTipo(TKN_VAR, "Se esperaba 'var'");
            ParseListaDeclaraciones();
        }

        private void ParseListaDeclaraciones()
        {
            while (CheckTipo(TKN_ID))
                ParseDeclaracion();
        }

        private void ParseDeclaracion()
        {
            MatchTipo(TKN_ID, "Se esperaba identificador");

            while (CheckTipo(TKN_COMMA))
            {
                Avanzar();
                MatchTipo(TKN_ID, "Se esperaba identificador después de ','");
            }

            MatchTipo(TKN_COLON, "Se esperaba ':'");
            ParseTipo();
            MatchTipo(TKN_SEMICOL, "Se esperaba ';'");
        }

        private void ParseTipo()
        {
            if (CheckTipo(TKN_INT) || CheckTipo(TKN_FLOAT) ||
                CheckLexema("integer") || CheckLexema("int") ||
                CheckLexema("real") || CheckLexema("float"))
            {
                Avanzar();
            }
            else
            {
                string encontrado = _tokenActual != null ? _tokenActual.Lexema : "EOF";
                Errores.Add($"[Error Sintáctico] Se esperaba tipo (int/integer/real/float) — encontrado: '{encontrado}'");
                Avanzar();
            }
        }

        private void ParseSentenciaCompuesta()
        {
            MatchTipo(TKN_BEGIN, "Se esperaba 'begin'");
            ParseListaSentencias();
            MatchTipo(TKN_END, "Se esperaba 'end'");
        }

        private void ParseListaSentencias()
        {
            ParseSentencia();

            while (CheckTipo(TKN_SEMICOL))
            {
                Avanzar();
                if (CheckTipo(TKN_END) || _tokenActual == null)
                    break;
                ParseSentencia();
            }
        }

        private void ParseSentencia()
        {
            if (_tokenActual == null) return;
            if (CheckTipo(TKN_END)) return;

            if (CheckTipo(TKN_IF)) ParseIf();
            else if (CheckTipo(TKN_WHILE)) ParseWhile();
            else if (CheckTipo(TKN_BEGIN)) ParseSentenciaCompuesta();
            else if (CheckTipo(TKN_WRITE)) ParseWrite();
            else if (CheckTipo(TKN_WRITELN)) ParseWrite();
            else if (CheckTipo(TKN_ID)) ParseAsignacion();
            else
            {
                string encontrado = _tokenActual.Lexema;
                Errores.Add($"[Error Sintáctico] Sentencia inválida — encontrado: '{encontrado}'");
                Avanzar();
            }
        }

        private void ParseAsignacion()
        {
            MatchTipo(TKN_ID, "Se esperaba identificador");
            MatchTipo(TKN_ASSIGN, "Se esperaba ':='");
            ParseExpresion();
        }

        private void ParseIf()
        {
            MatchTipo(TKN_IF, "Se esperaba 'if'");
            ParseExpresion();
            MatchTipo(TKN_THEN, "Se esperaba 'then'");
            ParseSentencia();

            if (CheckTipo(TKN_ELSE))
            {
                Avanzar();
                ParseSentencia();
            }
        }

        private void ParseWhile()
        {
            MatchTipo(TKN_WHILE, "Se esperaba 'while'");
            ParseExpresion();
            MatchTipo(TKN_DO, "Se esperaba 'do'");
            ParseSentencia();
        }

        private void ParseWrite()
        {
            Avanzar();

            if (CheckTipo(TKN_LPAREN))
            {
                Avanzar();
                ParseExpresion();

                while (CheckTipo(TKN_COMMA))
                {
                    Avanzar();
                    ParseExpresion();
                }

                MatchTipo(TKN_RPAREN, "Se esperaba ')'");
            }
        }

        private void ParseExpresion()
        {
            ParseExpresionSimple();

            if (EsOperadorRelacional())
            {
                Avanzar();
                ParseExpresionSimple();
            }
        }

        private void ParseExpresionSimple()
        {
            ParseTermino();

            while (CheckTipo(TKN_PLUS) || CheckTipo(TKN_MINUS) || CheckLexema("or"))
            {
                Avanzar();
                ParseTermino();
            }
        }

        private void ParseTermino()
        {
            ParseFactor();

            while (CheckTipo(TKN_MUL) || CheckTipo(TKN_DIV) ||
                   CheckLexema("div") || CheckLexema("mod") || CheckLexema("and"))
            {
                Avanzar();
                ParseFactor();
            }
        }

        private void ParseFactor()
        {
            if (CheckTipo(TKN_ID) || CheckTipo(TKN_ENTERO) || CheckTipo(TKN_REAL))
            {
                Avanzar();
            }
            else if (CheckTipo(TKN_LPAREN))
            {
                Avanzar();
                ParseExpresion();
                MatchTipo(TKN_RPAREN, "Se esperaba ')'");
            }
            else if (CheckLexema("not"))
            {
                Avanzar();
                ParseFactor();
            }
            else
            {
                string encontrado = _tokenActual != null ? _tokenActual.Lexema : "EOF";
                Errores.Add($"[Error Sintáctico] Factor inválido — encontrado: '{encontrado}'");
                if (_tokenActual != null) Avanzar();
            }
        }

        private bool EsOperadorRelacional()
        {
            if (_tokenActual == null) return false;
            string l = _tokenActual.Lexema;
            return l == "=" || l == "<>" || l == "<" ||
                   l == ">" || l == "<=" || l == ">=";
        }
    }
}