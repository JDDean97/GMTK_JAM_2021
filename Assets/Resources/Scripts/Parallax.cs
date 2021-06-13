using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    Camera cam;
    Vector3 dir;
    float speed = 0.2f;
    Vector3 camPos;
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
        dir = cam.transform.position - transform.position;
        dir.z = 0;
        transform.position += dir.normalized * 0.2f * speed;
        camPos = cam.transform.position;
    }
}
