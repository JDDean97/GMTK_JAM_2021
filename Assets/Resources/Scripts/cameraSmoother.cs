// Smooth towards the target

using UnityEngine;
using System.Collections;

public class cameraSmoother : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    PlayerControl[] climbers;
    float zoomSpeed = 1;

    private void Start()
    {
        climbers = FindObjectsOfType<PlayerControl>();
    }

    void Update()
    {

        //find characters screen position and zoom out until they're both on screen
        Vector2 p1pos = Camera.main.WorldToViewportPoint((Vector2)climbers[0].transform.position + climbers[0].GetComponent<Rigidbody2D>().velocity);
        Vector2 p2pos = Camera.main.WorldToViewportPoint((Vector2)climbers[1].transform.position + climbers[1].GetComponent<Rigidbody2D>().velocity);
        Debug.Log("p1: " + p1pos + "  p2: " + p2pos);
        if(p1pos.x >1 || p1pos.x<0)
        {
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
        }
        if (p1pos.y > 1 || p1pos.y < 0)
        {
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
        }
        if (p2pos.x > 1 || p2pos.x < 0)
        {
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
        }
        if (p2pos.y > 1 || p2pos.y < 0)
        {
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
        }

        // Define a target position above and behind the target transform
        //Vector3 targetPosition = target.TransformPoint(new Vector3(0, 0, -5));
        Vector3 targetPosition = climbers[0].transform.position + (climbers[1].transform.position - climbers[0].transform.position) / 2;
        targetPosition.z = -40;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
