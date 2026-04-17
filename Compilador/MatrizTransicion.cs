using System;
using System.Collections.Generic;

namespace Compilador
{
    /// 
    /// ═══════════════════════════════════════════════════════════════
    ///  COLUMNAS DE LA MATRIZ
    /// ═══════════════════════════════════════════════════════════════
    ///   0  = letra o _
    ///   1  = dígito
    ///   2  = espacio / tab
    ///   3  = punto  '.'
    ///   4  = =
    ///   5  = >
    ///   6  = <
    ///   7  = /
    ///   8  = símbolos simples: ; + - * : ( ) { } ,
    ///   9  = comilla simple '
    ///  10  = otro (error)
    ///
    /// ═══════════════════════════════════════════════════════════════
    ///  ESTADOS
    /// ═══════════════════════════════════════════════════════════════
    ///   0 = inicio
    ///   1 = leyendo identificador / palabra reservada
    ///   2 = leyendo número entero
    ///   3 = leyendo número real (dígitos después del punto)
    ///   4 = punto recién leído (aún sin dígito decimal)
    ///   5 = '=' recién leído  → puede ser = o ==
    ///   6 = '>' recién leído  → puede ser > o >=
    ///   7 = '<' recién leído  → puede ser < o <=
    ///   8 = '/' recién leído  → puede ser / o //
    ///
    /// ═══════════════════════════════════════════════════════════════
    ///  TOKENS DE RETORNO
    /// ═══════════════════════════════════════════════════════════════
    ///   100 = fin ID / palabra reservada
    ///   200 = número entero
    ///   201 = número real
    ///   300 = ;
    ///   301 = +
    ///   302 = /  (división simple)
    ///   303 = *
    ///   304 = :=
    ///   305 = ==
    ///   306 = >
    ///   307 = >=
    ///   308 = <
    ///   309 = <=
    ///   310 = cadena literal 'texto'
    ///   311 = :
    ///   312 = (
    ///   313 = )
    ///   314 = {
    ///   315 = }
    ///   316 = ,
    ///   317 = -
    ///   320 = .
    ///   400 = comentario // (ignorar resto de línea)
    ///   500 = error léxico
    ///
    public class MatrizTransicion
    {
        //               L    D   ESP   .    =    >    <    /   SIM   '  OTRO
        private readonly int[,] _matriz =
        {
        /*q0*/ {   1,   2,   0,   4,   5,   6,   7,   8,   0,   9, 500 },
        /*q1*/ {   1,   1, 100, 100, 100, 100, 100, 100, 100, 100, 100 },
        /*q2*/ { 200,   2, 200,   3, 200, 200, 200, 200, 200, 200, 200 },  // FIX: col1(dígito)=2, no 200
        /*q3*/ { 201,   3, 201, 201, 201, 201, 201, 201, 201, 201, 201 },
        /*q4*/ { 500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 500 },
        /*q5*/ { 304, 304, 304, 304, 305, 304, 304, 304, 304, 304, 304 },
        /*q6*/ { 306, 306, 306, 306, 307, 306, 306, 306, 306, 306, 306 },
        /*q7*/ { 308, 308, 308, 308, 309, 308, 308, 308, 308, 308, 308 },
        /*q8*/ { 302, 302, 302, 302, 302, 302, 302, 400, 302, 302, 302 },
        /*q9*/ {   9,   9,   9,   9,   9,   9,   9,   9,   9, 310,   9 },
        };

        // Símbolos simples: emitidos directamente desde estado 0 sin lookahead.
        private readonly Dictionary<char, int> _simbolosSimples =
            new Dictionary<char, int>
        {
            { ';', 300 },
            { '+', 301 },
            { '-', 317 },
            { '*', 303 },
            { ':', 311 },
            { '(', 312 },
            { ')', 313 },
            { '{', 314 },
            { '}', 315 },
            { ',', 316 },
        };

        public int ObtenerColumna(char c)
        {
            if (char.IsLetter(c) || c == '_') return 0;
            if (char.IsDigit(c)) return 1;
            if (char.IsWhiteSpace(c)) return 2;
            if (c == '.') return 3;
            if (c == '=') return 4;
            if (c == '>') return 5;
            if (c == '<') return 6;
            if (c == '/') return 7;
            if (c == ';' || c == '+' || c == '-' || c == '*' ||
                c == ':' || c == '(' || c == ')' || c == '{' ||
                c == '}' || c == ',') return 8;
            if (c == '\'') return 9;
            return 10;
        }

        public int SiguienteEstado(int estado, int columna)
        {
            if (estado < 0 || estado >= _matriz.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(estado), $"Estado inválido: {estado}");
            if (columna < 0 || columna >= _matriz.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(columna), $"Columna inválida: {columna}");

            return _matriz[estado, columna];
        }

        public bool EsSimbolo(char c, out int token)
            => _simbolosSimples.TryGetValue(c, out token);
    }
}