using UnityEngine;
using System.Collections;

public class Round : MonoBehaviour {
    public BlockManager BlockManager;
    public Grid Grid;
    public Creep Creep;
    public int DyingCount;
    public Score Score;
	public Timer Timer;

    public enum RoundState
    {
        Countdown,
        Gameplay
    }

    public RoundState State;

	// Use this for initialization
	void Start () {
        BlockManager.StartRound();
        Grid.StartRound();
        Creep.StartRound();

        State = RoundState.Countdown;
	}

    public void Lose()
    {
        print("Game over");
    }

	// Update is called once per frame
	void Update () {
	
	}
}
