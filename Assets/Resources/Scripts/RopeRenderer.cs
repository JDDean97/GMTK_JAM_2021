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
    public float scaleFactor = 0.03f;
    public float mass = 2;
    public float totalRopeLength;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();

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
        ropeSegments.Add(new RopeSegment(currentSegment));
        for (int i = 2; i <= numberOfSegments; i++)
        {
          AttachRopeSegment(currentSegment, i);
          ropeSegments.Add(new RopeSegment(currentSegment));
        }
        AttachFinalRopeSegment(currentSegment);
        ropeSegments.Add(new RopeSegment(currentSegment));

        totalRopeLength = ropeSegments[0].ropeLength * numberOfSegments;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 gravity = new Vector3(0.0f, -0.5f, 0.0f);

        for(int i = 0; i < numberOfSegments; i++)
        {
            RopeSegment currentSegment = ropeSegments[i];
            Vector3 velocity = currentSegment.nowPos - currentSegment.oldPos;
            currentSegment.oldPos = currentSegment.nowPos;
            currentSegment.nowPos += velocity;
            currentSegment.nowPos += gravity * Time.fixedDeltaTime;
            ropeSegments[i] = currentSegment;
        }

        for(int i = 0; i < numberOfSegments; i++)
        {
            this.Constrain();
        }
    }

    void Update()
    {
        for (int i = 0; i < numberOfSegments; i++)
        {
            RopeSegment ropeSegment = ropeSegments[i];
            ropeSegment.ropeObject.transform.position = ropeSegments[i].nowPos;
        }
    }

    void Constrain()
    {

        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.nowPos = ropeStart.transform.position;
        this.ropeSegments[0] = firstSegment;

        RopeSegment lastSegment = this.ropeSegments[numberOfSegments - 1];
        lastSegment.nowPos = ropeEnd.transform.position;
        this.ropeSegments[numberOfSegments - 1] = lastSegment;

        for(int i = 0; i < numberOfSegments - 1; i++)
        {
            firstSegment = ropeSegments[i];
            RopeSegment secondSegment = ropeSegments[i + 1];

            float dist = (firstSegment.nowPos - secondSegment.nowPos).magnitude;
            float error = Mathf.Abs(dist - firstSegment.ropeLength);

            Vector3 changeDir = Vector3.zero;

            if (dist > firstSegment.ropeLength)
            {
                changeDir = (firstSegment.nowPos - secondSegment.nowPos).normalized;
            }
            else if (dist < firstSegment.ropeLength)
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

    void AttachInitalRopeSegment(GameObject previousSegment)
    {
        GameObject ropeSegment = GenerateRopeSegment(1);
        ropeSegment.GetComponent<SpriteRenderer>().sortingOrder = 1;

        Vector3 extents = ropeSegment.GetComponent<SpriteRenderer>().bounds.extents;

        Vector3 adjustedRopePosition = ropePosition;
        adjustedRopePosition.x += extents[0];
        ropeSegment.transform.position = adjustedRopePosition;

        // Vector3 connectedAnchorPosition = previousSegment.transform.localPosition;
        // connectedAnchorPosition.x += (extents[0] / scaleFactor);
        //
        // previousSegment.AddComponent<HingeJoint2D>();
        // previousSegment.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
        // previousSegment.GetComponent<HingeJoint2D>().connectedBody = ropeSegment.GetComponent<Rigidbody2D>();
        // previousSegment.GetComponent<HingeJoint2D>().anchor = localRopePosition;
        // previousSegment.GetComponent<HingeJoint2D>().connectedAnchor = localRopePosition;

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
        Vector3 finalRopePosition = ropeEnd.transform.localPosition;

        Vector3 extents = previousSegment.GetComponent<SpriteRenderer>().bounds.extents;
        previousSegment.GetComponent<SpriteRenderer>().sortingOrder = 1;
        previousSegment.GetComponent<Rigidbody2D>().mass = 0;

        float unscaledExtent = extents[0] / scaleFactor;
        Vector3 anchorPosition = new Vector3((float)(unscaledExtent - 0.5f), 0, 0);

        // previousSegment.AddComponent<HingeJoint2D>();
        // previousSegment.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
        // previousSegment.GetComponent<HingeJoint2D>().connectedBody = climber2.GetComponent<Rigidbody2D>();
        // previousSegment.GetComponent<HingeJoint2D>().anchor = anchorPosition;
        // previousSegment.GetComponent<HingeJoint2D>().connectedAnchor = finalRopePosition;
    }

    GameObject GenerateRopeSegment(int segmentNum)
    {
      GameObject ropeSegment = new GameObject("segment" + segmentNum);
      ropeSegment.layer = 8;
      ropeSegment.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1.0f);

      ropeSegment.AddComponent<SpriteRenderer>();
      ropeSegment.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/rope_segment");
      ropeSegment.GetComponent<SpriteRenderer>().sortingOrder = 2;

      ropeSegment.AddComponent<Rigidbody2D>();
      ropeSegment.GetComponent<Rigidbody2D>().mass = mass;

      // ropeSegment.AddComponent<CapsuleCollider2D>();
      // ropeSegment.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;

      return ropeSegment;
    }

    public struct RopeSegment
    {
        public GameObject ropeObject;
        public Vector3 nowPos;
        public Vector3 oldPos;
        public float ropeLength;

        public RopeSegment(GameObject ropeObject)
        {
            this.ropeObject = ropeObject;

            Vector3 pos = ropeObject.transform.position;
            this.oldPos = pos;
            this.nowPos = pos;

            Vector3 extents = ropeObject.GetComponent<SpriteRenderer>().bounds.extents;
            this.ropeLength = extents[0]*2;
        }
    }
}
