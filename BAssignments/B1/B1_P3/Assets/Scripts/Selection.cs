using UnityEngine;
using System.Collections;

public class Selection : MonoBehaviour {

	//the pointer moved where the user right-click (for moving unit(s))
	private GameObject selectionTarget;
	
	//the list of selected units
	private ArrayList listSelected  = new ArrayList();

	private Material selectedMaterial;
	private Material unselectedMaterial;

	private RaycastHit hit;
	
	//boolean to know if the left mouse button is down
	private bool leftButtonIsDown = false;

	void Start ()
	{
		selectedMaterial = new Material(Shader.Find("Specular"));
		selectedMaterial.color = new Color (0.6f, 0.2f, 0.3f);

		unselectedMaterial = new Material(Shader.Find("Specular"));
		unselectedMaterial.color = new Color (255.0f, 255.0f, 255.0f);
	}

	void Update () {

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (Input.GetButtonDown ("Fire1"))
		{
			if (Physics.Raycast(ray, out hit, 100))
			{
				if (hit.collider.CompareTag("Player"))
				{
					AddSelectedUnit(hit.transform.gameObject);
					//playerClicked = true;
				}
				
				else
				{
					ClearSelectedUnitsList();
				}
			}
		}

		/*
		if (Input.GetButtonUp ("Fire2"))
		{
			//if left button is up set the leftButtonIsDown to false
			leftButtonIsDown = false;

		}
		
		if (Input.GetButtonUp ("Fire2"))
		{
			//right click, move the pointer to the position

		}
		
		//if the left button is down and the mouse is moving, start dragging
		
		if(leftButtonIsDown)
		{

		}
		*/
		
		
		
	}
	
	
	
	void AddSelectedUnit(GameObject unitToAdd) {
		unitToAdd.transform.FindChild("sSphere").GetComponent<Renderer> ().material = selectedMaterial;

		ClickToMove c = unitToAdd.GetComponent<ClickToMove>();
		c.select();

		listSelected.Add(unitToAdd);
		//unitToAdd.GetComponent(pathTest).setSelectCircleVisible(true); 
	}
	
	void ClearSelectedUnitsList() {
		
		int length = listSelected.Count;
		GameObject targetObject;
		
		//iterate trough the all unit's array
		for (int i = 0; i < length; i++) {
			targetObject = (GameObject) listSelected[i];
			targetObject.transform.FindChild("sSphere").GetComponent<Renderer>().material = unselectedMaterial;
			ClickToMove c = targetObject.GetComponent<ClickToMove>();
			c.unselect();
		}

		listSelected.Clear();
	}

	
	int GetSelectedUnitsCount() {
		return listSelected.Count;
	}

	public ArrayList getSelection()
	{
		return listSelected;
	}
	

}
