﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CLink : MonoBehaviour {

	float lineWidth = 1f; //controls how thick the link lines are

	public List<CNode> nodes = new List<CNode>();

	CanvasCreator parentCC;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy() {
		parentCC.links.Remove (this);

		foreach (CNode node in nodes) {
			if (node != null) {
				node.links.Remove (this);
			}
		}
	}

	public void EstablishLink (CNode nodeA, CNode nodeB, CanvasCreator parent) {
		parentCC = parent;

		float nodeAngle = nodeA.GetAngle (nodeB);

		Vector3[] linePoints = new Vector3[4];

		Vector3 leftPoint = new Vector3 (Mathf.Cos (nodeAngle + Mathf.PI / 2) * lineWidth, Mathf.Sin (nodeAngle + Mathf.PI / 2) * lineWidth, 0f);
		Vector3 rightPoint = leftPoint * -1;

		linePoints [0] = nodeA.gameObject.transform.position + leftPoint;
		linePoints [1] = nodeA.gameObject.transform.position + rightPoint;
		linePoints [2] = nodeB.gameObject.transform.position + rightPoint;
		linePoints [3] = nodeB.gameObject.transform.position + leftPoint;

		Mesh lineMesh = gameObject.AddComponent<MeshFilter> ().mesh;
		lineMesh.vertices = linePoints;
		lineMesh.triangles = new int[6] {0, 1, 2, 0, 2, 3};
		lineMesh.RecalculateBounds ();
		lineMesh.RecalculateNormals ();

		MeshRenderer newRenderer = gameObject.AddComponent<MeshRenderer> ();
		newRenderer.material.color = Color.black;
		newRenderer.material.shader = Shader.Find ("UI/Default");
		
		gameObject.AddComponent<MeshCollider> ();

		nodes.Add (nodeA);
		nodeA.links.Add (this);
		nodes.Add (nodeB);
		nodeB.links.Add (this);

		parentCC.links.Add (this);
		gameObject.name = "Link " + parentCC.links.Count.ToString ();
		gameObject.transform.Translate (Vector3.back / 2);
	}
}
