using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	public bool tethered;
	public Transform buddy;
    Vector3 tether;
	float length = 20;
	float _length = 20;
	public int playerNum = 1;
	Rigidbody2D rb;
	private Movement playerMovement;
	private float?[] movementDirection = new float?[2];
    private Transform playerTransform;
	public int speed;
	private int runSpeed;
	public bool canMove;
	public bool climbing;
	Dictionary<string, KeyCode> keys;
	Animator anim;
	const float Stamina = 5f;
	float _stamina = Stamina;
	List<GameObject> tetherPoints;
	int startNum = 0;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		playerMovement = GetComponent<Movement>();
		playerTransform = GetComponent<Transform>();
		tetherPoints = new List<GameObject>() { buddy.gameObject };
		tethered = true;
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
		wrapping();
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log("Climb:" + climbing);
		if (!climbing)
		{ _stamina = Mathf.Clamp(_stamina + 0.2f * Time.deltaTime, -2f, Stamina); }
		//Debug.Log("Player "+playerNum + " Stamina: "+_stamina);
		length = FindObjectOfType<Director>().ropeLength;
		_length = length;
		movementDirection[1] = 0f;
		movementDirection[0] = 0f;
		if(Input.GetKeyDown(KeyCode.U))
        {
			tethered = !tethered;
			if(tethered)
            {
				FindObjectOfType<Director>().ropeLength = Vector2.Distance(transform.position, buddy.position) * 1.2f;
				Debug.Log(FindObjectOfType<Director>().ropeLength);
				FindObjectOfType<RopeRenderer>().gameObject.SetActive(true);
			}
            else
            {
				FindObjectOfType<RopeRenderer>().gameObject.SetActive(false);
            }
        }
		if (canMove)
		{
			wrapping();
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
		tether = tetherPoints[startNum].transform.position;
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
	{  //DO NOT FUCK WITH THIS FUNCTION ANYMORE HOLY SHIT LEAVE IT AS IS
		Vector3 testPos = transform.position + (Vector3)rb.velocity * Time.deltaTime;
		if ((testPos - tether).magnitude > _length)
		{
			testPos = tether + (testPos - tether).normalized * _length;
			Vector3 vel;
			vel = (Vector2)(testPos - transform.position)/Time.deltaTime;
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

	void wrapping()
    {
		tetherPoints = FindObjectOfType<Director>().masterTetherPoints;
		if(playerNum==2)
        {
			List<GameObject> temp = new List<GameObject>();
			temp.Add(buddy.gameObject);
			for (int iter = 0; iter < tetherPoints.Count; iter++)
			{
				temp.Add(tetherPoints[iter]);
			}
			tetherPoints = temp;
		}
        else
        {
			tetherPoints.Add(buddy.gameObject);
        }
		if(tetherPoints.Contains(this.gameObject))
        {
			tetherPoints.Remove(this.gameObject);
        }

		int listDir = playerNum == 2 ? -1 : 1;

		if (playerNum == 2 && tetherPoints.Count>1)
			startNum = tetherPoints.Count - 1;

		RaycastHit2D hit = Physics2D.Raycast(transform.position, tetherPoints[startNum].transform.position - transform.position);
		if (hit.transform != tetherPoints[startNum].transform && !hit.transform.name.Contains("tetherPoint"))
		{
			bool createPoint = true;
			if (Vector2.Distance(tetherPoints[startNum].transform.position, hit.point) < 0.2f)
            {
				createPoint = false;
            }
			if (createPoint)
			{
				GameObject temp = new GameObject("tetherPoint");
				temp.layer = 10;
				BoxCollider2D bc = temp.AddComponent<BoxCollider2D>();
				//bc.size = new Vector2(0.1f, 0.1f);
				//bc.isTrigger = true;
				temp.transform.position = hit.point;
				if (playerNum == 2)
				{
					tetherPoints.Add(temp);
				}
				else
				{
					List<GameObject> tp = new List<GameObject>();
					tp.Add(temp);
					for (int iter = 0; iter < tetherPoints.Count; iter++)
					{
						tp.Add(tetherPoints[iter]);
					}
					tetherPoints = tp;
				}
			}
		}

		else
		{
			if (tetherPoints.Count > 1)
			{
				Vector2 dir = tetherPoints[startNum + listDir].transform.position - transform.position;
				hit = Physics2D.Raycast(transform.position, dir);
				if (hit.transform == tetherPoints[startNum + listDir].transform)
				{
					Destroy(tetherPoints[startNum]);
					List<GameObject> l = new List<GameObject>();
					foreach (GameObject g in tetherPoints)
					{
						if(g != tetherPoints[startNum]) //destroy function and list sort are delayed. for guarante lack of nulls rebuild new list and assign it to tetherPoints
						l.Add(g);
					}					
					tetherPoints = l;
				}
			}
		}

		float reduction = 0; // amount to reduce rope length;
		if (tetherPoints.Count > 1)
		{
			for (int iter = 0; iter < tetherPoints.Count - 1; iter++)
			{
				float dist = Vector2.Distance(tetherPoints[iter].transform.position, tetherPoints[iter + 1].transform.position);
				reduction += dist;
			}
		}
		_length = length - reduction;
		Color col = playerNum > 1 ? Color.blue : Color.red;
		Debug.DrawRay(transform.position, tetherPoints[startNum].transform.position - transform.position, col);
		
		FindObjectOfType<Director>().masterTetherPoints = tetherPoints;
		if (playerNum == 2)
		{
			FindObjectOfType<Director>().masterTetherPoints = tetherPoints;
			Debug.Log("updated master");
		}
		


		//clean up nulls in list
		if (tetherPoints.Contains(null))
		{
			List<GameObject> tempTP = new List<GameObject>();
			foreach (GameObject t in tetherPoints)
			{
				if (t != null)
				{
					tempTP.Add(t);
				}
			}
		}


		//if (playerNum == 2)
		//{
		//	for (int iter = Mathf.Clamp(tetherPoints.Count -1,0,100); iter > 0; iter--)
		//	{
		//		Debug.DrawRay(tetherPoints[iter].transform.position, tetherPoints[iter + 1].transform.position - tetherPoints[iter].transform.position, Color.blue);
		//	}
		//}
		//else
		//{
		//	for (int iter = 1; iter < tetherPoints.Count - 1; iter++)
		//	{
		//		Debug.DrawRay(tetherPoints[iter].transform.position, tetherPoints[iter - 1].transform.position - tetherPoints[iter].transform.position, Color.red);
		//	}
		//}
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
