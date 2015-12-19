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
public class SpacebarBodyMecanim : MonoBehaviour
{

	private bool _spacebar;
	    
    void Awake()
    {
    }

    void Update()
    {
    }

	public bool isSpacebar() {
		return _spacebar;
	}

	public bool didSpacebar() {
		return this.GetComponent<SpacebarController> ().didSpacebar ();
	}

	public bool didntSpacebar() {
		return this.GetComponent<SpacebarController> ().didntSpacebar ();
	}
	
	public void spacebar(bool val) {
		if (val && !isSpacebar ())
			startSpacebar();

		_spacebar = val;
	}

	private void startSpacebar() {
		this.GetComponent<SpacebarController> ().Spacebar();
	}
    

}
