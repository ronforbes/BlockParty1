using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System;
using BlockPartyShared;
using System.Collections.Generic;
using Facebook.MiniJSON;

public class Game : MonoBehaviour 
{
    public NetworkingManager NetworkingManager;
    public Round RoundPrefab;

    Round round;
    bool startRound, endRound;
	List<KeyValuePair<string, int>> rankings;
		string name;

	// Use this for initialization
	void Start () 
    {
				if (FB.IsLoggedIn) {
						FB.API ("/me", Facebook.HttpMethod.GET, OnGetName);
				}
	}

		void OnGetName(FBResult result)
		{
				var dictionary = Json.Deserialize (result.Text) as Dictionary<string, object>;
				name = dictionary["name"] as string;
		}

    public void ProcessData(NetworkMessage data)
    {
        switch(data.Type)
        {
				case NetworkMessage.MessageType.GameState:
						if ((string)data.Content == "Gameplay") {
								startRound = true;
								Debug.Log ("Starting gameplay");
						}

						if ((string)data.Content == "Pregame") 
						{
								endRound = true;
								Debug.Log("Starting pregame");
						}
                		break;

				case NetworkMessage.MessageType.RoundResults:
						rankings = (List<KeyValuePair<string, int>>)data.Content;
						for (int i = 0; i < rankings.Count; i++) {
								Debug.Log(rankings[i].Key + ": " + rankings[i].Value);
						}
						break;
        }
    }

    public void StartRound()
    {
        if(round != null)
        {
            EndRound();
        }

        round = Instantiate(RoundPrefab) as Round;
    }

    public void EndRound()
    {
        if(round != null)
        {
            if(NetworkingManager.Connected)
            {
                NetworkingManager.SendData("GameResults " + round.Score.RoundScore);
                //NetworkingManager.SendRoundResults(round.Score);
            }
            
            Destroy(round.gameObject);
            round = null;
        }
    }

	// Update is called once per frame
	void Update () 
    {
        if(startRound)
        {
            StartRound();

            startRound = false;
        }

        if(endRound)
        {
            EndRound();
						rankings = null;
            endRound = false;
        }
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 20), "Start round"))
        {
            StartRound();
        }

        if(GUI.Button(new Rect(0, 20, 100, 20), "End round"))
        {
            EndRound();
        }

        if(GUI.Button(new Rect(0, 40, 200, 20), "NetworkingManager.Connect"))
        {
            if(!NetworkingManager.Connected)
            {
                NetworkingManager.Connect();
            }
        }

        if(GUI.Button(new Rect(0, 60, 200, 20), "NetworkingManager.Disconnect"))
        {
            if(NetworkingManager.Connected)
            {
                NetworkingManager.Disconnect();
            }
        }

				if (round == null && rankings != null) {
						for (int i = 0; i < rankings.Count; i++) {
								GUI.Label(new Rect(0, 100 + i * 20, 200, 20), rankings[i].Key + ": " + rankings[i].Value.ToString());
						}

				}

				if (FB.IsLoggedIn) {
						GUI.Label (new Rect (0, 200, 200, 20), name);
				}
    }
}
