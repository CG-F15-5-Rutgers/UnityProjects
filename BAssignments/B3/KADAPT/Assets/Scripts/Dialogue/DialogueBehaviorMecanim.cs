using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class DialogueBehaviorMecanim : MonoBehaviour
{
    [HideInInspector]
    public DialogueCharacterMecanim Character = null;

    void Awake() { this.Initialize(); }

    protected void Initialize()
    {
        this.Character = this.GetComponent<DialogueCharacterMecanim>();
    }

    protected void StartTree(
        Node root,
        BehaviorObject.StatusChangedEventHandler statusChanged = null)
    {
    }

	public Node Speak(Speaker speaker, string words, float time) {
		return new LeafInvoke(() => this.Character.speak(speaker, words, time));
	}

	public Node Speak(Speaker speaker, Func<string> willBeWords, float time) {
		return new LeafInvoke (() => this.Character.speak (speaker, willBeWords (), time));
	}
	
}
