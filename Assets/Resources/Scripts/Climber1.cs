using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber1 : MonoBehaviour
{
    private Movement playerMovement;
    private float?[] movementDirection = new float?[2];
    public int speed;
    private int runSpeed;
    public bool canMove;
    // Start is called before the first frame update
    void Start()
    {
      playerMovement = GetComponent<Movement>();
  		canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && canMove) {
        movementDirection[1] = 1f;
      }
      else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && canMove) {
        movementDirection[1] = -1f;
      }
      else {
        movementDirection[1] = 0f;
      }
      if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && canMove) {
        movementDirection[0] = 1f;
      }
      else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && canMove) {
        movementDirection[0] = -1f;
      }
      else {
        movementDirection[0] = 0f;
      }
    }
}
