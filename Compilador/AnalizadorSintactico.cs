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

        // ── Constantes de tokens
        private const int TKN_ID = 101;
        private const int TKN_VAR = 102;
        private const int TKN_INT = 103;   // int / integer
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

        private const int TKN_ENTERO = 200;   // literal entero
        private const int TKN_REAL = 201;   // literal real

        private const int TKN_SEMICOL = 300;   // ;
        private const int TKN_PLUS = 301;   // +
        private const int TKN_MINUS = 317;   // -
        private const int TKN_DIV = 302;   // /
        private const int TKN_MUL = 303;   // *
        private const int TKN_ASSIGN = 304;   // :=
        private const int TKN_COLON = 311;   // :
        private const int TKN_LPAREN = 312;   // (
        private const int TKN_RPAREN = 313;   // )
        private const int TKN_COMMA = 316;   // ,
        private const int TKN_DOT = 320;   // .

        // ═══════════════════════════════════════════════════════════════
        // ENTRADA
        // ═══════════════════════════════════════════════════════════════
        public void Parse(List<Token> tokensTotales)
        {
            _tokens = tokensTotales.ToList();
            _posicionActual = 0;
            _tokenActual = _tokens.Count > 0 ? _tokens[0] : null;

            try
            {
                ParsePrograma();
            }
            catch (Exception ex)
            {
                Errores.Add("Error sintáctico inesperado: " + ex.Message);
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
            if (CheckTipo(tipo))
            {
                Avanzar();
            }
            else
            {
                string encontrado = _tokenActual != null ? _tokenActual.Lexema : "EOF";
                Errores.Add($"[Error Sintáctico] {mensaje} — encontrado: '{encontrado}'");
                // No avanzar para no perder sincronía; el llamador decide
            }
        }

        private void MatchLexema(string esperado, string mensaje)
        {
            if (CheckLexema(esperado))
            {
                Avanzar();
            }
            else
            {
                string encontrado = _tokenActual != null ? _tokenActual.Lexema : "EOF";
                Errores.Add($"[Error Sintáctico] {mensaje} — encontrado: '{encontrado}'");
            }
        }

        // ── Versión que sí avanza aunque haya error (recuperación de pánico mínima)
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

        // <programa> ::= PROGRAM id ; <bloque> .
        private void ParsePrograma()
        {
            MatchTipo(TKN_PROGRAM, "Se esperaba 'program'");
            MatchTipo(TKN_ID, "Se esperaba identificador del programa");
            MatchTipo(TKN_SEMICOL, "Se esperaba ';'");

            ParseBloque();

            MatchTipo(TKN_DOT, "Se esperaba '.' al final del programa");
        }

        // <bloque> ::= [VAR <declaraciones>] BEGIN <sentencias> END
        private void ParseBloque()
        {
            if (CheckTipo(TKN_VAR))
                ParseSeccionVar();

            ParseSentenciaCompuesta();
        }

        // <seccion-var> ::= VAR <lista-declaraciones>
        private void ParseSeccionVar()
        {
            MatchTipo(TKN_VAR, "Se esperaba 'var'");
            ParseListaDeclaraciones();
        }

        // <lista-declaraciones> ::= <declaracion> { <declaracion> }
        private void ParseListaDeclaraciones()
        {
            while (CheckTipo(TKN_ID))
                ParseDeclaracion();
        }

        // <declaracion> ::= id { , id } : tipo ;
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

        // <tipo> ::= INTEGER | INT | REAL | FLOAT
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

        // <sentencia-compuesta> ::= BEGIN <lista-sentencias> END
        private void ParseSentenciaCompuesta()
        {
            MatchTipo(TKN_BEGIN, "Se esperaba 'begin'");
            ParseListaSentencias();
            MatchTipo(TKN_END, "Se esperaba 'end'");
        }

        // <lista-sentencias> ::= <sentencia> { ; <sentencia> }
        private void ParseListaSentencias()
        {
            ParseSentencia();

            while (CheckTipo(TKN_SEMICOL))
            {
                Avanzar();
                // ';' después de END es el separador del programa, no de sentencia
                if (CheckTipo(TKN_END) || _tokenActual == null)
                    break;
                ParseSentencia();
            }
        }

        // <sentencia> ::= asignacion | if | while | begin-end | write | writeln | ε
        private void ParseSentencia()
        {
            if (_tokenActual == null) return;
            if (CheckTipo(TKN_END)) return;

            if (CheckTipo(TKN_IF)) ParseIf();
            else if (CheckTipo(TKN_WHILE)) ParseWhile();
            else if (CheckTipo(TKN_BEGIN)) ParseSentenciaCompuesta();
            else if (CheckTipo(TKN_WRITE)) ParseWrite();
            else if (CheckTipo(TKN_WRITELN)) ParseWrite();   // writeln igual que write
            else if (CheckTipo(TKN_ID)) ParseAsignacion();
            else
            {
                string encontrado = _tokenActual.Lexema;
                Errores.Add($"[Error Sintáctico] Sentencia inválida — encontrado: '{encontrado}'");
                Avanzar();
            }
        }

        // <asignacion> ::= id := <expresion>
        private void ParseAsignacion()
        {
            MatchTipo(TKN_ID, "Se esperaba identificador");
            MatchTipo(TKN_ASSIGN, "Se esperaba ':='");
            ParseExpresion();
        }

        // <if> ::= IF <expr> THEN <sentencia> [ELSE <sentencia>]
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

        // <while> ::= WHILE <expr> DO <sentencia>
        private void ParseWhile()
        {
            MatchTipo(TKN_WHILE, "Se esperaba 'while'");
            ParseExpresion();
            MatchTipo(TKN_DO, "Se esperaba 'do'");
            ParseSentencia();
        }

        // <write> ::= WRITE [ ( <expr> { , <expr> } ) ]
        // <writeln> ::= WRITELN [ ( <expr> { , <expr> } ) ]
        private void ParseWrite()
        {
            Avanzar(); // consumir WRITE o WRITELN

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
            // Si no hay paréntesis, WRITE sin argumentos → válido
        }

        // ═══════════════════════════════════════════════════════════════
        // EXPRESIONES
        // ═══════════════════════════════════════════════════════════════

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