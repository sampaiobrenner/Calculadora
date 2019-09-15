namespace CalculadoraServer
{
    public class InformacoesParaSeremProcessadasDto
    {
        public decimal Numero1 { get; set; }
        public decimal Numero2 { get; set; }
        public CalculadoraServices.Operacao Operacao { get; set; }
        public decimal Resultado { get; set; }
    }
}