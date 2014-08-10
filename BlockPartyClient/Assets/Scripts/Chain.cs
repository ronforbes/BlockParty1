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
    SignManager signManager;

    public void Initialize(SignManager signManager)
    {
        CreationTimestamp = Time.time;
        this.signManager = signManager;
    }

    public void ReportMatch(int magnitude, Block block)
    {
        X = block.X;
        Y = block.Y;

        Timestamp = Time.time;

        if (Time.time != CreationTimestamp)
        {
            Multiplier++;
            MultiplierCount++;

            signManager.CreateSign(X, Y, Multiplier.ToString() + "x");
        }

        Magnitude += magnitude;

        if (magnitude > Grid.MinimumPatternLength)
        {
            signManager.CreateSign(X, Y, Magnitude.ToString());
        }

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