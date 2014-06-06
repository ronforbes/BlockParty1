﻿using UnityEngine;
using System.Collections;

public class Creep : MonoBehaviour 
{
	public BlockManager BlockManager;
	public Grid Grid;
	public Round Round;
	public CreepController Controller;
	public float CreepElapsed;

	public const float CreepDuration = 1.0f;

	bool creepFreeze;
	float lossElapsed;
	bool advance;
	float creepDelayElapsed;
	float creepDelaySpeed = 1.0f;

	const float lossDuration = 3.0f;
	const float advanceDelaySpeed = 100.0f;
	const float creepDelayDuration = 1.0f;

    public void StartRound()
    {
        CreepElapsed = 0.0f;
        creepFreeze = false;
        lossElapsed = 0.0f;
        advance = false;
        creepDelayElapsed = 0.0f;
        creepDelaySpeed = 1.0f;

        BlockManager.CreateCreepRow();
    }
	
	// Update is called once per frame
	void Update () 
	{
		if(Round.DyingCount != 0)
		{
			return;
		}

		if(creepFreeze)
		{
			if(!Grid.CheckSafeHeightViolation())
				creepFreeze = false;
			else
			{
				lossElapsed += Time.deltaTime;

				if(lossElapsed >= lossDuration)
					Round.Lose();

				return;
			}
		}
		else
		{
			if(Grid.CheckSafeHeightViolation())
			{
				creepFreeze = true;
				lossElapsed = 0.0f;
			}
		}

		if(advance || Controller.AdvanceCommand)
		{
			if(creepDelaySpeed < advanceDelaySpeed)
			{
				creepDelayElapsed += advanceDelaySpeed * Time.deltaTime;
			}
			else
			{
				creepDelayElapsed += creepDelaySpeed * Time.deltaTime;
			}
				
			advance = true;
		}
		else
		{
			creepDelayElapsed += creepDelaySpeed * Time.deltaTime;
		}

		while(creepDelayElapsed >= creepDelayDuration)
		{
			creepDelayElapsed -= creepDelayDuration;

			CreepElapsed += Time.deltaTime;

			if(CreepElapsed >= CreepDuration)
			{
				CreepElapsed = 0.0f;
				
				// shift everything up one grid row
				if(Grid.ShiftUp())
				{
					// create a new bottom creep row
					BlockManager.CreateCreepRow();
					
					//link the elimination requests
					for(int x = 0; x < Grid.PlayWidth; x++)
					{
						Grid.RequestMatchCheck(Grid.BlockAt(x, 1));
					}
				}
				else
				{
					creepDelayElapsed += creepDelayDuration;
					CreepElapsed = CreepDuration - 0.1f;
				}
				
				if(advance && !Controller.AdvanceCommand)
				{
					advance = false;
				}
			}
		}
	}
}
