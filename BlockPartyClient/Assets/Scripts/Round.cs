using UnityEngine;
using System.Collections;

public class Round : MonoBehaviour {
    public BlockManager BlockManager;
    public Grid Grid;
    public Creep Creep;
    public int DyingCount;
    public int Score;

	// Use this for initialization
	void Start () {
        BlockManager.StartRound();
        Grid.StartRound();
        Creep.StartRound();
	}

    public void Lose()
    {
        print("Game over");
    }

	// Update is called once per frame
	void Update () {
	
	}
}
