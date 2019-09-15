using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientAsync
{
    // The port number for the remote device.
    private const int Port = 11000;

    // ManualResetEvent instances signal completion.
    private readonly ManualResetEvent _connectDone = new ManualResetEvent(false);

    private readonly ManualResetEvent _receiveDone = new ManualResetEvent(false);

    private readonly ManualResetEvent _sendDone = new ManualResetEvent(false);

    // The response from the remote device.
    private string _response;

    public string IniciarCliente(string mensagem)
    {
        // Connect to a remote device.
        try
        {
            // Establish the remote endpoint for the socket.
            // The name of the
            // remote device is "host.contoso.com".
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, Port);

            // Create a TCP/IP socket.
            var client = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.
            client.BeginConnect(remoteEP, ConnectCallback, client);
            _connectDone.WaitOne();

            // Send test data to the remote device.
            Send(client, $"{mensagem}<EOF>");
            _sendDone.WaitOne();

            // Receive the response from the remote device.
            Receive(client);
            _receiveDone.WaitOne();

            // Release the socket.
            client.Shutdown(SocketShutdown.Both);
            client.Close();

            return _response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return "";
        }
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            var client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            // Signal that the connection has been made.
            _connectDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void Receive(Socket client)
    {
        try
        {
            // Create the state object.
            var state = new StateObject { workSocket = client };

            // Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket
            // from the asynchronous state object.
            var state = (StateObject)ar.AsyncState;
            var client = state.workSocket;

            // Read data from the remote device.
            var bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.
                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                // Get the rest of the data.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            }
            else
            {
                // All the data has arrived; put it in response.
                if (state.sb.Length > 1) _response = state.sb.ToString();

                // Signal that all bytes have been received.
                _receiveDone.Set();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void Send(Socket client, string data)
    {
        // Convert the string data to byte data using UTF8 encoding.
        var byteData = Encoding.UTF8.GetBytes(data);

        // Begin sending the data to the remote device.
        client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            var client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            var bytesSent = client.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            // Signal that all bytes have been sent.
            _sendDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}

// State object for receiving data from remote device.
public class StateObject
{
    // Size of receive buffer.
    public const int BufferSize = 256;

    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];

    // Received data string.
    public StringBuilder sb = new StringBuilder();

    // Client socket.
    public Socket workSocket;
}