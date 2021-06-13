using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public float ropeLength = 20;
    float ropeSlide = 2;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("length: " + ropeLength);
    }

    public void shortenRope()
    {
        ropeLength += ropeSlide * Time.deltaTime;
        ropeLength = Mathf.Clamp(ropeLength, 0, 25);
    }

    public void lengthenRope()
    {
        ropeLength -= ropeSlide * Time.deltaTime;
        ropeLength = Mathf.Clamp(ropeLength, 0.1f, 25);
    }
}
