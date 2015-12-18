using UnityEngine;
using TreeSharpPlus;
using System;

public class CrowdCharacterMecanim : MonoBehaviour
{
    public const float MAX_BOUNCE_HEIGHT = 1.1f;  //Temp
    public const float MAX_BOUNCE_ANGLE = 20.0f;  //20 degrees I hope

  
    [HideInInspector]
    public CrowdBodyMecanim Body = null;

    void Awake() { this.Initialize(); }

    /// <summary>
    /// Searches for and binds a reference to the Body interface
    /// </summary>
    public void Initialize()
    {
        this.Body = this.GetComponent<CrowdBodyMecanim>();
    }

	public RunStatus cheer() {

		if(!this.Body.isCheering())
			this.Body.cheer (true);
		
		if (this.Body.finishedCheering ()) {
			this.Body.cheer (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

}
