using UnityEngine;
using System.Collections;

public class Translator : MonoBehaviour {

	// Update is called once per frame
	private bool forward = true;
	private int steps = 0;
	private int distance = 60;

	void Update () 
	{

		//transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime); // Rotater
		if (forward == true && steps < distance) 
		{
			transform.Translate(new Vector3 (0, 0, 2) * Time.deltaTime, Space.World);
			steps++;
		}
		if (forward == true && steps == distance) 
		{
			transform.Translate(new Vector3 (0, 0, 2) * Time.deltaTime, Space.World);
			steps++;
			forward = false;
		}
		if (forward == false && steps > 0) 
		{
			transform.Translate(new Vector3 (0, 0, -2) * Time.deltaTime, Space.World);
			steps--;
		}
		if (forward == false && steps == 0) 
		{
			transform.Translate(new Vector3 (0, 0, -2) * Time.deltaTime, Space.World);
			steps = steps - 1;
			forward = true;
		}		
	

	}
}
