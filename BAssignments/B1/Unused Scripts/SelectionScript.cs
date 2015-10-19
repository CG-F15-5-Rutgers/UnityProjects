/*
using UnityEngine;
using System.Collections;

public class SelectionScript : MonoBehaviour {

	//the mouse position when the user move it ("bottom right" corner of the rect if you start from left to right and from top to bottom)
	private Vector2 mouseButton1DownPoint;
	//the mouse position where the user first click ("top left" corner of the rect if you start from left to right and from top to bottom)
	private Vector2 mouseButton1UpPoint;
	
	//the world position of the 4 corners
	private Vector3 topLeft;
	private Vector3 topRight;
	private Vector3 bottomLeft;
	private Vector3 bottomRight;
	
	//the pointer moved where the user right-click (for moving unit(s))
	public Transform target;
	
	//the list of selected units
	private ArrayList listSelected  = new ArrayList();
	
	//the list of ALL units in the scene
	private ArrayList listAllUnits  = new ArrayList();
	
	private RaycastHit hit;
	
	//boolean to know if the left mouse button is down
	private bool leftButtonIsDown = false;
	
	//cube placed at the 4 corner of the polygon, for visual debug
	private Transform topLeftCube;
	private Transform bottomRightCube;
	private Transform topRightCube;
	private Transform bottomLeftCube;
	
	//the layer mask for walkable zones
	private  int layerMask = 1 << 9;
	//the layer mask for selectable objects
	private int selectableLayerMask = 1 << 10;
	
	//width and height of the 2D rectangle
	public float width;
	public float height;
	
	public bool debugMode = false;
	
	public Texture selectionTexture;
	
	// range in which a mouse down and mouse up event will be treated as "the same location" on the map.
	private int mouseButtonReleaseBlurRange = 20;
	
	void OnGUI() {
		if (leftButtonIsDown) {
			
			width = mouseButton1UpPoint.x - mouseButton1DownPoint.x;
			height = (Screen.height - mouseButton1UpPoint.y) - (Screen.height - mouseButton1DownPoint.y);
			Rect rect = new Rect(mouseButton1DownPoint.x, Screen.height - mouseButton1DownPoint.y, width, height);
			GUI.DrawTexture (rect, selectionTexture, ScaleMode.StretchToFill, true);       
			
		}
	}
	
	
	void Start()
	{
		
		if(debugMode)
		{
			topRightCube = GameObject.Find("topRight").transform;  
			bottomLeftCube = GameObject.Find("bottomLeft").transform;  
			topLeftCube = GameObject.Find("topLeft").transform;
			bottomRightCube = GameObject.Find("bottomRight").transform;
		}
		
	}
	
	void Update () {
		
		if (Input.GetButtonDown ("Fire1"))
		{
			//if left button is down, save the mouse position, set the leftButtonIsDown to true and save the world position of the rectangle's "top left" corner
			mouseButton1UpPoint=Input.mousePosition;
			leftButtonIsDown = true;
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000);
			topLeft = hit.point;       
		}
		
		if (Input.GetButtonUp ("Fire1"))
		{
			//if left button is up set the leftButtonIsDown to false
			leftButtonIsDown = false;
			
			//if the range is not big enough, it's a simple clic, not a dragg-select operation
			if (IsInRange(mouseButton1DownPoint, mouseButton1UpPoint))
			{      
				// user just did a click, no dragging. mouse 1 down and up pos are equal.
				// if units are selected, move them. If not, select that unit.
				if (GetSelectedUnitsCount() == 0)
				{
					// no units selected, select the one we clicked - if any.
					
					if ( Physics.Raycast (Camera.main.ScreenPointToRay (mouseButton1DownPoint), out hit, 1000, selectableLayerMask) )
					{
						// Ray hit something. Try to select that hit target.
						//print ("Hit something: " + hit.collider.name);
						AddSelectedUnit(hit.collider.gameObject);
					}
					
				}
			}
			
		}
		
		if (Input.GetButtonUp ("Fire2"))
		{
			//right click, move the pointer to the position
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000,layerMask))
			{      
				target.position=hit.point;
				target.position.y=0;
				//todo : move the selected units to the position of the pointer.
			}
			
			
		}
		
		//if the left button is down and the mouse is moving, start dragging
		
		if(leftButtonIsDown)
		{
			//actual position of the mouse
			mouseButton1DownPoint=Input.mousePosition;
			
			//set the 3 other corner of the polygon in the world space
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000);
			bottomRight = hit.point;
			
			Physics.Raycast(Camera.main.ScreenPointToRay(Vector2(Input.mousePosition.x+width,Input.mousePosition.y)), out hit, 1000);
			bottomLeft= hit.point;
			
			Physics.Raycast(Camera.main.ScreenPointToRay(Vector2(Input.mousePosition.x,Input.mousePosition.y-height)), out hit, 1000);
			topRight= hit.point;
			
			ClearSelectedUnitsList();      
			SelectUnitsInArea();
			
			if(debugMode)
			{
				bottomRightCube.position = bottomRight;
				topRightCube.position = topRight;
				bottomLeftCube.position = bottomLeft;
				topLeftCube.position = topLeft;    
			}
			
		}
		
		
		
		
	}
	
	
	
	void AddSelectedUnit(GameObject unitToAdd) {
		listSelected.Add(unitToAdd);
		//unitToAdd.GetComponent(pathTest).setSelectCircleVisible(true); 
	}
	
	void ClearSelectedUnitsList() {
		
		//for (var unitToClear : GameObject in listSelected) {
		//	unitToClear.GetComponent(pathTest).setSelectCircleVisible(false);  
		//}
		listSelected.Clear();
	}
	
	void fillAllUnits(GameObject unitToAdd)
	{
		listAllUnits.Add(unitToAdd);
	}
	
	void SelectUnitsInArea() {
		Vector3[] poly = new Vector3[4];
		
		//set the array with the 4 points of the polygon
		poly[0] =  topLeft;
		poly[1] = topRight;
		poly[2] = bottomRight;
		poly[3] = bottomLeft;

		int length = listAllUnits.Count;
		GameObject targetObject;

		//iterate trough the all unit's array
		for (int i = 0; i < length; i++) {
			targetObject = listAllUnits[i];
			Vector3 toPos = targetObject.transform.position;
			//if the unit is in the polygon, select it.
			if (isPointInPoly(poly, toPos))
			{
				AddSelectedUnit(targetObject);
			}
		}
	}  
	
	
	//math formula to know if a given point is inside a polygon
	bool isPointInPoly(Vector3[] poly, Vector3 pt){
		bool c = false;
		int l = poly.Length;
		int j = l - 1;
		
		for(int i = -1 ; i++ < l; j = i){      
			if(((poly[i].z <= pt.z && pt.z < poly[j].z) || (poly[j].z <= pt.z && pt.z < poly[i].z))
			   (pt.x < (poly[j].x - poly[i].x) * (pt.z - poly[i].z) / (poly[j].z - poly[i].z) + poly[i].x))
				c = !c;
		}
		return c;
	}
	
	int GetSelectedUnitsCount() {
		return listSelected.Count;
	}
	
	bool IsInRange(Vector2 v1, Vector2 v2) {
		var dist = Vector2.Distance(v1, v2);
		
		if (Vector2.Distance(v1, v2) < mouseButtonReleaseBlurRange) {
			return true;
		}

		return false;
	}
}*/
