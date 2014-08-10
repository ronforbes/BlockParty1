using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour
{
    public NetworkingManager NetworkingManager;

    // Use this for initialization
    void Start()
    {
        FB.Init(OnInit);
    }

    void OnInit()
    {

    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 100, 100), "Play"))
        {
            if (!Application.isEditor && !FB.IsLoggedIn)
            {
                FB.Login("public_profile", OnLogin);
            }
            else
            {
                if (!NetworkingManager.Connected)
                {
                    NetworkingManager.Connect();
                }

                Application.LoadLevel("Lobby");
            }
            
        }
    }

    void OnLogin(FBResult result)
    {
        Application.LoadLevel("Lobby");
    }
}
