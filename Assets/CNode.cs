using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CNode : MonoBehaviour {

	CanvasCreator parentCC;

	public bool menuOpen = false;
	public bool linkActive = false;

	public bool deletable = true;

	float nodeSize = 2f;

	public LineRenderer lineRenderer;

	public List<CLink> links = new List<CLink> ();

		// Use this for initialization
	public void Initialise (Vector3 createPos, CanvasCreator newParent) {
		gameObject.transform.position = createPos;
		parentCC = newParent;
		parentCC.nodes.Add (this);
		lineRenderer = gameObject.AddComponent<LineRenderer> ();
		lineRenderer.SetPosition (0, gameObject.transform.position);

		GenNodeObject ();

		transform.Translate (Vector3.back);
	}
	
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnDestroy() {
		menuOpen = false;
		parentCC.nodes.Remove(this);

		while (links.Count > 0) {

			CLink link = links [0];
			links.Remove (link);

			GameObject.Destroy (link.gameObject);
		}
	}

	void GenNodeObject() {
		gameObject.name = "Node " + parentCC.nodes.Count.ToString();

		MeshFilter newFilter = gameObject.AddComponent<MeshFilter> ();
		newFilter.mesh = new Mesh ();
		newFilter.mesh.vertices = new Vector3[4] {
			new Vector3 (-nodeSize, 0, 0),
			new Vector3 (0, -nodeSize, 0),
			new Vector3 (nodeSize, 0, 0),
			new Vector3 (0, nodeSize, 0)
		};
		newFilter.mesh.triangles = new int[6]{0,2,1,0,3,2};
		newFilter.mesh.RecalculateBounds ();
		newFilter.mesh.RecalculateNormals ();

		MeshRenderer newRenderer = gameObject.AddComponent<MeshRenderer> ();
		newRenderer.material.color = Color.black;
		newRenderer.material.shader = Shader.Find ("UI/Default");

		gameObject.AddComponent<MeshCollider> ();
	}

	//Handles the graphical element of creating links between nodes
	IEnumerator LinkDraw () {
		while (Input.GetMouseButton(0)) {
			//linkActive = true;
			lineRenderer.enabled = true;
			lineRenderer.SetPosition (1, Camera.main.ScreenToWorldPoint (Input.mousePosition));
			yield return null;
		}

		lineRenderer.SetPosition (1, gameObject.transform.position);
	}

	public void EstablishLink(CNode other) {
		foreach (CLink checkLink in parentCC.links) {
			if (!CanEstablishLink (other, checkLink) && !other.CanEstablishLink (this, checkLink)) {
				return;
			}
		}

		CLink createdLink = new GameObject().AddComponent<CLink>();
		createdLink.EstablishLink(this, other, parentCC);
	}

	public bool CanEstablishLink(CNode other, CLink checkLink) {

		float nodeAngle = GetAngle (other);
		
		if (checkLink.nodes.Contains(this)) {
			return false;
		}
		
		float firstAngle = GetAngle (checkLink.nodes [0]);
		float secondAngle = GetAngle (checkLink.nodes [1]);

		if(InAngleRange(nodeAngle, firstAngle, secondAngle)) {
			Debug.Log ("Found that " + firstAngle.ToString() + " and " + secondAngle.ToString() + " contained " + nodeAngle.ToString());
			return false;
		}
		//Debug.Log ("Found that " + firstAngle.ToString() + " and " + secondAngle.ToString() + " differed by more than pi or didn't contain " + nodeAngle.ToString());
		return true;
	}

	public float GetAngle (CNode other) {
		return Mathf.Atan2 (other.gameObject.transform.position.y - gameObject.transform.position.y, 
		                    other.gameObject.transform.position.x - gameObject.transform.position.x);
	}

	bool InAngleRange(float checkNum, float boundA, float boundB) {
		float greaterAngle = Mathf.Max (boundA, boundB);
		float lesserAngle = Mathf.Min (boundA, boundB);

		float angleDistance = greaterAngle - lesserAngle;

		if (angleDistance >= Mathf.PI) {
			return (checkNum > greaterAngle || checkNum < lesserAngle);
		} else {
			return (checkNum < greaterAngle && checkNum > lesserAngle);
		}
	}

	public List<CNode> ConnectedTo() {
		List<CNode> connectedTo = new List<CNode> ();

		foreach (CLink link in links) {
			connectedTo.Add (link.nodes.Find (x => x != this));
		}

		return connectedTo;
	}

	public List<CNode> CClockwiseConnected() {
		List<CNode> connected = ConnectedTo ();
		connected.Sort (delegate(CNode a, CNode b) {
			return (MeshMaker.GetCWAngle (gameObject.transform.position, 
			                             gameObject.transform.position + Vector3.up,
			                             a.gameObject.transform.position)).CompareTo (
				MeshMaker.GetCWAngle (gameObject.transform.position, 
			                     gameObject.transform.position + Vector3.up,
			                     b.gameObject.transform.position));
		});

		return connected;
	}

	public CNode CClockwiseFrom(CNode start) {
		List<CNode> connected = CClockwiseConnected ();

		int index = connected.FindIndex (x => x == start);

		if (index == connected.Count - 1) {
			index = 0;
		} else {
			index++;
		}

		return connected [index];
	}
	
	void OnGUI() {
		if (menuOpen == true) {
			Vector3 boxPos = Camera.main.WorldToScreenPoint(gameObject.transform.position - new Vector3(0f,0f,9f));
			float screenX = boxPos.x;
			float screenY = (Screen.height - boxPos.y);

			if(screenX > Screen.width - 150) {
				screenX -= 150;
			}

			if(screenY > Screen.height - 150) {
				screenY -= 150;
			}

			GUI.BeginGroup (new Rect (screenX, screenY, 150, 150));

			//Debug.Log("Opening menu at " + screenX.ToString() + " and " + screenY.ToString());

			GUI.Box (new Rect (0, 0, 150, 150), "Options");
			name = GUI.TextField(new Rect(10, 30, 130, 30), name);
			if(deletable) {
				if (GUI.Button (new Rect (10, 70, 130, 30), "Delete")) {
					Destroy(gameObject);
				}
			} if (GUI.Button(new Rect (10, 100, 130, 30), "Close Menu")) {
				menuOpen = false;
			}

			GUI.EndGroup();
		}
	}
}
