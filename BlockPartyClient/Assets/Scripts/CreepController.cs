using UnityEngine;
using System.Collections;

public class CreepController : MonoBehaviour
{
    public Round Round;
    public bool AdvanceCommand;

    float previousClickTime;
    const float doubleClickSpeed = 0.5f;
	
    // Update is called once per frame
    void Update()
    {
        if (Round.State == Round.RoundState.Countdown || Round.State == Round.RoundState.Loss)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - previousClickTime <= doubleClickSpeed)
                AdvanceCommand = true;
            else
                AdvanceCommand = false;

            previousClickTime = Time.time;
        }
        else
            AdvanceCommand = false;
    }
}
