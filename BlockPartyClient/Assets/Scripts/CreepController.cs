using UnityEngine;
using System.Collections;

public class CreepController : MonoBehaviour 
{
	public bool AdvanceCommand;

	float previousClickTime;
	const float doubleClickSpeed = 0.5f;
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			if(Time.time - previousClickTime <= doubleClickSpeed)
				AdvanceCommand = true;
			else
				AdvanceCommand = false;

			previousClickTime = Time.time;
		}
		else
			AdvanceCommand = false;
	}
}
