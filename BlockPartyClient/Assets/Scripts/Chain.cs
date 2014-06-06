using UnityEngine;

public class Chain
{
	public float Timestamp;
	public float CreationTimestamp;
	public int InvolvementCount;
	public int Magnitude;
	public int Multiplier = 1;
	public int MultiplierCount;
	public int BaseAccumulatedScore;
	public int BaseScore;
	public int LatestMagnitude;
	public int X, Y;
	public bool MatchJustOccurred;

	public void Initialize()
	{
		CreationTimestamp = Time.time;
	}

	public void ReportMatch(int magnitude, Block block)
	{
		X = block.X;
		Y = block.Y;

		Timestamp = Time.time;

		if(Time.time != CreationTimestamp)
		{
			Multiplier++;
			MultiplierCount++;
		}

		Magnitude += magnitude;

		MatchJustOccurred = true;
	}

	public void IncrementInvolvement()
	{
		InvolvementCount++;
	}

	public void DecrementInvolvement()
	{
		InvolvementCount--;
	}
}