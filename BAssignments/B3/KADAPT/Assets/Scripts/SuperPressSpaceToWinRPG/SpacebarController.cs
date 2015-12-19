using UnityEngine;
using System.Collections;

public class SpacebarController : MonoBehaviour {

	private float timeLeft;
	private bool _didSpacebar;
	private Vector3 startPos;
	private Vector3 hiddenPos;

	void Start () {
		startPos = this.transform.position;
		hiddenPos = startPos + (Vector3.right * 2000.0f);
		this.transform.position = hiddenPos; //Start off with spacebar warning hidden
	}
	
	// Update is called once per frame
	void Update () {

		timeLeft -= Time.deltaTime;
		if (timeLeft < 0.0f) {
			this.transform.position = hiddenPos;
			_didSpacebar = false;
			return;
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			_didSpacebar = true;
		}
	
	}

	public void Spacebar () {
		timeLeft = 1.0f;
		this.transform.position = startPos;
	} 

	public bool didSpacebar () {
		return _didSpacebar;
	}

	public bool didntSpacebar () {
		return (timeLeft < 0);
	}

}
