using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country {

	public Faction faction;

	public string countryName = "";

	public bool locked = false;

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

		foreach (CountryNode node in other.nodes) {
			foreach(CLink link in node.links) { //Merge the other links made into this one
				CNode otherNode = link.nodes.Find(x => x != node && x.GetType() != System.Type.GetType("UnitNode"));
				if(otherNode != null) {
					nodes[0].EstablishLink(otherNode);
				}
			}
			GameObject.Destroy (node.gameObject);
		}

		faction = null;
		territories = null;
	}
}
