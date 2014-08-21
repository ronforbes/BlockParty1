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
    public GUIText PersonalName;
    public GUITexture PersonalPicture;

    void Start()
    {
        if (!Application.isEditor && FB.IsLoggedIn)
        {
            FB.API("/me", Facebook.HttpMethod.GET, OnGetMe);
        }
        else
        {
            PersonalName.text = "Player Name";
        }

        networkingManager = GameObject.Find("Networking Manager").GetComponent<NetworkingManager>();
        networkingManager.MessageReceived += networkingManager_MessageReceived;
    }

    void OnGetMe(FBResult result)
    {
        var dictionary = Json.Deserialize(result.Text) as Dictionary<string, object>;
        PersonalName.text = dictionary["name"] as string;
        string facebookId = dictionary["id"] as string;

        StartCoroutine("GetProfilePicture", facebookId);
    }

    IEnumerator GetProfilePicture(string facebookId)
    {
        WWW www = new WWW("https" + "://graph.facebook.com/" + facebookId + "/picture");
        Debug.Log("Loading profile picture");
        yield return www;
        Texture2D pictureTexture = new Texture2D(128, 128, TextureFormat.DXT1, false);
        PersonalPicture.texture = pictureTexture;
        www.LoadImageIntoTexture(pictureTexture);
        Debug.Log("Loaded profile picture");
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
