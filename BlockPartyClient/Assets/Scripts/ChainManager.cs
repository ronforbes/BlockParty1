using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChainManager : MonoBehaviour 
{
    public Score Score;

    List<Chain> chains = new List<Chain>(chainCapacity);

	const int chainCapacity = 8;

	public Chain CreateChain()
	{
        if (chains.Count == chains.Capacity)
            return null;

        Chain chain = new Chain();
        chains.Add(chain);
        chain.Initialize();
        
        return chain;
	}

	public void DeleteChain(Chain chain)
	{
        chains.Remove(chain);
	}

	// Update is called once per frame
	void Update() 
	{
        List<Chain> chainsToRemove = new List<Chain>();

        foreach(Chain chain in chains)
        {
            if(chain.InvolvementCount == 0)
            {
                chainsToRemove.Add(chain);
            }
            else
            {
                if(chain.MatchJustOccurred)
                {
                    // notify the score
                    Score.RoundScore++;

                    chain.MatchJustOccurred = false;
                }
            }
        }

        foreach(Chain chain in chainsToRemove)
        {
            DeleteChain(chain);
        }
	}
}
