using System.Globalization;

namespace Calculadora
{
    public static class CalculadoraUtils
    {
        public enum Operacao : short
        {
            Somar = 1,
            Subtrair = 2,
            Multiplicar = 3,
            Dividir = 4
        }

        public static string DecimalToString(this decimal dec)
        {
            var strdec = dec.ToString(CultureInfo.CurrentCulture);
            return strdec.Contains(",") ? strdec.TrimEnd('0').TrimEnd(',') : strdec;
        }
    }
}