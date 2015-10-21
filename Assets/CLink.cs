using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CLink : MonoBehaviour {

	float lineWidth = 1f; //controls how thick the link lines are

	public List<CNode> nodes = new List<CNode>();

	CanvasCreator parentCC;

	float lastAngle = 0f;
	float lastDist = 0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (nodes.Count != 2) {
			return;
		}

		if(LinkAngle() != lastAngle || LinkDist () != lastDist) {
			DrawLink();
		}
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
		parentCC.links.Add (this);

		gameObject.name = "Link " + parentCC.links.Count.ToString ();
		gameObject.transform.Translate (Vector3.back / 2);

		nodes.Add (nodeA);
		nodeA.links.Add (this);
		nodes.Add (nodeB);
		nodeB.links.Add (this);

		Mesh lineMesh = gameObject.AddComponent<MeshFilter> ().mesh;
		lineMesh.vertices = new Vector3[4];
		lineMesh.triangles = new int[6] {0, 1, 2, 0, 2, 3};

		DrawLink ();

		MeshRenderer newRenderer = gameObject.AddComponent<MeshRenderer> ();
		newRenderer.material.color = nodeB.LinkColor ();
		newRenderer.material.shader = Shader.Find ("UI/Default");
		
		gameObject.AddComponent<MeshCollider> ();
	}

	float LinkAngle() {
		return nodes [0].GetAngle (nodes [1]);
	}

	float LinkDist() {
		return MeshMaker.GetDistance (nodes [0].transform.position, nodes [1].transform.position);
	}

	void DrawLink() {
		float nodeAngle = LinkAngle();

		Vector3 averagePoint = (nodes[0].gameObject.transform.position + nodes[1].gameObject.transform.position) / 2;
		
		Vector3[] linePoints = gameObject.GetComponent<MeshFilter> ().mesh.vertices;
		
		Vector3 leftPoint = new Vector3 (Mathf.Cos (nodeAngle + Mathf.PI / 2) * lineWidth, Mathf.Sin (nodeAngle + Mathf.PI / 2) * lineWidth, 0f);
		Vector3 rightPoint = leftPoint * -1;
		
		linePoints [0] = nodes[0].gameObject.transform.position + leftPoint - averagePoint;
		linePoints [1] = nodes[0].gameObject.transform.position + rightPoint - averagePoint;
		linePoints [2] = nodes[1].gameObject.transform.position + rightPoint - averagePoint;
		linePoints [3] = nodes[1].gameObject.transform.position + leftPoint - averagePoint;

		gameObject.GetComponent<MeshFilter> ().mesh.vertices = linePoints;
		gameObject.GetComponent<MeshFilter> ().mesh.RecalculateNormals ();
		gameObject.GetComponent<MeshFilter> ().mesh.RecalculateBounds ();

		gameObject.transform.Translate (averagePoint - gameObject.transform.position);

		lastAngle = nodeAngle;
		lastDist = LinkDist ();
	}
}
