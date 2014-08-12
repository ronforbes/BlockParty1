using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    public int RoundTimer;
    public GUIStyle Style;
    float elapsed;
    const float duration = 1.0f;

    void Start()
    {
	
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            RoundTimer--;
            elapsed = 0.0f;
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 110, 120, 100, 100), "Timer");
        GUI.Label(new Rect(Screen.width - 110, 150, 100, 20), RoundTimer.ToString(), Style);
    }
}
