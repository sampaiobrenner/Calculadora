using Calculadora;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

namespace Trabalho2.Services
{
    public class CalculadoraServices
    {
        public decimal ProcessarResultado(decimal numero1, decimal numero2, CalculadoraUtils.Operacao operacao)
        {
            var dtoDeEnvio = new InformacoesParaSeremProcessadasDto()
            {
                Numero1 = numero1,
                Numero2 = numero2,
                Operacao = operacao
            };

            var json = JsonConvert.SerializeObject(dtoDeEnvio);

            var jsonRetorno = new ClientServices().IniciarCliente(json);
            if (jsonRetorno is null) return default;

            var dtoDeRetorno = JsonConvert.DeserializeObject<InformacoesParaSeremProcessadasDto>(jsonRetorno);

            return dtoDeRetorno?.Resultado ?? default;
        }

        public bool TestarConexaoComServidor()
        {
            try
            {
                var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                var ipAddress = ipHostInfo.AddressList[0];

                using (new TcpClient(ipAddress.ToString(), 11000))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}