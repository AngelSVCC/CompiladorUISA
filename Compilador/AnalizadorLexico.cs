using Compilador.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador
{
    public class AnalizadorLexico
    {
        private readonly MatrizTransicion _matriz;
        private readonly PalabrasReservadas _palabrasReservadas;

        public AnalizadorLexico()
        {
            _matriz = new MatrizTransicion();
            _palabrasReservadas = new PalabrasReservadas();
        }

        public ResultadoLexico Analizar(CodigoFuente fuente)
        {
            var resultado = new ResultadoLexico();
            resultado.AgregarAviso("LÉXICO INICIADO");

            try
            {
                for (int numLinea = 1; numLinea <= fuente.NumeroLineas; numLinea++)
                    ProcesarLinea(fuente.ObtenerLinea(numLinea), numLinea, resultado);

                resultado.AgregarAviso("LÉXICO FINALIZADO EXITOSAMENTE");
            }
            catch (Exception ex)
            {
                resultado.AgregarAviso("ERROR GRAVE EN LÉXICO: " + ex.Message);
            }

            return resultado;
        }

        private void ProcesarLinea(string lineaOriginal, int numLinea, ResultadoLexico resultado)
        {
            int estado = 0;
            var lexema = new StringBuilder();

            string linea = (lineaOriginal ?? string.Empty) + " ";

            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];

                // ── Ignorar espacios/tabs solo en estado 0
                //    Si estamos acumulando lexema, el espacio lo cierra
                if (char.IsWhiteSpace(c))
                {
                    if (estado != 0 && lexema.Length > 0)
                    {
                        EmitirLexemaAcumulado(lexema, numLinea, resultado);
                        estado = 0;
                    }
                    continue;
                }

                // ── Reconocer := como token doble (antes de verificar símbolos simples)
                if (estado == 0 && c == ':' && i + 1 < linea.Length && linea[i + 1] == '=')
                {
                    EmitirToken(304, ":=", numLinea, resultado);
                    i++;
                    continue;
                }

                // ── Si estamos acumulando un lexema y llega un símbolo simple
                //    → primero emitir el lexema, luego procesar el símbolo
                if (estado != 0 && _matriz.EsSimbolo(c, out int _))
                {
                    EmitirLexemaAcumulado(lexema, numLinea, resultado);
                    estado = 0;
                    i--;   // reprocesar el símbolo en la siguiente iteración
                    continue;
                }

                // ── Símbolos simples desde estado 0
                if (estado == 0 && _matriz.EsSimbolo(c, out int tokenSimbolo))
                {
                    EmitirToken(tokenSimbolo, c.ToString(), numLinea, resultado);
                    continue;
                }

                // ── Punto final de programa '.' — emitir y cerrar
                if (estado == 0 && c == '.')
                {
                    EmitirToken(320, ".", numLinea, resultado);
                    continue;
                }

                // ── Si estamos acumulando y llega un punto (número real o punto final)
                //    La matriz ya maneja esto: q2 + '.' → q3 (real)
                //    Si estamos en q1 (identificador) + '.' → terminar ID, luego emitir '.'
                if (estado == 1 && c == '.')
                {
                    EmitirLexemaAcumulado(lexema, numLinea, resultado);
                    estado = 0;
                    EmitirToken(320, ".", numLinea, resultado);
                    continue;
                }

                // ── Consultar la matriz de transición
                int columna = _matriz.ObtenerColumna(c);
                int valorMatriz = _matriz.SiguienteEstado(estado, columna);

                // Error léxico
                if (valorMatriz >= 500)
                {
                    resultado.AgregarAviso($"Error léxico en línea {numLinea}: símbolo no reconocido '{c}'");
                    estado = 0;
                    lexema.Clear();
                    continue;
                }

                // Comentario // → ignorar el resto de la línea
                if (valorMatriz == 400)
                {
                    if (lexema.Length > 0)
                    {
                        EmitirLexemaAcumulado(lexema, numLinea, resultado);
                        estado = 0;
                    }
                    break;
                }

                // Estado intermedio: seguir acumulando
                if (valorMatriz >= 1 && valorMatriz <= 9)
                {
                    estado = valorMatriz;
                    lexema.Append(c);
                    continue;
                }

                // Token 100 → fin de identificador / palabra reservada
                if (valorMatriz == 100)
                {
                    string lex = lexema.ToString();
                    int token = _palabrasReservadas.ObtenerToken(lex);
                    EmitirToken(token, lex, numLinea, resultado);
                    lexema.Clear();
                    estado = 0;
                    i--;   // reprocesar el carácter que cerró el ID
                    continue;
                }

                // Token 200 → número entero completado
                if (valorMatriz == 200)
                {
                    EmitirToken(200, lexema.ToString(), numLinea, resultado);
                    lexema.Clear();
                    estado = 0;
                    i--;
                    continue;
                }

                // Token 201 → número real completado
                if (valorMatriz == 201)
                {
                    EmitirToken(201, lexema.ToString(), numLinea, resultado);
                    lexema.Clear();
                    estado = 0;
                    i--;
                    continue;
                }

                // Tokens de operadores relacionales (304, 305, 306…)
                if (valorMatriz >= 300 && valorMatriz < 400)
                {
                    // El lexema acumulado hasta ahora forma parte del operador
                    lexema.Append(c);
                    EmitirToken(valorMatriz, lexema.ToString(), numLinea, resultado);
                    lexema.Clear();
                    estado = 0;
                    continue;
                }

                // Token 310 → cadena literal cerrada
                if (valorMatriz == 310)
                {
                    lexema.Append(c);
                    EmitirToken(310, lexema.ToString(), numLinea, resultado);
                    lexema.Clear();
                    estado = 0;
                    continue;
                }

                // Cualquier otro caso: acumular
                estado = valorMatriz;
                lexema.Append(c);
            }

            // Al terminar la línea, si quedó lexema pendiente emitirlo
            if (lexema.Length > 0)
            {
                EmitirLexemaAcumulado(lexema, numLinea, resultado);
            }
        }

        // ── Emite el lexema acumulado consultando palabras reservadas
        private void EmitirLexemaAcumulado(StringBuilder lexema, int numLinea, ResultadoLexico resultado)
        {
            string lex = lexema.ToString();
            int token = _palabrasReservadas.ObtenerToken(lex);
            EmitirToken(token, lex, numLinea, resultado);
            lexema.Clear();
        }

        private void EmitirToken(int token, string lexema, int linea, ResultadoLexico resultado)
        {
            resultado.Tokens.Add(new Token(token, lexema, linea));
        }
    }
}