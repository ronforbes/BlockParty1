using UnityEngine;
using System.Collections;
using LostPolygon.System.Net.Sockets;
using System.IO;
using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using BlockPartyShared;
using UnityEditor.VersionControl;

public class NetworkingManager : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    BinaryFormatter formatter;

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    public bool Connected
    {
        get
        {
            return client != null && client.Connected;
        }
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void Connect()
    {
        #if DEBUG || UNITY_EDITOR
        client = new TcpClient("localhost", 1337);
        #else
        client = new TcpClient("54.183.32.220", 1337);
        #endif

        if (client.Connected)
        {
            Debug.Log("Connected to server " + client.Client.RemoteEndPoint.ToString());

            stream = client.GetStream();
            formatter = new BinaryFormatter();

            if (stream.CanRead)
            {
                Thread receiveThread = new Thread(Receive);
                receiveThread.Start();
            }
        }
    }

    void Receive()
    {
        while (true)
        {
            NetworkMessage message = (NetworkMessage)formatter.Deserialize(stream);
            Debug.Log("Received message from server: " + message.ToString());

            // process message
            MessageReceivedEventArgs args = new MessageReceivedEventArgs();
            args.Message = message;
            OnMessageReceived(args);
        }
    }

    protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
    {
        EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public void Send(NetworkMessage message)
    {
        formatter.Serialize(stream, message);
        Debug.Log("Sent message to server: " + message.ToString());
    }

    public void Disconnect()
    {
        client.Close();
        Debug.Log("Disconnected from server");
    }

    void Update()
    {
	
    }

    void Destroy()
    {
        Disconnect();
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }
}
