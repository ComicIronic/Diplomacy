using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CountryObject : MonoBehaviour {

	Color landColor = new Color (62f/255, 165f/225, 224f/255);
	Color lockedColor = new Color (0.4f, 0.4f, 0.4f, 0f);

	public Country parentCountry;

	public bool menuOpen = false;
	public bool parentMenuOpen = false;

	public bool land = false;

	CanvasCreator parentCC;

	public CountryObject (){
		parentCC = Camera.main.GetComponent<CanvasCreator> ();

		parentCC.territories.Add (this);
		name = "Country " + parentCC.territories.Count.ToString ();
	}

	void Start() {
		parentCountry = new Country ();
		parentCountry.SetupCountry (this);
	}

	IEnumerator MergeLink () {
		LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer> ();
		if (lineRenderer == null) {
			lineRenderer = gameObject.AddComponent<LineRenderer> ();
		}

		lineRenderer.SetPosition (0, gameObject.transform.position);
		lineRenderer.SetPosition (1, gameObject.transform.position);

		while (Input.GetMouseButton(0)) {
			//linkActive = true;
			lineRenderer.enabled = true;
			lineRenderer.SetPosition (1, Camera.main.ScreenToWorldPoint (Input.mousePosition));
			yield return null;
		}
		
		lineRenderer.SetPosition (1, gameObject.transform.position);

		Ray castRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		//Debug.Log ("Raycasting from " + castRay.origin.ToString());
		RaycastHit hit;
		if (Physics.Raycast (castRay, out hit)) {
			//Debug.Log ("Hit something!");
			CountryObject country = hit.collider.gameObject.GetComponent<CountryObject>();
			if(country != null) {
				parentCountry.MergeInto(country.parentCountry);
			}
		}

	}

	void OnGUI () {

		if (menuOpen == false && parentMenuOpen == false) {
			return;
		}

		Vector3 boxPos = Camera.main.WorldToScreenPoint (gameObject.transform.position - new Vector3 (0f, 0f, 10f));
		float screenX = boxPos.x;
		float screenY = (Screen.height - boxPos.y);

		float guiWidth = 150;
		float guiHeight = 80;
		if (parentMenuOpen == true) {
			guiHeight = 250;
		}

		if (screenX > Screen.width - guiWidth) {
			screenX -= guiWidth;
		}
		if (screenY > Screen.height - guiHeight) {
			screenY -= guiHeight;
		}

		GUI.BeginGroup (new Rect (screenX, screenY, guiWidth, guiHeight));
		
		GUI.Box (new Rect (0, 0, guiWidth, guiHeight), "Options");
		
		if (menuOpen == true) {
			string buttonName = "Make Sea";
			if(land == false) {
				buttonName = "Make Land";
			}

			if(GUI.Button (new Rect(10, 20, guiWidth-20, 20), buttonName)) {
				land = !land;
				ColorToFaction();
			}

			if(parentCountry.territories.Count > 1) {
				if(GUI.Button(new Rect(10, 50, guiWidth-20, 20), "Split Off")) {
					parentCountry.RemoveTerritory(this);
				}
			}
		}

		if (parentMenuOpen == true) {
			parentCountry.countryName = GUI.TextField(new Rect(10, 30, guiWidth-20, 20), parentCountry.countryName);

			bool lastLocked = parentCountry.locked;

			parentCountry.locked = GUI.Toggle (new Rect(10, 60, guiWidth-20, 20), parentCountry.locked, "Locked");

			if(parentCountry.locked != lastLocked) {
				parentCountry.ColorTerritories();
			}

			string[] toolButtons = new string[parentCC.factions.Count];

			for(int i = 0; i < toolButtons.Length; i++) {
				if(parentCC.factions[i].factionName == "") {
					toolButtons[i] = "N/A";
				} else {
					toolButtons[i] = parentCC.factions[i].factionName.Substring (0, 1);
				}
			}

			int factionCount = -1;
			factionCount = GUI.Toolbar (new Rect(10, 90, guiWidth-20, 30), factionCount, toolButtons);

			if(factionCount != -1 && factionCount != parentCC.factions.IndexOf(parentCountry.faction)) {
				parentCC.factions[factionCount].AddCountry(parentCountry);
			}

			parentCountry.land = GUI.Toggle (new Rect(10, 130, guiWidth-20, 20), parentCountry.land, "Land");
			if(parentCountry.land == true) {
				bool lastCentre = parentCountry.centre;
				parentCountry.centre = GUI.Toggle (new Rect(10, 160, guiWidth-20, 20), parentCountry.centre, "Centre");
				if(lastCentre != parentCountry.centre) {
					parentCountry.ColorNodes();
				}
			} else if(parentCountry.centre == true) { //can't have a centre in a sea territory
				parentCountry.centre = false;
				parentCountry.ColorNodes();
			}

			if(parentCountry.nodes.Count > 1) {
				if(GUI.Button (new Rect(10, 210, 30, 30), "-")) {
					CountryNode toDelete = parentCountry.nodes[parentCountry.nodes.Count - 1];
					parentCountry.nodes.Remove (toDelete);
					Destroy (toDelete.gameObject);
				}
			}

			GUI.Label (new Rect(60, 210, 30, 30), parentCountry.nodes.Count.ToString());

			if(GUI.Button (new Rect(100, 210, 30, 30), "+")) {
				parentCountry.CreateCountryNode();
			}
		}
		
		GUI.EndGroup ();
	}

	public void ColorToFaction() {
		if (land == true) {
			gameObject.GetComponent<MeshRenderer> ().material.color = parentCountry.faction.factionColor;
		} else {
			gameObject.GetComponent<MeshRenderer> ().material.color = landColor;
		}

		if (parentCountry.locked == true) {
			gameObject.GetComponent<MeshRenderer> ().material.color -= lockedColor;
		}
	}


	public string ExportTerritory() {
		string contents = "<\n";
		contents += "land:" + land.ToString () + "\n";

		contents += "position:" + MeshMaker.ExportVector(gameObject.transform.position) + "\n";

		string points = "points:";
		Vector3[] vertices = gameObject.GetComponent<MeshFilter> ().mesh.vertices;
		for(int i = 0; i < vertices.Length; i++){
			Vector3 vertex = vertices[i];
			points += MeshMaker.ExportVector(vertex);
			if(i != vertices.Length - 1) {
				points += " ";
			}
		}

		contents += points + "\n";
		contents += ">\n";

		return contents;
	}
}
