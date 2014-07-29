using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockPartyServer
{
    public class NetworkingManager
    {
        TcpListener listener;
        List<TcpClient> clients;
        byte[] buffer;
        public Game Game;

        public NetworkingManager()
        {
            // Initialize the TCP listener at the Azure instance endpoint
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 1337);
            listener = new TcpListener(endpoint);
            listener.Start();
            Console.WriteLine("Listening at endpoint {0}", endpoint.ToString());

            // Initialize the TCP client list
            clients = new List<TcpClient>();
            buffer = new byte[1024];

            // Start accepting new connections from TCP clients
            listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptTcpClient), listener);
        }

        void OnAcceptTcpClient(IAsyncResult result)
        {
            TcpListener resultingListener = (TcpListener)result.AsyncState;

            TcpClient client = resultingListener.EndAcceptTcpClient(result);

            Console.WriteLine("Accepted new client {0}", client.Client.RemoteEndPoint.ToString());
            clients.Add(client);

            // Start receiving data from this client
            ReceiveData(client);

            // Start accepting more new TCP clients
            listener.BeginAcceptTcpClient(new AsyncCallback(OnAcceptTcpClient), listener);
        }

        void ReceiveData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            //StreamReader reader = new StreamReader(stream);

            if (stream.CanRead)
            {
                stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), client);
            }
        }

        void OnRead(IAsyncResult result)
        {
            TcpClient resultingClient = (TcpClient)result.AsyncState;
            NetworkStream resultingStream = resultingClient.GetStream();

            int bytesRead = resultingStream.EndRead(result);

            // If no bytes can be read, then the client disconnected (TODO: test this)
            if (bytesRead == 0)
            {
                clients.Remove(resultingClient);

                Console.WriteLine("Disconnected from client {0}", resultingClient.Client.RemoteEndPoint.ToString());

                return;
            }

            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
            Console.WriteLine("Received data from client {0}: {1}", resultingClient.Client.RemoteEndPoint.ToString(), message);

            // Process data (TODO: Refactor this into a Game.Process method
            string[] words = message.Split(' ');
            if(words[0] == "GameResults")
            {
                Game.RoundResults.Add(resultingClient.Client.RemoteEndPoint.ToString(), int.Parse(words[1]));
            }

            resultingStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), resultingClient);
        }

        public void SendData(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine(message);
            writer.Flush();
            Console.WriteLine("Sent data to client {0}: {1}", client.Client.RemoteEndPoint.ToString(), message);
        }

        public void BroadcastData(string message)
        {
            foreach (TcpClient client in clients)
            {
                SendData(client, message);
            }

            Console.WriteLine("Broadcasted data to all clients: {0}", message);
        }
    }
}