using UnityEngine;
using System.Collections;

	
	public class ClickToMove : MonoBehaviour {
		
		private NavMeshAgent navMeshAgent;

		private Transform targetedObstacle;

		private bool jump;
		private bool selected;
		private bool obstacleClicked;
		
	    private AudioSource yay;
		// Use this for initialization
		void Awake () 
		{
			selected = false;
			navMeshAgent = GetComponent<NavMeshAgent> ();
			yay = GetComponent<AudioSource> ();
			jump = false;
		}
		
		// Update is called once per frame
		void Update () 
		{
			jump = false;
			if (selected == true) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Input.GetButtonDown ("Fire2")) 
				{
					if (Physics.Raycast(ray, out hit, 100))
					{
						obstacleClicked = false;
						navMeshAgent.speed = 3.5f;
						navMeshAgent.destination = hit.point;
						navMeshAgent.Resume();
					}
				}
				
				if (navMeshAgent.isOnOffMeshLink) {
					Vector3 x = navMeshAgent.currentOffMeshLinkData.endPos;
					float step = Time.deltaTime;
					Vector3 newDir = Vector3.RotateTowards(transform.forward, x, step, 0.0F);
					Debug.DrawRay(transform.position, newDir, Color.red);
					transform.rotation = Quaternion.LookRotation(newDir);

					jump = true;

				}
				
			}
		}
		

		public void select()
		{
			selected = true;
		}

		public void unselect()
		{
			selected = false;
		}
	}
	