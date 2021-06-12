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

        CreateInitalRopeSegment(currentSegment, 1);
        for (int i = 1; i <= numberOfSegments; i++)
        {
          CreateRopeSegment(currentSegment, i);
        }
        // CreateFinalRopeSegment(currentSegment, numberOfSegments+1);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateInitalRopeSegment(GameObject previousSegment, int segmentNum)
    {
        GameObject ropeSegment = new GameObject("segment" + segmentNum);
        ropeSegment.transform.localScale = new Vector3(0.02f, 0.02f, 1.0f);
        ropeSegment.AddComponent<SpriteRenderer>();
        ropeSegment.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/rope_segment");
        ropeSegment.GetComponent<SpriteRenderer>().sortingOrder = 3;
        ropeSegment.AddComponent<Rigidbody2D>();

        Vector3 extents = ropeSegment.GetComponent<SpriteRenderer>().bounds.extents;

        Vector3 adjustedRopePosition = ropePosition;
        adjustedRopePosition.x += extents[0];
        ropeSegment.transform.position = adjustedRopePosition;

        previousSegment.AddComponent<HingeJoint2D>();
        previousSegment.GetComponent<HingeJoint2D>().connectedBody = ropeSegment.GetComponent<Rigidbody2D>();
        previousSegment.GetComponent<HingeJoint2D>().anchor = localRopePosition;

        currentSegment = ropeSegment;
    }

    void CreateRopeSegment(GameObject previousSegment, int segmentNum)
    {
        GameObject ropeSegment = new GameObject("segment" + segmentNum);
        ropeSegment.transform.localScale = new Vector3(0.02f, 0.02f, 1.0f);
        ropeSegment.AddComponent<SpriteRenderer>();
        ropeSegment.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/rope_segment");
        ropeSegment.GetComponent<SpriteRenderer>().sortingOrder = 1;
        ropeSegment.AddComponent<Rigidbody2D>();

        Vector3 extents = ropeSegment.GetComponent<SpriteRenderer>().bounds.extents;

        Vector3 adjustedRopePosition = previousSegment.transform.position;
        adjustedRopePosition.x += (extents[0] * 2);
        ropeSegment.transform.position = adjustedRopePosition;

        // The positions are local but the localPosition is scale invarient
        Vector3 anchorPosition = previousSegment.transform.localPosition;
        anchorPosition.x += (extents[0] * 50);
        Vector3 connectedAnchorPosition = previousSegment.transform.localPosition;
        connectedAnchorPosition.x -= (extents[0] * 50);

        previousSegment.AddComponent<HingeJoint2D>();
        previousSegment.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
        previousSegment.GetComponent<HingeJoint2D>().connectedBody = ropeSegment.GetComponent<Rigidbody2D>();
        previousSegment.GetComponent<HingeJoint2D>().anchor = anchorPosition;
        previousSegment.GetComponent<HingeJoint2D>().connectedAnchor = connectedAnchorPosition;

        currentSegment = ropeSegment;
    }
}
