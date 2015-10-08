using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum EditorState {Creation, Editing};

public class CanvasCreator : MonoBehaviour {

	EditorState currentState = EditorState.Creation;

	public GameObject canvas = null;

	bool looping = false;
	string width = "";
	string height = "";

	public List<CNode> nodes = new List<CNode>();

	// Use this for initialization
	void Start () {

	}

	void OnGUI() {

		if (currentState == EditorState.Creation || canvas == null) {

			GUI.BeginGroup (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));

			GUI.Box (new Rect (0, 0, 200, 200), "Options");
			width = GUI.TextField (new Rect (10, 30, 180, 20), width);
			height = GUI.TextField (new Rect (10, 70, 180, 20), height);
			looping = GUI.Toggle (new Rect (40, 100, 120, 20), looping, "Looping");

			float testW = 0;
			float testH = 0;

			if (float.TryParse (width, out testW) && float.TryParse (height, out testH)) {
				if (GUI.Button (new Rect (40, 160, 120, 30), "Create Map")) {
					canvas = new GameObject ("Canvas");
					MeshFilter newFilter = canvas.AddComponent<MeshFilter> ();
					newFilter.mesh = new Mesh ();
					float widthNum = float.Parse (width) / 2;
					float heightNum = float.Parse (height) / 2;

					newFilter.mesh.vertices = new Vector3[4] {
						new Vector3 (-widthNum, -heightNum, 0),
						new Vector3 (widthNum, -heightNum, 0),
						new Vector3 (widthNum, heightNum, 0),
						new Vector3 (-widthNum, heightNum, 0)
					};
					newFilter.mesh.triangles = new int[6]{0,2,1,0,3,2};
					newFilter.mesh.RecalculateNormals ();

					MeshRenderer newRenderer = canvas.AddComponent<MeshRenderer> ();

					newRenderer.material.shader = Shader.Find ("Sprites/Default");
					newRenderer.material.color = Color.grey;

					canvas.AddComponent<MeshCollider>();
					CanvasClick newClick = canvas.AddComponent<CanvasClick>();
					newClick.parentCC = this;

					currentState = EditorState.Editing;

				}
			}

			GUI.EndGroup ();
		} else {

			GUI.BeginGroup (new Rect (0, 0, 200, 150));

			GUI.Box (new Rect (0, 0, 200, 150), "Options");
			GUI.Label (new Rect (20, 30, 160, 20), "Node count: " + nodes.Count.ToString ());

			if(nodes.Count == 0) {
				if (GUI.Button (new Rect (40, 60, 120, 30), "Delete Canvas")) {
					GameObject.Destroy(canvas);
				}
			}

			GUI.Button(new Rect(40, 100, 120, 30), "Convert Countries");

			GUI.EndGroup();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
