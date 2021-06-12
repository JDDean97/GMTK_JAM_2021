using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Handles the physics of player movement
public class Movement : MonoBehaviour {

	private Rigidbody2D rb;
	private Vector2 objectVelocity;

	private double xPrevious;
	private double yPrevious;
	private float magnitude;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		name = rb.name;
	}

	// Update is called every significant frame
  public void Update () {
		xPrevious = rb.position.x;
		yPrevious = rb.position.y;
	}

	public void FixedUpdate () {
	}

	// Moves the player up by the ySpeed as a normalized physics vector
	// Needs to be edited
	// Current moves player at a stepper diagonal
	public void Move(float?[] direction, int speed,bool climb) {
		Vector2 vector = new Vector2((float)direction[0], (float)direction[1]);
		objectVelocity = speed * (vector.normalized);
		float vert = rb.velocity.y;
		if(direction[1]>0 && !climb)
        {
			vert = 7;
        }
		if(climb)
        {
			vert = (float)direction[1] * speed;
        }

		objectVelocity.y = vert;
		//Debug.Log(objectVelocity);
		if (objectVelocity.x == 0f && objectVelocity.y == 0f) {
			rb.velocity = Vector2.Scale(rb.velocity, new Vector2(.99f, .99f));
		}
		else {
			rb.velocity = objectVelocity;
		}
	}

	// Will immidiatly stop the players movement
	// Generally don't like this as a way to stop the player
	public void Stop () {
		rb.velocity = new Vector2(0, 0);
	}
}
