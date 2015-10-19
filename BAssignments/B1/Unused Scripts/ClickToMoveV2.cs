using UnityEngine;
using System.Collections;

namespace CompleteProject
{
	
	public class ClickToMoveV2 : MonoBehaviour {

		public Selection Selector;

		//private Animator anim;
		private NavMeshAgent navMeshAgent;
		private Transform targetedObstacle;

		private bool walking;
		private bool obstacleClicked;

		
		// Use this for initialization
		void Awake () 
		{
			//anim = GetComponent<Animator> ();
			navMeshAgent = GetComponent<NavMeshAgent> ();
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (Selector.getSelection() == null)
				return;

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Input.GetButtonDown ("Fire2")) 
			{
				if (Physics.Raycast(ray, out hit, 100))
				{
					if (hit.collider.CompareTag("Obstacle"))
					{
						targetedObstacle = hit.transform;
						obstacleClicked = true;
					}
					
					else
					{
						ArrayList temp = Selector.getSelection();
						int length = temp.Count;

						GameObject targetObject;
						walking = true;
						obstacleClicked = false;
						
						//iterate trough the all unit's array
						for (int i = 0; i < length; i++) {
							targetObject = (GameObject) temp[i];
							NavMeshAgent agent = targetObject.GetComponent<NavMeshAgent>();
							agent.destination = hit.point;
							agent.Resume();
						}
					}
				}
			}
			
			if (obstacleClicked) {

			}
			
			if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
				if (!navMeshAgent.hasPath || Mathf.Abs (navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
					walking = false;
			} else {
				walking = true;
			}
			
			//anim.SetBool ("IsWalking", walking);
		}
		

		
	}
	
}