using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour 
{
	public int Type;
    public const int TypeCount = 5;

	public int X, Y;

    public enum BlockState
    {
        Idle,
        Sliding,
        Falling,
        Dying,
    }

	public BlockState State;
    bool changeState; // Used to signal transitioning to a new state on the next frame (TODO: This is a complete hack! Investigate why Unity crashes when I change state from static to falling)

	public Chain Chain;

	public Slider.SlideDirection Direction;
	public bool SlideFront;

	public float FallElapsed;
    public const float FallDuration = 0.1f;

	public float DieElapsed;
    public const float DieDuration = 1.5f;	
	public Vector2 DyingAxis;

	BlockManager blockManager;

	Grid grid;

	Round round;

	// Use this for initialization
	void Start() 
	{
		blockManager = GameObject.Find("Block Manager").GetComponent<BlockManager>();
		grid = GameObject.Find("Grid").GetComponent<Grid>();
		round = GameObject.Find ("Round(Clone)").GetComponent<Round>();
	}

	public void InitializeIdle(int x, int y, int type)
	{
		X = x;
		Y = y;
		Type = type;

		State = BlockState.Idle;

		// Initializing Grid here again. It looks like Start isn't getting called until the next frame
		grid = GameObject.Find("Grid").GetComponent<Grid>();
		grid.AddBlock(x, y, this, GridElement.ElementState.Block);

		transform.position = new Vector3(X, Y, 0.0f);
	}

	// Update is called once per frame
	void Update () {
		// don't update the creep row
		if(Y == 0)
			return;

        if(changeState)
        {
            State = BlockState.Falling;

            changeState = false;
        }

		switch(State)
		{
		case BlockState.Idle:
			// we may have to fall
			if(grid.StateAt(X, Y - 1) == GridElement.ElementState.Empty)
                StartFalling();
			break;

		case BlockState.Falling:
			FallElapsed += Time.deltaTime;

			if(FallElapsed >= FallDuration)
			{
				if(grid.StateAt(X, Y - 1) == GridElement.ElementState.Empty)
				{
					// shift our grid position down to the next row
					Y--;
					FallElapsed = 0.0f;

					grid.Remove(X, Y + 1, this);
					grid.AddBlock(X, Y, this, GridElement.ElementState.Falling);
				}
				else
				{
					// we've landed

					// change our state
                    State = BlockState.Idle;

					// update the grid
					grid.ChangeState(X, Y, this, GridElement.ElementState.Block);

					// register for elimination checking
					grid.RequestMatchCheck(this);
				}
			}
			break;
		case BlockState.Dying:
			DieElapsed += Time.deltaTime;

			if(DieElapsed >= DieDuration)
			{
				// change the game state
				round.DyingCount--;

				// update the grid
				grid.Remove(X, Y, this);

				// tell our upward neighbor to fall
				if(Y < Grid.PlayHeight - 1)
				{
					if(grid.StateAt(X, Y + 1) == GridElement.ElementState.Block)
						grid.BlockAt(X, Y + 1).StartFalling(Chain);
					// TODO: do the same for garbage
				}

				Chain.DecrementInvolvement();

                ParticleManager particleManager = FindObjectOfType<ParticleManager>();
                particleManager.CreateParticles(X, Y, Chain.Magnitude);

				blockManager.DeleteBlock(this);
			}
			break;
		}
	}

	public void StartSliding(Slider.SlideDirection direction, bool slideFront)
	{
		State = BlockState.Sliding;

		Direction = direction;

		SlideFront = slideFront;

		grid.ChangeState(X, Y, this, GridElement.ElementState.Immutable);
	}

	public void FinishSliding(int slideX)
	{
		State = BlockState.Idle;

		Direction = Slider.SlideDirection.None;

		X = slideX;

		grid.AddBlock(X, Y, this, GridElement.ElementState.Block);
	}

	public void StartFalling(Chain chain = null)
	{
		if(State != BlockState.Idle)
			return;
        
		// change our state
        changeState = true;
        
		FallElapsed = FallDuration;
        
		grid.ChangeState(X, Y, this, GridElement.ElementState.Falling);

		if(chain != null)
		{
			BeginChainInvolvement(chain);
		}

		if(Y < Grid.PlayHeight - 1)
		{
			if(grid.StateAt(X, Y + 1) == GridElement.ElementState.Block)
				grid.BlockAt(X, Y + 1).StartFalling(Chain);
		}
	}

	public void StartDying(Chain chain, int sparkNumber)
	{
		// change the game state
		round.DyingCount++;

		BeginChainInvolvement(chain);

		State = BlockState.Dying;
		DieElapsed = 0.0f;

		grid.ChangeState(X, Y, this, GridElement.ElementState.Immutable);

		DyingAxis = Random.insideUnitCircle;
	}

	public void BeginChainInvolvement(Chain chain)
	{
		if(Chain != null)
		{
			Chain.DecrementInvolvement();
		}

		Chain = chain;
		Chain.IncrementInvolvement();
	}

	public void EndChainInvolvement(Chain chain)
	{
		if(Chain != null && Chain == chain)
		{
			Chain.DecrementInvolvement();
			Chain = null;
		}
	}
}
