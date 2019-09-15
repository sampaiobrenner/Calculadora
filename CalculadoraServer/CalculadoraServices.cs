namespace CalculadoraServer
{
    public class CalculadoraServices
    {
        public enum Operacao : short
        {
            Somar = 1,
            Subtrair = 2,
            Multiplicar = 3,
            Dividir = 4
        }

        public void ProcessarResultado(InformacoesParaSeremProcessadasDto dto)
        {
            switch (dto.Operacao)
            {
                case Operacao.Multiplicar:
                    dto.Resultado = dto.Numero1 * dto.Numero2;
                    break;

                case Operacao.Dividir:
                    dto.Resultado = dto.Numero1 / dto.Numero2;
                    break;

                case Operacao.Somar:
                    dto.Resultado = dto.Numero1 + dto.Numero2;
                    break;

                case Operacao.Subtrair:
                    dto.Resultado = dto.Numero1 - dto.Numero2;
                    break;

                default:
                    dto.Resultado = 0;
                    break;
            }
        }
    }
}