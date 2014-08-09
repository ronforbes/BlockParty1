using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
				FB.Init(OnInit);
	}

		void OnInit()
		{

		}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 100, 100), "Play"))
        {
						if (!FB.IsLoggedIn) {
								FB.Login ("public_profile", OnLogin);
						} else {
								Application.LoadLevel("Game");
						}
            
        }
    }

		void OnLogin(FBResult result)
		{

		}
}
