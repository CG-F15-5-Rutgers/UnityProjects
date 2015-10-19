using UnityEngine;
using System.Collections;

	
	public class ClickToMove : MonoBehaviour {
		
		private Animator anim;
		private NavMeshAgent navMeshAgent;

		private Transform targetedObstacle;

		private bool jump;
		private bool selected;
		private bool obstacleClicked;
		private int movementState;
		
	    private AudioSource yay;
		// Use this for initialization
		void Awake () 
		{
			anim = GetComponent<Animator> ();
			selected = false;
			navMeshAgent = GetComponent<NavMeshAgent> ();
			yay = GetComponent<AudioSource> ();
			jump = false;
			movementState = 0;
		}
		
		// Update is called once per frame
		void Update () 
		{
			jump = false;
			if (selected == true) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Input.GetButtonDown ("Fire2") &&  (Input.GetAxis("Shift") != 0)) 
				{
					if (Physics.Raycast(ray, out hit, 100))
					{
						movementState = 2;
						obstacleClicked = false;
						navMeshAgent.speed = 5.0f;
						navMeshAgent.destination = hit.point;
						navMeshAgent.Resume();
					}
				}
				else if (Input.GetButtonDown ("Fire2")) 
				{
					if (Physics.Raycast(ray, out hit, 100))
					{
						movementState = 1;
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
				
				if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
					if (!navMeshAgent.hasPath || Mathf.Abs (navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
						movementState = 0;
				} else {
					if(navMeshAgent.speed == 3.5f)
						movementState = 1;
					else if (navMeshAgent.speed == 5.0f)
						movementState = 2;
				}
				
			anim.SetInteger ("ShouldJump", (jump? 1:0));
			anim.SetInteger("MovementState", movementState);
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
	