using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country {

	public Faction faction;

	public string countryName = "";

	public bool locked = false;

	public bool land = false;
	public bool centre = false;

	public List<CountryNode> nodes = new List<CountryNode> ();

	public List<CountryObject> territories = new List<CountryObject>();

	public Country () {
	}

	public void SetupCountry (CountryObject firstTerritory) {
		Camera.main.GetComponent<CanvasCreator> ().factions [0].AddCountry (this);
	
		AddTerritory (firstTerritory);
		countryName = firstTerritory.name;
	
		CreateCountryNode ();
	}
	
	public void ColorTerritories() {
		foreach (CountryObject territory in territories) {
			territory.ColorToFaction ();
		}
	}

	public void ColorNodes() {
		for (int i = 0; i < nodes.Count; i++) {
			CountryNode node = nodes [i];
			if (i == 0 && centre == true) {
				node.GetComponent<MeshRenderer> ().material.color = Color.red;
			} else {
				node.GetComponent<MeshRenderer> ().material.color = Color.blue;
			}
		}
	}

	public void CreateCountryNode() {
		CountryNode newNode = new GameObject ().AddComponent<CountryNode> ();
		nodes.Add (newNode);
		newNode.country = this;
		
		newNode.Initialise (territories [0].gameObject.transform.position, Camera.main.GetComponent<CanvasCreator> ());
	}

	public void AddTerritory(CountryObject territory) {
		territories.Add (territory);
		territory.parentCountry = this;
		territory.ColorToFaction ();
	}

	//If we're not moving it into another country but want to split it off
	public void RemoveTerritory(CountryObject territory) {
		territories.Remove (territory);
		territory.parentCountry = new Country();
		territory.parentCountry.SetupCountry (territory);
	}

	//If we're moving it into another country and don't want the inbetween
	public void AddTo(CountryObject territory, Country other) {
		territories.Remove (territory);
		other.AddTerritory (territory);
	}

	public void MergeInto(Country other) {
		if (other == this) {
			return;
		}

		while(territories.Count > 0) {
			AddTo (territories[0], other);
		}

		while(nodes.Count > 0) {
			CountryNode node = nodes[0];
			foreach(CLink link in node.links) { //Merge the other links made into this one
				CNode otherNode = link.nodes.Find(x => x != node && x.GetType() != System.Type.GetType("UnitNode"));
				if(otherNode != null) {
					other.nodes[0].EstablishLink(otherNode);
				}
			}
			nodes.Remove (node);
			GameObject.Destroy (node.gameObject);
		}

		faction = null;
		territories = null;
	}

	public string ExportCountry() {
		string contents = "[\n";
		contents += "name:" + countryName+ "\n";
		contents += "faction:" + faction.factionName+ "\n";
		contents += "centre:" + centre.ToString ()+ "\n";
		contents += "locked:" + locked.ToString ()+ "\n";
		contents += "land:" + land.ToString ()+ "\n";

		foreach (CountryNode node in nodes) {
			contents += node.ExportNode ();
		}

		foreach (CountryObject territory in territories) {
			contents += territory.ExportTerritory();
		}

		contents += "]\n";

		return contents;
	}
}
