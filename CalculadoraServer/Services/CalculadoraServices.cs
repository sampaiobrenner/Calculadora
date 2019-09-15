using Calculadora;
using System;
using System.Threading;

namespace CalculadoraServer.Services
{
    public class CalculadoraServices
    {
        public InformacoesParaSeremProcessadasDto ProcessarResultado(InformacoesParaSeremProcessadasDto dto)
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));

            switch (dto.Operacao)
            {
                case CalculadoraUtils.Operacao.Multiplicar:
                    dto.Resultado = dto.Numero1 * dto.Numero2;
                    break;

                case CalculadoraUtils.Operacao.Dividir:
                    dto.Resultado = dto.Numero1 / dto.Numero2;
                    break;

                case CalculadoraUtils.Operacao.Somar:
                    dto.Resultado = dto.Numero1 + dto.Numero2;
                    break;

                case CalculadoraUtils.Operacao.Subtrair:
                    dto.Resultado = dto.Numero1 - dto.Numero2;
                    break;

                default:
                    dto.Resultado = 0;
                    break;
            }

            return dto;
        }
    }
}