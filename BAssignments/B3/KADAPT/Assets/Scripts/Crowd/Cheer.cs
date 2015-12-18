using UnityEngine;
using System.Collections;

public class Cheer : MonoBehaviour {

	public int row; //1 for black, 2 for dark grey, 3 for grey

	private int jumpsLeft;
	private bool grounded;
	private float delay;
	private bool _finishedCheer;

	// Use this for initialization
	void Start () {

		jumpsLeft = 0;
		grounded = false;
		_finishedCheer = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (jumpsLeft == 0)
			return;
		if (delay > 0) {
			delay -= Time.deltaTime;
			return;
		}
		if (grounded) {
			//Jump
			//TODO different forces for different rows
			this.GetComponent<Rigidbody>().AddForce(Vector3.up * Random.Range(200,270));
			grounded = false;
			jumpsLeft--;
		}
	}

	void OnCollisionEnter () {
		grounded = true;
		if (jumpsLeft == 0)
			_finishedCheer = true;
	}

	public void StartCheer() {
		jumpsLeft = Random.Range(3,4);
		delay = ((float) Random.Range(10, 30)) / 45.0f;
		_finishedCheer = false;
	}

	public bool finishedCheer() {
		return _finishedCheer;
	}
}
