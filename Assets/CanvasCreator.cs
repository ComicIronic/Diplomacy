using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum EditorState {Creation, Editing, Finalising};

public class CanvasCreator : MonoBehaviour {

	public EditorState currentState = EditorState.Creation;

	public ClickBehaviour currentClick = null;

	public GameObject canvas = null;

	bool looping = false;
	string width = "";
	string height = "";

	public List<CNode> nodes = new List<CNode>();

	public List<CLink> links = new List<CLink>();

	public List<Faction> factions = new List<Faction>();

	public List<CountryObject> territories = new List<CountryObject>();

	// Use this for initialization
	void Start () {
		Faction unowned = new Faction ();
		unowned.factionName = "Unowned";
		unowned.factionColor = new Color (208f / 255f, 164f / 255f, 80f / 255f);
		unowned.colorString = MeshMaker.ColorToHex (unowned.factionColor);
		factions.Add (unowned);
	}
	
	void OnGUI() {

		if (currentState == EditorState.Creation) {

			GUI.BeginGroup (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));

			GUI.Box (new Rect (0, 0, 200, 200), "Options");
			GUI.Label (new Rect (10, 10, 180, 20), "Width");
			width = GUI.TextField (new Rect (10, 40, 180, 20), width);
			GUI.Label (new Rect (10, 60, 180, 20), "Height");
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

					canvas.AddComponent<MeshCollider> ();


					CanvasClick newClick = new CanvasClick();
					newClick.parentCC = this;
					currentClick = newClick;

					CameraMove cameraMove = Camera.main.GetComponent<CameraMove>();
					cameraMove.xBound = widthNum;
					cameraMove.yBound = heightNum;

					currentState = EditorState.Editing;
				}
			}

			GUI.EndGroup ();

		} else if (currentState == EditorState.Editing) {

			GUI.BeginGroup (new Rect (0, 0, 200, 150));

			GUI.Box (new Rect (0, 0, 200, 150), "Options");
			GUI.Label (new Rect (20, 30, 160, 20), "Node count: " + nodes.Count.ToString ());
			GUI.Label (new Rect (20, 50, 160, 20), "Link count: " + links.Count.ToString ());

			if (nodes.Count == 0) {
				if (GUI.Button (new Rect (40, 70, 120, 30), "Delete Canvas")) {
					GameObject.Destroy (canvas);
				}
			} else {
				if (GUI.Button (new Rect (40, 110, 120, 30), "Convert Countries")) {
					if(ConvertCountries ()) {
						foreach(CNode node in nodes) {
							Destroy (node.gameObject);
						}

						Destroy(canvas);
						currentClick = new CountryClick();
						currentState = EditorState.Finalising;
					}
				}
			}

			GUI.EndGroup ();

		} else if (currentState == EditorState.Finalising) {

			int groupWidth = 100;

			GUI.BeginGroup(new Rect(0, 0, groupWidth*factions.Count + 50, 180));

			for(int factionCount = 0; factionCount < factions.Count; factionCount++) {
				Faction faction = factions[factionCount];

				GUI.Box( new Rect(groupWidth*factionCount, 0, groupWidth, 180), faction.factionName);

				if(factionCount != 0) {
					faction.factionName = GUI.TextField(new Rect(groupWidth*factionCount + 10, 20, groupWidth-20, 30), faction.factionName);
				} else {
					GUI.Label(new Rect(groupWidth*factionCount + 10, 20, groupWidth-20, 30), faction.factionName);
				}

				GUI.Label (new Rect(groupWidth*factionCount + 10, 60, groupWidth-20, 20), "Countries: " + faction.countries.Count.ToString());

				faction.colorString = GUI.TextField(new Rect(groupWidth*factionCount + 10, 80, groupWidth-20, 20), faction.colorString);

				if(faction.colorString.Length == 6) {
					bool canParse = true;

					for(int i = 0; i < faction.colorString.Length; i += 2) {
						byte outByte;
						if(!byte.TryParse(faction.colorString.Substring(0,2), System.Globalization.NumberStyles.HexNumber, null, out outByte)) {
							canParse = false;
						}
					}

					if(canParse) {
						if(GUI.Button (new Rect(groupWidth*factionCount + 10, 110, groupWidth-20, 20), "Colour")) {
							faction.factionColor = MeshMaker.HexToColor(faction.colorString);
							faction.ColorCountries();
						}
					}
				}

				if(factionCount != 0) {
					if(GUI.Button (new Rect(groupWidth*factionCount + 10, 140, groupWidth-20, 20), "Delete")) {
						factions.Remove (faction);
						while(faction.countries.Count != 0) {
							factions[0].AddCountry(faction.countries[0]);
						}
						factionCount--;
					}
				}
			}

			GUI.Box (new Rect(groupWidth*factions.Count, 50, 50, 50), "");
			if(GUI.Button (new Rect(groupWidth*factions.Count, 50, 50, 50), "+")) {
				factions.Add (new Faction());
			}
			GUI.EndGroup();
		}
	}

	//Given a node, fans a recursive tree out to catch all nodes connected by links to it
	void ConnectedNodes(CNode startingNode, List<CNode> toIgnore) {
		toIgnore.Add (startingNode);
		foreach (CNode otherNode in startingNode.ConnectedTo()) {
			if(!toIgnore.Contains(otherNode)) {
				ConnectedNodes(otherNode, toIgnore);
			}
		}
	}

	bool ConvertCountries() {
		List<CNode> foundNodes = new List<CNode> ();
		
		ConnectedNodes (nodes [0], foundNodes);
		if (foundNodes.Count == nodes.Count && nodes.FindAll (x => x.links.Count < 2).Count == 0) {
			//Debug.Log ("All nodes are connected by two links");
			List<List<CNode>> nodeShapes = new List<List<CNode>> ();
			
			List<CNode> multiNodes = nodes.FindAll (x => x.links.Count > 2);
			/*if(multiNodes.Count == 0) {
						nodeShapes.Add(nodes);
					}*/
			
			List<CNode[]> doneNodes = new List<CNode[]> ();
			
			foreach (CNode junction in multiNodes) { //we loop through each junction to catch all the shapes
				List<CNode> cwNodes = junction.CClockwiseConnected ();

				foreach (CNode node in cwNodes) { //we go around each node, looking for its clockwise next point
					if (doneNodes.Find (x => x [0] == node && x [1] == junction) != null) {
						continue;
					}

					float angleSum = 0f;

					doneNodes.Add (new CNode[2]{node, junction}); //we start by adding the node so we don't go around it clockwise again
					
					CNode currentNode = junction;
					CNode lastNode = node;
					
					//Debug.Log ("Started from " + lastNode.gameObject.name + " and " + currentNode.gameObject.name);
					
					List<CNode> currentShape = new List<CNode> ();
					currentShape.Add (node);
					
					do {
						CNode temp = currentNode;
						
						currentShape.Add (currentNode);
						
						currentNode = currentNode.CClockwiseFrom (lastNode);

						float addAngle =  MeshMaker.GetCWAngle(temp.gameObject.transform.position,
						                                 lastNode.gameObject.transform.position,
						                                 currentNode.gameObject.transform.position) - Mathf.PI; //we look at the exterior angle, which is why we subtract Pi

						//Debug.Log ("Angle between " + lastNode.gameObject.name + " and " + currentNode.gameObject.name + " is " + addAngle.ToString());

						angleSum += addAngle;

						lastNode = temp;
						
						//Debug.Log ("Added " + currentNode.gameObject.name);
						
						if (currentNode.links.Count > 2) { //if it's a node before a junction, we might loop through it in the future and want to avoid it now
							doneNodes.Add (new CNode[2]{lastNode, currentNode});
						}
					} while(currentNode != node);

					//Debug.Log (angleSum.ToString());

					if(angleSum < 0) { //This is true for shapes we've gone around the inside of - otherwise, we can include a negative shape
						nodeShapes.Add (currentShape);
					}
				}
				
			}
			
			for (int nodeIndex = 0; nodeIndex < nodeShapes.Count; nodeIndex++) { //we generate the shapes out of the list of CNodes
				
				List<CNode> nodeList = nodeShapes [nodeIndex];
				
				Vector3[] vertices = new Vector3[nodeList.Count];

				Vector3 averageVert = new Vector3();
				
				for (int i = 0; i < nodeList.Count; i++) {
					averageVert += nodeList [i].gameObject.transform.position;
				}

				averageVert /= nodeList.Count;

				for (int i = 0; i < nodeList.Count; i++) {
					vertices[i] = nodeList [i].gameObject.transform.position - averageVert;
				}
				
				Triangulator triangulator = new Triangulator (vertices);
				
				triangulator.Triangulate ();
				
				GameObject newObject = new GameObject ();
				
				MeshFilter newFilter = newObject.AddComponent<MeshFilter> ();
				newFilter.mesh.vertices = vertices;
				newFilter.mesh.triangles = triangulator.Triangulate ();
				newFilter.mesh.RecalculateBounds ();
				newFilter.mesh.RecalculateNormals ();
				
				MeshRenderer newRenderer = newObject.AddComponent<MeshRenderer> ();
				newRenderer.material.shader = Shader.Find ("UI/Default");
				float colorFraction = nodeIndex / (nodeShapes.Count * 1.0f);
				//Debug.Log ("Coloring at " + colorFraction.ToString ());
				newRenderer.material.color = new Color (colorFraction, colorFraction, colorFraction, 1f);	

				newObject.AddComponent<MeshCollider>();

				newObject.AddComponent<CountryObject>();

				newObject.transform.Translate(averageVert + new Vector3(0f, 0f, 1f));

				MeshMaker.GenerateOutline(newObject);
			}

			return true;
		}

		return false;
	}

	// Update is called once per frame
	void Update () {
		if (currentClick != null) {
			currentClick.ClickUpdate ();
		}
	}
}
