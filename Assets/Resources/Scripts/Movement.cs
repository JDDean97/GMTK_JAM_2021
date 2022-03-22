using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Handles the physics of player movement
public class Movement : MonoBehaviour {

	Rigidbody2D rb;
	private Vector2 objectVelocity;
	private PlayerControl playerControl; 

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		playerControl = GetComponent<PlayerControl>();
	}

	public void Move(float?[] direction, int speed,bool climb) {
		//const int jumpForce = 7;
		const int jumpForce = 12;
		Vector2 vector = new Vector2((float)direction[0], (float)direction[1]);
		objectVelocity = speed * (vector.normalized);
		float vert = rb.velocity.y;
		if(direction[1]>0 && !climb)
        {
			vert = jumpForce;
        }
		if(climb)
        {
			if(direction[1]!=1)
            {
				vert = 0;
            }
            else
            {
				vert = (float)direction[1] * jumpForce;
			}			
        }
		
		objectVelocity.y = vert;
		Vector2 constrainedVelocity = rb.GetComponent<PlayerControl>().swing();
		//Debug.Log(objectVelocity);
		if (!playerControl.tethered) //dont bother with rest of script if player is not tethered
        {
			rb.velocity = objectVelocity;
			return;
		}
			       
		if(!playerControl.isGrounded() &&!climb)
        {
			rb.velocity = constrainedVelocity;
		}
        else
        {
			rb.velocity = objectVelocity;
		}
	}
}
