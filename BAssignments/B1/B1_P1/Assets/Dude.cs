using UnityEngine;
using System.Collections;

public class Dude : MonoBehaviour {

	enum MovementState {Idle=0, Walking, Running};

	Transform camera;
	Animator anim;

	public float turnIncrement;
	float currTurnAngle;

	// Use this for initialization
	void Awake() {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		float vertical = Input.GetAxis ("Vertical");
		float horizontal = Input.GetAxis ("Horizontal");
		bool shift = (Input.GetAxis ("Shift") != 0);
		bool jump = (Input.GetAxis ("Jump") != 0);

		int currMovementState;
		float speedMultiplier;

		if (vertical == 0) {
			currMovementState = (int)MovementState.Idle;
		} else {
			if (shift) {
				currMovementState = (int)MovementState.Running;
			} else {
				currMovementState = (int)MovementState.Walking;
			}
			if (horizontal < 0 && currTurnAngle > -3) {
				currTurnAngle -= turnIncrement;
			} else if (horizontal > 0 && currTurnAngle < 3) {
				currTurnAngle += turnIncrement;
			}
		}
		if (horizontal == 0) {
			if (currTurnAngle < turnIncrement && currTurnAngle > -1* turnIncrement) {
				currTurnAngle = 0;
			} else if (currTurnAngle > 0) {
				currTurnAngle -= turnIncrement;
			} else {
				currTurnAngle += turnIncrement;
			}
		}

		anim.SetInteger("MovementState", (int)currMovementState);
		anim.SetFloat("TurnDirection", currTurnAngle);
		anim.SetInteger ("ShouldJump", (jump? 1 : 0));

	}
}
