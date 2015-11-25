using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource source;
	public AudioClip polka;
	public AudioClip metal;
	public AudioClip electronic;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeSongById (int songId) {
		switch (songId) {
		case 1:
			source.clip = polka;
			source.loop = true;
			source.Play();
			break;
		case 2:
			source.clip = electronic;
			source.loop = true;
			source.Play();
			break;
		case 3:
			source.clip = metal;
			source.loop = true;
			source.Play();
			break;
		default:
			Debug.LogWarning ("SoundManager: Unrecognized songId: " + songId);
			break;
		}

	}
}
