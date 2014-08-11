using UnityEngine;
using System.Collections;

public class Round : MonoBehaviour
{
    public BlockManager BlockManager;
    public Grid Grid;
    public Creep Creep;
    public Score Score;
    public Timer Timer;

    public enum RoundState
    {
        Countdown,
        Gameplay,
        Loss
    }

    public RoundState State;

    // Use this for initialization
    void Start()
    {
        BlockManager.StartRound();
        Grid.StartRound();
        Creep.StartRound();

        State = RoundState.Countdown;
    }

    public void Lose()
    {
        State = RoundState.Loss;
    }

    // Update is called once per frame
    void Update()
    {
	
    }
}
