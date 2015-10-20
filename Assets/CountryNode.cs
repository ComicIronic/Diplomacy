using System;
using System.Collections;
using UnityEngine;

public class CountryNode : CNode {

	public Country country;

	public CountryNode () {
	}

	public override void GenNodeObject() {
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

	IEnumerator NodeMove() {
		while ( Input.GetMouseButton (1) ) {
			gameObject.transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3(0f, 0f, 9f);
			yield return null;
		}
	}
}


