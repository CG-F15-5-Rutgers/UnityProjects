using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class SpacebarBehaviorMecanim : MonoBehaviour
{
    [HideInInspector]
	public SpacebarCharacterMecanim Character = null;

    void Awake() { this.Initialize(); }

    protected void Initialize()
    {
		this.Character = this.GetComponent<SpacebarCharacterMecanim>();
    }

    protected void StartTree(
        Node root,
        BehaviorObject.StatusChangedEventHandler statusChanged = null)
    {
    }

	public Node PressSpaceOrLose() {
		return new LeafInvoke(() => this.Character.PressSpaceOrLose());
	}
	
}
