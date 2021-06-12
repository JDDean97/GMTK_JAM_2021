// Smooth towards the target

using UnityEngine;
using System.Collections;

public class cameraSmoother : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    PlayerControl[] climbers;

    private void Start()
    {
        climbers = FindObjectsOfType<PlayerControl>();
    }

    void Update()
    {
        // Define a target position above and behind the target transform
        //Vector3 targetPosition = target.TransformPoint(new Vector3(0, 0, -5));
        Vector3 targetPosition = climbers[0].transform.position + (climbers[1].transform.position - climbers[0].transform.position) / 2;
        targetPosition.z = -40;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
