using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class CrowdBehaviorMecanim : MonoBehaviour
{
    [HideInInspector]
    public CrowdCharacterMecanim Character = null;

    void Awake() { this.Initialize(); }

    protected void Initialize()
    {
        this.Character = this.GetComponent<CrowdCharacterMecanim>();
    }

    protected void StartTree(
        Node root,
        BehaviorObject.StatusChangedEventHandler statusChanged = null)
    {
    }

	public Node Cheer() {
		return new LeafInvoke(() => this.Character.cheer());
	}
	
}
