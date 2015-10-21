using System;
using System.Collections;
using UnityEngine;

public class CountryNode : CNode {

	public Country country;

	public UnitNode unitNode;

	public CountryNode () {
	}

	public override void OnDestroy() {
		if (unitNode != null) {
			GameObject.Destroy (unitNode.gameObject);
		}
		base.OnDestroy ();
	}

	public override void Initialise (Vector3 createPos, CanvasCreator newParent) {
		base.Initialise (createPos, newParent);

		unitNode = new GameObject ().AddComponent<UnitNode> ();
		unitNode.country = country;
		unitNode.Initialise (gameObject.transform.position, newParent);
		EstablishLink (unitNode);
	}

	public override void GenNodeObject() {
		parentCC.nodes.Add (this);

		gameObject.name = country.countryName + " Centre";
		if (country.nodes.Count > 1) {
			gameObject.name += " " + country.nodes.Count.ToString ();
		}
		
		MeshFilter newFilter = gameObject.AddComponent<MeshFilter> ();
		newFilter.mesh = new Mesh ();
		newFilter.mesh.vertices = new Vector3[4] {
			new Vector3 (-nodeSize, -nodeSize, 0),
			new Vector3 (nodeSize, -nodeSize, 0),
			new Vector3 (nodeSize, nodeSize, 0),
			new Vector3 (-nodeSize, nodeSize, 0)
		};
		newFilter.mesh.triangles = new int[6]{0,2,1,0,3,2};
		newFilter.mesh.RecalculateBounds ();
		newFilter.mesh.RecalculateNormals ();
		
		MeshRenderer newRenderer = gameObject.AddComponent<MeshRenderer> ();
		newRenderer.material.color = Color.blue;
		newRenderer.material.shader = Shader.Find ("UI/Default");	
		gameObject.AddComponent<MeshCollider> ();
	}

	public override bool CanEstablishLink (CNode other, CLink checkLink) {
	/*	bool countryBlock = false;
		if (other.GetType () == System.Type.GetType ("CountryNode")) {
			CountryNode otherCountry = other as CountryNode;
			countryBlock = (otherCountry.country == country);
		}*/

		return !checkLink.nodes.Contains (this);
	}

	public override Color LinkColor () {
		return Color.cyan;
	}

	public string ExportNode() {
		string contents = "{\n";
		contents += "name:" + gameObject.name + "\n";
		contents += "position:" + MeshMaker.ExportVector (gameObject.transform.position) + "\n";
		contents += "unit:" + System.Enum.GetName (System.Type.GetType ("UnitType"), unitNode.unitType) + "\n";
		contents += "unit-displace:" + MeshMaker.ExportVector (unitNode.gameObject.transform.position - gameObject.transform.position) + "\n";

		string slinks = "link-to:";

		foreach (CLink link in links) {
			CNode otherNode = link.nodes.Find (x => x != this);
			if(otherNode.GetType() == System.Type.GetType("CountryNode")) {
				CountryNode country = otherNode as CountryNode;
				slinks += country.country.countryName + "[" + country.country.nodes.IndexOf(country).ToString() + "]";

				if(links.IndexOf (link) != links.Count - 1) {
					slinks += ",";
				}
				//e.g. Moscow[0] would be Moscow's base node - the number really only matters for coasts, which have multiple nodes
			}
		}

		slinks += "\n";

		contents += slinks;
		contents += "}\n";
		return contents;
	}
	
	IEnumerator NodeMove() {
		while ( Input.GetMouseButton (1) ) {
			gameObject.transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3(0f, 0f, 9f);
			yield return null;
		}

		foreach(CountryNode node in parentCC.nodes) {
			if(node == this) { 
				continue;
			}
			if(MeshMaker.GetDistance(node.gameObject.transform.position, gameObject.transform.position) <= nodeSize) {
				country.MergeInto(node.country);
				break;
			}
		}
	}

	void OnGUI() {
		if (menuOpen == true) {
			Vector3 boxPos = Camera.main.WorldToScreenPoint(gameObject.transform.position - new Vector3(0f,0f,9f));
			float screenX = boxPos.x;
			float screenY = (Screen.height - boxPos.y);
			
			if(screenX > Screen.width - 150) {
				screenX -= 150;
			}
			
			if(screenY > Screen.height - 50) {
				screenY -= 60;
			}
			
			GUI.BeginGroup (new Rect (screenX, screenY, 150, 60));
			
			GUI.Box (new Rect (0, 0, 150, 60), "Options");
			name = GUI.TextField(new Rect(10, 20, 130, 30), name);
			
			GUI.EndGroup();
		}
	}
}


