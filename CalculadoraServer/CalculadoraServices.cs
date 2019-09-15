namespace CalculadoraServer
{
    public class CalculadoraServices
    {
        private enum Operacao : short
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
                case (short)Operacao.Multiplicar:
                    dto.Resultado = dto.Numero1 * dto.Numero2;
                    break;

                case (short)Operacao.Dividir:
                    dto.Resultado = dto.Numero1 / dto.Numero2;
                    break;

                case (short)Operacao.Somar:
                    dto.Resultado = dto.Numero1 + dto.Numero2;
                    break;

                case (short)Operacao.Subtrair:
                    dto.Resultado = dto.Numero1 - dto.Numero2;
                    break;

                default:
                    dto.Resultado = 0;
                    break;
            }
        }
    }
}