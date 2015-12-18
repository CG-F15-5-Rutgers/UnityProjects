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
public class CrowdBodyMecanim : MonoBehaviour
{

	private bool _cheer;
    
    void Awake()
    {
    }

    void Update()
    {
    }

	public bool isCheering() {
		return _cheer;
	}

	public bool finishedCheering() {
		return this.GetComponent<Cheer> ().finishedCheer ();
	}

	public void cheer(bool val) {
		if (val && !isCheering ()) {
			startCheer();
		}
		_cheer = val;
	}

	private void startCheer() {
		this.GetComponent<Cheer> ().StartCheer();
	}
    

}
