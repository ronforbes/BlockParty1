using UnityEngine;
using System.Collections;
//using LostPolygon.System.Net.Sockets;
using System.IO;
using System;
using System.Text;
using System.Net.Sockets;

public class NetworkingManager : MonoBehaviour 
{
    public Game Game;
    public NetworkView NetworkView;

    TcpClient client;
    NetworkStream stream;
    StreamWriter writer;
    byte[] readBuffer = new byte[1024];

    public bool Connected
    {
        get
        {
            return client != null && client.Connected;
        }
    }

	// Use this for initialization
	void Start () 
    {
        
	}

    public void Connect()
    {
        client = new TcpClient("localhost", 1337);
		//client = new TcpClient("54.183.32.220", 1337);

        if (client.Connected)
        {
            Debug.Log("Connected to server " + client.Client.RemoteEndPoint.ToString());

            stream = client.GetStream();
            writer = new StreamWriter(stream);

            if (stream.CanRead)
            {
                stream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(ReceiveData), stream);
            }
        }
    }

    public void Disconnect()
    {
        client.Close();
        Debug.Log("Disconnected from server");
    }

    void ReceiveData(IAsyncResult result)
    {
        NetworkStream resultingStream = (NetworkStream)result.AsyncState;

        int bytesRead = resultingStream.EndRead(result);

        string message = Encoding.ASCII.GetString(readBuffer, 0, bytesRead).Trim();
        Debug.Log("Received data from server " + client.Client.RemoteEndPoint.ToString() + ": " + message);

        // process message
        Game.ProcessData(message);

        resultingStream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(ReceiveData), resultingStream);
    }

    public void SendData(string message)
    {
        writer.WriteLine(message);
        writer.Flush();
        Debug.Log("Sent data to server " + client.Client.RemoteEndPoint.ToString() + ": " + message);
    }

	// Update is called once per frame
	void Update () 
    {
	
	}
    /* Unity Networking
    public bool Connected
    {
        get
        {
            return false;
        }
    }
    public void Connect()
    {
        Debug.Log(Network.Connect("localhost", 1337));
    }

    public void Disconnect()
    {

    }

    [RPC]
    public void StartRound()
    {
        Game.StartRound();
    }

    [RPC]
    public void EndRound()
    {
        Game.EndRound();
    }

    public void SendRoundResults(int score)
    {
        NetworkView.RPC("ReceiveRoundResults", RPCMode.Server, score);
        Debug.Log("Sent round results: " + score);
    }

    [RPC]
    void ReceiveRoundResults(int score) { }

    public void SendData(string message)
    {

    }*/
}
