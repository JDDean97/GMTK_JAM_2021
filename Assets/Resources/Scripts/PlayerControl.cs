using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	public Transform buddy;
    Vector3 tether;
	float length = 20;
	public int playerNum = 1;
	Rigidbody2D rb;
	private Movement playerMovement;
	private float?[] movementDirection = new float?[2];
    private GameObject player;
    private Transform playerTransform;
    private float power = 0.0f;
	public int arrowCount = 0;
	private bool canFire = true;
	public int speed;
	private int runSpeed;
	public bool canMove;
	public bool climbing;
	Dictionary<string, KeyCode> keys;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		playerMovement = GetComponent<Movement>();
		canMove = true;
		speed = 5;
		if(playerNum == 1)
        {
			keys = keybinds.kb1;
        }
		else if(playerNum == 2)
        {
			keys = keybinds.kb2;
		}
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log("Climb:" + climbing);
		length = FindObjectOfType<Director>().ropeLength;
		tether = buddy.position;
		movementDirection[1] = 0f;
		movementDirection[0] = 0f;
		if (canMove)
		{
			if (Input.GetKey(keys["up"]) && !Input.GetKey(keys["down"]))
			{
				if (!climbing)
				{
					if (isGrounded())
					{
						movementDirection[1] = 1f;
						transform.Find("Emitter").GetComponent<ParticleSystem>().Emit(4);
					}
				}
                else
                {
					movementDirection[1] = 1f;
				}
			}
			else if (Input.GetKey(keys["down"]) && !Input.GetKey(keys["up"]))
			{
				float ropeSlide = 3;
				//movementDirection[1] = -1f;
				if(playerNum == 1)
                {
					FindObjectOfType<Director>().lengthenRope();
                }
				else
                {
					FindObjectOfType<Director>().shortenRope();
				}
				//length = Mathf.Clamp(length, 5, 25);
				//Debug.Log("length: " + length);
				//movementDirection[1] = -1f;
			}
			if (Input.GetKey(keys["right"]) && !Input.GetKey(keys["left"]))
			{
				movementDirection[0] = 1f;
			}
			else if (Input.GetKey(keys["left"]) && !Input.GetKey(keys["right"]))
			{
				movementDirection[0] = -1f;
			}
		}
		
		
	}

    private void FixedUpdate()
    {
		if (Input.GetKey(KeyCode.LeftShift))
		{
			runSpeed = speed + speed / 2;
			playerMovement.Move(movementDirection, runSpeed, climbing);
		}
		else
		{
			playerMovement.Move(movementDirection, speed, climbing);
		}		
		swing();
	}

    public bool isGrounded()
    {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up,GetComponent<Collider2D>().bounds.extents.y*1.1f);
		Debug.DrawRay(transform.position, -Vector3.up * (GetComponent<Collider2D>().bounds.extents.y * 1.1f));
		if(hit.transform!=null)
        {
			//Debug.Log("grounded");
			return true; //grounded
        }
        else
        {
			//Debug.Log("midair");
			return false;//midair
        }
    }

	public Vector3 swing()
    {
		Vector3 testPos = transform.position + (Vector3)rb.velocity * Time.deltaTime;
		if ((testPos - tether).magnitude > length)
		{
			testPos = (testPos - tether).normalized * length + tether;
			Debug.DrawRay(transform.position, testPos, Color.green, 2);
			return (testPos - transform.position) / Time.deltaTime;
		}
		else
		{
			Debug.DrawRay(transform.position, rb.velocity, Color.red);
			return Vector3.zero;
		}
	}

	IEnumerator shootArrow() {
		player = GameObject.Find("player_character");
		playerTransform = player.GetComponent(typeof(Transform)) as Transform;
		// Debug.Log(playerTransform.position);
		GameObject shot = (GameObject) Instantiate(Resources.Load("Shot"),
																							 new Vector3(playerTransform.position[0] + .5f,
																													 playerTransform.position[1] + 1,
																													 0),
																							 Quaternion.identity);
		Rigidbody2D shotRb = shot.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
		shotRb.velocity = new Vector2(2, .1f) * power;
		power = 0.0f;
		arrowCount -= 1;
		canFire = false;
		yield return new WaitForSeconds(.5f);
		canFire = true;
	}

    private void OnCollisionStay2D(Collision2D collision)
    {
		if (!collision.transform.CompareTag("Player"))
		{
			if (Input.GetKey(keys["left"]) || Input.GetKey(keys["right"]))
			{
				Vector2 contactDir = collision.contacts[0].point - (Vector2)transform.position;
				contactDir = contactDir.normalized;
				if (Vector2.Angle(contactDir, rb.velocity) < 45)
				{
					climbing = true;
				}
				else
				{
					climbing = false;
				}
			}
		}
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
		climbing = false;
    }
}
