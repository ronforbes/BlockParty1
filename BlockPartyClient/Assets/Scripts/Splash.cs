using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour
{
    void Start()
    {
	
    }

    void Update()
    {
        if (Time.time > 5.0f)
        {
            Application.LoadLevel("Menu");
        }
    }
}
