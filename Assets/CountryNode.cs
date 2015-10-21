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
		newRenderer.material.color = Color.red;
		newRenderer.material.shader = Shader.Find ("UI/Default");
		
		gameObject.AddComponent<MeshCollider> ();
	}

	public override bool CanEstablishLink (CNode other, CLink checkLink) {
		bool countryBlock = false;
		if (other.GetType () == System.Type.GetType ("CountryNode")) {
			CountryNode otherCountry = other as CountryNode;
			countryBlock = (otherCountry.country == country);
		}

		return !checkLink.nodes.Contains (this) && !countryBlock;
	}

	public override Color LinkColor () {
		return Color.cyan;
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
				screenY -= 50;
			}
			
			GUI.BeginGroup (new Rect (screenX, screenY, 150, 50));
			
			GUI.Box (new Rect (0, 0, 150, 150), "Options");
			name = GUI.TextField(new Rect(10, 20, 130, 30), name);
			
			GUI.EndGroup();
		}
	}
}


