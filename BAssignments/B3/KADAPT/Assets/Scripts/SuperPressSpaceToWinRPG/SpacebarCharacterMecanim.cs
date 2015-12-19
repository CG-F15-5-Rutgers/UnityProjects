using UnityEngine;
using TreeSharpPlus;
using System;

public class SpacebarCharacterMecanim : MonoBehaviour
{
 
    [HideInInspector]
    public SpacebarBodyMecanim Body = null;

    void Awake() { this.Initialize(); }

    /// <summary>
    /// Searches for and binds a reference to the Body interface
    /// </summary>
    public void Initialize()
    {
		this.Body = this.GetComponent<SpacebarBodyMecanim>();
    }

	public RunStatus PressSpaceOrLose() {

		if(!this.Body.isSpacebar())
			this.Body.spacebar (true);
		
		if (this.Body.didSpacebar ()) {
			this.Body.spacebar (false);
			return RunStatus.Success;
		}

		if (this.Body.didntSpacebar ()) {
			this.Body.spacebar (false);
			return RunStatus.Failure;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

}
