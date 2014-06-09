using UnityEngine;
using System.Collections;

public class CountdownManager : MonoBehaviour {
    float elapsed;
    public Round Round;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        elapsed += Time.deltaTime;
        TextMesh mesh = transform.Find("Text").GetComponent<TextMesh>() as TextMesh;

        if(elapsed < 1.0f)
        {
            mesh.text = "3";
        }
        else if(elapsed < 2.0f)
        {
            mesh.text = "2";
        }
        else if(elapsed < 3.0f)
        {
            mesh.text = "1";
        }
        else if(elapsed < 5.0f)
        {
            mesh.text = "GO!";
            Round.State = global::Round.RoundState.Gameplay;
        }
        else
        {
            mesh.gameObject.SetActive(false);
        }
	}
}
