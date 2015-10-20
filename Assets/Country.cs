using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : ScriptableObject {

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
		CountryNode newNode = new GameObject().AddComponent<CountryNode> ();
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
		territory.parentCountry = ScriptableObject.CreateInstance ("Country") as Country;
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

		foreach (CountryObject territory in territories) {
			AddTo (territory, other);
		}

		foreach (CountryNode node in nodes) {
			Destroy (node.gameObject);
		}

		faction = null;
		territories = null;
	}
}
