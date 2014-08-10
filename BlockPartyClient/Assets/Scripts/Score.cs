using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
    public int RoundScore;
    public GUIStyle Style;

    // Use this for initialization
    void Start()
    {
	
    }

    public void ReportMatch(Chain chain)
    {
        RoundScore += chain.Magnitude * 100;
    }

    // Update is called once per frame
    void Update()
    {
	
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 110, 10, 100, 100), "Score");
        GUI.Label(new Rect(Screen.width - 110, 40, 100, 20), RoundScore.ToString(), Style);
    }
}
