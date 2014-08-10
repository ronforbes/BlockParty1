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
    bool startRound, endRound;

    string name;

    // Use this for initialization
    void Start()
    {
        networkingManager = GameObject.Find("Networking Manager").GetComponent<NetworkingManager>();
        networkingManager.MessageReceived += networkingManager_MessageReceived;

        round = Instantiate(RoundPrefab) as Round;
    }

    void networkingManager_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        switch (e.Message.Type)
        {
            case NetworkMessage.MessageType.GameState:
                if ((string)e.Message.Content == "Pregame")
                {
                    endRound = true;
                    Debug.Log("Starting pregame");
                }
                break;
        }
    }

    public void EndRound()
    {
        if (round != null)
        {
            if (networkingManager.Connected)
            {
                networkingManager.SendData("GameResults " + round.Score.RoundScore);
                //NetworkingManager.SendRoundResults(round.Score);
            }
            
            Application.LoadLevel("Lobby");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (endRound)
        {
            EndRound();
            endRound = false;
        }
    }
}
