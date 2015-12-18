using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

using RootMotion.FinalIK;

/// <summary>
/// A very basic graphical interface for what should be treated as basic motor
/// skills performed by the character1. These actions include no preconditions
/// and can fail if executed in impossible/nonsensical situations. In this
/// case, the functions will usually try their best.
/// 
/// Used with a BodyCoordinator and/or a SteeringController. Needs at least
/// one on the same GameObject to be able to do anything.
/// </summary>
public class DialogueBodyMecanim : MonoBehaviour
{

	private bool _speak;
    
    void Awake()
    {
    }

    void Update()
    {
    }

	public bool isSpeaking() {
		return _speak;
	}

	public bool finishedSpeaking() {
		return this.GetComponent<DialogueController> ().finishedSpeaking ();
	}

	public void speak(bool val, Speaker speaker=Speaker.Host, string words=null, float time=-1) {
		if (val && !isSpeaking ()) {
			startSpeak(speaker, words, time);
		}
		_speak = val;
	}

	private void startSpeak(Speaker speaker, string words, float time) {
		this.GetComponent<DialogueController> ().Speak(speaker, words, time);
	}
    

}
