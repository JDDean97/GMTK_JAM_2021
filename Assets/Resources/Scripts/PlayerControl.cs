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
	Animator anim;
	const float Stamina = 5f;
	float _stamina = Stamina;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		playerMovement = GetComponent<Movement>();
		playerTransform = GetComponent<Transform>();
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
		if (!climbing)
		{ _stamina = Mathf.Clamp(_stamina + 0.2f * Time.deltaTime, -2f, Stamina); }
		Debug.Log("Player "+playerNum + " Stamina: "+_stamina);
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
				playerTransform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
				movementDirection[0] = 1f;
			}
			else if (Input.GetKey(keys["left"]) && !Input.GetKey(keys["right"]))
			{
				playerTransform.localScale = new Vector3(-1.0f, 1.0f, 0.0f);
				movementDirection[0] = -1f;
			}
		}
		anim.SetBool("notGrounded", !isGrounded());
		if(isGrounded()&&rb.velocity!=Vector2.zero)
        {
			anim.SetBool("walking", true);
        }
        else
        {
			anim.SetBool("walking", false);
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
	{ //TODO: FIX BOUNCINESS  has something to do with delta time i think idk    used to use vel = (rb.velocity + (Vector2)(testPos - transform.position)) / time.deltatime
		const float speedCap = 5;
		Vector3 testPos = transform.position + (Vector3)rb.velocity * Time.deltaTime;
		if ((testPos - tether).magnitude > length)
		{
			testPos = tether + (testPos - tether).normalized * length;
			Vector3 vel;
			vel = (rb.velocity + (Vector2)(testPos - transform.position));
			Debug.DrawRay(transform.position,vel, Color.green, 2);
			return vel;
		}
		else
		{
			Debug.DrawRay(transform.position, rb.velocity * 5, Color.red);
			Vector3 vel = rb.velocity;
			return vel;
			//return Vector3.zero;
		}
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
				if (angle < 70 && _stamina>0)
				{
					Debug.Log("player" + playerNum + "  climbing");
					climbing = true;
					_stamina -= 1 * Time.deltaTime;
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
