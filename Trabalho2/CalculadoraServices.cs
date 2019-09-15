using Newtonsoft.Json;

namespace Trabalho2
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

        public decimal ProcessarResultado(decimal numero1, decimal numero2, Operacao operacao)
        {
            var dtoDeEnvio = new InformacoesParaSeremProcessadasDto()
            {
                Numero1 = numero1,
                Numero2 = numero2,
                Operacao = (short)operacao
            };

            var json = JsonConvert.SerializeObject(dtoDeEnvio);

            var jsonRetorno = new ClientAsync().IniciarCliente(json);
            if (jsonRetorno is null) return default;

            var dtoDeRetorno = JsonConvert.DeserializeObject<InformacoesParaSeremProcessadasDto>(jsonRetorno);

            return dtoDeRetorno?.Resultado ?? default;
        }
    }
}