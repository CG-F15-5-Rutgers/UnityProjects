using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	//public GameObject player;

	private Vector3 offset;
	private Vector3 movement;

	// Use this for initialization
	void Start () {
		offset = transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () { //guarenteed to run after update runs
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical"); // keyboard strokes for forward, back, etc.
		movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		offset = offset + movement;

		transform.position = offset;
	}
}
