using Calculadora;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CalculadoraServer.Services
{
    public static class ServerServices
    {
        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            ComecarAEscutar();
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            // Sinaliza o segmento principal para continuar.
            AllDone.Set();

            // Obtenha o soquete que lida com a solicitação do cliente.
            var listener = (Socket)ar.AsyncState;
            var handler = listener.EndAccept(ar);

            // Create the state object.
            var state = new StateObject { WorkSocket = handler };
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, LerRetorno, state);
        }

        private static void ComecarAEscutar()
        {
            // Carrega o endereço local para o soquete.
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Cria o socket TCP/IP.
            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Liga o soquete ao terminal local e escuta as conexões de entrada.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Defina o evento para um estado não sinalizado.
                    AllDone.Reset();

                    // Inicie um soquete assíncrono para ouvir as conexões.
                    listener.BeginAccept(AcceptCallback, listener);

                    // Aguarde até que seja feita uma conexão antes de continuar.
                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Enviar(Socket handler, string data)
        {
            // Convert the string data to byte data using UTF8 encoding.
            var byteData = Encoding.UTF8.GetBytes(data);

            // Comece a enviar os dados para o dispositivo remoto.
            handler.BeginSend(byteData, 0, byteData.Length, 0, EnviarRetorno, handler);
        }

        private static void EnviarRetorno(IAsyncResult ar)
        {
            try
            {
                // Recupere o soquete do objeto de estado.
                var handler = (Socket)ar.AsyncState;

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void LerRetorno(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            var state = (StateObject)ar.AsyncState;
            var handler = state.WorkSocket;

            // Read data from the client socket.
            var bytesRead = handler.EndReceive(ar);

            if (bytesRead <= 0) return;

            // Pode haver mais dados, portanto, armazene os dados recebidos até o momento.
            state.Sb.Append(Encoding.UTF8.GetString(state.Buffer, 0, bytesRead));

            // Verifique a tag de fim de arquivo. Se não estiver lá, leia mais dados
            var content = state.Sb.ToString();
            if (content.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
            {
                // Remove caractere de fim de arquivo
                var str = content.Replace("<EOF>", string.Empty);

                // Desserializa os valores enviados pelo cliente para que possam ser processados
                var informacoesParaSeremProcessadasDto = JsonConvert.DeserializeObject<InformacoesParaSeremProcessadasDto>(str);

                // Efetua o processamento dos valores
                informacoesParaSeremProcessadasDto = new CalculadoraServices().ProcessarResultado(informacoesParaSeremProcessadasDto);

                // Serializa os valores em JSON para envio ao cliente
                var json = JsonConvert.SerializeObject(informacoesParaSeremProcessadasDto);

                // Resposta a ser enviada ao cliente
                Console.WriteLine();
                Console.WriteLine(" Resposta enviada ao cliente:                                                          ");
                Console.WriteLine(" --------------------------------------------------------------------------------------");
                Console.WriteLine($" Numero 1:           {informacoesParaSeremProcessadasDto.Numero1.DecimalToString()}   ");
                Console.WriteLine($" Numero 2:           {informacoesParaSeremProcessadasDto.Numero2.DecimalToString()}   ");
                Console.WriteLine($" Tipo de operação:   {informacoesParaSeremProcessadasDto.Operacao.ToString()}         ");
                Console.WriteLine($" Resultado:          {informacoesParaSeremProcessadasDto.Resultado.DecimalToString()} ");
                Console.WriteLine(" --------------------------------------------------------------------------------------");
                Console.WriteLine($" JSON: {json}");
                Console.WriteLine(" --------------------------------------------------------------------------------------");

                // Efetua o retorno das informações ao cliente
                Enviar(handler, json);
            }
            else
            {
                //  Nem todos os dados recebidos. Pegue mais.
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, LerRetorno, state);
            }
        }
    }

    public class StateObject
    {
        // Tamanho do buffer de recebimento.
        public const int BufferSize = 1024;

        // Receber buffer.
        public readonly byte[] Buffer = new byte[BufferSize];

        // Sequência de dados recebida.
        public readonly StringBuilder Sb = new StringBuilder();

        // Soquete do cliente.
        public Socket WorkSocket;
    }
}