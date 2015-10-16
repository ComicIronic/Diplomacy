using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CanvasClick : ClickBehaviour {

	public CanvasCreator parentCC;

	CNode lastNode;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	public override void ClickUpdate () {

		if (Input.GetMouseButtonDown (0) && GUIUtility.hotControl==0) { //We check that we haven't clicked on an active GUI element
			Ray castRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			//Debug.Log ("Raycasting from " + castRay.origin.ToString());
			RaycastHit hit;
			if (Physics.Raycast (castRay, out hit)) {
				//Debug.Log ("Hit something!");
				GameObject hitObject = hit.collider.gameObject;
				if (hitObject.GetComponent<CNode> () != null) {
					if(Input.GetButton("LeftShift")) {
						GameObject.Destroy(hitObject);
					} else {
						lastNode = hitObject.GetComponent<CNode>();
						//Debug.Log ("Clicked a node");
						hitObject.GetComponent<CNode>().StartCoroutine("LinkDraw");
					}
				} else if(hitObject.GetComponent<CLink>() != null) {
					if(Input.GetButtonDown("LeftShift")) {
						GameObject.Destroy(hitObject);
					}
				} else {
					lastNode = CreateNode(hit.point);
				}
			}
		}

		if (Input.GetMouseButtonUp (1) && GUIUtility.hotControl == 0) {
			Ray castRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			//Debug.Log ("Raycasting from " + castRay.origin.ToString());
			RaycastHit hit;
			if (Physics.Raycast (castRay, out hit)) {
				//Debug.Log ("Hit something!");
				GameObject hitObject = hit.collider.gameObject;
				if (hitObject.GetComponent<CNode> () != null) {
					hitObject.GetComponent<CNode> ().menuOpen = !hitObject.GetComponent<CNode> ().menuOpen;
				}
			}
		}

		if (Input.GetMouseButtonUp (0) && GUIUtility.hotControl == 0) {
			Ray castRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (castRay, out hit)) {
				GameObject hitObject = hit.collider.gameObject;
				if (hitObject.GetComponent<CNode> () != null) {
					if (lastNode != hitObject.GetComponent<CNode> ()) { //we dropped and lifted on two different nodes 
						lastNode.EstablishLink(hitObject.GetComponent<CNode>());
					}
				} else if (lastNode != null) { //we were dragging a node and want to make a new one
					lastNode.EstablishLink(CreateNode (hit.point));
				}
			}

			lastNode = null;
		}
	}

	CNode CreateNode(Vector3 newPos) {
		float xBound = Camera.main.GetComponent<CameraMove> ().xBound;
		float yBound = Camera.main.GetComponent<CameraMove> ().yBound;

		if (Mathf.Abs (newPos.x) >= xBound - 10f) {
			newPos.x += (xBound - Mathf.Abs (newPos.x)) * Mathf.Sign (newPos.x);
		}

		if (Mathf.Abs (newPos.y) >= yBound - 10f) {
			newPos.y += (yBound - Mathf.Abs (newPos.y)) * Mathf.Sign (newPos.y);
		}

		CNode newNode = new GameObject ().AddComponent<CNode> ();
		newNode.Initialise (newPos, parentCC);

		return newNode;
	}
}
