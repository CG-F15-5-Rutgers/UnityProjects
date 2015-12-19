using UnityEngine;
using System.Collections;

public enum Char {
	Robert,
	Bobby,
	Gary,
	NoActive
}

public class CharSelect : MonoBehaviour {

	[HideInInspector]
	public Char active;

	public GameObject marker;

	public Vector3 marker_robert;
	public Vector3 marker_gary;
	public Vector3 marker_bobby;

	private Vector3 marker_noactive; //start pos, off screen

	// Use this for initialization
	void Start () {
		active = Char.NoActive;
		marker_noactive = marker.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RobertClicked() {
		if (active != Char.Robert) {
			active = Char.Robert;
			marker.transform.localPosition = marker_robert;
		} else {
			active = Char.NoActive;
			marker.transform.position = marker_noactive;
		}	}

	public void BobbyClicked() {
		if (active != Char.Bobby) {
			active = Char.Bobby;
			marker.transform.localPosition = marker_bobby;
		} else {
			active = Char.NoActive;
			marker.transform.position = marker_noactive;
		}	}

	public void GaryClicked() {
		if (active != Char.Gary) {
			active = Char.Gary;
			marker.transform.localPosition = marker_gary;
		} else {
			active = Char.NoActive;
			marker.transform.position = marker_noactive;
		}
	}

}
