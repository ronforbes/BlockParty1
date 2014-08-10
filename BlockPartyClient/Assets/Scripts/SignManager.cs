using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignManager : MonoBehaviour
{
    public List<Sign> Signs;
    public Sign SignPrefab;

    // Use this for initialization
    void Start()
    {
        Signs = new List<Sign>();
    }

    public void CreateSign(int x, int y, string text)
    {
        Sign sign = Instantiate(SignPrefab, Vector3.zero, Quaternion.identity) as Sign;
        sign.transform.parent = transform;
        sign.Initialize(x, y, text);
        Signs.Add(sign);
    }

    // Update is called once per frame
    void Update()
    {
        List<Sign> signsToRemove = new List<Sign>();

        foreach (Sign sign in Signs)
        {
            if (!sign.Active)
            {
                signsToRemove.Add(sign);
            }
        }

        foreach (Sign sign in signsToRemove)
        {
            Signs.Remove(sign);
            Destroy(sign.gameObject);
        }
    }
}
