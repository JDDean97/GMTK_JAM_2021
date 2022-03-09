using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    Camera cam;
    Vector3 dir;
    float speed = 0.9f;
    Vector3 camPos;
    public enum focus { behind, front };
    public focus _focus = focus.behind;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        camPos = cam.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dir = cam.transform.position - camPos;
        dir.z = 0;
        if (_focus == focus.behind)
        {

            speed = 0.9f;
            
        }
        else
        {
            speed = 1.4f;
            dir = -dir;
        }
        transform.position += dir * speed;
        camPos = cam.transform.position;
    }
}
