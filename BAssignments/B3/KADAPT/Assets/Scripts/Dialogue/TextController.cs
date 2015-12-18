using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextController : MonoBehaviour {

	Text textObj;
	// Use this for initialization
	void Start () {
		textObj = this.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setText (string text) {
		textObj.text = text;
	}
}
