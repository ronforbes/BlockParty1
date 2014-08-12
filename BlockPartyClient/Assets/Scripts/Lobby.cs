using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;
using System.Collections.Generic;
using BlockPartyShared;
using System;

public class Lobby : MonoBehaviour
{
    NetworkingManager networkingManager;
    List<KeyValuePair<string, int>> rankings;
    bool startGame = false;
    bool updateRanking = false;
    public GUIText Rank;
    public GUIText Name;
    public GUIText Score;
    public GUIText Countdown;

    void Start()
    {
        if (FB.IsLoggedIn)
        {
            FB.API("/me", Facebook.HttpMethod.GET, OnGetName);
        }

        networkingManager = GameObject.Find("Networking Manager").GetComponent<NetworkingManager>();
        networkingManager.MessageReceived += networkingManager_MessageReceived;
    }

    void OnGetName(FBResult result)
    {
        var dictionary = Json.Deserialize(result.Text) as Dictionary<string, object>;
        name = dictionary["name"] as string;
    }

    void networkingManager_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        switch (e.Message.Type)
        {
            case NetworkMessage.MessageType.ServerGameState:
                if ((string)e.Message.Content == "Game")
                {
                    startGame = true;
                }
                break;

            case NetworkMessage.MessageType.ServerGameResults:
                rankings = (List<KeyValuePair<string, int>>)e.Message.Content;

                updateRanking = true;
                break;
        }
    }

    void Update()
    {
        if (startGame)
        {
            if (networkingManager != null)
            {
                networkingManager.MessageReceived -= networkingManager_MessageReceived;
            }

            Application.LoadLevel("Game");
        }

        if (updateRanking)
        {
            for (int i = 0; i < rankings.Count; i++)
            {
                Rank.text += (i + 1).ToString() + "\n";
                Name.text += rankings[i].Key + "\n";
                Score.text += rankings[i].Value + "\n";
            }
            updateRanking = false;
        }
    }
}
