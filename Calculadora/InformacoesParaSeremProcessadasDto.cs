namespace Calculadora
{
    public class InformacoesParaSeremProcessadasDto
    {
        public decimal Numero1 { get; set; }
        public decimal Numero2 { get; set; }
        public CalculadoraUtils.Operacao Operacao { get; set; }
        public decimal Resultado { get; set; }
    }
}