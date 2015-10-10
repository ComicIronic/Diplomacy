using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CanvasClick : MonoBehaviour {

	public CanvasCreator parentCC;

	CNode lastNode;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0) && GUIUtility.hotControl==0) { //We check that we haven't clicked on an active GUI element
			Ray castRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			//Debug.Log ("Raycasting from " + castRay.origin.ToString());
			RaycastHit hit;
			if (Physics.Raycast (castRay, out hit)) {
				//Debug.Log ("Hit something!");
				GameObject hitObject = hit.collider.gameObject;
				if (hitObject.GetComponent<CNode> () != null) {
					lastNode = hitObject.GetComponent<CNode>();
					//Debug.Log ("Clicked a node");
					hitObject.GetComponent<CNode>().StartCoroutine("LinkDraw");
				} else {
					CNode newNode = new GameObject ().AddComponent<CNode> ();
					newNode.Initialise (hit.point, parentCC);
					lastNode = newNode;
				}
			}
		}

		if (Input.GetMouseButtonUp (0) && GUIUtility.hotControl == 0) {
			Ray castRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (castRay, out hit)) {
				GameObject hitObject = hit.collider.gameObject;
				if (hitObject.GetComponent<CNode> () != null) {
					if (lastNode == hitObject.GetComponent<CNode> ()) { //we dropped and lifted the mouse on the same node, toggle the menu
						lastNode.menuOpen = !lastNode.menuOpen;
					} else { //we dropped and lifted on two different nodes
						CLink createdLink = new GameObject().AddComponent<CLink>();
						createdLink.EstablishLink(lastNode, hitObject.GetComponent<CNode>());
					}
				} else if (lastNode != null) { //we were dragging a node and want to make a new one
					CNode newNode = new GameObject ().AddComponent<CNode> (); //create the new node
					newNode.Initialise (hit.point, parentCC);

					CLink createdLink = new GameObject().AddComponent<CLink>();
					createdLink.EstablishLink(lastNode, newNode);
				}
			}

			lastNode = null;
		}
	}
}
