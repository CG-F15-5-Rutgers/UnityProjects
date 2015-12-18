using UnityEngine;
using TreeSharpPlus;
using System;

public class DialogueCharacterMecanim : MonoBehaviour
{
 
    [HideInInspector]
    public DialogueBodyMecanim Body = null;

    void Awake() { this.Initialize(); }

    /// <summary>
    /// Searches for and binds a reference to the Body interface
    /// </summary>
    public void Initialize()
    {
        this.Body = this.GetComponent<DialogueBodyMecanim>();
    }

	public RunStatus speak(Speaker speaker, string words, float time) {

		if(!this.Body.isSpeaking())
			this.Body.speak (true, speaker, words, time);
		
		if (this.Body.finishedSpeaking ()) {
			this.Body.speak (false);
			Debug.Log ("Just finished speaking!");
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

}
