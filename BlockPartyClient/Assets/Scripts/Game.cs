using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BlockPartyShared;
using Facebook.MiniJSON;
using Microsoft.Win32;
using UnityEngine;

public class Game : MonoBehaviour
{
    NetworkingManager networkingManager;
    public Round RoundPrefab;

    Round round;
    bool hasEnded;

    // Use this for initialization
    void Start()
    {
        GameObject networkingManagerObject = GameObject.Find("Networking Manager");

        if (networkingManagerObject == null)
        {
            Debug.LogWarning("Couldn't find the Networking Manager. Proceeding without networking...");
        }
        else
        {
            networkingManager = networkingManagerObject.GetComponent<NetworkingManager>();
            networkingManager.MessageReceived += networkingManager_MessageReceived;
        }

        round = Instantiate(RoundPrefab) as Round;
    }

    void networkingManager_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        switch (e.Message.Type)
        {
            case NetworkMessage.MessageType.ServerGameState:
                if ((string)e.Message.Content == "Lobby")
                {
                    hasEnded = true;
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasEnded)
        {
            End();
            hasEnded = false;
        }
    }

    void End()
    {
        if (networkingManager != null)
        {
            if (networkingManager.Connected)
            {
                NetworkMessage message = new NetworkMessage();
                message.Type = NetworkMessage.MessageType.ClientGameResults;
                message.Content = round.Score.RoundScore;
                networkingManager.Send(message);
            }
        }
            
        Application.LoadLevel("Lobby");
    }
}
