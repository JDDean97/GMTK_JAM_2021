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
						// transform.Find("Emitter").GetComponent<ParticleSystem>().Emit(4);
					}
				}
                else
                {
                    movementDirection[1] = 1f;
                    //rb.AddForce(Vector2.up * 3);
				}
			}
			else if (Input.GetKey(keys["down"]) && !Input.GetKey(keys["up"]))
			{
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
	}

    public bool isGrounded()
    {	
		int hits = 0;
		for(int iter = 0;iter<3;iter++)
        {
			RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3.left * 0.4f) + (Vector3.right * 0.4f * iter), -Vector2.up, GetComponent<Collider2D>().bounds.extents.y * 1.4f);
			if (hit.transform != null)
			{
				Debug.DrawRay(transform.position + (Vector3.left * 0.4f) + (Vector3.right * 0.4f * iter), -Vector3.up * (GetComponent<Collider2D>().bounds.extents.y * 1.4f),Color.red);
				hits++;
			}
			else
			{
				Debug.DrawRay(transform.position + (Vector3.left * 0.4f) + (Vector3.right * 0.4f * iter), -Vector3.up * (GetComponent<Collider2D>().bounds.extents.y * 1.4f), Color.blue);
			}
		}		
		Debug.DrawRay(transform.position + (Vector3.left * 0.4f), -Vector3.up * (GetComponent<Collider2D>().bounds.extents.y * 1.4f));
		if(hits>=2)
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
			Vector3 vel = rb.velocity;
			return vel;
			//return Vector3.zero;
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
		Vector2 realPos = (Vector2)transform.position + GetComponent<Collider2D>().offset;
		if (!collision.transform.CompareTag("Player"))
		{
			if (Input.GetKey(keys["left"]) || Input.GetKey(keys["right"]))
			{
				Vector2 contactDir = collision.GetContact(0).point - realPos;
				contactDir = contactDir.normalized;
				Vector2 dir = rb.velocity;
				if(Input.GetKey(keys["left"]))
                {
					dir = Vector2.left;
                }
				else if(Input.GetKey(keys["right"]))
                {
					dir = Vector2.right;
                }
				Debug.DrawRay(transform.position, contactDir * 10, Color.cyan);
				float angle = Mathf.Abs(Vector2.SignedAngle(contactDir, dir));
				if (angle < 95)
				{
					Debug.Log("player" + playerNum + "  climbing");
					climbing = true;
				}
				else
				{
					Debug.Log("player" + playerNum + "  not climbing");
					climbing = false;
				}
			}
		}
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
		climbing = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if(collision.CompareTag("Killbox"))
        {
			FindObjectOfType<SceneChanger>().load(1);
        }
		if (collision.CompareTag("Coin"))
        {
			FindObjectOfType<Director>().coinCollect(collision.transform.position);
			Destroy(collision.gameObject);
		}
		else if(collision.CompareTag("Flag"))
        {
			FindObjectOfType<Director>().flagCollect(collision.transform.position);
			Destroy(collision.gameObject);
		}
	}
}
