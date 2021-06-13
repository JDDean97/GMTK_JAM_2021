using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeRenderer : MonoBehaviour
{

    private GameObject climber1;
    private GameObject climber2;
    private GameObject ropeStart;
    private GameObject ropeEnd;
    public int numberOfSegments = 20;
    private int totalRopeLength;
    private float ropeSegLen = 1.0f;
    private float lineWidth = 0.5f;
    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();

    // Start is called before the first frame update
    void Start()
    {
        climber1 = GameObject.Find("spr_climber1");
        climber2 = GameObject.Find("spr_climber2");

        ropeStart = climber1.transform.Find("rope_attachment").gameObject;
        ropeEnd = climber2.transform.Find("rope_attachment").gameObject;

        totalRopeLength = (int)FindObjectOfType<Director>().ropeLength;

        this.lineRenderer = this.GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = ropeStart.transform.position;

        for (int i = 0; i < totalRopeLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        totalRopeLength = (int)FindObjectOfType<Director>().ropeLength;

        if (numberOfSegments > totalRopeLength) {
          ropeSegments.RemoveAt(numberOfSegments - 1);
          numberOfSegments -= 1;
        }
        else if(numberOfSegments < totalRopeLength) {
          ropeSegments.Add(new RopeSegment(ropeEnd.transform.position));
          numberOfSegments += 1;
        }

        Vector3 gravity = new Vector3(0.0f, -0.5f, 0.0f);

        for(int i = 0; i < numberOfSegments; i++)
        {
            RopeSegment firstSegment = ropeSegments[i];
            Vector3 velocity = firstSegment.nowPos - firstSegment.oldPos;
            firstSegment.oldPos = firstSegment.nowPos;
            firstSegment.nowPos += velocity;
            firstSegment.nowPos += gravity * Time.fixedDeltaTime;
            ropeSegments[i] = firstSegment;
        }

        for(int i = 0; i < numberOfSegments; i++)
        {
            this.Constrain();
        }

    }

    void Update()
    {
      float lineWidth = this.lineWidth;
      lineRenderer.startWidth = lineWidth;
      lineRenderer.endWidth = lineWidth;

      Vector3[] ropePositions = new Vector3[totalRopeLength];
      for (int i = 0; i < totalRopeLength; i++)
      {
          ropePositions[i] = this.ropeSegments[i].nowPos;
      }

      lineRenderer.positionCount = ropePositions.Length;
      lineRenderer.SetPositions(ropePositions);
    }

    void Constrain()
    {

        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.nowPos = ropeStart.transform.position;
        // Vector3 extents = firstSegment.ropeObject.GetComponent<SpriteRenderer>().bounds.extents;
        // firstSegment.nowPos.x += extents.x;
        // firstSegment.nowPos.y -= extents.y;
        this.ropeSegments[0] = firstSegment;

        RopeSegment lastSegment = this.ropeSegments[numberOfSegments - 1];
        lastSegment.nowPos = ropeEnd.transform.position;
        this.ropeSegments[numberOfSegments - 1] = lastSegment;

        for(int i = 0; i < numberOfSegments - 1; i++)
        {
            firstSegment = ropeSegments[i];
            RopeSegment secondSegment = ropeSegments[i + 1];

            float dist = (firstSegment.nowPos - secondSegment.nowPos).magnitude;
            float error = Mathf.Abs(dist - ropeSegLen);

            Vector3 changeDir = Vector3.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSegment.nowPos - secondSegment.nowPos).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSegment.nowPos - firstSegment.nowPos).normalized;
            }

            Vector3 updatedPosVec = changeDir * error;
            if (i != 0 || i != 1)
            {
                firstSegment.nowPos -= updatedPosVec * 0.5f;
                this.ropeSegments[i] = firstSegment;
            }
            secondSegment.nowPos += updatedPosVec * 0.5f;
            this.ropeSegments[i + 1] = secondSegment;
        }
    }
    //
    // void AttachInitalRopeSegment(GameObject previousSegment)
    // {
    //     GameObject ropeSegment = GenerateRopeSegment(1);
    //     ropeSegment.GetComponent<SpriteRenderer>().sortingOrder = 1;
    //
    //     Vector3 extents = ropeSegment.GetComponent<SpriteRenderer>().bounds.extents;
    //
    //     Vector3 adjustedRopePosition = ropePosition;
    //     adjustedRopePosition.x += extents[0];
    //     ropeSegment.transform.position = adjustedRopePosition;
    //
    //     // Vector3 connectedAnchorPosition = previousSegment.transform.localPosition;
    //     // connectedAnchorPosition.x += (extents[0] / scaleFactor);
    //     //
    //     // previousSegment.AddComponent<HingeJoint2D>();
    //     // previousSegment.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
    //     // previousSegment.GetComponent<HingeJoint2D>().connectedBody = ropeSegment.GetComponent<Rigidbody2D>();
    //     // previousSegment.GetComponent<HingeJoint2D>().anchor = localRopePosition;
    //     // previousSegment.GetComponent<HingeJoint2D>().connectedAnchor = localRopePosition;
    //
    //     currentSegment = ropeSegment;
    // }
    //
    // void AttachRopeSegment(GameObject previousSegment, int segmentNum)
    // {
    //     GameObject ropeSegment = GenerateRopeSegment(segmentNum);
    //
    //     Vector3 extents = ropeSegment.GetComponent<SpriteRenderer>().bounds.extents;
    //
    //     Vector3 adjustedRopePosition = previousSegment.transform.position;
    //     adjustedRopePosition.x += (extents[0] * 2);
    //     ropeSegment.transform.position = adjustedRopePosition;
    //
    //     float unscaledExtent = extents[0] / scaleFactor;
    //     Vector3 anchorPosition = new Vector3((float)(unscaledExtent - 0.5f), 0, 0);
    //     Vector3 connectedAnchorPosition = new Vector3((float)(-(unscaledExtent - 0.5f)), 0, 0);
    //
    //     previousSegment.AddComponent<HingeJoint2D>();
    //     previousSegment.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
    //     previousSegment.GetComponent<HingeJoint2D>().connectedBody = ropeSegment.GetComponent<Rigidbody2D>();
    //     previousSegment.GetComponent<HingeJoint2D>().anchor = anchorPosition;
    //     previousSegment.GetComponent<HingeJoint2D>().connectedAnchor = connectedAnchorPosition;
    //
    //     currentSegment = ropeSegment;
    //     ropeSegments.Add(new RopeSegment(currentSegment));
    // }
    //
    // void AttachFinalRopeSegment(GameObject previousSegment)
    // {
    //     Vector3 finalRopePosition = ropeEnd.transform.localPosition;
    //
    //     Vector3 extents = previousSegment.GetComponent<SpriteRenderer>().bounds.extents;
    //     previousSegment.GetComponent<SpriteRenderer>().sortingOrder = 1;
    //     previousSegment.GetComponent<Rigidbody2D>().mass = 0;
    //
    //     float unscaledExtent = extents[0] / scaleFactor;
    //     Vector3 anchorPosition = new Vector3((float)(unscaledExtent - 0.5f), 0, 0);
    //
    //     // previousSegment.AddComponent<HingeJoint2D>();
    //     // previousSegment.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
    //     // previousSegment.GetComponent<HingeJoint2D>().connectedBody = climber2.GetComponent<Rigidbody2D>();
    //     // previousSegment.GetComponent<HingeJoint2D>().anchor = anchorPosition;
    //     // previousSegment.GetComponent<HingeJoint2D>().connectedAnchor = finalRopePosition;
    // }
    //
    // void InsertRopeSegment()
    // {
    //   AttachRopeSegment()
    // }
    //
    // void RemoveRopeSegment()
    // {
    //     GameObject ropeSegment = ropeSegments[numberOfSegments - 2].ropeObject;
    //     GameObject lastSegment = ropeSegments[numberOfSegments - 1].ropeObject;
    //     ropeSegments.RemoveAt(numberOfSegments - 1);
    //     Destroy(lastSegment);
    //     numberOfSegments -= 1;
    //
    //     ropeSegments[numberOfSegments - 1] = new RopeSegment(ropeSegment);
    //     AttachFinalRopeSegment(ropeSegment);
    // }
    //
    // GameObject GenerateRopeSegment(int segmentNum)
    // {
    //   GameObject ropeSegment = new GameObject("segment" + segmentNum);
    //   ropeSegment.layer = 8;
    //   ropeSegment.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1.0f);
    //
    //   ropeSegment.AddComponent<SpriteRenderer>();
    //   ropeSegment.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/rope_segment");
    //   ropeSegment.GetComponent<SpriteRenderer>().sortingOrder = 2;
    //
    //   ropeSegment.AddComponent<Rigidbody2D>();
    //   ropeSegment.GetComponent<Rigidbody2D>().mass = mass;
    //
    //   // ropeSegment.AddComponent<CapsuleCollider2D>();
    //   // ropeSegment.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
    //
    //   return ropeSegment;
    // }

    public struct RopeSegment
    {
        public Vector3 nowPos;
        public Vector3 oldPos;

        public RopeSegment(Vector3 pos)
        {
            this.oldPos = pos;
            this.nowPos = pos;
        }
    }
}
