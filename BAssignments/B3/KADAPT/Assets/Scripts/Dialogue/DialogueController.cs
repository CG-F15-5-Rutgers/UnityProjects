using UnityEngine;
using System.Collections;

public enum Speaker {
	Robert,
	Gary,
	Bobby,
	Host
}

public class DialogueController : MonoBehaviour {

	private float timeLeft;
	private Vector3 startPos;
	private Vector3 hiddenPos;

	public TextController text;
	public GameObject dialogueBG;
	public GameObject faces;
	//TODO add speaker's image

	void Start () {
		startPos = this.transform.position;
		hiddenPos = startPos + (Vector3.right * 2000.0f);
		this.transform.position = hiddenPos; //Start off with dialogue hidden
	}
	
	// Update is called once per frame
	void Update () {

		timeLeft -= Time.deltaTime;
		//This is set to occur at -0.1f instead of just 0 so that
		//if there are two consecutive dialogues it won't flicker away
		//for a frame or two
		if (timeLeft < -0.1f) {
			this.transform.position = hiddenPos;
		}
	
	}

	public void Speak (Speaker speaker, string words, float time) {
		Debug.Log ("Starting to speak");
		timeLeft = time;
		this.transform.position = startPos;
		text.setText (words);

		//set speakers image
		foreach (Transform child in this.faces.transform) {
			child.gameObject.SetActive(false);
			switch (speaker) {
			case Speaker.Bobby:
				if(child.name == "Bobby") {
					child.gameObject.SetActive(true);
				}
				break;
			case Speaker.Gary:
				if(child.name == "Gary") {
					child.gameObject.SetActive(true);
				}
				break;
			case Speaker.Robert:
				if(child.name == "Robert") {
					child.gameObject.SetActive(true);
				}
				break;
			case Speaker.Host:
				if(child.name == "Host") {
					child.gameObject.SetActive(true);
				}
				break;
			}
		}
	} 

	public bool finishedSpeaking () {
		return (timeLeft < 0);
	}

}
