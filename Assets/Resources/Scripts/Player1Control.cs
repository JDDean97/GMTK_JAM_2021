using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player1Control : MonoBehaviour {
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

	// Use this for initialization
	void Start () {
		playerMovement = GetComponent<Movement>();
		canMove = true;
		speed = 5;
	}

	// Update is called once per frame
	void Update () {
		movementDirection[1] = 0f;
		movementDirection[0] = 0f;
		if (canMove)
		{
			if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
			{
				if (isGrounded())
				{
					movementDirection[1] = 1f;
				}
			}
			else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
			{
				movementDirection[1] = -1f;
			}
			if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
			{
				movementDirection[0] = 1f;
			}
			else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
			{
				movementDirection[0] = -1f;
			}
		}
		// while (Input.GetKey(KeyCode.Space)) {
		// 	float?[] jumpBack = new float?[1];
		// 	jumpBack[0] = Mathf.PI;
		// 	playerMovement.Move(jumpBack, 20);
		// 	canMove = false;
		// }
		if (Input.GetKey(KeyCode.LeftShift)) {
			runSpeed = speed + speed/2;
			playerMovement.Move(movementDirection, runSpeed);
		}
		else {
			playerMovement.Move(movementDirection, speed);
		}
	}

	bool isGrounded()
    {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up,GetComponent<Collider2D>().bounds.extents.y*1.1f);
		Debug.DrawRay(transform.position, -Vector3.up * (GetComponent<Collider2D>().bounds.extents.y * 1.1f));
		if(hit.transform!=null)
        {
			Debug.Log("grounded");
			return true; //grounded
        }
        else
        {
			Debug.Log("midair");
			return false;//midair
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

	// FixedUpdate is called once per physics tick
	void FixedUpdate () {
	}
}
