using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;
using System.Collections.Generic;
using BlockPartyShared;
using System;

public class Lobby : MonoBehaviour
{
    List<KeyValuePair<string, int>> rankings;
    bool startGame = false;
    bool updateRanking = false;
    public GUIText Rank;
    public GUIText Name;
    public GUIText Score;
    public GUIText Countdown;

    // Use this for initialization
    void Start()
    {
        if (FB.IsLoggedIn)
        {
            FB.API("/me", Facebook.HttpMethod.GET, OnGetName);
        }

        NetworkingManager networkingManager = GameObject.Find("Networking Manager").GetComponent<NetworkingManager>();
        networkingManager.MessageReceived += networkingManager_MessageReceived;
        networkingManager.SendData("Time");
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
            case NetworkMessage.MessageType.GameState:
                if ((string)e.Message.Content == "Gameplay")
                {
                    Debug.Log("Starting gameplay");
                    startGame = true;
                }
                break;

            case NetworkMessage.MessageType.RoundResults:
                rankings = (List<KeyValuePair<string, int>>)e.Message.Content;

                updateRanking = true;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (startGame)
        {
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
