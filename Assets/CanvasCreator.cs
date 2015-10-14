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

	public List<CLink> links = new List<CLink>();

	// Use this for initialization
	void Start () {

	}
	
	void OnGUI() {

		if (currentState == EditorState.Creation || canvas == null) {

			GUI.BeginGroup (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));

			GUI.Box (new Rect (0, 0, 200, 200), "Options");
			GUI.Label(new Rect(10, 10, 180, 20), "Width");
			width = GUI.TextField (new Rect (10, 40, 180, 20), width);
			GUI.Label(new Rect(10, 60, 180, 20), "Height");
			height = GUI.TextField (new Rect (10, 90, 180, 20), height);
			looping = GUI.Toggle (new Rect (40, 120, 120, 20), looping, "Looping");

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
					newClick.xBound = widthNum;
					newClick.yBound = heightNum;

					currentState = EditorState.Editing;
				}
			}

			GUI.EndGroup ();
		} else {

			GUI.BeginGroup (new Rect (0, 0, 200, 150));

			GUI.Box (new Rect (0, 0, 200, 150), "Options");
			GUI.Label (new Rect (20, 30, 160, 20), "Node count: " + nodes.Count.ToString ());
			GUI.Label (new Rect (20, 50, 160, 20), "Link count: " + links.Count.ToString ());

			if(nodes.Count == 0) {
				if (GUI.Button (new Rect (40, 70, 120, 30), "Delete Canvas")) {
					GameObject.Destroy(canvas);
				}
			} else {

				if(GUI.Button(new Rect(40, 110, 120, 30), "Convert Countries")) {
					List<CNode> foundNodes = new List<CNode>();

					connectedNodes(nodes[0], foundNodes);
					if(foundNodes.Count == nodes.Count && nodes.FindAll(x => x.links.Count < 2).Count == 0) {
						//Debug.Log ("All nodes are connected by two links");
						List<List<CNode>> nodeShapes = new List<List<CNode>>();

						List<CNode> multiNodes = nodes.FindAll(x => x.links.Count > 2);
						/*if(multiNodes.Count == 0) {
							nodeShapes.Add(nodes);
						}*/

						List<CNode> doneNodes = new List<CNode>();

						foreach(CNode junction in multiNodes) { //we loop through each junction to catch all the shapes
							List<CNode> cwNodes = junction.CClockwiseConnected();

							foreach(CNode node in cwNodes) { //we go around each node, looking for its clockwise next point
								if(doneNodes.Contains(node)) {
									continue;
								}

								doneNodes.Add (node); //we start by adding the node 

								CNode currentNode = junction;
								CNode lastNode = node;

								List<CNode> currentShape = new List<CNode>();
								currentShape.Add (node);

								//List<Vector3> vertices = new List<Vector3>();

								//vertices.Add (node.gameObject.transform.position);

								do {
									CNode temp = currentNode;
									//vertices.Add (nextNode.gameObject.transform.position);

									currentShape.Add (currentNode);

									currentNode = currentNode.CClockwiseFrom(lastNode);
									lastNode = temp;

									if(currentNode.links.Count > 2) { //if it's a node before a junction, we might loop through it in the future and want to avoid it now
										doneNodes.Add (lastNode);
									}
								} while(currentNode != node);

								nodeShapes.Add (currentShape);
							}

						}

						foreach(List<CNode> nodeList in nodeShapes) { //we generate the shapes out of the list of CNodes

							Vector3[] vertices = new Vector3[nodeList.Count];

							for(int i = 0; i < nodeList.Count; i++) {
								vertices[i] = nodeList[i].gameObject.transform.position;
							}

							Triangulator triangulator = new Triangulator(vertices);

							triangulator.Triangulate();

							GameObject newObject = new GameObject();

							MeshFilter newFilter = newObject.AddComponent<MeshFilter>();
							newFilter.mesh.vertices = vertices;
							newFilter.mesh.triangles = triangulator.Triangulate();
							newFilter.mesh.RecalculateBounds();
							newFilter.mesh.RecalculateNormals();

							MeshRenderer newRenderer = newObject.AddComponent<MeshRenderer>();
							newRenderer.material.color = Color.black;
							newRenderer.material.shader = Shader.Find ("UI/Default");
								
						}
					}
				}
			}

			GUI.EndGroup();
		}
	}

	//Given a node, fans a recursive tree out to catch all nodes connected by links to it
	void connectedNodes(CNode startingNode, List<CNode> toIgnore) {
		toIgnore.Add (startingNode);
		foreach (CNode otherNode in startingNode.ConnectedTo()) {
			if(!toIgnore.Contains(otherNode)) {
				connectedNodes(otherNode, toIgnore);
			}
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
