using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    public class MatrizTransicion
    {
        private readonly int[,] _matriz =
        {
        //        L    D    ESP  OTRO
        /*0*/  {  1,   2,   0,   501 },  // inicio
        /*1*/  {  1,   1,   100, 100 },  // ID o Palabra Reservada
        /*2*/  { 501,  2,   200, 200 }   // número entero
    };

        public int ObtenerColumna(char c)
        {
            if (char.IsLetter(c) || c == '_')
                return 0; // letra/underscore

            if (char.IsDigit(c))
                return 1; // dígito

            if (char.IsWhiteSpace(c))
                return 2; // espacio

            return 3; // otros caracteres
        }

        public int SiguienteEstado(int estado, int columna)
        {
            if (estado < 0 || estado > 2)
                throw new ArgumentOutOfRangeException(nameof(estado));

            return _matriz[estado, columna];
        }
    }
}
