using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player2Control : MonoBehaviour {
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
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && canMove) {
			movementDirection[1] = 1f;
		}
		else if (Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.I) && canMove) {
			movementDirection[1] = -1f;
		}
		else {
			movementDirection[1] = 0f;
		}
		if (Input.GetKey(KeyCode.L) && !Input.GetKey(KeyCode.J) && canMove) {
			movementDirection[0] = 1f;
		}
		else if (Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.L) && canMove) {
			movementDirection[0] = -1f;
		}
		else {
			movementDirection[0] = 0f;
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
