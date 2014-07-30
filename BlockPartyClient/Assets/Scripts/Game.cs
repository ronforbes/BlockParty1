using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System;

public class Game : MonoBehaviour 
{
    public NetworkingManager NetworkingManager;
    public Round RoundPrefab;

    Round round;
    bool startRound, endRound;

	// Use this for initialization
	void Start () 
    {
        
	}

    public void ProcessData(string data)
    {
        switch(data)
        {
            case "GameState Gameplay":
                startRound = true;
                Debug.Log("Starting gameplay");
                break;
                
            case "GameState Pregame":
                endRound = true;
                Debug.Log("Starting pregame");
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
    }
}
