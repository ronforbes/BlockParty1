using Microsoft.WindowsAzure.ServiceRuntime;
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

namespace BlockPartyWorkerRole
{
    public class NetworkingManager
    {
        TcpListener listener;
        List<TcpClient> clients;
        public Game Game;

        public NetworkingManager()
        {
            // Initialize the TCP listener at the Azure instance endpoint
            IPEndPoint endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["BlockPartyEndpoint"].IPEndpoint;
            listener = new TcpListener(endpoint);
            listener.Start();
            Trace.TraceInformation("Listening at endpoint {0}", endpoint.ToString());

            // Initialize the TCP client list
            clients = new List<TcpClient>();

            // Accept new connections from TCP clients
            AcceptTcpClients();
        }

        async void AcceptTcpClients()
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Trace.TraceInformation("Accepted new client {0}", client.Client.RemoteEndPoint.ToString());
                clients.Add(client);
                
                // Start receiving data from this client
                ReceiveData(client);
            }
        }

        async void ReceiveData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);

            while (true)
            {
                string message = await reader.ReadLineAsync();

                // If message is null then the client has disconnected
                if(message == null)
                {
                    clients.Remove(client);

                    Trace.TraceInformation("Disconnected from client {0}", client.Client.RemoteEndPoint.ToString());

                    return;
                }

                Trace.TraceInformation("Received data from client {0}: {1}", client.Client.RemoteEndPoint.ToString(), message);

                // Process data
                string[] words = message.Split(' ');
                if(words[0] == "GameResults")
                {
                    Game.RoundResults.Add(client.Client.RemoteEndPoint.ToString(), int.Parse(words[1]));
                }
            }
        }

        public async Task SendData(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);

            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
            Trace.TraceInformation("Sent data to client {0}: {1}", client.Client.RemoteEndPoint.ToString(), message);
        }

        public async Task BroadcastData(string message)
        {
            foreach (TcpClient client in clients)
            {
                await SendData(client, message);
            }

            Trace.TraceInformation("Broadcasted data to all clients: {0}", message);
        }
    }
}
