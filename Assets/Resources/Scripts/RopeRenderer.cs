using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeRenderer : MonoBehaviour
{

    private GameObject climber1;
    private GameObject climber2;
    private GameObject ropeStart;
    private GameObject ropeEnd;
    private GameObject currentSegment;
    private Vector3 ropeStartAnchor;
    private Vector3 ropePosition;
    private Vector3 localRopePosition;
    public int numberOfSegments = 30;
    public float scaleFactor = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        climber1 = GameObject.Find("spr_climber1");
        climber2 = GameObject.Find("spr_climber2");

        ropeStart = climber1.transform.Find("rope_attachment").gameObject;
        ropeEnd = climber2.transform.Find("rope_attachment").gameObject;

        ropePosition = ropeStart.transform.position;
        localRopePosition = ropeStart.transform.localPosition;

        currentSegment = climber1;

        AttachInitalRopeSegment(currentSegment);
        for (int i = 2; i <= numberOfSegments; i++)
        {
          AttachRopeSegment(currentSegment, i);
        }
        AttachFinalRopeSegment(currentSegment);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void AttachInitalRopeSegment(GameObject previousSegment)
    {
        GameObject ropeSegment = GenerateRopeSegment(1);
        ropeSegment.GetComponent<SpriteRenderer>().sortingOrder = 1;

        Vector3 extents = ropeSegment.GetComponent<SpriteRenderer>().bounds.extents;

        Vector3 adjustedRopePosition = ropePosition;
        adjustedRopePosition.x += extents[0];
        ropeSegment.transform.position = adjustedRopePosition;

        Vector3 connectedAnchorPosition = previousSegment.transform.localPosition;
        connectedAnchorPosition.x += (extents[0] / scaleFactor);

        previousSegment.AddComponent<HingeJoint2D>();
        previousSegment.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
        previousSegment.GetComponent<HingeJoint2D>().connectedBody = ropeSegment.GetComponent<Rigidbody2D>();
        previousSegment.GetComponent<HingeJoint2D>().anchor = localRopePosition;
        previousSegment.GetComponent<HingeJoint2D>().connectedAnchor = connectedAnchorPosition;

        currentSegment = ropeSegment;
    }

    void AttachRopeSegment(GameObject previousSegment, int segmentNum)
    {
        GameObject ropeSegment = GenerateRopeSegment(segmentNum);

        Vector3 extents = ropeSegment.GetComponent<SpriteRenderer>().bounds.extents;

        Vector3 adjustedRopePosition = previousSegment.transform.position;
        adjustedRopePosition.x += (extents[0] * 2);
        ropeSegment.transform.position = adjustedRopePosition;

        float unscaledExtent = extents[0] / scaleFactor;
        Vector3 anchorPosition = new Vector3((float)(unscaledExtent - 0.5f), 0, 0);
        Vector3 connectedAnchorPosition = new Vector3((float)(-(unscaledExtent - 0.5f)), 0, 0);

        previousSegment.AddComponent<HingeJoint2D>();
        previousSegment.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
        previousSegment.GetComponent<HingeJoint2D>().connectedBody = ropeSegment.GetComponent<Rigidbody2D>();
        previousSegment.GetComponent<HingeJoint2D>().anchor = anchorPosition;
        previousSegment.GetComponent<HingeJoint2D>().connectedAnchor = connectedAnchorPosition;

        currentSegment = ropeSegment;
    }

    void AttachFinalRopeSegment(GameObject previousSegment)
    {
        Vector3 finalRopePosition = climber2.transform.position;
        finalRopePosition += ropeEnd.transform.localPosition;
        // GameObject ropeSegment = GenerateRopeSegment(1);
        // ropeSegment.GetComponent<SpriteRenderer>().sortingOrder = 1;

        Vector3 extents = previousSegment.GetComponent<SpriteRenderer>().bounds.extents;

        // Vector3 adjustedRopePosition = ropePosition;
        // adjustedRopePosition.x += extents[0];
        // ropeSegment.transform.position = adjustedRopePosition;

        // Vector3 connectedAnchorPosition = previousSegment.transform.localPosition;
        // connectedAnchorPosition.x += (extents[0] / scaleFactor);
        float unscaledExtent = extents[0] / scaleFactor;
        Vector3 anchorPosition = new Vector3((float)(unscaledExtent - 0.5f), 0, 0);

        previousSegment.AddComponent<HingeJoint2D>();
        previousSegment.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
        previousSegment.GetComponent<HingeJoint2D>().connectedBody = climber2.GetComponent<Rigidbody2D>();
        previousSegment.GetComponent<HingeJoint2D>().anchor = anchorPosition;
        previousSegment.GetComponent<HingeJoint2D>().connectedAnchor = finalRopePosition;
    }

    GameObject GenerateRopeSegment(int segmentNum)
    {
      GameObject ropeSegment = new GameObject("segment" + segmentNum);
      ropeSegment.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1.0f);
      ropeSegment.AddComponent<SpriteRenderer>();
      ropeSegment.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/rope_segment");
      ropeSegment.GetComponent<SpriteRenderer>().sortingOrder = 2;
      ropeSegment.AddComponent<Rigidbody2D>();
      ropeSegment.GetComponent<Rigidbody2D>().mass = 3;

      return ropeSegment;
    }
}
