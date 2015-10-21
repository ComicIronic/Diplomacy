using System;
using System.Collections;
using UnityEngine;

public enum UnitType {None, Army, Fleet};

public class UnitNode : CNode {

	UnitType unitType = UnitType.None;

	public UnitNode () {
	}

	public override void GenNodeObject() {
		gameObject.name = "Unit Node";
		
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
		newRenderer.material.color = Color.yellow;
		newRenderer.material.shader = Shader.Find ("UI/Default");
		
		gameObject.AddComponent<MeshCollider> ();
	}

	public override bool CanEstablishLink (CNode other, CLink checkLink) {
		return links.Count == 0;
	}

	public override Color LinkColor () {
		return Color.green;
	}

	IEnumerator NodeMove() {
		while ( Input.GetMouseButton (1) ) {
			gameObject.transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3(0f, 0f, 9f);
			yield return null;
		}
	}

	void OnGUI() {
		if (menuOpen == true) {
			Vector3 boxPos = Camera.main.WorldToScreenPoint (gameObject.transform.position - new Vector3 (0f, 0f, 9f));
			float screenX = boxPos.x;
			float screenY = (Screen.height - boxPos.y);
		
			if (screenX > Screen.width - 150) {
				screenX -= 150;
			}
		
			if (screenY > Screen.height - 100) {
				screenY -= 110;
			}
		
			GUI.BeginGroup (new Rect (screenX, screenY, 150, 110));
		
			GUI.Box (new Rect (0, 0, 150, 110), "Options");

			string[] names = System.Enum.GetNames (System.Type.GetType ("UnitType"));
			GUI.Label (new Rect (10, 30, 130, 30), "Current Unit: " + names[(int)unitType]);

			for (int i = 0; i < names.Length; i++) {
				if (GUI.Button (new Rect (10, 50 + i * 20, 130, 20), names [i])) {
					unitType = (UnitType)i;
				}
			}
		
			GUI.EndGroup ();
		}
	}
}
