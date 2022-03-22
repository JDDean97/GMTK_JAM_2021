// Smooth towards the target

using UnityEngine;
using System.Collections;

public class cameraSmoother : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    PlayerControl[] climbers;
    float zoomSpeed = 2.4f;
    const float relief = 5;
    float _relief = 5; //time before zooming in

    private void Start()
    {
        climbers = FindObjectsOfType<PlayerControl>();
    }

    void Update()
    {

        //find characters screen position and zoom out until they're both on screen        
        if(!CheckInMargin(0.1f,0.9f))
        {
            Camera.main.orthographicSize += zoomSpeed * Time.deltaTime;
            _relief = relief;
        }
        else if(CheckInMargin(0.25f,0.75f))
        {
            _relief -= 1 * Time.deltaTime;
            if (_relief < 0)
            {
                //Camera.main.orthographicSize -= zoomSpeed * Time.deltaTime;
            }
        }

        // Define a target position above and behind the target transform
        //Vector3 targetPosition = target.TransformPoint(new Vector3(0, 0, -5));
        Vector3 targetPosition = climbers[0].transform.position + (climbers[1].transform.position - climbers[0].transform.position) / 2;
        targetPosition.z = -40;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    bool CheckInMargin(float low, float high)
    {
        
        Vector2 p1pos = Camera.main.WorldToViewportPoint((Vector2)climbers[0].transform.position /*+ climbers[0].GetComponent<Rigidbody2D>().velocity*/);
        Vector2 p2pos = Camera.main.WorldToViewportPoint((Vector2)climbers[1].transform.position + climbers[1].GetComponent<Rigidbody2D>().velocity);
        //Debug.Log("p1pos: " + p1pos);
        if (p1pos.x > high || p1pos.x < low)
        {
            return false;
        }
        if (p1pos.y > high || p1pos.y < low)
        {
            return false;
        }
        if (p2pos.x > high || p2pos.x < low)
        {
            return false;
        }
        if (p2pos.y > high || p2pos.y < low)
        {
            return false;
        }
        return true;
    }
}
