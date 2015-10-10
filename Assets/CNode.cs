using UnityEngine;
using System.Collections;

public class CNode : MonoBehaviour {

	CanvasCreator parentCC;

	public bool menuOpen = false;
	public bool linkActive = false;

	float nodeSize = 2f;

	public LineRenderer lineRenderer;

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
	
	void OnGUI() {
		if (menuOpen == true) {
			Vector3 boxPos = Camera.main.WorldToScreenPoint(gameObject.transform.position - new Vector3(0f,0f,9f));
			float screenX = boxPos.x;
			float screenY = (Screen.height - boxPos.y);

			GUI.BeginGroup (new Rect (screenX, screenY, 150, 150));

			//Debug.Log("Opening menu at " + screenX.ToString() + " and " + screenY.ToString());

			GUI.Box (new Rect (0, 0, 150, 150), "Options");
			name = GUI.TextField(new Rect(10, 30, 130, 30), name);
			if (GUI.Button (new Rect (10, 70, 130, 30), "Delete")) {
				menuOpen = false;
				parentCC.nodes.Remove(this);
				Destroy(gameObject);
			} else if (GUI.Button(new Rect (10, 100, 130, 30), "Close Menu")) {
				menuOpen = false;
			}

			GUI.EndGroup();
		}
	}
}
