using UnityEngine;
using System.Collections;

public class Sign : MonoBehaviour
{
    public int X, Y;
    public float Elapsed;
    public const float Duration = 3.0f;
    public bool Active;

    void Start()
    {
	
    }

    public void Initialize(int x, int y, string text)
    {
        X = x;
        Y = y;
        TextMesh mesh = transform.Find("Text").GetComponent<TextMesh>() as TextMesh;
        mesh.text = text;
        Active = true;
    }

    void Update()
    {
        Elapsed += Time.deltaTime;

        if (Elapsed >= Duration)
        {
            Active = false;
        }
    }
}
